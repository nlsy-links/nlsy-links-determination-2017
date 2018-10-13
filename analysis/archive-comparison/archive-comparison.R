rm(list=ls(all=TRUE)) #Clear the memory of variables from previous run. This is not called by knitr, because it's above the first chunk.

# ---- load-sources ------------------------------------------------------------
#Load any source files that contain/define functions, but that don't load any other types of variables
#   into memory.  Avoid side effects and don't pollute the global environment.
source("./utility/connectivity.R")

# ---- load-packages -----------------------------------------------------------
library(xtable)
library(ggplot2)

library(magrittr) #Pipes
# library(ggplot2) #For graphing
requireNamespace("plyr")
requireNamespace("dplyr")
requireNamespace("scales") #For formating values in graphs
requireNamespace("knitr") #For the kable function for tables

# ---- declare-globals ---------------------------------------------------------
# includedRelationshipPaths <- c(2)
# includedRelationshipPaths <- c(1)
includedRelationshipPaths <- c(1, 2)

sql <- paste("
  SELECT
    a.ID,
    a.AlgorithmVersion,
    s.RelationshipPath,
    a.SubjectTag_S1,
    a.SubjectTag_S2,
    a.MultipleBirthIfSameSex,
    a.IsMz,
    a.LastSurvey_S1,
    a.LastSurvey_S2,
    a.RRoster,
    a.RImplicitPass1,
    a.RImplicit,
    a.RImplicit2004,
    a.RExplicitOldestSibVersion,
    a.RExplicitYoungestSibVersion,
    a.RExplicitPass1,
    a.RExplicit,
    a.RPass1,
    a.RFull
  FROM Archive.tblRelatedValuesArchive AS a
    INNER JOIN Process.tblRelatedStructure AS s ON
      a.SubjectTag_S1 = s.SubjectTag_S1
      AND
      a.SubjectTag_S2 = s.SubjectTag_S2
  WHERE
    s.RelationshipPath IN (1, 2)
    AND
    (
      a.AlgorithmVersion IN (
        SELECT TOP (2)
          AlgorithmVersion
        FROM Archive.tblRelatedValuesArchive AS a2
        GROUP BY AlgorithmVersion
        ORDER BY AlgorithmVersion DESC
      )
    )
")

# sql <- paste("SELECT Process.tblRelatedValuesArchive.ID, Process.tblRelatedValuesArchive.AlgorithmVersion, Process.tblRelatedStructure.RelationshipPath, Process.tblRelatedValuesArchive.SubjectTag_S1, Process.tblRelatedValuesArchive.SubjectTag_S2, Process.tblRelatedValuesArchive.MultipleBirthIfSameSex, Process.tblRelatedValuesArchive.IsMz, Process.tblRelatedValuesArchive.LastSurvey_S1, Process.tblRelatedValuesArchive.LastSurvey_S2, Process.tblRelatedValuesArchive.RRoster, Process.tblRelatedValuesArchive.RImplicitPass1, Process.tblRelatedValuesArchive.RImplicit, Process.tblRelatedValuesArchive.RImplicit2004, Process.tblRelatedValuesArchive.RExplicitOldestSibVersion, Process.tblRelatedValuesArchive.RExplicitYoungestSibVersion, Process.tblRelatedValuesArchive.RExplicitPass1, Process.tblRelatedValuesArchive.RExplicit, Process.tblRelatedValuesArchive.RPass1, Process.tblRelatedValuesArchive.RFull
#   FROM Process.tblRelatedValuesArchive INNER JOIN Process.tblRelatedStructure ON Process.tblRelatedValuesArchive.SubjectTag_S1 = Process.tblRelatedStructure.SubjectTag_S1 AND Process.tblRelatedValuesArchive.SubjectTag_S2 = Process.tblRelatedStructure.SubjectTag_S2
#   WHERE Process.tblRelatedStructure.RelationshipPath IN (", paste0(includedRelationshipPaths, collapse=","), ")
#     AND (Process.tblRelatedValuesArchive.AlgorithmVersion IN (73, 75))")


# sql <- gsub(pattern="\\n", replacement=" ", sql)
# sqlDescription <- "SELECT AlgorithmVersion, Description, Date2 FROM Archive.tblArchiveDescription where AlgorithmVersion=72" #AlgorithmVersion, Description
sqlDescription <- "SELECT AlgorithmVersion, Description FROM Archive.tblArchiveDescription" #AlgorithmVersion, Description
startTime <- Sys.time()

# ---- load-data ---------------------------------------------------------------
startTime <- Sys.time()
channel            <- open_dsn_channel_odbc(study = "79")
# DBI::dbGetInfo(channel)
dsRaw           <- DBI::dbGetQuery(channel, sql)
dsDescription   <- DBI::dbGetQuery(channel, sqlDescription)
DBI::dbDisconnect(channel, sql, sqlDescription)
elapsedTime <- Sys.time() - startTime
(Sys.time() - startTime);  rm(startTime)
nrow(dsRaw)

# ---- tweak-data --------------------------------------------------------------
olderVersionNumber <- min(dsRaw$AlgorithmVersion)
olderDescription <- dsDescription[dsDescription$AlgorithmVersion==olderVersionNumber, 'Description']
newerVersionNumber <- max(dsRaw$AlgorithmVersion)
newerDescription <- dsDescription[dsDescription$AlgorithmVersion==newerVersionNumber, 'Description']

columnsToConsider <- c("RImplicit2004", "RImplicit", "RExplicit", "RRoster", "RelationshipPath")
# dsLatestGen2Sibs <- dsRaw[dsRaw$AlgorithmVersion==newerVersionNumber & dsRaw$RelationshipPath %in% includedRelationshipPaths, ]
# dsPreviousGen2Sibs <- dsRaw[dsRaw$AlgorithmVersion==olderVersionNumber & dsRaw$RelationshipPath %in% includedRelationshipPaths, ]
dsLatest <- dsRaw[dsRaw$AlgorithmVersion==newerVersionNumber, ]
dsPrevious <- dsRaw[dsRaw$AlgorithmVersion==olderVersionNumber, ]

# head(dsLatest, 30)
# head(dsPrevious, 30)



dsCollapsedLatest <- dsLatest %>%
  dplyr::count_(columnsToConsider) %>%
  dplyr::arrange(dplyr::desc(n))

dsCollapsedPrevious <- dsPrevious %>%
  dplyr::count_(columnsToConsider) %>%
  dplyr::arrange(dplyr::desc(n))

ds <- dsCollapsedLatest %>%
  dplyr::full_join(dsCollapsedPrevious, by=columnsToConsider) %>%
  dplyr::mutate(
    n.x         = dplyr::coalesce(n.x, 0L),
    n.y         = dplyr::coalesce(n.y, 0L),
    Delta       = n.x - n.y
  ) %>%
  dplyr::select(-n.y) %>%
  dplyr::rename("Count" = "n.x")




# ds <- ds[order(-ds$Count, ds$Delta), c(4,1,2,3,5)]
# dsG <- ds
# dsG$RExplicit <- as.character(dsG$RExplicit)
# dsG$RExplicit <- gsub("0.5", "666", dsG$RExplicit)
# dsG
# ds <- dsG

# ---- define-tables ----------------------------------------------------------
colorGood <- "goodColor"
colorSoso <- "sosoColor"
colorBad <- "badColor"
colorNull <- "nullColor"

DetermineGoodRowIDs <- function( dsTable ) { # DetermineGoodRowIDs(ds)
  return( which(dsTable$RImplicit==dsTable$RExplicit) )
}

DetermineBadRowIDs <- function( dsTable ) { # DetermineBadRowIDs(ds)
  return( which(abs(dsTable$RImplicit - dsTable$RExplicit) >= .25) )
}

PrintConditionalTable <- function( relationshipPathID, tabelCaption=paste("Joint Frequencies for RelationshipPath", relationshipPathID, "(single-entered)")) {
#  relationshipPathID <- 1
  dsT <- ds[ds$RelationshipPath==relationshipPathID, ]
  dsT <- dsT[, colnames(dsT)!="RelationshipPath"]
  dsT <- dsT[order(-dsT$Count, dsT$Delta), c(5,1,2,3,4,6)]

#   idGoodRows <- which(dsT$RImplicit==dsT$RExplicit)# & (ds$RImplicit2004 !=.375 | is.na(ds$RImplicit2004)))
#   idBadRows <- which(abs(dsT$RImplicit - dsT$RExplicit) >= .25)# & ds$RImplicit!=1)
  idGoodRows <- DetermineGoodRowIDs(dsT)
  idSosoRows <- which((dsT$RImplicit==.375 | is.na(dsT$RImplicit)) & !is.na(dsT$RExplicit))
  idBadRows <- DetermineBadRowIDs(dsT)
  idNullRows <- which(is.na(dsT$RImplicit) & is.na(dsT$RExplicit))

  idRows <- c(idGoodRows, idSosoRows, idBadRows, idNullRows) -1 #Subtract one, b/c LaTeX row indices are zero-based
  idRowsList <- as.list(idRows)# as.list(unlist(idRows))
  colorRows <- c(rep(colorGood, length(idGoodRows)), rep(colorSoso, length(idSosoRows)), rep(colorBad, length(idBadRows)), rep(colorNull, length(idNullRows)))
  colorRows <- paste0("\\rowcolor{", colorRows, "} ")

  digitsFormat <- c(0, 0, 3, 3, 3, 3, 0) #Include a dummy at the beginning, for the row.names.
  textTable <- xtable(dsT, digits=digitsFormat, caption=tabelCaption)
  print(textTable, include.rownames=F, add.to.row=list(idRowsList, colorRows), NA.string="-")#, size="small")
}
# PrintConditionalTable(1)
#  PrintConditionalTable(relationshipPathID=1,tabelCaption="Counts for Gen1Housemates")

CreateMarginalTable  <- function(dsJoint,  relationshipPathID ) {
  dsJoint <- dsJoint[dsJoint$RelationshipPath==relationshipPathID, ]

  dsImplicitTable <- data.frame(table(dsJoint$RImplicit, useNA="always"))
  dsImplicit2004Table <- data.frame(table(dsJoint$RImplicit2004, useNA="always"))
  dsTable <- merge(x=dsImplicit2004Table, y=dsImplicitTable, by="Var1", all=T)
  colnames(dsTable)[colnames(dsTable)=="Freq.x"] <- "Implicit2004"
  colnames(dsTable)[colnames(dsTable)=="Freq.y"] <- "Implicit"

  if( relationshipPathID==1 ) {
    dsRosterTable <- data.frame(table(dsJoint$RRoster, useNA="always"))
    dsTable <- merge(x=dsTable, y=dsRosterTable, by="Var1", all=T)
    colnames(dsTable)[colnames(dsTable)=="Freq"] <- "Roster"
  }

  dsExplicitTable <- data.frame(table(dsJoint$RExplicit, useNA="always"))
  dsTable <- merge(x=dsTable, y=dsExplicitTable, by="Var1", all=T)
  colnames(dsTable)[colnames(dsTable)=="Freq"] <- "Explicit"

  dsRTable <- data.frame(table(dsJoint$RFull, useNA="always"))
  dsTable <- merge(x=dsTable, y=dsRTable, by="Var1", all=T)
  colnames(dsTable)[colnames(dsTable)=="Freq"] <- "Eventual"
  colnames(dsTable)[colnames(dsTable)=="Var1"] <- "R"

  dsTable <- dsTable[order(as.numeric(as.character(dsTable$R))), ]
  return( dsTable )
}
# CreateMarginalTable(dsJoint=dsLatest, relationshipPathID=1)
# CreateMarginalTable(dsJoint=dsPrevious, relationshipPathID=2)
# CreateMarginalTable(2)

PrintMarginalTable <- function(dsJoint, relationshipPathID, title=paste("Marginal Frequencies for RelationshipPath", relationshipPathID) ) {
  dsTable <- CreateMarginalTable(dsJoint,relationshipPathID)#[, 1:2]
  textTable <- xtable(dsTable, caption=title)
  print(textTable, include.rownames=F, NA.string="-", size="large")#, add.to.col=list(list(0, 1), c("\\rowcolor[gray]{.8} ", "\\rowcolor[gray]{.8} ")))
}
# PrintMarginalTable(dsJoint=dsLatest, relationshipPathID=1)
# PrintMarginalTable(dsJoint=dsLatest, relationshipPathID=2)

CreateRoc <- function( relationshipPathID ) {
  dsT <- ds[ds$RelationshipPath==relationshipPathID, ]
  idGoodRows <- DetermineGoodRowIDs(dsT)
  idSosoRows <- which((dsT$RImplicit==.375 | is.na(dsT$RImplicit)) & !is.na(dsT$RExplicit))
  idBadRows <- DetermineBadRowIDs(dsT)

  goodSumLatest <- sum(dsT[idGoodRows, "Count"])
  badSumLatest <- sum(dsT[idBadRows, "Count"])

  goodSumPrevious <- goodSumLatest - sum(dsT[idGoodRows, "Delta"])
  badSumPrevious <- badSumLatest - sum(dsT[idBadRows, "Delta"])
  dsRoc <- data.frame(Version=c(newerVersionNumber, olderVersionNumber), Agree=c(goodSumLatest, goodSumPrevious), Disagree=c(badSumLatest, badSumPrevious))

  rocLag1 <- ggplot(dsRoc, aes(y=Agree, x=Disagree, label=Version)) +
    geom_path() +
    geom_text()
    # coord_cartesian(xlim=c(0, 8000), ylim=c(0, 8000))#+ #xlim(0, 8000)
  return( rocLag1 )
}


# CreateRoc(relationshipPathID=1)
# CreateRoc(relationshipPathID=2)
