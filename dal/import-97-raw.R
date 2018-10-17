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
directory_in              <- "data-unshared/raw/nlsy97"
columns_to_drop           <- c("A0002600", "Y2267000")

ds_extract <- tibble::tribble(
  ~table_name_qualified             , ~file_name_base
  ,"Extract.tblDemographics"        , "97-demographics"
  ,"Extract.tblRoster"              , "97-roster"
  ,"Extract.tblSurveyTime"          , "97-survey-time"
  ,"Extract.tblLinksExplicit"       , "97-links-explicit"
  ,"Extract.tblLinksImplicit"       , "97-links-implicit"
  ,"Extract.tblTwins"              , "97-twins"
)

col_types_default <- readr::cols(
  .default    = readr::col_integer()
)

checkmate::assert_character(ds_extract$table_name_qualified , min.chars=10, any.missing=F, unique=T)
checkmate::assert_character(ds_extract$file_name_base       , min.chars= 8, any.missing=F, unique=T)


# sql_template_not_null <- " ALTER TABLE {table_name_qualified} ALTER COLUMN [R0000100] INTEGER NOT NULL"
# sql_template_not_null <- " ALTER TABLE {table_name_qualified} ALTER COLUMN [{variable_code}] INTEGER NOT NULL"
# sql_template_not_null <- "
#   ALTER TABLE {table_name_qualified} ALTER COLUMN [R0000100] INTEGER NOT NULL
#   ALTER TABLE {table_name_qualified} ALTER COLUMN [R0536300] INTEGER NOT NULL
# "

sql_template_primary_key <- "
  ALTER TABLE {table_name_qualified} ADD CONSTRAINT
  	PK_{table_name} PRIMARY KEY CLUSTERED ( R0000100 )
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
"

# ---- load-data ---------------------------------------------------------------
start_time <- Sys.time()

ds_inventory <- database_inventory(study)

ds_extract <- ds_extract %>%
  dplyr::mutate(
    table_name      = sub("^Extract\\.(\\w+)$", "\\1", table_name_qualified),
    path_zip        = file.path(directory_in, paste0(file_name_base, ".zip")),
    name_csv        = paste0(file_name_base, ".csv"),
    # path_csv        = file.path(directory_in, name_csv),
    extract_exist   = file.exists(path_zip),
    sql_select      = glue::glue("SELECT TOP(100) * FROM {table_name_qualified}"),
    sql_truncate    = glue::glue("TRUNCATE TABLE {table_name_qualified}"),
    # sql_not_null    = glue::glue(sql_template_not_null),
    sql_primary_key = glue::glue(sql_template_primary_key)
  )
testit::assert("All files should be found.", all(ds_extract$extract_exist))

print(ds_extract, n=20)

ds_extract %>%
  dplyr::select(table_name_qualified, path_zip) %>%
  print(n=20)

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
# for( i in 1 ) { # i <- 1L
  message(glue::glue("Uploading from `{ds_extract$path_zip[i]}` to `{ds_extract$table_name_qualified[i]}`."))

  # Create temp zip file
  temp_directory  <- tempdir()
  temp_csv        <- file.path(temp_directory, ds_extract$name_csv[i])
  utils::unzip(ds_extract$path_zip[i], files=ds_extract$name_csv[i], exdir=temp_directory)
  if( !file.exists(temp_csv) ) stop("The decompressed csv, `", temp_csv, "` was not found.")

  # Read the temp csv, and delete it
  # d <- readr::read_csv(ds_extract$path_csv[i], col_types=col_types_default)
  d <- readr::read_csv(temp_csv, col_types=col_types_default)
  unlink(temp_csv)

  # Drop pre-specified columns from all extracts
  columns_to_drop_specific <- intersect(colnames(d), columns_to_drop)
  if( length(columns_to_drop_specific) >= 1L ) {
    d <- d %>%
      dplyr::select_(.dots=paste0("-", columns_to_drop_specific))
  }

  # Print diagnostic info
  # print(dim(d))
  # purrr::map_chr(d, class)
  print(d, n=20)

  # Write the table to the database.  Different operations, depending if the table existings already.
  if( ds_extract$table_name_qualified[i] %in% ds_inventory$table_name_qualified ) {
    #RODBC::sqlQuery(channel_odbc, ds_extract$sql_truncate[i], errors=FALSE)
    # d_peek <- RODBC::sqlQuery(channel_odbc, ds_extract$sql_select[i], errors=FALSE)

    # Remove existing records
    DBI::dbGetQuery(channel_odbc, ds_extract$sql_truncate[i])

    # Compare columns in the database table and in the extract.
    d_peek <- DBI::dbGetQuery(channel_odbc, ds_extract$sql_select[i])
    peek <- colnames(d_peek)
    # peek <- DBI::dbListFields(channel_odbc, ds_extract$table_name_qualified[i])
    missing_in_extract    <- setdiff(peek       , colnames(d))
    missing_in_database   <- setdiff(colnames(d), peek       )
    testit::assert("All columns in the database should be in the extract.", length(missing_in_extract )==0L)
    testit::assert("All columns in the extract should be in the database.", length(missing_in_database)==0L)

    # Write to the database
    RODBC::sqlSave(
      channel     = channel_rodbc,
      dat         = d,
      tablename   = ds_extract$table_name_qualified[i],
      safer       = FALSE,       # Don't keep the existing table.
      rownames    = FALSE,
      append      = TRUE
    ) %>%
    print()

    # I'd like to use the odbc package, but it's still having problems with schema names.
    # system.time({
    # DBI::dbWriteTable(
    #   conn    = channel_odbc,
    #   name    = DBI::SQL(ds_extract$table_name_qualified[i]),
    #   value   = d, #[, 1:10],
    #   # append  = T,
    #   overwrite = T
    # )
    # })

  } else {
    # If the table doesn't already exist in the database, create it.
    OuhscMunge::upload_sqls_rodbc(
      d               = d,
      # d               = d[1:100, ],
      table_name      = ds_extract$table_name_qualified[i] ,
      dsn_name        = "local-nlsy-links-97",
      clear_table     = F,
      create_table    = T
    )

    colnames(d)

    sql_template_not_null <- "
      ALTER TABLE {table_name_qualified} ALTER COLUMN [{variable_code}] INTEGER NOT NULL
    "
    # sql_template_not_null <- "
    #   ALTER TABLE {%s} ALTER COLUMN [{%s}] INTEGER NOT NULL
    # "
    sql_not_null <- glue::glue(sql_template_not_null, table_name_qualified=ds_extract$table_name_qualified[i] , variable_code=colnames(d))
    sql_not_null <- paste(sql_not_null, collapse="; ")
    # sql_not_null <- sprintf(sql_template_not_null, table_name_qualified=ds_extract$table_name_qualified[i] , variable_code=colnames(d))
    # sql_not_null


    # Make the subject id the primary key.
    # DBI::dbGetQuery(channel_odbc, ds_extract$sql_not_null[i])
    DBI::dbGetQuery(channel_odbc, sql_not_null)
    DBI::dbGetQuery(channel_odbc, ds_extract$sql_primary_key[i])
  }

  message(glue::glue("Tibble size: {format(object.size(d), units='MB')}"))
}
# Diconnect the connections/channels.
DBI::dbDisconnect(channel_odbc); rm(channel_odbc)
RODBC::odbcClose(channel_rodbc); rm(channel_rodbc)

duration_in_seconds <- round(as.numeric(difftime(Sys.time(), start_time, units="secs")))
cat("File completed by `", Sys.info()["user"], "` at ", strftime(Sys.time(), "%Y-%m-%d, %H:%M %z"), " in ",  duration_in_seconds, " seconds.", sep="")
