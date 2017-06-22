# knitr::stitch_rmd(script="./dal/import-raw.R", output="./stitched-output/dal/import-raw.md") # dir.create(output="./stitched-output/dal/", recursive=T)
rm(list=ls(all=TRUE))  #Clear the variables from previous runs.

# ---- load-sources ------------------------------------------------------------
# Call `base::source()` on any repo file that defines functions needed below.  Ideally, no real operations are performed.
base::source("utility/connectivity.R")

# ---- load-packages -----------------------------------------------------------
# Attach these package(s) so their functions don't need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
library(magrittr            , quietly=TRUE)

# Verify these packages are available on the machine, but their functions need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
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
schema_name               <- "Extract"

col_types_extract <- readr::cols(
  .default            = readr::col_integer()
)
col_types_mapping <- readr::cols_only(
  name                = readr::col_character(),
  subdirectory        = readr::col_character(),
  upload              = readr::col_logical()
)

# ---- load-data ---------------------------------------------------------------
ds_mapping <- readr::read_csv("data-public/metadata/tables/_mapping-unshared.csv", col_types=col_types_mapping)
ds_mapping


ds_file <- ds_mapping %>%
  dplyr::mutate(
    table_name    = paste0(schema_name, ".tbl", name),
    path          = file.path(directory_in, subdirectory, paste0(name, ".csv")),
    exists        = purrr::map_lgl(path, file.exists)
  ) %>%
  dplyr::filter(upload) %>%
  dplyr::select(name, exists, dplyr::everything())
ds_file

testit::assert("All metadata files must exist.", all(ds_file$exists))
rm(ds_mapping)

# ds_entries <- ds_file %>%
#   dplyr::select(name, path) %>%
#   dplyr::mutate(
#     entries = purrr::map(.$path, readr::read_csv, col_types = col_types_extract)
#   )
# ds_entries
# print(object.size(ds_entries), units="MB")


rm(directory_in) # rm(col_types_tulsa)

# ---- tweak-data --------------------------------------------------------------
# ds_entries %>%
#   purrr::walk(print)

ds_file$table_name
ds_file


# ---- verify-values -----------------------------------------------------------
# Sniff out problems

# ---- specify-columns-to-upload -----------------------------------------------


# ---- upload-to-db ----------------------------------------------------------
upload_start_time <- Sys.time()

channel <- open_dsn_channel()
RODBC::odbcGetInfo(channel)


# d <- ds_file %>%
#   dplyr::select(table_name, entries) %>%
#   dplyr::filter(table_name=="Enum.tblLURosterGen1") %>%
#   tibble::deframe() %>%
#   .[[1]]

# d2 <- d[, 1:16]
# RODBC::sqlSave(channel, dat=d, tablename="Enum.tblLURosterGen1", safer=TRUE, rownames=FALSE, append=TRUE)

for( i in seq_len(nrow(ds_file)) ) {

  cat(ds_file$table_name[[i]], ":", ds_file$path[[i]], "\n")

  d <- readr::read_csv(ds_file$path[[i]], col_types = col_types_extract)
  print(object.size(d), units="MB")
  print(d)
  cat("\n")

  # summary(d)
  # d2 <- d[1:3, ]

  result_save <- RODBC::sqlSave(
    channel     = channel,
    dat         = d,
    tablename   = ds_file$table_name[[i]],
    safer       = FALSE,       # Don't keep the existing table.
    rownames    = FALSE,
    append      = FALSE        # Toggle this to 'TRUE' the first time a table is uploaded.
  )

  cat("Save result:", result_save)
  cat("\n---------------------------------------------------------------\n")
}

RODBC::odbcClose(channel); rm(channel)
cat("upload_duration_in_seconds:", round(as.numeric(difftime(Sys.time(), upload_start_time, units="secs"))), "\n")
