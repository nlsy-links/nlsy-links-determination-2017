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

requireNamespace("DBI")
requireNamespace("xtable")
requireNamespace("dplyr")
requireNamespace("scales") #For formating values in graphs
requireNamespace("knitr") #For the kable function for tables

# ---- declare-globals ---------------------------------------------------------
output_type   <- "html"
colorGood <- "goodColor"
colorSoso <- "sosoColor"
colorBad  <- "badColor"
colorNull <- "nullColor"

sql <- paste("
  SELECT
    a.ID,
    a.AlgorithmVersion,
    --s.RelationshipPath,
    a.SubjectTag_S1,
    a.SubjectTag_S2,
    a.MultipleBirthIfSameSex,
    a.IsMz,
    a.LastSurvey_S1,
    a.LastSurvey_S2,
    a.RRoster,
    a.RImplicitPass1,
    a.RImplicit,
    --a.RImplicit2004,
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
    (
      a.AlgorithmVersion IN (
        SELECT TOP (2)
          AlgorithmVersion
        FROM Archive.tblRelatedValuesArchive AS a2
        GROUP BY AlgorithmVersion
        ORDER BY AlgorithmVersion DESC
      )
    )
    --AND
    --s.RelationshipPath IN (1, 2)
  ORDER BY AlgorithmVersion, SubjectTag_S1, SubjectTag_S2
")

# sql <- paste("SELECT Process.tblRelatedValuesArchive.ID, Process.tblRelatedValuesArchive.AlgorithmVersion, Process.tblRelatedStructure.RelationshipPath, Process.tblRelatedValuesArchive.SubjectTag_S1, Process.tblRelatedValuesArchive.SubjectTag_S2, Process.tblRelatedValuesArchive.MultipleBirthIfSameSex, Process.tblRelatedValuesArchive.IsMz, Process.tblRelatedValuesArchive.LastSurvey_S1, Process.tblRelatedValuesArchive.LastSurvey_S2, Process.tblRelatedValuesArchive.RRoster, Process.tblRelatedValuesArchive.RImplicitPass1, Process.tblRelatedValuesArchive.RImplicit, Process.tblRelatedValuesArchive.RImplicit2004, Process.tblRelatedValuesArchive.RExplicitOldestSibVersion, Process.tblRelatedValuesArchive.RExplicitYoungestSibVersion, Process.tblRelatedValuesArchive.RExplicitPass1, Process.tblRelatedValuesArchive.RExplicit, Process.tblRelatedValuesArchive.RPass1, Process.tblRelatedValuesArchive.RFull
#   FROM Process.tblRelatedValuesArchive INNER JOIN Process.tblRelatedStructure ON Process.tblRelatedValuesArchive.SubjectTag_S1 = Process.tblRelatedStructure.SubjectTag_S1 AND Process.tblRelatedValuesArchive.SubjectTag_S2 = Process.tblRelatedStructure.SubjectTag_S2
#   WHERE Process.tblRelatedStructure.RelationshipPath IN (", paste0(includedRelationshipPaths, collapse=","), ")
#     AND (Process.tblRelatedValuesArchive.AlgorithmVersion IN (73, 75))")

DetermineGoodRowIDs <- function( dsTable ) { # DetermineGoodRowIDs(ds)
  return( which(dsTable$RImplicit==dsTable$RExplicit) )
}

DetermineBadRowIDs <- function( dsTable ) { # DetermineBadRowIDs(ds)
  return( which(abs(dsTable$RImplicit - dsTable$RExplicit) >= .25) )
}

# sql <- gsub(pattern="\\n", replacement=" ", sql)
# sqlDescription <- "SELECT AlgorithmVersion, Description, Date2 FROM Archive.tblArchiveDescription where AlgorithmVersion=72" #AlgorithmVersion, Description
sqlDescription <- "SELECT AlgorithmVersion, Description FROM Archive.tblArchiveDescription" #AlgorithmVersion, Description
startTime <- Sys.time()

# ---- load-data ---------------------------------------------------------------
# startTime <- Sys.time()
channel            <- open_dsn_channel_odbc(study = "97")
# DBI::dbGetInfo(channel)
dsRaw           <- DBI::dbGetQuery(channel, sql)
dsDescription   <- DBI::dbGetQuery(channel, sqlDescription)
DBI::dbDisconnect(channel, sql, sqlDescription)
# (Sys.time() - startTime);  rm(startTime)
# nrow(dsRaw)

# ---- tweak-data --------------------------------------------------------------
olderVersionNumber <- min(dsRaw$AlgorithmVersion)
olderDescription <- dsDescription[dsDescription$AlgorithmVersion==olderVersionNumber, 'Description']
newerVersionNumber <- max(dsRaw$AlgorithmVersion)
newerDescription <- dsDescription[dsDescription$AlgorithmVersion==newerVersionNumber, 'Description']

columnsToConsider <- c("RImplicit", "RExplicit", "RRoster")
# dsLatestGen2Sibs <- dsRaw[dsRaw$AlgorithmVersion==newerVersionNumber & dsRaw$RelationshipPath %in% includedRelationshipPaths, ]
# dsPreviousGen2Sibs <- dsRaw[dsRaw$AlgorithmVersion==olderVersionNumber & dsRaw$RelationshipPath %in% includedRelationshipPaths, ]
dsLatest    <- dsRaw[dsRaw$AlgorithmVersion==newerVersionNumber, ]
dsPrevious  <- dsRaw[dsRaw$AlgorithmVersion==olderVersionNumber, ]

# head(dsLatest, 30)
# head(dsPrevious, 30)


# dsCollapsedLatest <- ddply(dsLatest, .variables=columnsToConsider, .fun=nrow)
dsCollapsedLatest <- dsLatest %>%
  dplyr::count_(vars=columnsToConsider) %>%
  dplyr::rename(
    "Count" = "n"
  )

dsCollapsedPrevious <- dsPrevious %>%
  dplyr::count_(vars=columnsToConsider) %>%
  dplyr::rename(
    "count_previous" = "n"
  )

ds <- dsCollapsedLatest %>%
  dplyr::full_join(dsCollapsedPrevious, by = columnsToConsider) %>%
  dplyr::mutate(
    Count           = dplyr::coalesce(.data$Count   , 0L),
    count_previous  = dplyr::coalesce(count_previous, 0L),
    Delta           = Count - count_previous
  ) %>%
  dplyr::select(-count_previous) %>%
  dplyr::arrange(desc(Count))


# ---- graph-roc ---------------------------------------------------------------

dsT        <- as.data.frame(ds)
idGoodRows <- DetermineGoodRowIDs(dsT)
idSosoRows <- which((dsT$RImplicit==.375 | is.na(dsT$RImplicit)) & !is.na(dsT$RExplicit))
idBadRows  <- DetermineBadRowIDs(dsT)

goodSumLatest <- sum(dsT[idGoodRows, ]$Count)
badSumLatest  <- sum(dsT[idBadRows , ]$Count)

goodSumPrevious <- goodSumLatest - sum(dsT[idGoodRows, ]$Delta)
badSumPrevious  <- badSumLatest  - sum(dsT[idBadRows , ]$Delta)
dsRoc <- tibble::tibble(
  Version   = c(newerVersionNumber, olderVersionNumber  ),
  Agree     = c(goodSumLatest     , goodSumPrevious     ),
  Disagree  = c(badSumLatest      , badSumPrevious      )
)

ggplot(dsRoc, aes(y=Agree, x=Disagree, label=Version)) +
  geom_path() +
  geom_text()
  # coord_cartesian(xlim=c(0, 8000), ylim=c(0, 8000))#+ #xlim(0, 8000)


# ---- table-marginal ----------------------------------------------------------
CreateMarginalTable  <- function( dsJoint ) {
  dsJoint %>%
    dplyr::count(RImplicit) %>%
    dplyr::rename(R=RImplicit, Implicit=n) %>%
    dplyr::full_join(
      dsJoint %>%
        dplyr::count(RExplicit) %>%
        dplyr::rename(R=RExplicit, Explicit=n),
      by = "R"
    ) %>%
    dplyr::full_join(
      dsJoint %>%
        dplyr::count(RRoster) %>%
        dplyr::rename(R=RRoster, Roster=n),
      by = "R"
    ) %>%
    dplyr::full_join(
      dsJoint %>%
        dplyr::count(RFull) %>%
        dplyr::rename(R=RFull, Eventual=n),
      by = "R"
    ) %>%
    dplyr::mutate(
      R         = sprintf("%.3f", R),
      Eventual  = dplyr::coalesce(Eventual, 0L),
      # Implicit  = prettyNum(Implicit  , big.mark = ",", width=5),
      # Explicit  = prettyNum(Explicit  , big.mark = ",", width=5),
      # Roster    = prettyNum(Roster    , big.mark = ",", width=5),
      # Eventual  = prettyNum(Eventual  , big.mark = ",", width=5),
      # Implicit  = scales::comma(Implicit),
      # Explicit  = scales::comma(Explicit),
      # Roster    = scales::comma(Roster  ),
      # Eventual  = scales::comma(Eventual),


      R         = dplyr::if_else(R=="NA", "-", R),
      # Implicit  = dplyr::if_else(Implicit=="NA", "-", Implicit),
      # Explicit  = dplyr::if_else(Explicit=="NA", "-", Explicit),
      # Roster    = dplyr::if_else(Roster  =="NA", "-", Roster  ),
      # Eventual  = dplyr::if_else(Eventual=="NA", "-", Eventual)
      # R       = dplyr::if_else(is.na(R), "-", sprintf("%.3f", R))
      # R       = dplyr::coalesce(R, "-")
    ) %>%
    dplyr::arrange(R) #%>% dput()
}
# CreateMarginalTable(dsJoint=dsLatest)

PrintMarginalTable <- function( dsJoint, caption ) {
  dsTable   <- CreateMarginalTable(dsJoint)#[, 1:2]
  textTable <- xtable(dsTable, caption=caption)
  print(textTable, include.rownames=F, NA.string="-", size="large",  right =T, type=output_type)#, add.to.col=list(list(0, 1), c("\\rowcolor[gray]{.8} ", "\\rowcolor[gray]{.8} ")))
}
PrintMarginalTable(dsJoint=dsLatest  , caption="Counts for 97 Housemates")
PrintMarginalTable(dsJoint=dsPrevious, caption="Counts for 97 Housemates (Previous version of links)")


# ---- table-conditional -------------------------------------------------------
PrintConditionalTable <- function( ) {
  dsT <- ds %>%
    dplyr::select(Count, RImplicit, RExplicit, RRoster, Delta) %>%
    dplyr::arrange(desc(Count), Delta)

  idGoodRows <- DetermineGoodRowIDs(dsT)
  idSosoRows <- which((dsT$RImplicit==.375 | is.na(dsT$RImplicit)) & !is.na(dsT$RExplicit))
  idBadRows  <- DetermineBadRowIDs(dsT)
  idNullRows <- which(is.na(dsT$RImplicit) & is.na(dsT$RExplicit))

  idRows     <- c(idGoodRows, idSosoRows, idBadRows, idNullRows) -1 #Subtract one, b/c LaTeX row indices are zero-based
  colorRows  <- c(rep(colorGood, length(idGoodRows)), rep(colorSoso, length(idSosoRows)), rep(colorBad, length(idBadRows)), rep(colorNull, length(idNullRows)))
  colorRows  <- paste0("\\rowcolor{", colorRows, "} ")

  digitsFormat <- c(0, 0, 3, 3, 3, 0) #Include a dummy at the beginning, for the row.names.
  textTable <- xtable(dsT, digits=digitsFormat, caption="Joint Frequencies for 97 Housemates")
  print(textTable, include.rownames=F, add.to.row=list(as.list(idRows), colorRows), NA.string="-", type=output_type)#, size="small")
}
PrintConditionalTable()
