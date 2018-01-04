rm(list=ls(all=TRUE)) #Clear the memory of variables from previous run. This is not called by knitr, because it's above the first chunk.

# ---- load-sources ------------------------------------------------------------
#Load any source files that contain/define functions, but that don't load any other types of variables
#   into memory.  Avoid side effects and don't pollute the global environment.
source("./utility/connectivity.R")

# ---- load-packages -----------------------------------------------------------
library(magrittr) #Pipes
# library(ggplot2) #For graphing
requireNamespace("RODBC")
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
sql_variable <- "
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

# ---- load-data ---------------------------------------------------------------
ds <- database_inventory()

channel            <- open_dsn_channel_rodbc()
ds_item            <- RODBC::sqlQuery(channel, sql_item    , stringsAsFactors=F)
ds_variable        <- RODBC::sqlQuery(channel, sql_variable, stringsAsFactors=F)
RODBC::odbcClose(channel); rm(channel, sql_item, sql_variable)

# ---- tweak-data --------------------------------------------------------------
ds_pretty <- ds %>%
  dplyr::mutate(
    row_count       = scales::comma(row_count),
    column_count    = scales::comma(column_count)
  )

ds_item <- ds_item %>%
  tibble::as_tibble()

ds_variable <- ds_variable %>%
  tibble::as_tibble() %>%
  dplyr::mutate(
    translate       = as.logical(translate)
  )

# ---- table ---------------------------------------------------------------
ds_pretty %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    align       = "llrr",
    digits      = 2,
    format      = "markdown"
  )



# ---- item ----------------------------------------------------------
ds_item %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    # align       = "r",
    format      = "markdown"
  )


# ---- variable ----------------------------------------------------------
ds_variable %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    # align       = "r",
    format      = "markdown"
  )
