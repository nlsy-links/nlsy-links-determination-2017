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

sql_table <- "
  ;WITH
  t_row AS (
    SELECT
      sc.name        AS schema_name,
      ta.name        AS table_name,
      SUM(pa.rows)   AS row_count
    FROM sys.tables ta
      INNER JOIN sys.partitions pa ON pa.OBJECT_ID = ta.OBJECT_ID
      INNER JOIN sys.schemas sc ON ta.schema_id = sc.schema_id
    WHERE
      ta.is_ms_shipped = 0
      AND
      pa.index_id IN (1, 0)
    GROUP BY sc.name, ta.name
  ),
  t_column AS (
    SELECT
      TABLE_SCHEMA     AS schema_name,
      TABLE_NAME       AS table_name,
      COUNT(*)         AS column_count
    FROM INFORMATION_SCHEMA.COLUMNS
    GROUP BY TABLE_SCHEMA, TABLE_NAME
  )

  SELECT
    t_row.schema_name,
    t_row.table_name,
    t_row.row_count,
    t_column.column_count
  FROM t_row
    INNER JOIN t_column
      ON t_row.schema_name=t_column.schema_name AND t_row.table_name=t_column.table_name
  ORDER BY t_row.schema_name, t_row.table_name
"

# ---- load-data ---------------------------------------------------------------
channel            <- open_dsn_channel()
ds    <- RODBC::sqlQuery(channel, sql_table, stringsAsFactors=F)
# ds_row_count       <- RODBC::sqlTables(channel)
RODBC::odbcClose(channel); rm(channel, sql_table)

# ---- tweak-data --------------------------------------------------------------
ds_pretty <- ds %>%
  dplyr::mutate(
    row_count       = scales::comma(row_count),
    column_count    = scales::comma(column_count)
  )

# ---- table ---------------------------------------------------------------
ds_pretty %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    align       = "llrr",
    digits      = 2,
    format      = "markdown"
  )

