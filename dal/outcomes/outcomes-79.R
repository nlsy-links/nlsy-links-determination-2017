# knitr::stitch_rmd(script="./dal/import-79-raw.R", output="./stitched-output/dal/import-raw.md") # dir.create(output="./stitched-output/dal/", recursive=T)
rm(list=ls(all=TRUE))  #Clear the variables from previous runs.

# ---- load-sources ------------------------------------------------------------
source("./utility/connectivity.R")

# ---- load-packages -----------------------------------------------------------
# Attach these package(s) so their functions don't need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
library(magrittr            , quietly=TRUE)
# library(DBI                 , quietly=TRUE)

# Verify these packages are available on the machine, but their functions need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
requireNamespace("readr"        )
requireNamespace("tidyr"        )
requireNamespace("dplyr"        ) # Avoid attaching dplyr, b/c its function names conflict with a lot of packages (esp base, stats, and plyr).
requireNamespace("testit"       ) # For asserting conditions meet expected patterns.
requireNamespace("checkmate"    ) # For asserting conditions meet expected patterns. # devtools::install_github("mllg/checkmate")
requireNamespace("DBI"          )
requireNamespace("odbc"         )
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

# ---- load-data ---------------------------------------------------------------

channel             <- open_dsn_channel_odbc()
ds_outcome          <- DBI::dbGetQuery(channel, sql_outcome    )
ds_survey_time      <- DBI::dbGetQuery(channel, sql_survey_time)
DBI::dbDisconnect(channel); rm(channel, sql_outcome, sql_survey_time)

# ---- tweak-data --------------------------------------------------------------
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call ato `dplyr::rename()`.
ds_survey_time <- ds_survey_time %>%
  tibble::as_tibble()

ds <- ds_outcome %>%
  tibble::as_tibble() %>%
  dplyr::mutate(
    item_label    = OuhscMunge::snake_case(item_label)
  ) %>%
  tidyr::spread(key=item_label, value=value) %>%
  dplyr::left_join(ds_survey_time, by=c("subject_tag", "survey_year")) %>%
  dplyr::mutate(
    afqt_scaled_gen1   = gen1_afqt_scaled3_decimals / 1000
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


# ---- verify-values -----------------------------------------------------------
# Sniff out problems
# summary(ds)
checkmate::assert_integer(ds$subject_tag          , lower=         100L   , upper=1268600L, any.missing=F, unique=F)
checkmate::assert_integer(ds$survey_year          , lower=        1979L   , upper=   2016L, any.missing=F, unique=F)
checkmate::assert_integer(ds$generation           , lower=           1L   , upper=      2L, any.missing=F)
checkmate::assert_numeric(ds$age                  , lower=           0    , upper=     40 , any.missing=T)
checkmate::assert_integer(ds$height_inches        , lower=          48L   , upper=     83L, any.missing=T)
checkmate::assert_integer(ds$weight_pounds        , lower=          47L   , upper=    400L, any.missing=T)
checkmate::assert_numeric(ds$afqt_scaled_gen1     , lower=           0    , upper=    100 , any.missing=T)
checkmate::assert_integer(ds$father_alive         , lower=          -3L   , upper=      1L, any.missing=T)

# ---- specify-columns-to-upload -----------------------------------------------
# dput(colnames(ds)) # Print colnames for line below.
columns_to_write <- c(
  "subject_tag", "survey_year", "generation", "age",
  "height_inches", "weight_pounds", "afqt_scaled_gen1",
  "father_alive"
  )
ds_slim <- ds %>%
  # dplyr::slice(1:100) %>%
  dplyr::select(!!columns_to_write)
ds_slim

rm(columns_to_write)
#
# # ---- save-to-disk ------------------------------------------------------------
# # If there's no PHI, a rectangular CSV is usually adequate, and it's portable to other machines and software.
# readr::write_csv(ds, path_out_unified)
# # readr::write_rds(ds, path_out_unified, compress="gz") # Save as a compressed R-binary file if it's large or has a lot of factors.
#
#
# # ---- save-to-db --------------------------------------------------------------
# # If there's no PHI, a local database like SQLite fits a nice niche if
# #   * the data is relational and
# #   * later, only portions need to be queried/retrieved at a time (b/c everything won't need to be loaded into R's memory)
#
# sql_create_tbl_county <- "
# CREATE TABLE `tbl_county` (
# county_id              INTEGER NOT NULL PRIMARY KEY,
# county_name            VARCHAR NOT NULL,
# region_id              INTEGER NOT NULL
# );"
#
# sql_create_tbl_te_month <- "
# CREATE TABLE `tbl_te_month` (
# county_month_id                    INTEGER NOT NULL PRIMARY KEY,
# county_id                          INTEGER NOT NULL,
# month                              VARCHAR NOT NULL,         -- There's no date type in SQLite.  Make sure it's ISO8601: yyyy-mm-dd
# fte                                REAL    NOT NULL,
# fte_approximated                   REAL    NOT NULL,
# month_missing                      INTEGER NOT NULL,         -- There's no bit/boolean type in SQLite
# fte_rolling_median_11_month        INTEGER, --  NOT NULL
#
# FOREIGN KEY(county_id) REFERENCES tbl_county(county_id)
# );"
#
# # Remove old DB
# if( file.exists(path_db) ) file.remove(path_db)
#
# # Open connection
# cnn <- DBI::dbConnect(drv=RSQLite::SQLite(), dbname=path_db)
# RSQLite::dbSendQuery(cnn, "PRAGMA foreign_keys=ON;") #This needs to be activated each time a connection is made. #http://stackoverflow.com/questions/15301643/sqlite3-forgets-to-use-foreign-keys
# dbListTables(cnn)
#
# # Create tables
# dbSendQuery(cnn, sql_create_tbl_county)
# dbSendQuery(cnn, sql_create_tbl_te_month)
# dbListTables(cnn)
#
# # Write to database
# dbWriteTable(cnn, name='tbl_county',              value=ds_county,        append=TRUE, row.names=FALSE)
# ds %>%
#   dplyr::mutate(
#     month               = strftime(month, "%Y-%m-%d"),
#     fte_approximated    = as.logical(fte_approximated),
#     month_missing       = as.logical(month_missing)
#   ) %>%
#   dplyr::select(county_month_id, county_id, month, fte, fte_approximated, month_missing, fte_rolling_median_11_month) %>%
#   dbWriteTable(value=., conn=cnn, name='tbl_te_month', append=TRUE, row.names=FALSE)
#
# # Close connection
# dbDisconnect(cnn)
#
# # # ---- upload-to-db ----------------------------------------------------------
# # If there's PHI, write to a central database server that authenticates users (like SQL Server).
# # (startTime <- Sys.time())
# # dbTable <- "Osdh.tblC1TEMonth"
# # channel <- RODBC::odbcConnect("te-example") #getSqlTypeInfo("Microsoft SQL Server") #;odbcGetInfo(channel)
# #
# # columnInfo <- RODBC::sqlColumns(channel, dbTable)
# # varTypes <- as.character(columnInfo$TYPE_NAME)
# # names(varTypes) <- as.character(columnInfo$COLUMN_NAME)  #varTypes
# #
# # RODBC::sqlClear(channel, dbTable)
# # RODBC::sqlSave(channel, ds_slim, dbTable, append=TRUE, rownames=FALSE, fast=TRUE, varTypes=varTypes)
# # RODBC::odbcClose(channel)
# # rm(columnInfo, channel, columns_to_write, dbTable, varTypes)
# # (elapsedDuration <-  Sys.time() - startTime) #21.4032 secs 2015-10-31
#
#
# #Possibly consider writing to sqlite (with RSQLite) if there's no PHI, or a central database if there is PHI.
