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
  subdirectory        = readr::col_character()
)

# ---- load-data ---------------------------------------------------------------
ds_mapping <- readr::read_csv(file.path(directory_in, "_mapping.csv"), col_types=col_types_mapping)
ds_mapping


ds_file <- ds_mapping %>%
  dplyr::mutate(
    path          = file.path(directory_in, subdirectory, paste0(name, ".csv")),
    table_name    = paste0(schema_name, ".tbl", name),
    exists        = purrr::map_lgl(path, file.exists)
  ) %>%
  dplyr::select(name, path, dplyr::everything())
ds_file

testit::assert("All metadata files must exist.", all(ds_file$exists))
rm(ds_mapping)

ds_entries <- ds_file %>%
  dplyr::select(name, path) %>%
  dplyr::mutate(
    entries = purrr::map(.$path, readr::read_csv, col_types = col_types_extract)
  )
ds_entries
print(object.size(ds_entries), units="MB")


rm(directory_in) # rm(col_types_tulsa)

# ---- tweak-data --------------------------------------------------------------
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call ato `dplyr::rename()`.

# ds_file <- ds_file %>%
#   dplyr::mutate(
#     table_name    = paste0(schema_name, ".tbl", name)
#   ) %>%
#   dplyr::left_join(
#     ds_entries %>%
#       dplyr::select(name, entries)
#     , by="name"
#   )
# rm(ds_entries)

ds_entries %>%
  purrr::walk(print)

ds_file$table_name
ds_file


# ---- verify-values -----------------------------------------------------------
# Sniff out problems

# ---- specify-columns-to-upload -----------------------------------------------


# ---- upload-to-db ----------------------------------------------------------
# lst_ds %>%
#   purrr::map(function(x)paste(names(x)))

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

  d <- readr::read_csv(ds_file$path[[i]], col_types = col_types_extract)
  cat(ds_file$table_name[[i]], ":", ds_file$path[[i]], "\n")
  print(d)
  cat("\n")
}

# purrr::map_int(
#   ds_file$table_name,
#   ds_file$path,
#   function( table_name, path ) {
#
#     d <- readr::read_csv(path, col_types = col_types_extract)
#
#     RODBC::sqlSave(
#       channel     = channel,
#       dat         = d,
#       tablename   = table_name,
#       safer       = FALSE,       # Don't keep the existing table.
#       rownames    = FALSE,
#       append      = TRUE
#     )
#   }
# ) %>%
#   purrr::set_names(ds_file$table_name)

RODBC::odbcClose(channel); rm(channel)
