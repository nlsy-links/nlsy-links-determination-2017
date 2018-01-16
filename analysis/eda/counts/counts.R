rm(list=ls(all=TRUE)) #Clear the memory of variables from previous run. This is not called by knitr, because it's above the first chunk.

# ---- load-sources ------------------------------------------------------------
#Load any source files that contain/define functions, but that don't load any other types of variables
#   into memory.  Avoid side effects and don't pollute the global environment.
source("./utility/connectivity.R")

# ---- load-packages -----------------------------------------------------------
library(magrittr) #Pipes
# library(ggplot2) #For graphing
requireNamespace("odbc")
requireNamespace("dplyr")
requireNamespace("scales") #For formating values in graphs
requireNamespace("knitr") #For the kable function for tables

# ---- declare-globals ---------------------------------------------------------
options(show.signif.stars=F) #Turn off the annotations on p-values


sql_item <- "
  SELECT
  	[ID]
  	,[Label]
  	,[MinValue]
  	,[MinNonnegative]
  	,[MaxValue]
  	,[Active]
  	,[Notes]
  FROM [Metadata].[tblItem]
"
sql_variable_79 <- "
  SELECT
  -- v.ID                 AS variable_id,
  v.VariableCode       AS variable_code,
  v.Item               AS item_id,
  i.Label              AS item_label,
  v.Generation         AS generation,
  v.ExtractSource      AS extract_source_id,
  e.Label              AS extract_source_label,
  v.SurveySource       AS survey_source_id,
  s.Label              AS survey_source_label,
  v.SurveyYear         AS survey_year,
  v.LoopIndex          AS loop_index,
  v.Translate          AS translate,
  v.Active             AS variable_active,
  v.Notes              AS variable_notes
  FROM Metadata.tblVariable v
  INNER JOIN Enum.tblLUSurveySource  s      ON v.SurveySource       = s.ID
  INNER JOIN Enum.tblLUExtractSource e      ON v.ExtractSource      = e.ID
  LEFT OUTER JOIN Metadata.tblItem   i      ON v.Item               = i.ID
"
sql_variable_97 <- "
  SELECT
    -- v.ID                 AS variable_id,
    v.VariableCode       AS variable_code,
    v.Item               AS item_id,
    i.Label              AS item_label,
    v.ExtractSource      AS extract_source_id,
    e.Label              AS extract_source_label,
    --v.SurveySource       AS survey_source_id,
    v.SurveyYear         AS survey_year,
    v.LoopIndex1         AS loop_index_1,
    v.LoopIndex2         AS loop_index_2,
    v.Translate          AS translate,
    v.Active             AS variable_active,
    v.Notes              AS variable_notes
  FROM Metadata.tblVariable v
    INNER JOIN Enum.tblLUExtractSource e      ON v.ExtractSource      = e.ID
    LEFT OUTER JOIN Metadata.tblItem   i      ON v.Item               = i.ID
"

# ---- load-data ---------------------------------------------------------------
ds_79 <- database_inventory("79")

channel_79             <- open_dsn_channel_odbc("79")
ds_item_79             <- DBI::dbGetQuery(channel_79, sql_item    )
ds_variable_79         <- DBI::dbGetQuery(channel_79, sql_variable_79)
DBI::dbDisconnect(channel_79); rm(channel_79, sql_variable_79)

ds_97  <- database_inventory("97")

channel_97             <- open_dsn_channel_odbc("97")
ds_item_97             <- DBI::dbGetQuery(channel_97, sql_item    )
ds_variable_97         <- DBI::dbGetQuery(channel_97, sql_variable_97)
DBI::dbDisconnect(channel_97); rm(channel_97, sql_item, sql_variable_97)

# ---- tweak-data --------------------------------------------------------------


# ---- groom-79 ----------------------------------------------------------------
ds_pretty_79 <- ds_79 %>%
  dplyr::mutate(
    row_count       = scales::comma(row_count),
    column_count    = scales::comma(column_count),
    space_total_kb  = scales::comma(space_total_kb),
    space_used_kb   = scales::comma(space_used_kb )
  )

ds_item_79 <- ds_item_79 %>%
  tibble::as_tibble()

ds_variable_79 <- ds_variable_79 %>%
  tibble::as_tibble() %>%
  dplyr::mutate(
    translate       = as.logical(translate)
  )

# ---- groom-97 ----------------------------------------------------------------
ds_pretty_97 <- ds_97 %>%
  dplyr::mutate(
    row_count       = scales::comma(row_count),
    column_count    = scales::comma(column_count),
    space_total_kb  = scales::comma(space_total_kb),
    space_used_kb   = scales::comma(space_used_kb )
  )

ds_item_97 <- ds_item_97 %>%
  tibble::as_tibble()

ds_variable_97 <- ds_variable_97 %>%
  tibble::as_tibble() %>%
  dplyr::mutate(
    translate       = as.logical(translate)
  )

# ---- table-79-structure ---------------------------------------------------------------
ds_pretty_79 %>%
  dplyr::select(schema_name, table_name, row_count, column_count) %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    align       = "llrr",
    digits      = 2,
    format      = "markdown"
  )

# ---- item-79 ----------------------------------------------------------
ds_item_79 %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    # align       = "r",
    format      = "markdown"
  )

# ---- variable-79 ----------------------------------------------------------
ds_variable_79 %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    # align       = "r",
    format      = "markdown"
  )

# ---- table-79-size ---------------------------------------------------------------
ds_pretty_79 %>%
  dplyr::select(schema_name, table_name, space_total_kb, space_used_kb) %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    align       = "llrr",
    digits      = 2,
    format      = "markdown"
  )

# ---- table-97-structure ---------------------------------------------------------------
ds_pretty_97 %>%
  dplyr::select(schema_name, table_name, row_count, column_count) %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    align       = "llrr",
    digits      = 2,
    format      = "markdown"
  )

# ---- item-97 ----------------------------------------------------------
ds_item_97 %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    # align       = "r",
    format      = "markdown"
  )

# ---- variable-97 ----------------------------------------------------------
ds_variable_97 %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    # align       = "r",
    format      = "markdown"
  )

# ---- table-97-size ---------------------------------------------------------------
ds_pretty_97 %>%
  dplyr::select(schema_name, table_name, space_total_kb, space_used_kb) %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    align       = "llrr",
    digits      = 2,
    format      = "markdown"
  )

