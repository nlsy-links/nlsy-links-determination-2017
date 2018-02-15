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
palette_conflict <- list("good"="#a6e8a1", "soso"="#ffeed1", "bad"="#ff9f6e", "null"="#7c9fb0") #http://colrd.com/image-dna/24445/

determine_row_good <- function( d_t ) { # DetermineGoodRowIDs(ds)
  return( d_t$RImplicit==d_t$RExplicit )
}
determine_row_soso <- function( d_t ) { # DetermineGoodRowIDs(ds)
  return( (dsT$RImplicit==.375 | is.na(dsT$RImplicit)) & !is.na(dsT$RExplicit) )
}
determine_row_bad <- function( d_t ) { # DetermineGoodRowIDs(ds)
  integer(0)
  # return( abs(dsTable$RImplicit - dsTable$RExplicit) >= .25 )
}
determine_row_bad <- function( d_t ) { # DetermineGoodRowIDs(ds)
  is.na(dsT$RImplicit) & is.na(dsT$RExplicit)
  # return( abs(dsTable$RImplicit - dsTable$RExplicit) >= .25 )
}


DetermineGoodRowIDs <- function( d_t ) { # DetermineGoodRowIDs(ds)
  return( which(determine_row_good(d_t)) )
}
DetermineSosoRowIDs <- function( d_t ) { # DetermineGoodRowIDs(ds)
  return( which(determine_row_soso(d_t)) )
}
DetermineBadRowIDs <- function( d_t ) { # DetermineBadRowIDs(ds)
  integer(0)
  # return( which(abs(dsTable$RImplicit - dsTable$RExplicit) >= .25) )
}
DetermineNullRowIDs <- function( d_t ) { # DetermineGoodRowIDs(ds)
  return( which(determine_row_bad(d_t)) )
}

# col_types <- c(# glue::collapse(paste0(colnames(dsRaw), '                     = "', purrr:::map_chr(dsRaw, class), '"'), sep = ",\n")
#   "AlgorithmVersion"                = "integer",       #   AlgorithmVersion                = readr::col_integer(),
#   "ExtendedID"                      = "integer",       #   ExtendedID                      = readr::col_integer(),
#   "SubjectTag_S1"                   = "integer",       #   SubjectTag_S1                   = readr::col_integer(),
#   "SubjectTag_S2"                   = "integer",       #   SubjectTag_S2                   = readr::col_integer(),
#   "SubjectID_S1"                    = "integer",       #   SubjectID_S1                    = readr::col_integer(),
#   "SubjectID_S2"                    = "integer",       #   SubjectID_S2                    = readr::col_integer(),
#   "MultipleBirthIfSameSex"          = "integer",       #   MultipleBirthIfSameSex          = readr::col_integer(),
#   "IsMz"                            = "integer",       #   IsMz                            = readr::col_integer(),
#   "SameGeneration"                  = "integer",       #   SameGeneration                  = readr::col_integer(),
#   "RosterAssignmentID"              = "integer",       #   RosterAssignmentID              = readr::col_integer(),
#   "RRoster"                         = "numeric",       #   RRoster                         = readr::col_double(),
#   "LastSurvey_S1"                   = "integer",       #   LastSurvey_S1                   = readr::col_integer(),
#   "LastSurvey_S2"                   = "integer",       #   LastSurvey_S2                   = readr::col_integer(),
#   "RImplicitPass1"                  = "numeric",       #   RImplicitPass1                  = readr::col_double(),
#   "RImplicit"                       = "numeric",       #   RImplicit                       = readr::col_double(),
#   "RImplicitSubject"                = "numeric",       #   RImplicitSubject                = readr::col_double(),
#   "RImplicitMother"                 = "numeric",       #   RImplicitMother                 = readr::col_double(),
#   "RExplicitOlderSibVersion"        = "numeric",       #   RExplicitOlderSibVersion        = readr::col_double(),
#   "RExplicitYoungerSibVersion"      = "numeric",       #   RExplicitYoungerSibVersion      = readr::col_double(),
#   "RExplicitPass1"                  = "numeric",       #   RExplicitPass1                  = readr::col_double(),
#   "RExplicit"                       = "numeric",       #   RExplicit                       = readr::col_double(),
#   "RPass1"                          = "numeric",       #   RPass1                          = readr::col_double(),
#   "R"                               = "numeric",       #   R                               = readr::col_double(),
#   "RFull"                           = "numeric",       #   RFull                           = readr::col_double(),
#   "RPeek"                           = "numeric"        #   RPeek                           = readr::col_double()
# )

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

dsRaw <- sqldf::read.csv.sql(
  file        = "data-public/derived/links-archive-2017-97.csv",
  sql         = sql,
  eol         = "\n"#,
  # colClasses  = col_types
)
# table(dsRaw$RRoster, useNA = "always")


# dsRaw2 <- readr::read_csv("data-public/derived/links-archive-2017-97.csv", col_types=col_types) %>%
#   dplyr::filter(AlgorithmVersion %in% 2:3)
# table(dsRaw2$RRoster, useNA="always")


# ---- tweak-data --------------------------------------------------------------
# glue::collapse(paste(colnames(dsRaw), "=", purrr:::map_chr(dsRaw, class)), sep = ",\n")
ds_description <- ds_description %>%
  tibble::as_tibble()

format_r_digits <- function( x ) sprintf("%0.3f", as.numeric(dplyr::na_if(x, "NA")))
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
idSosoRows <- DetermineSosoRowIDs(dsT)
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
  geom_text() +
  theme_light()


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
      # Eventual  = prettyNum(Eventual  , big.mark = ",", width=5)
      # Implicit  = scales::comma(Implicit),
      # Explicit  = scales::comma(Explicit),
      # Roster    = scales::comma(Roster  ),
      # Eventual  = scales::comma(Eventual),


      R         = dplyr::if_else(R=="NA", "--", R),
      # Implicit  = dplyr::coalesce(Implicit, "-"),
      # Explicit  = dplyr::coalesce(Explicit, "-"),
      # Roster    = dplyr::coalesce(Roster  , "-"),
      # Eventual  = dplyr::coalesce(Eventual, "-")

      # Implicit  = dplyr::if_else(Implicit=="NA", "-", Implicit),
      # Explicit  = dplyr::if_else(Explicit=="NA", "-", Explicit),
      # Roster    = dplyr::if_else(Roster  =="NA", "-", Roster  ),
      # Eventual  = dplyr::if_else(Eventual=="NA", "-", Eventual)

      Implicit  = dplyr::if_else(is.na(Implicit), "--", scales::comma(Implicit)),
      Explicit  = dplyr::if_else(is.na(Explicit), "--", scales::comma(Explicit)),
      Roster    = dplyr::if_else(is.na(Roster  ), "--", scales::comma(Roster  )),
      Eventual  = dplyr::if_else(is.na(Eventual), "--", scales::comma(Eventual))

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
    # format.args = list(big.mark=","),
    align       = "lrrrr",
    caption     = "Counts for 97 Housemates"
  )
dsPrevious %>%
  CreateMarginalTable() %>%
  knitr::kable(
    format      = output_type,
    # format.args = list(big.mark=","),
    align       = "lrrrr",
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



ds_condition <- ds %>%
  dplyr::select(Count, RImplicit, RExplicit, RRoster, Delta) %>%
  dplyr::mutate(
    Count         = scales::comma(Count),
    RImplicit     = dplyr::coalesce(dplyr::na_if(RImplicit, "NA"), "--"),
    RExplicit     = dplyr::coalesce(dplyr::na_if(RExplicit, "NA"), "--"),
    RRoster       = dplyr::coalesce(dplyr::na_if(RRoster  , "NA"), "--")
  ) %>%
  dplyr::arrange(desc(Count), Delta)

ds_condition %>%
  knitr::kable(
    format      = output_type,
    # format.args = list(big.mark=","),
    align       = "r",
    caption     = "Joint Frequencies for 97 Housemates"
  ) %>%
  kableExtra::kable_styling(
    full_width        = F,
    position          = "left",
    bootstrap_options = c("striped", "hover", "condensed", "responsive")
  ) %>%
  kableExtra::row_spec(DetermineGoodRowIDs(ds_condition), bold=F, background = palette_conflict$good) %>%
  kableExtra::row_spec(DetermineSosoRowIDs(ds_condition), bold=F, background = palette_conflict$soso) %>%
  kableExtra::row_spec(DetermineBadRowIDs( ds_condition), bold=T, background = palette_conflict$bad) %>%
  kableExtra::row_spec(DetermineNullRowIDs(ds_condition), bold=F, background = palette_conflict$null)

