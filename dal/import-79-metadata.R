# knitr::stitch_rmd(script="./dal/import-79-metadata.R", output="./stitched-output/dal/import-metadata.md") # dir.create(output="./stitched-output/dal/", recursive=T)
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
requireNamespace("odbc"                   ) #For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.
requireNamespace("OuhscMunge"             ) # remotes::install_github("OuhscBbmc/OuhscMunge")

# ---- declare-globals ---------------------------------------------------------
# Constant values that won't change.
directory_in              <- "data-public/metadata/tables-79"
study                     <- "79"

col_types_minimal <- readr::cols_only(
  ID                                  = readr::col_integer(),
  Label                               = readr::col_character(),
  Active                              = readr::col_logical(),
  Notes                               = readr::col_character()
)

# The order of this list matters.
#   - Tables are WRITTEN from top to bottom.
#   - Tables are DELETED from bottom to top.
lst_col_types <- list(
  ArchiveDescription = readr::cols_only(
    AlgorithmVersion                    = readr::col_integer(),
    Description                         = readr::col_character(),
    Date                                = readr::col_date()
  ),
  item = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character(),
    MinValue                            = readr::col_integer(),
    MinNonnegative                      = readr::col_integer(),
    MaxValue                            = readr::col_integer(),
    Active                              = readr::col_logical(),
    Notes                               = readr::col_character()
  ),
  LUExtractSource = col_types_minimal,
  LUMarkerEvidence = col_types_minimal,
  LUGender = col_types_minimal,
  LUMarkerType = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character(),
    Explicit                            = readr::col_integer(),
    Active                              = readr::col_logical(),
    Notes                               = readr::col_character()
  ),
  LUMultipleBirth = col_types_minimal,
  LURaceCohort = col_types_minimal,
  LURelationshipPath = col_types_minimal,
  LURosterGen1 = col_types_minimal,
  LUSurveySource = col_types_minimal,
  LUTristate = col_types_minimal,
  LUYesNo = col_types_minimal,
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
  #   ID                                = readr::col_integer(),
  #   AlgorithmVersion                  = readr::col_integer(),
  #   SubjectTag_S1                     = readr::col_integer(),
  #   SubjectTag_S2                     = readr::col_integer(),
  #   MultipleBirthIfSameSex            = readr::col_integer(),
  #   IsMz                              = readr::col_integer(),
  #   SameGeneration                    = readr::col_character(),
  #   RosterAssignmentID                = readr::col_character(),
  #   RRoster                           = readr::col_character(),
  #   LastSurvey_S1                     = readr::col_integer(),
  #   LastSurvey_S2                     = readr::col_integer(),
  #   RImplicitPass1                    = readr::col_double(),
  #   RImplicit                         = readr::col_double(),
  #   RImplicitSubject                  = readr::col_double(),
  #   RImplicitMother                   = readr::col_double(),
  #   RImplicit2004                     = readr::col_double(),
  #   RExplicitOldestSibVersion         = readr::col_double(),
  #   RExplicitYoungestSibVersion       = readr::col_double(),
  #   RExplicitPass1                    = readr::col_double(),
  #   RExplicit                         = readr::col_double(),
  #   RPass1                            = readr::col_double(),
  #   R                                 = readr::col_double(),
  #   RFull                             = readr::col_double(),
  #   RPeek                             = readr::col_character()
  # ),
  RosterGen1Assignment    = readr::cols_only(
    ID                                  = readr::col_integer(),
    ResponseLower                       = readr::col_integer(),
    ResponseUpper                       = readr::col_integer(),
    Freq                                = readr::col_integer(),
    Resolved                            = readr::col_integer(),
    R                                   = readr::col_double(),
    RBoundLower                         = readr::col_double(),
    RBoundUpper                         = readr::col_double(),
    SameGeneration                      = readr::col_integer(),
    ShareBiodad                         = readr::col_integer(),
    ShareBiomom                         = readr::col_integer(),
    ShareBiograndparent                 = readr::col_integer(),
    Inconsistent                        = readr::col_integer(),
    Notes                               = readr::col_character(),
    ResponseLowerLabel                  = readr::col_character(),
    ResponseUpperLabel                  = readr::col_character()
  ),
  variable = readr::cols_only(
    # ID                                  = readr::col_integer(),
    VariableCode                        = readr::col_character(),
    Item                                = readr::col_integer(),
    Generation                          = readr::col_integer(),
    ExtractSource                       = readr::col_integer(),
    SurveySource                        = readr::col_integer(),
    SurveyYear                          = readr::col_integer(),
    LoopIndex                           = readr::col_integer(),
    Translate                           = readr::col_integer(),
    Notes                               = readr::col_character(),
    Active                              = readr::col_integer(),
    Notes                               = readr::col_character()
  )
)

col_types_mapping <- readr::cols_only(
  table_name          = readr::col_character(),
  schema_name         = readr::col_character(),
  enum_name           = readr::col_character(),
  # enum_file         = readr::col_character(),
  c_sharp_type        = readr::col_character(),
  convert_to_enum     = readr::col_logical()
)

# ---- load-data ---------------------------------------------------------------
start_time <- Sys.time()

ds_mapping <- readr::read_csv(file.path(directory_in, "_mapping.csv"), col_types=col_types_mapping)
ds_mapping


ds_file <- lst_col_types %>%
  tibble::enframe(value = "col_types") %>%
  dplyr::mutate(
    path     = file.path(directory_in, paste0(name, ".csv")),
    # col_types = purrr::map(name, function(x) lst_col_types[[x]]),
    exists    = purrr::map_lgl(path, file.exists)
  ) %>%
  dplyr::select(name, path, dplyr::everything())
ds_file

testit::assert("All metadata files must exist.", all(ds_file$exists))

ds_entries <- ds_file %>%
  # dplyr::slice(15) %>%
  dplyr::select(name, path, col_types) %>%
  dplyr::mutate(
    entries = purrr::pmap(list(file=.$path, col_types=.$col_types), readr::read_csv, comment = "#")
  )
ds_entries

# d <- readr::read_csv("data-public/metadata/tables/variable_79.csv", col_types=lst_col_types$variable_79, comment = "#")
# readr::problems(d)
# ds_entries$entries[15]

ds_table <- database_inventory(study)
ds_table

rm(directory_in) # rm(col_types_tulsa)

# ---- tweak-data --------------------------------------------------------------
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call to `dplyr::rename()`.

ds_file <- ds_file %>%
  dplyr::left_join( ds_mapping, by=c("name"="table_name")) %>%
  dplyr::mutate(
    table_name    = paste0("tbl", name),
    sql_delete    = glue::glue("DELETE FROM {schema_name}.{table_name};")
    # table_name    = paste0(schema_name, ".tbl", name),
    # sql_delete    = paste0("DELETE FROM ", table_name)
  ) %>%
  dplyr::left_join(
    ds_entries %>%
      dplyr::select(name, entries)
    , by="name"
  )
rm(ds_entries)

ds_file$entries %>%
  purrr::walk(print)

# ds_file %>%
#   dplyr::group_by(name) %>%
#   dplyr::mutate(
#     a = purrr::map_int(entries, ~max(nchar(.), na.rm=T))
#   ) %>%
#   dplyr::ungroup() %>%
#   dplyr::pull(a)


# ds_file %>%
#   dplyr::select(name, entries) %>%
#   tibble::deframe() %>%
#   purrr::map(~max(nchar(.), na.rm=T))

# lst_ds %>%
#   purrr::map(nrow)
# lst_ds %>%
#   purrr::map(readr::spec)

ds_file$table_name
ds_file

# ---- convert-to-enum ---------------------------------------------------------
create_enum_body <- function( d ) {
  tab_spaces <- "    "
  labels   <- dplyr::if_else(      d$Active , d$Label, paste("//", d$Label))
  comments <- dplyr::if_else(is.na(d$Notes ), ""     , paste("//", d$Notes))

  paste0(sprintf("%s%-60s = %5s, %s\n", tab_spaces, labels, d$ID, comments), collapse="")
}

# ds_file %>%
#   dplyr::filter(name=="LURelationshipPath") %>%
#   dplyr::pull(entries)

ds_enum <- ds_file  %>%
  dplyr::filter(convert_to_enum) %>%
  dplyr::select(enum_name, entries, c_sharp_type) %>%
  dplyr::mutate(
    enum_header = paste0("\npublic enum ", .$enum_name, " {\n"),
    enum_body   = purrr::map_chr(.$entries, create_enum_body),
    enum_footer = "}\n",
    enum_cs     = paste0(enum_header, enum_body, enum_footer)
  ) %>%
  dplyr::select(-enum_header, -enum_body, -enum_footer)

ds_enum %>%
  dplyr::pull(enum_cs) %>%
  cat()

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


# ---- upload-to-db ----------------------------------------------------------
# lst_ds %>%
#   purrr::map(function(x)paste(names(x)))

ds_table_process <- ds_table %>%
  dplyr::filter(schema_name == "Process") %>%
  dplyr::mutate(
    # sql_truncate  = glue::glue("TRUNCATE TABLE {schema_name}.{table_name};")
    sql_truncate  = glue::glue("DELETE FROM {schema_name}.{table_name};")
  )

# Open channel
channel <- open_dsn_channel_odbc(study)
DBI::dbGetInfo(channel)

channel_rodbc <- open_dsn_channel_rodbc(study)
RODBC::odbcGetInfo(channel_rodbc)

# Clear process tables
delete_results_process <- ds_table_process$sql_truncate %>%
  purrr::set_names(ds_table_process$table_name) %>%
  rev() %>%
  purrr::map(DBI::dbGetQuery, conn=channel)
delete_results_process

# Delete metadata tables
# delete_result <- RODBC::sqlQuery(channel, "DELETE FROM [NlsLinks].[Metadata].[tblVariable]", errors=FALSE)
delete_results_metadata <- ds_file$sql_delete %>%
  purrr::set_names(ds_file$table_name) %>%
  rev() %>%
  purrr::map(DBI::dbGetQuery, conn=channel)

# DBI::dbGetQuery(conn=channel, ds_file$sql_delete[15])
delete_results_metadata

# d <- ds_file %>%
#   dplyr::select(table_name, entries) %>%
#   dplyr::filter(table_name=="Enum.tblLURosterGen1") %>%
#   tibble::deframe() %>%
#   .[[1]]

# d2 <- d[, 1:16]
# RODBC::sqlSave(channel, dat=d, tablename="Enum.tblLURosterGen1", safer=TRUE, rownames=FALSE, append=TRUE)

# Upload metadata tables
purrr::pmap_int(
  list(
    ds_file$entries,
    ds_file$table_name,
    ds_file$schema_name
  ),
  function( d, table_name, schema_name ) {
    # browser()
    # DBI::dbWriteTable(
    #   conn        = channel,
    #   name        = DBI::Id(schema=schema_name, table=table_name),
    #   value       = d,
    #   overwrite   = FALSE,
    #   append      = TRUE
    # )
    # DBI::dbWriteTable(
    #   conn    = channel,
    #   name    = table_name,
    #   schema  = schema_name,
    #   value   = d,
    #   append  = F
    # )
    RODBC::sqlSave(
      channel     = channel_rodbc,
      dat         = d,
      # tablename   = table_name,
      tablename   = paste0(schema_name, ".", table_name),
      safer       = TRUE,       # Don't keep the existing table.
      rownames    = FALSE,
      append      = TRUE
    )
  }
) #%>%
# purrr::set_names(ds_file$table_name)
# a <- ds_file$entries[[15]]
# table(a$ID)

# RODBC::sqlSave(
#   channel     = channel_rodbc,
#   dat         = ds_file$entries[[16]][, ],
#   tablename   = "Metadata.tblVariable",
#   safer       = TRUE,       # Don't keep the existing table.
#   rownames    = FALSE,
#   append      = TRUE
# )

# DBI::dbWriteTable(
#   conn        = channel,
#   name        = DBI::Id(catalog="NlsyLinks79", schema="Metadata", table="tblv"),
#   value       = ds_file$entries[[15]][1:10, 2],
#   overwrite   = FALSE,
#   append      = F
# )

# for( i in seq_len(nrow(ds_file)) ) {
#   message(glue::glue("Uploading from `{ basename(ds_file$path)[i]}` to `{ds_file$table_name[i]}`."))
#
#   d <- ds_file$entries[[i]]
#   print(d)
#
#   # RODBC::sqlQuery(channel, ds_extract$sql_truncate[i], errors=FALSE)
#
#   # d_peek <- RODBC::sqlQuery(channel, ds_extract$sql_select[i], errors=FALSE)
#   #
#   # missing_in_extract    <- setdiff(colnames(d_peek), colnames(d))
#   # missing_in_database   <- setdiff(colnames(d), colnames(d_peek))
#   #
#   # d_column <- tibble::tibble(
#   #   db        = colnames(d),
#   #   extract   = colnames(d_peek)
#   # ) %>%
#   #   dplyr::filter(db != extract)
#   #
#   # RODBC::sqlSave(
#   #   channel     = channel,
#   #   dat         = d,
#   #   tablename   = ds_extract$table_name[i],
#   #   safer       = TRUE,       # Don't keep the existing table.
#   #   rownames    = FALSE,
#   #   append      = TRUE
#   # ) %>%
#   #   print()
#
#   OuhscMunge::upload_sqls_rodbc(
#     d               = d,
#     table_name      = ds_file$table_name[i] ,
#     dsn_name        = "local-nlsy-links",
#     clear_table     = T,
#     create_table    = F
#   )
#
#
#   message(glue::glue("{format(object.size(d), units='MB')}"))
# }

# Close channel
DBI::dbDisconnect(channel); rm(channel)
RODBC::odbcClose(channel_rodbc); rm(channel_rodbc)

duration_in_seconds <- round(as.numeric(difftime(Sys.time(), start_time, units="secs")))
cat("`import-79-metadata.R` file completed by `", Sys.info()["user"], "` at ", strftime(Sys.time(), "%Y-%m-%d, %H:%M %z"), " in ",  duration_in_seconds, " seconds.", sep="")
