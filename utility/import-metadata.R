# knitr::stitch_rmd(script="./utility/import-metadata.R", output="./stitched-output/utility/import-metadata.md") # dir.create(output="./stitched-output/utility/", recursive=T)
rm(list=ls(all=TRUE))  #Clear the variables from previous runs.

# ---- load-sources ------------------------------------------------------------
# Call `base::source()` on any repo file that defines functions needed below.  Ideally, no real operations are performed.

# ---- load-packages -----------------------------------------------------------
# Attach these package(s) so their functions don't need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
library(magrittr            , quietly=TRUE)
library(DBI                 , quietly=TRUE)

# Verify these packages are available on the machine, but their functions need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
requireNamespace("readr"                  )
requireNamespace("tidyr"                  )
requireNamespace("dplyr"                  ) #Avoid attaching dplyr, b/c its function names conflict with a lot of packages (esp base, stats, and plyr).
requireNamespace("testit"                 ) #For asserting conditions meet expected patterns.
# requireNamespace("RODBC") #For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.

# ---- declare-globals ---------------------------------------------------------
# Constant values that won't change.
directory_in              <- "data-public/metadata/tables"
schamea_name              <- "Metadata"

# col_types_tulsa <- readr::cols_only(
#   Month       = readr::col_date("%m/%d/%Y"),
#   FteSum      = readr::col_double(),
#   FmlaSum     = readr::col_integer()
# )

# ---- load-data ---------------------------------------------------------------


lst_ds <- directory_in %>%
  list.files(pattern="*.csv", full.names=T) %>%
  purrr::map(readr::read_csv)

rm(directory_in) # rm(col_types_tulsa)

lst_ds %>%
  purrr::walk(print)

lst_ds %>%
  purrr::map("name")

# ---- tweak-data --------------------------------------------------------------
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call ato `dplyr::rename()`.

# ---- verify-values -----------------------------------------------------------
# # Sniff out problems
# testit::assert("The month value must be nonmissing & since 2000", all(!is.na(ds$month) & (ds$month>="2012-01-01")))
# testit::assert("The county_id value must be nonmissing & positive.", all(!is.na(ds$county_id) & (ds$county_id>0)))
# testit::assert("The county_id value must be in [1, 77].", all(ds$county_id %in% seq_len(77L)))
# testit::assert("The region_id value must be nonmissing & positive.", all(!is.na(ds$region_id) & (ds$region_id>0)))
# testit::assert("The region_id value must be in [1, 20].", all(ds$region_id %in% seq_len(20L)))
# testit::assert("The `fte` value must be nonmissing & positive.", all(!is.na(ds$fte) & (ds$fte>=0)))
# # testit::assert("The `fmla_hours` value must be nonmissing & nonnegative", all(is.na(ds$fmla_hours) | (ds$fmla_hours>=0)))
#
# testit::assert("The County-month combination should be unique.", all(!duplicated(paste(ds$county_id, ds$month))))
# testit::assert("The Region-County-month combination should be unique.", all(!duplicated(paste(ds$region_id, ds$county_id, ds$month))))
# table(paste(ds$county_id, ds$month))[table(paste(ds$county_id, ds$month))>1]

# ---- specify-columns-to-upload -----------------------------------------------
# dput(colnames(ds)) # Print colnames for line below.
# columns_to_write <- c("county_month_id", "county_id", "month", "fte", "fte_approximated", "region_id")
# ds_slim <- ds %>%
#   dplyr::select_(.dots=columns_to_write) %>%
#   dplyr::mutate(
#     fte_approximated <- as.integer(fte_approximated)
#   )
# ds_slim
#
# rm(columns_to_write)


# ---- upload-to-db ----------------------------------------------------------
