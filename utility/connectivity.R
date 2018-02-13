open_dsn_channel_odbc <- function( study ) {
  requireNamespace("odbc")

  checkmate::assert_subset(study, c("79", "97"))
  dsn <- dplyr::recode(study, "79"="local-nlsy-links-79", "97"="local-nlsy-links-97")

  channel <- DBI::dbConnect(
    drv   = odbc::odbc(),
    dsn   = dsn#"local-nlsy-links-79"
  )
  testit::assert("The ODBC channel should open successfully.", exists("channel"))

  return( channel )
}
# channel <- open_dsn_channel_odbc()
# DBI::dbDisconnect(channel); rm(channel)

open_dsn_channel_rodbc <- function( study ) {
  requireNamespace("RODBC")

  checkmate::assert_subset(study, c("79", "97"))
  dsn <- dplyr::recode(study, "79"="local-nlsy-links-79", "97"="local-nlsy-links-97")

  channel <- RODBC::odbcConnect(
    # Uses Trusted/integrated authentication
    dsn   = dsn #"local-nlsy-links-79"
    # dsn = "BeeNlsLinks",
    # uid = "NlsyReadWrite",
    # pwd = "nophi"
  )
  testit::assert("The ODBC channel should open successfully.", channel != -1L)

  return( channel )
}
# channel <- open_dsn_channel()
# RODBC::odbcClose(channel); rm(channel)


database_inventory <- function( study ) {
  checkmate::assert_subset(study, c("79", "97"))

  sql_table <- "
    ;WITH t_column AS (
      SELECT
        TABLE_SCHEMA     AS schema_name,
        TABLE_NAME       AS table_name,
        COUNT(*)         AS column_count
      FROM INFORMATION_SCHEMA.COLUMNS
      GROUP BY TABLE_SCHEMA, TABLE_NAME
    )
    SELECT
      s.Name                 AS schema_name,
      t.NAME                 AS table_name,
      p.rows                 AS row_count,
      c.column_count,
      SUM(a.total_pages) * 8 AS space_total_kb,
      SUM(a.used_pages ) * 8 AS space_used_kb
    FROM sys.tables t
      INNER JOIN sys.indexes i ON t.OBJECT_ID = i.object_id
      INNER JOIN sys.partitions p ON i.object_id = p.OBJECT_ID AND i.index_id = p.index_id
      INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
      LEFT  JOIN sys.schemas s ON t.schema_id = s.schema_id
      LEFT  JOIN t_column c ON t.name=c.table_name AND s.name=c.schema_name
    WHERE
      t.NAME NOT LIKE 'dt%'
      AND t.is_ms_shipped = 0
      AND i.OBJECT_ID > 255
    GROUP BY t.Name, s.Name, p.Rows, c.column_count
    ORDER BY s.Name, t.Name
  "

  # channel   <- open_dsn_channel_odbc()
  # ds        <- DBI::dbGetQuery(channel, sql_table)
  # DBI::dbDisconnect(channel);# rm(channel, sql_table)

  channel   <- open_dsn_channel_rodbc(study)
  ds        <- RODBC::sqlQuery(channel, sql_table, stringsAsFactors=F)
  # ds_row_count       <- RODBC::sqlTables(channel)
  RODBC::odbcClose(channel); rm(channel, sql_table)

  ds <- tibble::as_tibble(ds)

  return( ds )
}
# database_inventory("97")
