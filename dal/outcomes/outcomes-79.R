# knitr::stitch_rmd(script="./dal/outcomes/outcomes-79.R", output="./stitched-output/dal/outcomes/outcomes-79.md") # dir.create("./stitched-output/dal/outcomes/", recursive=T)
rm(list=ls(all=TRUE))  #Clear the variables from previous runs.

# ---- load-sources ------------------------------------------------------------
source("./utility/connectivity.R")

# ---- load-packages -----------------------------------------------------------
# Attach these package(s) so their functions don't need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
library(magrittr            , quietly=TRUE)

# Verify these packages are available on the machine, but their functions need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
requireNamespace("readr"        )
requireNamespace("tidyr"        )
requireNamespace("dplyr"        ) # Avoid attaching dplyr, b/c its function names conflict with a lot of packages (esp base, stats, and plyr).
requireNamespace("testit"       ) # For asserting conditions meet expected patterns.
requireNamespace("checkmate"    ) # For asserting conditions meet expected patterns. # devtools::install_github("mllg/checkmate")
requireNamespace("DBI"          )
requireNamespace("odbc"         )
requireNamespace("config"       )
requireNamespace("OuhscMunge"   ) # devtools::install_github(repo="OuhscBbmc/OuhscMunge")

# ---- declare-globals ---------------------------------------------------------
# Constant values that won't change.
sql_outcome <- "
  SELECT -- TOP (100000)
  	--o.ID
    o.SubjectTag      AS subject_tag
    --,s.Generation
    ,o.SurveyYear     AS survey_year
    --,ROUND(COALESCE(t.AgeCalculateYears, t.AgeSelfReportYears),1)     AS age
    --,o.Item
    ,i.Label          AS item_label
    ,o.Value          AS value
  FROM Process.tblOutcome o
    LEFT JOIN Metadata.tblItem i        ON o.Item       = i.ID
    --LEFT JOIN Process.tblSubject s      ON o.SubjectTag = s.SubjectTag
    --LEFT JOIN Process.tblSurveyTime t   ON (o.SubjectTag = t.SubjectTag) AND (o.SurveyYear = t.SurveyYear)
  ORDER BY o.SubjectTag, o.SurveyYear, i.Label
"
sql_survey_time <- "
  SELECT -- TOP (100000)
    t.SubjectTag          AS subject_tag
    ,t.SurveyYear         AS survey_year
    ,s.Generation         AS generation
    ,ROUND(COALESCE(t.AgeCalculateYears, t.AgeSelfReportYears),1)     AS age
  FROM Process.tblSurveyTime t
    LEFT JOIN Process.tblSubject s      ON t.SubjectTag = s.SubjectTag
  ORDER BY t.SubjectTag, t.SurveyYear
"
sql_subject_generation <- "
  SELECT
    SubjectTag    AS subject_tag,
    Generation    AS generation
  FROM Process.tblSubject
"

sql_algorithm_version_max <- "SELECT MAX(AlgorithmVersion) as version FROM Archive.tblRelatedValuesArchive"
path_out_subject_csv_raw                <- config::get("outcomes-79-subject-csv")
path_out_subject_rds                    <- config::get("outcomes-79-subject-rds")
path_out_subject_survey_csv_raw         <- config::get("outcomes-79-subject-survey-csv")
path_out_subject_survey_rds             <- config::get("outcomes-79-subject-survey-rds")

# ---- load-data ---------------------------------------------------------------

channel                   <- open_dsn_channel_odbc()
ds_outcome                <- DBI::dbGetQuery(channel, sql_outcome    )
ds_survey_time            <- DBI::dbGetQuery(channel, sql_survey_time)
ds_subject_generation     <- DBI::dbGetQuery(channel, sql_subject_generation)
ds_algorithm_version      <- DBI::dbGetQuery(channel, sql_algorithm_version_max)
DBI::dbDisconnect(channel); rm(channel, sql_outcome, sql_survey_time, sql_subject_generation, sql_algorithm_version_max)

# ---- tweak-data --------------------------------------------------------------
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call ato `dplyr::rename()`.
dim(ds_survey_time)
dim(ds_outcome)
dim(ds_subject_generation)
ds_algorithm_version

path_out_subject_csv        <- sprintf(path_out_subject_csv_raw         , ds_algorithm_version$version)
path_out_subject_survey_csv <- sprintf(path_out_subject_survey_csv_raw  , ds_algorithm_version$version)

ds_survey_time <- ds_survey_time %>%
  tibble::as_tibble()

ds_subject_generation <- ds_subject_generation %>%
  tibble::as_tibble()

ds_outcome <- ds_outcome %>%
  tibble::as_tibble() %>%
  dplyr::mutate(
    item_label          = OuhscMunge::snake_case(item_label),
    value               = dplyr::if_else(item_label=="gen1_afqt_scaled3_decimals", value / 1000, as.numeric(value)), # If the conversion is here, then the other variables must be double-precision floating points.
    item_label          = dplyr::recode(item_label, "gen1_afqt_scaled3_decimals"="afqt_scaled_gen1") # If the conversion is here, then the other variables must be double-precision floating points.
  )

# table(ds_outcome$item_label)

# ---- groom-subject -----------------------------------------------------------
ds_subject <- ds_outcome %>%
  dplyr::group_by(subject_tag, item_label) %>%
  dplyr::summarize(
    value  = OuhscMunge::first_nonmissing(value)
  ) %>%
  dplyr::ungroup() %>%
  tidyr::spread(key=item_label, value=value) %>%
  dplyr::left_join(ds_subject_generation, by="subject_tag") %>%
  dplyr::select(
    subject_tag,
    generation,
    height_inches   = gen1_height_inches,
    weight_pounds   = gen1_weight_pounds,
    afqt_scaled_gen1,
    father_alive    = gen2_c_father_alive
  )


# ---- groom-subject-year ------------------------------------------------------
ds_subject_survey <- ds_outcome %>%
  tidyr::spread(key=item_label, value=value) %>%
  dplyr::left_join(ds_survey_time, by=c("subject_tag", "survey_year")) %>%
  dplyr::mutate(
    # afqt_scaled_gen1   = gen1_afqt_scaled3_decimals / 1000
  ) %>%
  dplyr::select(
    subject_tag,
    survey_year,
    generation,
    age,
    height_inches   = gen1_height_inches,
    weight_pounds   = gen1_weight_pounds,
    afqt_scaled_gen1,
    father_alive    = gen2_c_father_alive
  )

ds_subject_survey

# ---- verify-values-ds_subject -----------------------------------------------------------
# Sniff out problems
summary(ds_subject)
checkmate::assert_integer(ds_subject$subject_tag          , lower=         100L   , upper=1268600L, any.missing=F, unique=F)
checkmate::assert_integer(ds_subject$generation           , lower=           1L   , upper=      2L, any.missing=F)
checkmate::assert_numeric(ds_subject$height_inches        , lower=          48L   , upper=     83L, any.missing=T)
checkmate::assert_numeric(ds_subject$weight_pounds        , lower=          47L   , upper=    400L, any.missing=T)
checkmate::assert_numeric(ds_subject$afqt_scaled_gen1     , lower=           0    , upper=    100 , any.missing=T)
checkmate::assert_numeric(ds_subject$father_alive         , lower=          -3L   , upper=      1L, any.missing=T)

# ---- verify-values-ds_subject_survey -----------------------------------------------------------
# Sniff out problems
summary(ds_subject_survey)
checkmate::assert_integer(ds_subject_survey$subject_tag          , lower=         100L   , upper=1268600L, any.missing=F, unique=F)
checkmate::assert_integer(ds_subject_survey$survey_year          , lower=        1979L   , upper=   2016L, any.missing=F, unique=F)
checkmate::assert_integer(ds_subject_survey$generation           , lower=           1L   , upper=      2L, any.missing=F)
checkmate::assert_numeric(ds_subject_survey$age                  , lower=           0    , upper=     40 , any.missing=T)
checkmate::assert_numeric(ds_subject_survey$height_inches        , lower=          48L   , upper=     83L, any.missing=T)
checkmate::assert_numeric(ds_subject_survey$weight_pounds        , lower=          47L   , upper=    400L, any.missing=T)
checkmate::assert_numeric(ds_subject_survey$afqt_scaled_gen1     , lower=           0    , upper=    100 , any.missing=T)
checkmate::assert_numeric(ds_subject_survey$father_alive         , lower=          -3L   , upper=      1L, any.missing=T)

# ---- specify-columns-to-upload-ds_subject -----------------------------------------------
# dput(colnames(ds_subject_survey)) # Print colnames for line below.
columns_to_write <- c(
  "subject_tag", "generation",
  "height_inches", "weight_pounds", "afqt_scaled_gen1",
  "father_alive"
)
ds_slim_subject <- ds_subject %>%
  # dplyr::slice(1:100) %>%
  dplyr::select(!!columns_to_write)
ds_slim_subject

rm(columns_to_write)

# ---- specify-columns-to-upload-ds_subject_survey -----------------------------------------------
# dput(colnames(ds_subject_survey)) # Print colnames for line below.
columns_to_write <- c(
  "subject_tag", "survey_year", "generation", "age",
  "height_inches", "weight_pounds", "afqt_scaled_gen1",
  "father_alive"
)
ds_slim_subject_survey <- ds_subject_survey %>%
  # dplyr::slice(1:100) %>%
  dplyr::select(!!columns_to_write)
ds_slim_subject_survey

rm(columns_to_write)

# ---- save-to-disk ------------------------------------------------------------
# The content of these two datasets are identical.  The first one is a plain-text csv.  The second one is a native R object.
readr::write_csv(ds_slim_subject, path_out_subject_csv)
readr::write_rds(ds_slim_subject, path_out_subject_rds, compress="xz")

# The content of these two datasets are identical.  The first one is a plain-text csv.  The second one is a native R object.
readr::write_csv(ds_slim_subject_survey, path_out_subject_survey_csv)
readr::write_rds(ds_slim_subject_survey, path_out_subject_survey_rds, compress="xz")
