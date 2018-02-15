rm(list=ls(all=TRUE)) #Clear the memory of variables from previous run. This is not called by knitr, because it's above the first chunk.

# ---- load-sources ------------------------------------------------------------
#Load any source files that contain/define functions, but that don't load any other types of variables
#   into memory.  Avoid side effects and don't pollute the global environment.
# source("./utility/connectivity.R")

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
config <- config::get()
path_in_description   <- "data-public/metadata/tables-97/ArchiveDescription.csv"

output_type   <- "html"
colorGood <- "goodColor"
colorSoso <- "sosoColor"
colorBad  <- "badColor"
colorNull <- "nullColor"

DetermineGoodRowIDs <- function( dsTable ) { # DetermineGoodRowIDs(ds)
  return( which(dsTable$RImplicit==dsTable$RExplicit) )
}

DetermineBadRowIDs <- function( dsTable ) { # DetermineBadRowIDs(ds)
  return( which(abs(dsTable$RImplicit - dsTable$RExplicit) >= .25) )
}

col_types <- c(# glue::collapse(paste0(colnames(dsRaw), '                     = "', purrr:::map_chr(dsRaw, class), '"'), sep = ",\n")
  "AlgorithmVersion"                = "integer",
  "ExtendedID"                      = "integer",
  "SubjectTag_S1"                   = "integer",
  "SubjectTag_S2"                   = "integer",
  "SubjectID_S1"                    = "integer",
  "SubjectID_S2"                    = "integer",
  "MultipleBirthIfSameSex"          = "integer",
  "IsMz"                            = "integer",
  "SameGeneration"                  = "integer",
  "RosterAssignmentID"              = "integer",
  "RRoster"                         = "numeric",
  "LastSurvey_S1"                   = "integer",
  "LastSurvey_S2"                   = "integer",
  "RImplicitPass1"                  = "numeric",
  "RImplicit"                       = "numeric",
  "RImplicitSubject"                = "numeric",
  "RImplicitMother"                 = "numeric",
  "RExplicitOlderSibVersion"        = "numeric",
  "RExplicitYoungerSibVersion"      = "numeric",
  "RExplicitPass1"                  = "numeric",
  "RExplicit"                       = "numeric",
  "RPass1"                          = "numeric",
  "R"                               = "numeric",
  "RFull"                           = "numeric",
  "RPeek"                           = "numeric"
)

col_types_description <- readr::cols_only(
  AlgorithmVersion  = readr::col_integer(),
  Description       = readr::col_character(),
  Date              = readr::col_date()
)

# ---- load-data ---------------------------------------------------------------
# readr::spec_csv(path_in_description)
ds_description <- readr::read_csv(path_in_description, col_types = col_types_description)

recent_versions <- ds_description %>%
  dplyr::pull(AlgorithmVersion) %>%
  sort() %>%
  tail(2)

sql <- glue::glue("SELECT * FROM file WHERE AlgorithmVersion IN ({versions})", versions=glue::collapse(recent_versions, sep=", "))

# system.time({
dsRaw <- sqldf::read.csv.sql(
  file        = "data-public/derived/links-archive-2017-97.csv",
  sql         = sql,
  # sql         = "SELECT * FROM file WHERE AlgorithmVersion IN (2, 3)",
  eol         = "\n",
  colClasses  = col_types
)
# })
table(dsRaw$RRoster, useNA = "always")

# purrr::map_chr(dsRaw, class)
# startTime <- Sys.time()
# channel            <- open_dsn_channel_odbc(study = "97")
# # DBI::dbGetInfo(channel)
# dsRaw           <- DBI::dbGetQuery(channel, sql)
# dsDescription   <- DBI::dbGetQuery(channel, sqlDescription)
# DBI::dbDisconnect(channel, sql, sqlDescription)
# (Sys.time() - startTime);  rm(startTime)
# nrow(dsRaw)

# ---- tweak-data --------------------------------------------------------------
# glue::collapse(paste(colnames(dsRaw), "=", purrr:::map_chr(dsRaw, class)), sep = ",\n")
ds_description <- ds_description %>%
  tibble::as_tibble()

format_r_digits <- function( x ) sprintf("%0.3f", as.numeric(x))
dsRaw <- dsRaw %>%
  tibble::as_tibble() %>%
  dplyr::mutate(
    RRoster                         = format_r_digits(RRoster                   ),
    # RImplicitPass1                  = format_r_digits(RImplicitPass1            ),
    # RImplicit                       = format_r_digits(RImplicit                 ),
    # RImplicitSubject                = format_r_digits(RImplicitSubject          ),
    # RImplicitMother                 = format_r_digits(RImplicitMother           ),
    # RExplicitOlderSibVersion        = format_r_digits(RExplicitOlderSibVersion  ),
    # RExplicitYoungerSibVersion      = format_r_digits(RExplicitYoungerSibVersion),
    # RExplicitPass1                  = format_r_digits(RExplicitPass1            ),
    RExplicit                       = format_r_digits(RExplicit                 ),
    # RPass1                          = format_r_digits(RPass1                    ),
    R                               = format_r_digits(R                         ),
    RFull                           = format_r_digits(RFull                     )
    # RPeek                           = format_r_digits(RPeek                     )
  )
# table(dsRaw$RPeek)
# dsRaw

olderVersionNumber <- min(dsRaw$AlgorithmVersion)
olderDescription <- ds_description[ds_description$AlgorithmVersion==olderVersionNumber, ]$Description
newerVersionNumber <- max(dsRaw$AlgorithmVersion)
newerDescription <- ds_description[ds_description$AlgorithmVersion==newerVersionNumber, ]$Description

columnsToConsider <- c("RImplicit", "RExplicit", "RRoster")
# dsLatestGen2Sibs <- dsRaw[dsRaw$AlgorithmVersion==newerVersionNumber & dsRaw$RelationshipPath %in% includedRelationshipPaths, ]
# dsPreviousGen2Sibs <- dsRaw[dsRaw$AlgorithmVersion==olderVersionNumber & dsRaw$RelationshipPath %in% includedRelationshipPaths, ]
dsLatest    <- dsRaw[dsRaw$AlgorithmVersion==newerVersionNumber, ]
dsPrevious  <- dsRaw[dsRaw$AlgorithmVersion==olderVersionNumber, ]

# ds <- dsRaw %>%
#   dplyr::mutate(
#     latest      = (.data$AlgorithmVersion == newerVersionNumber)
#   ) %>%
#   dplyr::count_(columnsToConsider)
# ds
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
idBadRows  <-integer(0) # DetermineBadRowIDs(dsT)

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
      # R         = sprintf("%.3f", R),
      Eventual  = dplyr::coalesce(Eventual, 0L),
      # Implicit  = prettyNum(Implicit  , big.mark = ",", width=5),
      # Explicit  = prettyNum(Explicit  , big.mark = ",", width=5),
      # Roster    = prettyNum(Roster    , big.mark = ",", width=5),
      # Eventual  = prettyNum(Eventual  , big.mark = ",", width=5),
      # Implicit  = scales::comma(Implicit),
      # Explicit  = scales::comma(Explicit),
      # Roster    = scales::comma(Roster  ),
      # Eventual  = scales::comma(Eventual),


      R         = dplyr::if_else(R=="NA", "-", R)
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

# dsLatest %>%
#   tibble::as_tibble()

dsLatest %>%
  CreateMarginalTable() %>%
  knitr::kable(
    format      = output_type,
    format.args = list(big.mark=","),
    caption     = "Counts for 97 Housemates"
  )
dsPrevious %>%
  CreateMarginalTable() %>%
  knitr::kable(
    format      = output_type,
    format.args = list(big.mark=","),
    caption     = "Counts for 97 Housemates (Previous version of links)"
  )

# PrintMarginalTable <- function( dsJoint, caption ) {
#   dsTable   <- CreateMarginalTable(dsJoint)#[, 1:2]
#   textTable <- xtable(dsTable, caption=caption)
#   print(textTable, include.rownames=F, NA.string="-", size="large",  right =T, type=output_type)#, add.to.col=list(list(0, 1), c("\\rowcolor[gray]{.8} ", "\\rowcolor[gray]{.8} ")))
# }
# PrintMarginalTable(dsJoint=dsLatest  , caption="Counts for 97 Housemates")
# PrintMarginalTable(dsJoint=dsPrevious, caption="Counts for 97 Housemates (Previous version of links)")


# ---- table-conditional -------------------------------------------------------
PrintConditionalTable <- function( ) {
  dsT <- ds %>%
    dplyr::select(Count, RImplicit, RExplicit, RRoster, Delta) %>%
    dplyr::arrange(desc(Count), Delta)

  idGoodRows <- DetermineGoodRowIDs(dsT)
  idSosoRows <- which((dsT$RImplicit==.375 | is.na(dsT$RImplicit)) & !is.na(dsT$RExplicit))
  idBadRows  <- integer(0) # DetermineBadRowIDs(dsT)
  idNullRows <- which(is.na(dsT$RImplicit) & is.na(dsT$RExplicit))

  idRows     <- c(idGoodRows, idSosoRows, idBadRows, idNullRows) -1 #Subtract one, b/c LaTeX row indices are zero-based
  colorRows  <- c(rep(colorGood, length(idGoodRows)), rep(colorSoso, length(idSosoRows)), rep(colorBad, length(idBadRows)), rep(colorNull, length(idNullRows)))
  colorRows  <- paste0("\\rowcolor{", colorRows, "} ")

  digitsFormat <- c(0, 0, 3, 3, 3, 0) #Include a dummy at the beginning, for the row.names.
  textTable <- xtable(dsT, digits=digitsFormat, caption="Joint Frequencies for 97 Housemates")
  print(textTable, include.rownames=F, add.to.row=list(as.list(idRows), colorRows), NA.string="-", type=output_type)#, size="small")
}
PrintConditionalTable()
