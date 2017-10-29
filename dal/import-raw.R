# knitr::stitch_rmd(script="./dal/import-raw.R", output="./stitched-output/dal/import-raw.md") # dir.create(output="./stitched-output/dal/", recursive=T)
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

# ---- declare-globals ---------------------------------------------------------
# Constant values that won't change.
directory_in              <- "data-unshared/raw"
columns_to_drop           <- c("A0002600", "Y2267000")

ds_extract <- tibble::tribble(
  ~table_name                       , ~file_name,
  "Extract.tblGen1Explicit"         , "nlsy79-gen1/Gen1Explicit.csv",
  "Extract.tblGen1Implicit"         , "nlsy79-gen1/Gen1Implicit.csv",
  "Extract.tblGen1Links"            , "nlsy79-gen1/Gen1Links.csv",
  "Extract.tblGen1Outcomes"         , "nlsy79-gen1/Gen1Outcomes.csv",
  "Extract.tblGen1GeocodeSanitized" , "nlsy79-gen1/Gen1GeocodeSanitized.csv",
  # "Process.tblLURosterGen1"         , "nlsy79-gen1/RosterGen1.csv",
  # tblGen1MzDzDistinction2010
  #
  "Extract.tblGen2FatherFromGen1"   , "nlsy79-gen2/Gen2FatherFromGen1.csv",
  "Extract.tblGen2ImplicitFather"   , "nlsy79-gen2/Gen2ImplicitFather.csv",
  "Extract.tblGen2Links"            , "nlsy79-gen2/Gen2Links.csv",
  "Extract.tblGen2LinksFromGen1"    , "nlsy79-gen2/Gen2LinksFromGen1.csv",
  "Extract.tblGen2OutcomesHeight"   , "nlsy79-gen2/Gen2OutcomesHeight.csv",
  "Extract.tblGen2OutcomesMath"     , "nlsy79-gen2/Gen2OutcomesMath.csv",
  "Extract.tblGen2OutcomesWeight"   , "nlsy79-gen2/Gen2OutcomesWeight.csv"
)

col_types_default <- readr::cols(
  .default    = readr::col_integer()
)

checkmate::assert_character(ds_extract$table_name       , min.chars=10, any.missing=F, unique=T)
checkmate::assert_character(ds_extract$file_name        , min.chars=10, any.missing=F, unique=T)

# ---- load-data ---------------------------------------------------------------

ds_extract <- ds_extract %>%
  dplyr::mutate(
    path            = file.path(directory_in, file_name),
    extract_exist   = file.exists(path),
    sql_select      = glue::glue("SELECT TOP(100) * FROM {table_name}"),
    sql_truncate    = glue::glue("TRUNCATE TABLE {table_name}")
  )
testit::assert("All files should be found.", all(ds_extract$extract_exist))

print(ds_extract, n=20)

# ---- tweak-data --------------------------------------------------------------

# ---- verify-values -----------------------------------------------------------
# Sniff out problems


# ---- specify-columns-to-upload -----------------------------------------------

# ---- upload-to-db ----------------------------------------------------------

channel <- open_dsn_channel()
RODBC::odbcGetInfo(channel)

for( i in seq_len(nrow(ds_extract)) ) { # i <- 1L
  message(glue::glue("Uploading from `{ds_extract$file_name[i]}` to `{ds_extract$table_name[i]}`."))

  d <- readr::read_csv(ds_extract$path[i], col_types=col_types_default)

  columns_to_drop_specific <- colnames(d) %>%
    intersect(columns_to_drop)
  # %>%
    # glue::glue("{.}")

  if( length(columns_to_drop_specific) >= 1L ) {
    d <- d %>%
      dplyr::select_(.dots=paste0("-", columns_to_drop_specific))
  }

  print(d)

  RODBC::sqlQuery(channel, ds_extract$sql_truncate[i], errors=FALSE)

  d_peek <- RODBC::sqlQuery(channel, ds_extract$sql_select[i], errors=FALSE)

  missing_in_extract    <- setdiff(colnames(d_peek), colnames(d))
  missing_in_database   <- setdiff(colnames(d), colnames(d_peek))

  d_column <- tibble::tibble(
    db        = colnames(d),
    extract   = colnames(d_peek)
  ) %>%
    dplyr::filter(db != extract)

  RODBC::sqlSave(
    channel     = channel,
    dat         = d,
    tablename   = ds_extract$table_name[i],
    safer       = TRUE,       # Don't keep the existing table.
    rownames    = FALSE,
    append      = TRUE
  ) %>%
  print()

  # OuhscMunge::upload_sqls_rodbc(
  #   d               = d,
  #   table_name      = ds_extract$table_name[i] ,
  #   dsn_name        = "local-nlsy-links",
  #   clear_table     = F,
  #   create_table    = T
  # )


  message(glue::glue("Tibble size: {format(object.size(d), units='MB')}"))
}

RODBC::odbcClose(channel); rm(channel)
