# knitr::stitch_rmd(script="./dal/import-79-raw.R", output="./stitched-output/dal/import-raw.md") # dir.create(output="./stitched-output/dal/", recursive=T)
rm(list=ls(all=TRUE))  #Clear the variables from previous runs.

# ---- load-sources ------------------------------------------------------------
# Call `base::source()` on any repo file that defines functions needed below.  Ideally, no real operations are performed.
base::source("utility/connectivity.R")

# ---- load-packages -----------------------------------------------------------
# Attach these package(s) so their functions don't need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
library(magrittr            , quietly=TRUE)

# Verify these packages are available on the machine, but their functions need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
requireNamespace("glue"                   )
requireNamespace("readr"                  )
requireNamespace("tidyr"                  )
requireNamespace("tibble"                 )
requireNamespace("purrr"                  )
requireNamespace("dplyr"                  ) #Avoid attaching dplyr, b/c its function names conflict with a lot of packages (esp base, stats, and plyr).
requireNamespace("testit"                 ) #For asserting conditions meet expected patterns.
requireNamespace("RODBC"                  ) #For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.
requireNamespace("odbc"                   ) #For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.

# ---- declare-globals ---------------------------------------------------------
# Constant values that won't change.
study                     <- "97"
directory_in              <- "data-unshared/raw"
columns_to_drop           <- c("A0002600", "Y2267000")

ds_extract <- tibble::tribble(
  ~table_name_qualified             , ~file_name
  ,"Extract.tblDemographics"        , "nlsy97/97-demographics.csv"
  ,"Extract.tblRoster"              , "nlsy97/97-roster.csv"
  ,"Extract.tblSurveyTime"          , "nlsy97/97-survey-time.csv"
  ,"Extract.tblLinksExplicit"       , "nlsy97/97-links-explicit.csv"
  ,"Extract.tblLinksImplicit"       , "nlsy97/97-links-implicit.csv"
)

col_types_default <- readr::cols(
  .default    = readr::col_integer()
)

checkmate::assert_character(ds_extract$table_name_qualified , min.chars=10, any.missing=F, unique=T)
checkmate::assert_character(ds_extract$file_name            , min.chars=10, any.missing=F, unique=T)


sql_template_not_null <- " ALTER TABLE {table_name_qualified} ALTER COLUMN [R0000100] INTEGER NOT NULL"
sql_template_primary_key <- "
  ALTER TABLE {table_name_qualified} ADD CONSTRAINT
  	PK_{table_name} PRIMARY KEY CLUSTERED ( R0000100 )
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
"

# ---- load-data ---------------------------------------------------------------
ds_inventory <- database_inventory(study)

start_time <- Sys.time()

ds_extract <- ds_extract %>%
  dplyr::mutate(
    table_name      = sub("^Extract\\.(\\w+)$", "\\1", table_name_qualified),
    path            = file.path(directory_in, file_name),
    extract_exist   = file.exists(path),
    sql_select      = glue::glue("SELECT TOP(100) * FROM {table_name_qualified}"),
    sql_truncate    = glue::glue("TRUNCATE TABLE {table_name_qualified}"),
    sql_not_null    = glue::glue(sql_template_not_null),
    sql_primary_key = glue::glue(sql_template_primary_key)
  )
testit::assert("All files should be found.", all(ds_extract$extract_exist))

print(ds_extract, n=20)

# ---- tweak-data --------------------------------------------------------------
ds_inventory <- ds_inventory %>%
  dplyr::mutate(
    table_name_qualified  =  glue::glue("{schema_name}.{table_name}")
  )

# ---- verify-values -----------------------------------------------------------
# Sniff out problems


# ---- specify-columns-to-upload -----------------------------------------------

# ---- upload-to-db ----------------------------------------------------------

channel_odbc <- open_dsn_channel_odbc(study)
DBI::dbGetInfo(channel_odbc)

channel_rodbc <- open_dsn_channel_rodbc(study)

for( i in seq_len(nrow(ds_extract)) ) { # i <- 1L
  message(glue::glue("Uploading from `{ds_extract$file_name[i]}` to `{ds_extract$table_name_qualified[i]}`."))

  d <- readr::read_csv(ds_extract$path[i], col_types=col_types_default)

  columns_to_drop_specific <- colnames(d) %>%
    intersect(columns_to_drop)
  # %>%
    # glue::glue("{.}")

  if( length(columns_to_drop_specific) >= 1L ) {
    d <- d %>%
      dplyr::select_(.dots=paste0("-", columns_to_drop_specific))
  }


  # print(dim(d))
  # purrr::map_chr(d, class)
  print(d, n=20)

  if( ds_extract$table_name_qualified[i] %in% ds_inventory$table_name_qualified ) {
    #RODBC::sqlQuery(channel_odbc, ds_extract$sql_truncate[i], errors=FALSE)
    # d_peek <- RODBC::sqlQuery(channel_odbc, ds_extract$sql_select[i], errors=FALSE)

    DBI::dbGetQuery(channel_odbc, ds_extract$sql_truncate[i])

    d_peek <- DBI::dbGetQuery(channel_odbc, ds_extract$sql_select[i])
    peek <- colnames(d_peek)
    # peek <- DBI::dbListFields(channel_odbc, ds_extract$table_name_qualified[i])

    missing_in_extract    <- setdiff(peek       , colnames(d))
    missing_in_database   <- setdiff(colnames(d), peek       )

    # d_column <- tibble::tibble(
    #   db        = colnames(d),
    #   extract   = peek
    # ) %>%
    #   dplyr::filter(db != extract)

    # system.time({
    # DBI::dbWriteTable(
    #   conn    = channel_odbc,
    #   name    = DBI::SQL(ds_extract$table_name_qualified[i]),
    #   value   = d, #[, 1:10],
    #   # append  = T,
    #   overwrite = T
    # )
    # })

    system.time({
    RODBC::sqlSave(
      channel     = channel_rodbc,
      dat         = d,
      tablename   = ds_extract$table_name_qualified[i],
      safer       = TRUE,       # Don't keep the existing table.
      rownames    = FALSE,
      append      = TRUE
    ) %>%
    print()
    })

  } else {
    OuhscMunge::upload_sqls_rodbc(
      d               = d,
      # d               = d[1:100, ],
      table_name      = ds_extract$table_name_qualified[i] ,
      dsn_name        = "local-nlsy-links-97",
      clear_table     = F,
      create_table    = T
    )

    DBI::dbGetQuery(channel_odbc, ds_extract$sql_not_null[i])
    DBI::dbGetQuery(channel_odbc, ds_extract$sql_primary_key[i])

  }

  message(glue::glue("Tibble size: {format(object.size(d), units='MB')}"))
}
DBI::dbDisconnect(channel_odbc); rm(channel_odbc)
RODBC::odbcClose(channel_rodbc); rm(channel_rodbc)

duration_in_seconds <- round(as.numeric(difftime(Sys.time(), start_time, units="secs")))
cat("File completed by `", Sys.info()["user"], "` at ", strftime(Sys.time(), "%Y-%m-%d, %H:%M %z"), " in ",  duration_in_seconds, " seconds.", sep="")
