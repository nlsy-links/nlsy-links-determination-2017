open_dsn_channel_odbc <- function( ) {
  requireNamespace("odbc")

  channel <- DBI::dbConnect(
    drv   = odbc::odbc(),
    dsn   = "local-nlsy-links"
  )
  testit::assert("The ODBC channel should open successfully.", exists("channel"))

  return( channel )
}
# channel <- open_dsn_channel_odbc()
# DBI::dbDisconnect(channel); rm(channel)

open_dsn_channel_rodbc <- function( ) {
  requireNamespace("RODBC")

  channel <- RODBC::odbcConnect(
    # Uses Trusted/integrated authentication
    dsn   = "local-nlsy-links"
    # dsn = "BeeNlsLinks",
    # uid = "NlsyReadWrite",
    # pwd = "nophi"
  )
  testit::assert("The ODBC channel should open successfully.", channel != -1L)

  return( channel )
}
# channel <- open_dsn_channel()
# RODBC::odbcClose(channel); rm(channel)


database_inventory <- function( ) {
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

  channel            <- open_dsn_channel_rodbc()
  ds    <- RODBC::sqlQuery(channel, sql_table, stringsAsFactors=F)
  # ds_row_count       <- RODBC::sqlTables(channel)
  RODBC::odbcClose(channel); rm(channel, sql_table)

  return( ds )
}
