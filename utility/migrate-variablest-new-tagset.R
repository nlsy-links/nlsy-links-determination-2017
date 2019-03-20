# knitr::stitch_rmd(script="./manipulation/te-ellis.R", output="./stitched-output/manipulation/te-ellis.md") # dir.create("./stitched-output/manipulation/", recursive=T)
# For a brief description of this file see the presentation at
#   - slides: https://rawgit.com/wibeasley/RAnalysisSkeleton/master/documentation/time-and-effort-synthesis.html#/
#   - code: https://github.com/wibeasley/RAnalysisSkeleton/blob/master/documentation/time-and-effort-synthesis.Rpres
rm(list=ls(all=TRUE))  #Clear the variables from previous runs.

# ---- load-sources ------------------------------------------------------------
# Call `base::source()` on any repo file that defines functions needed below.  Ideally, no real operations are performed.

# ---- load-packages -----------------------------------------------------------
# Attach these package(s) so their functions don't need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
library(magrittr            , quietly=TRUE)
library(DBI                 , quietly=TRUE)

# Verify these packages are available on the machine, but their functions need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
requireNamespace("readr"        )
requireNamespace("tidyr"        )
requireNamespace("dplyr"        ) # Avoid attaching dplyr, b/c its function names conflict with a lot of packages (esp base, stats, and plyr).
requireNamespace("testit"       ) # For asserting conditions meet expected patterns/conditions.
requireNamespace("checkmate"    ) # For asserting conditions meet expected patterns/conditions. # remotes::install_github("mllg/checkmate")
requireNamespace("RSQLite"      ) # Lightweight database for non-PHI data.
# requireNamespace("RODBC"      ) # For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.
requireNamespace("OuhscMunge"   ) # remotes::install_github(repo="OuhscBbmc/OuhscMunge")

# ---- declare-globals ---------------------------------------------------------
# Constant values that won't change.
directory_in                    <- "data-public/metadata/tagsets-79"
path_in_tagset_new              <- file.path(directory_in, "Gen2FatherFromGen1Death.NLSY79" )
path_in_tagset_old              <- file.path(directory_in, "Gen2FatherFromGen1.NLSY79"      )
path_out_tagset_old             <- path_in_tagset_old                     # In this case, overwrite the tagset. Make sure things are committed to Git first

variables_to_duplicate          <- c("R0000100", "R0214800")            # For Nlsy79 Gen1
# variables_to_duplicate          <- c("C0000100", "C0000200")          # For Nlsy79 Gen2
# variables_to_duplicate          <- c("R0000100")                      # For Nlsy97


col_types <- readr::cols_only( # readr::spec_csv(path_in_oklahoma)
  `id`          = readr::col_character()
)

# ---- load-data ---------------------------------------------------------------
# Read the CSVs
ds_new <- readr::read_csv(path_in_tagset_new   , col_types=col_types, col_names="id")
ds_old <- readr::read_csv(path_in_tagset_old   , col_types=col_types, col_names="id")

rm(path_in_tagset_new, path_in_tagset_old)

ds_new
ds_old

# ---- tweak-data --------------------------------------------------------------
# OuhscMunge::column_rename_headstart(ds) #Spit out columns to help write call ato `dplyr::rename()`.
ds_to_duplicate <-
  tibble::tibble(
    id  = variables_to_duplicate
  )

row_count_new_start       <- nrow(ds_new)
row_count_old_start       <- nrow(ds_old)
row_count_duplicate       <- nrow(ds_to_duplicate)

# ---- anti-join ----------------------------------------------------------
ds_old <-
  ds_old %>%
  dplyr::anti_join(ds_new, by="id") %>%
  dplyr::union_all(
    ds_to_duplicate
  ) %>%
  dplyr::arrange(id)

# ---- verify-values -----------------------------------------------------------
# Sniff out problems
# OuhscMunge::verify_value_headstart(ds_old)
checkmate::assert_character(ds_new$id , any.missing=F , pattern="^[A-Z]\\d{7}$" , unique=T)
checkmate::assert_character(ds_old$id , any.missing=F , pattern="^[A-Z]\\d{7}$" , unique=T)

row_count_old_end         <- nrow(ds_old)
row_count_old_expected    <- row_count_old_start - row_count_new_start + row_count_duplicate

testit::assert(row_count_old_end == row_count_old_expected)

# ---- specify-columns-to-upload -----------------------------------------------
# dput(colnames(ds)) # Print colnames for line below.
columns_to_write <- c(
  "id"
)
ds_slim <-
  ds_old %>%
  dplyr::select(!!columns_to_write) %>%
  # dplyr::slice(1:100) %>%
  dplyr::mutate_if(is.logical, as.integer)       # Some databases & drivers need 0/1 instead of FALSE/TRUE.
ds_slim

rm(columns_to_write)

# ---- save-to-disk ------------------------------------------------------------
readr::write_csv(ds_slim, path_out_tagset_old, col_names=F)
