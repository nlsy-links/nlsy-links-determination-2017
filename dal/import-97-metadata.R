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
requireNamespace("odbc"                   ) #For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.



# ---- declare-globals ---------------------------------------------------------
# Constant values that won't change.
config                    <- config::get()
directory_in              <- "data-public/metadata/tables-97"
study                     <- "97"
shallow_only              <- F   # If TRUE, update only the metadata tables that won't delete any other database tables.

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
  LUExtractSource         = col_types_minimal,
  LUMarkerEvidence        = col_types_minimal,
  LUGender                = col_types_minimal,
  LUMarkerType = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character(),
    Explicit                            = readr::col_integer(),
    Active                              = readr::col_logical(),
    Notes                               = readr::col_character()
  ),
  LUMultipleBirth         = col_types_minimal,
  LURaceCohort            = col_types_minimal,

  LURoster                = col_types_minimal,

  LUTristate              = col_types_minimal,
  LUYesNo                 = col_types_minimal,
  MzManual = readr::cols_only(
    ID                                  = readr::col_integer(),
    SubjectTag_S1                       = readr::col_integer(),
    SubjectTag_S2                       = readr::col_integer(),

    MultipleBirthIfSameSex              = readr::col_integer(),
    IsMz                                = readr::col_integer(),
    Undecided                           = readr::col_integer(),
    Related                             = readr::col_integer(),
    Notes                               = readr::col_character()
  ),
  RosterAssignment        = readr::cols_only(
    ID                                  = readr::col_integer(),
    ResponseLower                       = readr::col_integer(),
    ResponseUpper                       = readr::col_integer(),
    Freq                                = readr::col_integer(),
    Resolved                            = readr::col_integer(),
    R                                   = readr::col_double(),
    RBoundLower                         = readr::col_double(),
    RBoundUpper                         = readr::col_double(),
    SameGeneration                      = readr::col_double(),
    ShareBiodad                         = readr::col_integer(),
    ShareBiomom                         = readr::col_integer(),
    ShareBiograndparent                 = readr::col_integer(),
    Inconsistent                        = readr::col_integer(),
    Notes                               = readr::col_character(),
    ResponseLowerLabel                  = readr::col_character(),
    ResponseUpperLabel                  = readr::col_character()
  ),
  variable = readr::cols_only(
    # ID                                = readr::col_integer(),
    VariableCode                        = readr::col_character(),
    Item                                = readr::col_integer(),

    ExtractSource                       = readr::col_integer(),

    SurveyYear                          = readr::col_integer(),
    LoopIndex1                          = readr::col_integer(),
    LoopIndex2                          = readr::col_integer(),
    Translate                           = readr::col_integer(),
    Active                              = readr::col_integer(),
    Notes                               = readr::col_character(),
    QuestionName                        = readr::col_character(),
    VariableTitle                       = readr::col_character()
  )
)

col_types_mapping <- readr::cols_only(
  table_name          = readr::col_character(),
  schema_name         = readr::col_character(),
  enum_name           = readr::col_character(),
  # enum_file         = readr::col_character(),
  c_sharp_type        = readr::col_character(),
  convert_to_enum     = readr::col_logical(),
  shallow             = readr::col_logical()
)

ds_file <- lst_col_types %>%
  tibble::enframe(value = "col_types") %>%
  dplyr::mutate(
    path        = file.path(directory_in, paste0(name, ".csv")),
    exists      = purrr::map_lgl(path, file.exists)
    # col_types = purrr::map(name, function(x) lst_col_types[[x]]),
  ) %>%
  dplyr::select(name, path, dplyr::everything())
ds_file

# ---- load-data ---------------------------------------------------------------
start_time <- Sys.time()

ds_mapping <- readr::read_csv(file.path(directory_in, "_mapping.csv"), col_types=col_types_mapping)
ds_mapping

testit::assert("All metadata files must exist.", all(ds_file$exists))

ds_entries <- ds_file %>%
  # dplyr::slice(14) %>%
  dplyr::select(name, path, col_types) %>%
  dplyr::mutate(
    entries = purrr::pmap(list(file=.$path, col_types=.$col_types), readr::read_csv, comment = "#")
  )
ds_entries

# d <- readr::read_csv("data-public/metadata/tables/variable_97.csv", col_types=lst_col_types$variable_97, comment = "#")
# readr::problems(d)
# ds_entries$entries[15]

ds_table <- database_inventory(study)
ds_table

rm(directory_in) # rm(col_types_tulsa)

# ---- tweak-data --------------------------------------------------------------
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call to `dplyr::rename()`.
if( shallow_only ) {
  ds_mapping <- ds_mapping %>%
    dplyr::filter(.data$shallow)
}
ds_mapping

ds_file <- ds_file %>%
  dplyr::inner_join(ds_mapping, by=c("name"="table_name")) %>%
  dplyr::mutate(
    table_name    = paste0("tbl", name),
    sql_delete    = glue::glue("DELETE FROM {schema_name}.{table_name};")
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

# ---- verify-values-deep -----------------------------------------------------------
# Sniff out problems
if( !shallow_only ) {
  d_extract_source <- ds_file  %>%
    dplyr::filter(name=="LUExtractSource") %>%
    dplyr::pull(entries) %>%
    purrr::flatten_df()

  d_item <- ds_file  %>%
    dplyr::filter(name=="item") %>%
    dplyr::pull(entries) %>%
    purrr::flatten_df()

  checkmate::assert_integer(  d_item$ID           , lower=1, upper=2^15   , any.missing=F, unique=T)
  checkmate::assert_character(d_item$Label        , pattern="^\\w+"       , any.missing=F, unique=T)


  d_variable <- ds_file  %>%
    dplyr::filter(name=="variable") %>%
    dplyr::pull(entries) %>%
    purrr::flatten_df() %>%
    dplyr::mutate(
      item_found    = (ExtractSource %in% d_extract_source$ID),
      extract_found = (Item %in% d_item$ID),
      unique_index  = paste(Item, SurveyYear, LoopIndex1, LoopIndex2)
    ) %>%
    dplyr::group_by(unique_index) %>%
    dplyr::mutate(
      unique_index_violation  = (1L < n()),
      variables_codes         = paste(VariableCode, collapse = "; ")
    ) %>%
    dplyr::ungroup()


  pattern_unique_index <- "^\\d{1,5} \\d{4} \\d{1,2} \\d{1,2}$"
  checkmate::assert_character(d_variable$VariableCode                     , pattern="^[A-Z]\\d{7}$"            , any.missing=F, unique=T)
  checkmate::assert_integer(  d_variable$Item                             , lower=0    , any.missing=F)
  checkmate::assert_logical(  d_variable$item_found                                    , any.missing=F)
  testit::assert("All items referenced from the variables should be in the item table.", all(d_variable$item_found))
  testit::assert("All extract sources referenced from the variables should be in the item table.", all(d_variable$extract_found))
  checkmate::assert_character(d_variable$unique_index   , pattern=pattern_unique_index  , any.missing=F, unique=T)

  # d_variable %>%
  #   dplyr::filter(unique_index_violation)
  #
  # d_variable %>%
  #   dplyr::filter(!grepl(pattern_unique_index, unique_index))

  rm(d_item, d_variable)
}

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

if( !shallow_only ) {
  # Clear process tables
  delete_results_process <- ds_table_process$sql_truncate %>%
    purrr::set_names(ds_table_process$table_name) %>%
    rev() %>%
    purrr::map(DBI::dbGetQuery, conn=channel)
  delete_results_process
}

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
    message("Writing to table ", table_name)
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
# a <- ds_file$entries[[13]]
# table(a$ID)


# odbc::dbWriteTable(
#   conn    = channel,
#   name    = DBI::SQL("Metadata.tblRosterAssignment"),
#   # name    = "tblvariable_97",
#   # schema  = "Metadata",
#   value   = ds_file$entries[[13]],
#   append  = T
# )

# RODBC::sqlSave(
#   channel     = channel_rodbc,
#   dat         = ds_file$entries[[13]][1:1, ],
#   # tablename   = table_name,
#   tablename   = "Metadata.tblRosterAssignment",
#   safer       = TRUE,       # Don't keep the existing table.
#   rownames    = FALSE,
#   append      = TRUE
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

# Close channels
DBI::dbDisconnect(channel); rm(channel)
RODBC::odbcClose(channel_rodbc); rm(channel_rodbc)

duration_in_seconds <- round(as.numeric(difftime(Sys.time(), start_time, units="secs")))
cat("`import-97-metadata.R` file completed by `", Sys.info()["user"], "` at ", strftime(Sys.time(), "%Y-%m-%d, %H:%M %z"), " in ",  duration_in_seconds, " seconds.", sep="")
