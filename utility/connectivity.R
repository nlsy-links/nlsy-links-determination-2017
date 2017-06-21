open_dsn_channel <- function( ) {
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
