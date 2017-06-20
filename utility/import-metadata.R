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
schema_name               <- "Metadata"

lst_col_types <- list(
  Item = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character(),
    MinValue                            = readr::col_integer(),
    MinNonnegative                      = readr::col_integer(),
    MaxValue                            = readr::col_integer()
  ),
  LUExtractSource = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character()
  ),
  LUMarkerEvidence = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character()
  ),
  LUMarkerType = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character(),
    Explicit                            = readr::col_integer()
  ),
  LURelationshipPath = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character()
  ),
  LUSurveySource = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character()
  ),
  MzManual = readr::cols_only(
    ID                                  = readr::col_integer(),
    SubjectTag_S1                       = readr::col_integer(),
    SubjectTag_S2                       = readr::col_integer(),
    Generation                          = readr::col_integer(),
    MultipleBirthIfSameSex              = readr::col_integer(),
    IsMz                                = readr::col_integer(),
    Undecided                           = readr::col_integer(),
    Related                             = readr::col_integer(),
    Notes                               = readr::col_character()
  ),
  # RArchive = readr::cols_only(
  #   ID                                  = readr::col_integer(),
  #   AlgorithmVersion                    = readr::col_integer(),
  #   SubjectTag_S1                       = readr::col_integer(),
  #   SubjectTag_S2                       = readr::col_integer(),
  #   MultipleBirthIfSameSex              = readr::col_integer(),
  #   IsMz                                = readr::col_integer(),
  #   SameGeneration                      = readr::col_character(),
  #   RosterAssignmentID                  = readr::col_character(),
  #   RRoster                             = readr::col_character(),
  #   LastSurvey_S1                       = readr::col_integer(),
  #   LastSurvey_S2                       = readr::col_integer(),
  #   RImplicitPass1                      = readr::col_double(),
  #   RImplicit                           = readr::col_double(),
  #   RImplicitSubject                    = readr::col_double(),
  #   RImplicitMother                     = readr::col_double(),
  #   RImplicit2004                       = readr::col_double(),
  #   RExplicitOldestSibVersion           = readr::col_double(),
  #   RExplicitYoungestSibVersion         = readr::col_double(),
  #   RExplicitPass1                      = readr::col_double(),
  #   RExplicit                           = readr::col_double(),
  #   RPass1                              = readr::col_double(),
  #   R                                   = readr::col_double(),
  #   RFull                               = readr::col_double(),
  #   RPeek                               = readr::col_character()
  # ),
  Variable = readr::cols_only(
    ID                                  = readr::col_integer(),
    VariableCode                        = readr::col_character(),
    Item                                = readr::col_integer(),
    Generation                          = readr::col_integer(),
    ExtractSource                       = readr::col_integer(),
    SurveySource                        = readr::col_integer(),
    SurveyYear                          = readr::col_integer(),
    LoopIndex                           = readr::col_integer(),
    Translate                           = readr::col_integer(),
    Notes                               = readr::col_character()
  )
)


# ---- load-data ---------------------------------------------------------------
ds_file <- names(lst_col_types) %>%
  tibble::tibble(
    name = .
  ) %>%
  dplyr::mutate(
    path     = file.path(directory_in, paste0(name, ".csv")),
    # table_name = paste0(schema_name, ".tbl", name),
    table_name = paste0("tbl", name),
    col_types = purrr::map(name, function(x) lst_col_types[[x]]),
    exists    = purrr::map_lgl(path, file.exists)
  )

ds_file

testit::assert("All metadata files must exist.", all(ds_file$exists))

lst_ds <- ds_file %>%
  dplyr::select(
    file          = path,
    col_types
  ) %>%
  purrr::pmap(readr::read_csv) %>%
  purrr::set_names(nm=ds_file$table_name)

rm(directory_in) # rm(col_types_tulsa)

lst_ds %>%
  purrr::walk(print)

# lst_ds %>%
#   purrr::map(nrow)
# lst_ds %>%
#   purrr::map(readr::spec)

# ---- tweak-data --------------------------------------------------------------
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call ato `dplyr::rename()`.


# ---- verify-values -----------------------------------------------------------
# Sniff out problems
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
