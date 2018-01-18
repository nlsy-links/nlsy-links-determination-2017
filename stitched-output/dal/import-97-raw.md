



This report was automatically generated with the R package **knitr**
(version 1.18).


```r
# knitr::stitch_rmd(script="./dal/import-79-raw.R", output="./stitched-output/dal/import-raw.md") # dir.create(output="./stitched-output/dal/", recursive=T)
rm(list=ls(all=TRUE))  #Clear the variables from previous runs.
```

```r
# Call `base::source()` on any repo file that defines functions needed below.  Ideally, no real operations are performed.
base::source("utility/connectivity.R")
```

```r
# Attach these package(s) so their functions don't need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
library(magrittr            , quietly=TRUE)

# Verify these packages are available on the machine, but their functions need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
requireNamespace("glue"                   )
requireNamespace("readr"                  )
requireNamespace("tidyr"                  )
requireNamespace("tibble"                 )
requireNamespace("purrr"                  )
requireNamespace("dplyr"                  ) #Avoid attaching dplyr, b/c its function names conflict with a lot of packages (esp base, stats, and plyr).
requireNamespace("testit"                 ) #For asserting conditions meet expected patterns.
requireNamespace("RODBC"                  ) #For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.
requireNamespace("odbc"                   ) #For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.
```

```r
# Constant values that won't change.
study                     <- "97"
directory_in              <- "data-unshared/raw/nlsy97"
columns_to_drop           <- c("A0002600", "Y2267000")

ds_extract <- tibble::tribble(
  ~table_name_qualified             , ~file_name_base
  ,"Extract.tblDemographics"        , "97-demographics"
  ,"Extract.tblRoster"              , "97-roster"
  ,"Extract.tblSurveyTime"          , "97-survey-time"
  ,"Extract.tblLinksExplicit"       , "97-links-explicit"
  ,"Extract.tblLinksImplicit"       , "97-links-implicit"
)

col_types_default <- readr::cols(
  .default    = readr::col_integer()
)

checkmate::assert_character(ds_extract$table_name_qualified , min.chars=10, any.missing=F, unique=T)
checkmate::assert_character(ds_extract$file_name_base       , min.chars=9 , any.missing=F, unique=T)


# sql_template_not_null <- " ALTER TABLE {table_name_qualified} ALTER COLUMN [R0000100] INTEGER NOT NULL"
# sql_template_not_null <- " ALTER TABLE {table_name_qualified} ALTER COLUMN [{variable_code}] INTEGER NOT NULL"
# sql_template_not_null <- "
#   ALTER TABLE {table_name_qualified} ALTER COLUMN [R0000100] INTEGER NOT NULL
#   ALTER TABLE {table_name_qualified} ALTER COLUMN [R0536300] INTEGER NOT NULL
# "

sql_template_primary_key <- "
  ALTER TABLE {table_name_qualified} ADD CONSTRAINT
  	PK_{table_name} PRIMARY KEY CLUSTERED ( R0000100 )
    WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
"
```

```r
ds_inventory <- database_inventory(study)

start_time <- Sys.time()

ds_extract <- ds_extract %>%
  dplyr::mutate(
    table_name      = sub("^Extract\\.(\\w+)$", "\\1", table_name_qualified),
    path_zip        = file.path(directory_in, paste0(file_name_base, ".zip")),
    name_csv        = paste0(file_name_base, ".csv"),
    # path_csv        = file.path(directory_in, name_csv),
    extract_exist   = file.exists(path_zip),
    sql_select      = glue::glue("SELECT TOP(100) * FROM {table_name_qualified}"),
    sql_truncate    = glue::glue("TRUNCATE TABLE {table_name_qualified}"),
    # sql_not_null    = glue::glue(sql_template_not_null),
    sql_primary_key = glue::glue(sql_template_primary_key)
  )
testit::assert("All files should be found.", all(ds_extract$extract_exist))

print(ds_extract, n=20)
```

```
## # A tibble: 5 x 9
##   table_name_qualified     file~ tabl~ path~ name~ extr~ sql_~ sql_~ sql_~
##   <chr>                    <chr> <chr> <chr> <chr> <lgl> <chr> <chr> <chr>
## 1 Extract.tblDemographics  97-d~ tblD~ data~ 97-d~ T     SELE~ TRUN~ "  A~
## 2 Extract.tblRoster        97-r~ tblR~ data~ 97-r~ T     SELE~ TRUN~ "  A~
## 3 Extract.tblSurveyTime    97-s~ tblS~ data~ 97-s~ T     SELE~ TRUN~ "  A~
## 4 Extract.tblLinksExplicit 97-l~ tblL~ data~ 97-l~ T     SELE~ TRUN~ "  A~
## 5 Extract.tblLinksImplicit 97-l~ tblL~ data~ 97-l~ T     SELE~ TRUN~ "  A~
```

```r
ds_extract %>%
  dplyr::select(table_name_qualified, path_zip) %>%
  print(n=20)
```

```
## # A tibble: 5 x 2
##   table_name_qualified     path_zip                                      
##   <chr>                    <chr>                                         
## 1 Extract.tblDemographics  data-unshared/raw/nlsy97/97-demographics.zip  
## 2 Extract.tblRoster        data-unshared/raw/nlsy97/97-roster.zip        
## 3 Extract.tblSurveyTime    data-unshared/raw/nlsy97/97-survey-time.zip   
## 4 Extract.tblLinksExplicit data-unshared/raw/nlsy97/97-links-explicit.zip
## 5 Extract.tblLinksImplicit data-unshared/raw/nlsy97/97-links-implicit.zip
```

```r
ds_inventory <- ds_inventory %>%
  dplyr::mutate(
    table_name_qualified  =  glue::glue("{schema_name}.{table_name}")
  )
```


```r
# Sniff out problems
```


```r
channel_odbc <- open_dsn_channel_odbc(study)
DBI::dbGetInfo(channel_odbc)
```

```
## $dbname
## [1] "NlsyLinks97"
## 
## $dbms.name
## [1] "Microsoft SQL Server"
## 
## $db.version
## [1] "13.00.4206"
## 
## $username
## [1] "dbo"
## 
## $host
## [1] ""
## 
## $port
## [1] ""
## 
## $sourcename
## [1] "local-nlsy-links-97"
## 
## $servername
## [1] "GIMBLE\\EXPRESS_2016"
## 
## $drivername
## [1] "msodbcsql13.dll"
## 
## $odbc.version
## [1] "03.80.0000"
## 
## $driver.version
## [1] "14.00.1000"
## 
## $odbcdriver.version
## [1] "03.80"
## 
## $supports.transactions
## [1] TRUE
## 
## attr(,"class")
## [1] "Microsoft SQL Server" "driver_info"          "list"
```

```r
channel_rodbc <- open_dsn_channel_rodbc(study)

for( i in seq_len(nrow(ds_extract)) ) { # i <- 1L
# for( i in 1 ) { # i <- 1L
  message(glue::glue("Uploading from `{ds_extract$path_zip[i]}` to `{ds_extract$table_name_qualified[i]}`."))

  # Create temp zip file
  temp_directory  <- tempdir()
  temp_csv        <- file.path(temp_directory, ds_extract$name_csv[i])
  utils::unzip(ds_extract$path_zip[i], files=ds_extract$name_csv[i], exdir=temp_directory)
  if( !file.exists(temp_csv) ) stop("The decompressed csv, `", temp_csv, "` was not found.")

  # Read the temp csv, and delete it
  # d <- readr::read_csv(ds_extract$path_csv[i], col_types=col_types_default)
  d <- readr::read_csv(temp_csv, col_types=col_types_default)
  unlink(temp_csv)

  # Drop pre-specified columns from all extracts
  columns_to_drop_specific <- intersect(colnames(d), columns_to_drop)
  if( length(columns_to_drop_specific) >= 1L ) {
    d <- d %>%
      dplyr::select_(.dots=paste0("-", columns_to_drop_specific))
  }

  # Print diagnostic info
  # print(dim(d))
  # purrr::map_chr(d, class)
  print(d, n=20)

  # Write the table to teh database.  Different operations, depending if the table existings already.
  if( ds_extract$table_name_qualified[i] %in% ds_inventory$table_name_qualified ) {
    #RODBC::sqlQuery(channel_odbc, ds_extract$sql_truncate[i], errors=FALSE)
    # d_peek <- RODBC::sqlQuery(channel_odbc, ds_extract$sql_select[i], errors=FALSE)

    # Remove existing records
    DBI::dbGetQuery(channel_odbc, ds_extract$sql_truncate[i])

    # Compare columns in the database table and in the extract.
    d_peek <- DBI::dbGetQuery(channel_odbc, ds_extract$sql_select[i])
    peek <- colnames(d_peek)
    # peek <- DBI::dbListFields(channel_odbc, ds_extract$table_name_qualified[i])
    missing_in_extract    <- setdiff(peek       , colnames(d))
    missing_in_database   <- setdiff(colnames(d), peek       )
    testit::assert("All columns in the database should be in the extract.", length(missing_in_extract )==0L )
    testit::assert("All columns in the extract should be in the database.", length(missing_in_database)==0L)

    # Write to the database
    RODBC::sqlSave(
      channel     = channel_rodbc,
      dat         = d,
      tablename   = ds_extract$table_name_qualified[i],
      safer       = TRUE,       # Don't keep the existing table.
      rownames    = FALSE,
      append      = TRUE
    ) %>%
    print()

    # I'd like to use the odbc package, but it's still having problems with schema names.
    # system.time({
    # DBI::dbWriteTable(
    #   conn    = channel_odbc,
    #   name    = DBI::SQL(ds_extract$table_name_qualified[i]),
    #   value   = d, #[, 1:10],
    #   # append  = T,
    #   overwrite = T
    # )
    # })

  } else {
    # If the table doesn't already exist in the database, create it.
    OuhscMunge::upload_sqls_rodbc(
      d               = d,
      # d               = d[1:100, ],
      table_name      = ds_extract$table_name_qualified[i] ,
      dsn_name        = "local-nlsy-links-97",
      clear_table     = F,
      create_table    = T
    )

    colnames(d)

    sql_template_not_null <- "
      ALTER TABLE {table_name_qualified} ALTER COLUMN [{variable_code}] INTEGER NOT NULL
    "
    # sql_template_not_null <- "
    #   ALTER TABLE {%s} ALTER COLUMN [{%s}] INTEGER NOT NULL
    # "
    sql_not_null <- glue::glue(sql_template_not_null, table_name_qualified=ds_extract$table_name_qualified[i] , variable_code=colnames(d))
    sql_not_null <- paste(sql_not_null, collapse="; ")
    # sql_not_null <- sprintf(sql_template_not_null, table_name_qualified=ds_extract$table_name_qualified[i] , variable_code=colnames(d))
    # sql_not_null

    # Make the subject id the primary key.
    # DBI::dbGetQuery(channel_odbc, ds_extract$sql_not_null[i])
    DBI::dbGetQuery(channel_odbc, sql_not_null)
    DBI::dbGetQuery(channel_odbc, ds_extract$sql_primary_key[i])
  }

  message(glue::glue("Tibble size: {format(object.size(d), units='MB')}"))
}
```

```
## Uploading from `data-unshared/raw/nlsy97/97-demographics.zip` to `Extract.tblDemographics`.
```

```
## # A tibble: 8,984 x 8
##    R0000100 R0533400 R0536300 R0536401 R0536402 R1193000 R1235800 R1482600
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1        1        1        2        9     1981        1        1        4
##  2        2        1        1        7     1982        2        1        2
##  3        3        1        2        9     1983        3        1        2
##  4        4        1        2        2     1981        4        1        2
##  5        5        1        1       10     1982        6        1        2
##  6        6        1        2        1     1982        8        1        2
##  7        7        2        1        4     1983        8        1        2
##  8        8        1        2        6     1981        9        1        4
##  9        9        2        1       10     1982        9        1        4
## 10       10        3        1        3     1984        9        1        4
## 11       11        1        2        6     1982       10        1        2
## 12       12        1        1       10     1981       11        1        2
## 13       13        1        1       11     1984       12        1        2
## 14       14        1        1        7     1980       13        1        2
## 15       15        1        2        1     1983       14        1        2
## 16       16        1        1        2     1982       15        1        2
## 17       17        1        2       11     1981       16        1        2
## 18       18        1        1        2     1982       17        1        1
## 19       19        2        1        4     1984       17        1        1
## 20       20        1        1       12     1980       18        1        1
## # ... with 8,964 more rows
## [1] 1
```

```
## Tibble size: 0.3 Mb
```

```
## Uploading from `data-unshared/raw/nlsy97/97-roster.zip` to `Extract.tblRoster`.
```

```
## # A tibble: 8,984 x 464
##    R000~ R053~ R109~ R109~ R109~ R109~ R109~ R109~ R109~ R109~ R109~ R109~
##    <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int>
##  1     1     2     6     1     4     2     3     5    -4    -4    -4    -4
##  2     2     1     3     2     1     4    -4    -4    -4    -4    -4    -4
##  3     3     2     2     1    -4    -4    -4    -4    -4    -4    -4    -4
##  4     4     2     2     1    -4    -4    -4    -4    -4    -4    -4    -4
##  5     5     1     3     1     2     4    -4    -4    -4    -4    -4    -4
##  6     6     2     3     4     1     2     5    -4    -4    -4    -4    -4
##  7     7     1     3     4     1     2     5    -4    -4    -4    -4    -4
##  8     8     2     3     4     5     2     1    -4    -4    -4    -4    -4
##  9     9     1     3     4     5     2     1    -4    -4    -4    -4    -4
## 10    10     1     3     4     5     2     1    -4    -4    -4    -4    -4
## 11    11     2     2     1     3    -4    -4    -4    -4    -4    -4    -4
## 12    12     1     4     1     3     2    -4    -4    -4    -4    -4    -4
## 13    13     1     2     1     3     4     5    -4    -4    -4    -4    -4
## 14    14     1     3     1     2     4    -4    -4    -4    -4    -4    -4
## 15    15     2     3     1     2     4    -4    -4    -4    -4    -4    -4
## 16    16     1     2     1    -4    -4    -4    -4    -4    -4    -4    -4
## 17    17     2     4     1     2     3     5     6     7     8    -4    -4
## 18    18     1     6     2     1     3     4     5    -4    -4    -4    -4
## 19    19     1     6     2     1     3     4     5    -4    -4    -4    -4
## 20    20     1     2     3     4     1    -4    -4    -4    -4    -4    -4
## # ... with 8,964 more rows, and 452 more variables: R1098800 <int>,
## #   R1098900 <int>, R1099000 <int>, R1099100 <int>, R1099200 <int>,
## #   R1099300 <int>, R1101000 <int>, R1101100 <int>, R1101200 <int>,
## #   R1101300 <int>, R1101400 <int>, R1101500 <int>, R1101600 <int>,
## #   R1101700 <int>, R1101800 <int>, R1101900 <int>, R1102000 <int>,
## #   R1102100 <int>, R1102200 <int>, R1102300 <int>, R1102400 <int>,
## #   R1102500 <int>, R1102501 <int>, R1102600 <int>, R1102700 <int>,
## #   R1102800 <int>, R1102900 <int>, R1103000 <int>, R1103100 <int>,
## #   R1103200 <int>, R1103300 <int>, R1103400 <int>, R1103500 <int>,
## #   R1103600 <int>, R1103700 <int>, R1103800 <int>, R1103900 <int>,
## #   R1104000 <int>, R1104100 <int>, R1117000 <int>, R1117100 <int>,
## #   R1117200 <int>, R1117300 <int>, R1117400 <int>, R1117500 <int>,
## #   R1117600 <int>, R1117700 <int>, R1117800 <int>, R1117900 <int>,
## #   R1118000 <int>, R1118100 <int>, R1118200 <int>, R1118300 <int>,
## #   R1118400 <int>, R1118500 <int>, R1118600 <int>, R1118700 <int>,
## #   R1118800 <int>, R1118900 <int>, R1119000 <int>, R1119100 <int>,
## #   R1119200 <int>, R1119300 <int>, R1119400 <int>, R1119500 <int>,
## #   R1119600 <int>, R1119700 <int>, R1119800 <int>, R1119900 <int>,
## #   R1120000 <int>, R1120100 <int>, R1120200 <int>, R1120300 <int>,
## #   R1120400 <int>, R1120500 <int>, R1120600 <int>, R1120700 <int>,
## #   R1120800 <int>, R1120900 <int>, R1121000 <int>, R1121100 <int>,
## #   R1121200 <int>, R1121300 <int>, R1121400 <int>, R1121500 <int>,
## #   R1121600 <int>, R1121700 <int>, R1121800 <int>, R1121900 <int>,
## #   R1122000 <int>, R1122100 <int>, R1122200 <int>, R1122300 <int>,
## #   R1122400 <int>, R1122500 <int>, R1122600 <int>, R1122700 <int>,
## #   R1122800 <int>, R1122900 <int>, R1123000 <int>, ...
## [1] 1
```

```
## Tibble size: 16.1 Mb
```

```
## Uploading from `data-unshared/raw/nlsy97/97-survey-time.zip` to `Extract.tblSurveyTime`.
```

```
## # A tibble: 8,984 x 94
##    R000~ R000~ R000~ R000~ R053~ R054~ R054~ R054~ R119~ R119~ R119~ R120~
##    <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int>
##  1     1    23     7  1997     2    29     7  1997     1   190    15    23
##  2     2     2     5  1994     1   - 4    -4 -   4     2   178    14     2
##  3     3    23     4  1997     2    30     4  1997     3   163    13    23
##  4     4    17     2  1997     2    21     2  1997     4   192    15    17
##  5     5     7     4  1998     1     7     4  1998     6   186    15     7
##  6     6    23     9  1997     2    23     9  1997     8   188    15    23
##  7     7    23     9  1997     1    23     9  1997     8   173    14    23
##  8     8    25     4  1998     2    16     4  1998     9   202    16    25
##  9     9    24     4  1998     1    16     4  1998     9   186    15    24
## 10    10    22     4  1998     1    16     4  1998     9   169    14    22
## 11    11     6     7  1997     2     6     7  1997    10   181    15     6
## 12    12     8     7  1997     1    16     7  1997    11   189    15     8
## 13    13     8     9  1997     1   - 4    -4 -   4    12   154    12     8
## 14    14    17     8  1997     1    17     8  1997    13   205    17    17
## 15    15     1     4  1998     2   - 4    -4 -   4    14   183    15     1
## 16    16    13     4  1997     1    13     4  1997    15   182    15    13
## 17    17    25     3  1997     2    18     4  1997    16   184    15    25
## 18    18     9     3  1997     1     1     4  1997    17   181    15     9
## 19    19     9     3  1997     1     1     4  1997    17   155    12     9
## 20    20     1     4  1997     1     1     4  1997    18   196    16     1
## # ... with 8,964 more rows, and 82 more variables: R1209401 <int>,
## #   R1209402 <int>, R2553400 <int>, R2553500 <int>, R2568300 <int>,
## #   R2568301 <int>, R2568302 <int>, R3876200 <int>, R3876300 <int>,
## #   R3890300 <int>, R3890301 <int>, R3890302 <int>, R5453600 <int>,
## #   R5453700 <int>, R5472300 <int>, R5472301 <int>, R5472302 <int>,
## #   R7215900 <int>, R7216000 <int>, R7236100 <int>, R7236101 <int>,
## #   R7236102 <int>, S1531300 <int>, S1531400 <int>, S1550900 <int>,
## #   S1550901 <int>, S1550902 <int>, S2000900 <int>, S2001000 <int>,
## #   S2020800 <int>, S2020801 <int>, S2020802 <int>, S3801000 <int>,
## #   S3801100 <int>, S3822000 <int>, S3822001 <int>, S3822002 <int>,
## #   S5400900 <int>, S5401000 <int>, S5422000 <int>, S5422001 <int>,
## #   S5422002 <int>, S7501100 <int>, S7501200 <int>, S7524100 <int>,
## #   S7524101 <int>, S7524102 <int>, T0008400 <int>, T0008500 <int>,
## #   T0024500 <int>, T0024501 <int>, T0024502 <int>, T2011000 <int>,
## #   T2011100 <int>, T2019400 <int>, T2019401 <int>, T2019402 <int>,
## #   T3601400 <int>, T3601500 <int>, T3610000 <int>, T3610001 <int>,
## #   T3610002 <int>, T5201300 <int>, T5201400 <int>, T5210400 <int>,
## #   T5210401 <int>, T5210402 <int>, T6651200 <int>, T6651300 <int>,
## #   T6661400 <int>, T6661401 <int>, T6661402 <int>, T8123500 <int>,
## #   T8123600 <int>, T8132900 <int>, T8132901 <int>, T8132902 <int>,
## #   U0001700 <int>, U0001800 <int>, U0013200 <int>, U0013201 <int>,
## #   U0013202 <int>
## [1] "2018-01-17 20:13:29 CST"
```

```
## The table `Extract.tblSurveyTime` was written over dsn `local-nlsy-links-97` in 0.081 minutes.
```

```
## Tibble size: 3.3 Mb
```

```
## Uploading from `data-unshared/raw/nlsy97/97-links-explicit.zip` to `Extract.tblLinksExplicit`.
```

```
## # A tibble: 8,984 x 36
##    R000~ R053~ R082~ R082~ R082~ R082~ R082~ R082~ R082~ R082~ R082~ R082~
##    <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int>
##  1     1     2    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
##  2     2     1    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
##  3     3     2    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
##  4     4     2    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
##  5     5     1    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
##  6     6     2    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
##  7     7     1    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
##  8     8     2    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
##  9     9     1    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
## 10    10     1    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
## 11    11     2    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
## 12    12     1    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
## 13    13     1    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
## 14    14     1    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
## 15    15     2    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
## 16    16     1    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
## 17    17     2    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
## 18    18     1    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
## 19    19     1    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
## 20    20     1    -4    -4    -4    -4    -4    -4    -4    -4    -4    -4
## # ... with 8,964 more rows, and 24 more variables: R0823200 <int>,
## #   R0823300 <int>, R0823400 <int>, R0823500 <int>, R0823600 <int>,
## #   R0823700 <int>, R0823800 <int>, R0823900 <int>, R0824000 <int>,
## #   R0824100 <int>, R0824200 <int>, R0824300 <int>, R0824400 <int>,
## #   R0824500 <int>, R0824600 <int>, R0824700 <int>, R0824800 <int>,
## #   R0824900 <int>, R0825000 <int>, R0825100 <int>, R0825200 <int>,
## #   R0825300 <int>, R0825400 <int>, R1193000 <int>
## [1] 1
```

```
## Tibble size: 1.3 Mb
```

```
## Uploading from `data-unshared/raw/nlsy97/97-links-implicit.zip` to `Extract.tblLinksImplicit`.
```

```
## # A tibble: 8,984 x 43
##    R000~ R053~ R055~ R055~ R056~ R056~ R119~ R119~ R119~ R120~ R121~ R123~
##    <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int>
##  1     1     2     1     3    -4    -4     1     1     1     6    -4     1
##  2     2     1   - 4    -4    -4    -4     2     1     1     4    -4     1
##  3     3     2     1    -4     8    -4     3     1     0     2    -4     1
##  4     4     2     4    -4     8    -4     4     1     1     2    -4     1
##  5     5     1    11     7    -4    -4     6     1     1     4    -4     1
##  6     6     2     3    -4    -4    -4     8     2     1     5    -4     1
##  7     7     1     3    -4    -4    -4     8     2     1     5    -4     1
##  8     8     2     4     4    -4    -4     9     3     2     5    -4     1
##  9     9     1     4     4    -4    -4     9     3     2     5    -4     1
## 10    10     1     4     4    -4    -4     9     3     2     5    -4     1
## 11    11     2     6    -4     5    -4    10     1     1     3    -4     1
## 12    12     1     2     5    -4    -4    11     1     1     4    -4     1
## 13    13     1   - 4    -4    -4    -4    12     1     0     4    -4     1
## 14    14     1     4     5    -4    -4    13     1     1     4    -4     1
## 15    15     2   - 4    -4    -4    -4    14     1     0     4    -4     1
## 16    16     1     5    -4    -4    -4    15     1     1     2    -4     1
## 17    17     2     6    -4    -4    -4    16     1     1     8    -4     1
## 18    18     1     3    -4    -4    -4    17     2     0     6    -4     1
## 19    19     1     3    -4    -4    -4    17     2     0     6    -4     1
## 20    20     1     4    -4     5    -4    18     1     1     4    -4     1
## # ... with 8,964 more rows, and 31 more variables: R1302400 <int>,
## #   R1302500 <int>, R1482600 <int>, S0192900 <int>, S0193100 <int>,
## #   S0193500 <int>, S0193600 <int>, S0193800 <int>, S0193900 <int>,
## #   S5604900 <int>, S5605100 <int>, T3706800 <int>, T3706900 <int>,
## #   T4580500 <int>, T4580600 <int>, T4580700 <int>, T4580900 <int>,
## #   T4581100 <int>, Z0490900 <int>, Z0491000 <int>, Z0491100 <int>,
## #   Z0491200 <int>, Z0494800 <int>, Z0494900 <int>, Z0495000 <int>,
## #   Z0495100 <int>, Z0498500 <int>, Z0498700 <int>, Z0499200 <int>,
## #   Z0499400 <int>, Z0499500 <int>
## [1] 1
```

```
## Tibble size: 1.5 Mb
```

```r
# Diconnect the connections/channels.
DBI::dbDisconnect(channel_odbc); rm(channel_odbc)
RODBC::odbcClose(channel_rodbc); rm(channel_rodbc)

duration_in_seconds <- round(as.numeric(difftime(Sys.time(), start_time, units="secs")))
cat("File completed by `", Sys.info()["user"], "` at ", strftime(Sys.time(), "%Y-%m-%d, %H:%M %z"), " in ",  duration_in_seconds, " seconds.", sep="")
```

```
## File completed by `Will` at 2018-01-17, 20:13 -0600 in 30 seconds.
```

The R session information (including the OS info, R version and all
packages used):


```r
sessionInfo()
```

```
## R version 3.4.3 Patched (2017-12-05 r73849)
## Platform: x86_64-w64-mingw32/x64 (64-bit)
## Running under: Windows >= 8 x64 (build 9200)
## 
## Matrix products: default
## 
## locale:
## [1] LC_COLLATE=English_United States.1252 
## [2] LC_CTYPE=English_United States.1252   
## [3] LC_MONETARY=English_United States.1252
## [4] LC_NUMERIC=C                          
## [5] LC_TIME=English_United States.1252    
## 
## attached base packages:
## [1] stats     graphics  grDevices utils     datasets  methods   base     
## 
## other attached packages:
## [1] knitr_1.18   bindrcpp_0.2 magrittr_1.5
## 
## loaded via a namespace (and not attached):
##  [1] Rcpp_0.12.14          highr_0.6             pillar_1.0.1         
##  [4] compiler_3.4.3        plyr_1.8.4            bindr_0.1            
##  [7] tools_3.4.3           odbc_1.1.3            digest_0.6.13        
## [10] bit_1.1-12            memoise_1.1.0         evaluate_0.10.1      
## [13] tibble_1.4.1          checkmate_1.8.5       pkgconfig_2.0.1      
## [16] rlang_0.1.6           rstudioapi_0.7        DBI_0.7              
## [19] cli_1.0.0             yaml_2.1.16           withr_2.1.1.9000     
## [22] dplyr_0.7.4           stringr_1.2.0         devtools_1.13.4      
## [25] hms_0.4.0             bit64_0.9-7           rprojroot_1.3-2      
## [28] OuhscMunge_0.1.8.9005 glue_1.2.0            R6_2.2.2             
## [31] rmarkdown_1.8         tidyr_0.7.2           readr_1.1.1          
## [34] purrr_0.2.4           blob_1.1.0            backports_1.1.2      
## [37] scales_0.5.0.9000     RODBC_1.3-15          htmltools_0.3.6      
## [40] assertthat_0.2.0      testit_0.7.1          colorspace_1.3-2     
## [43] utf8_1.1.3            stringi_1.1.6         munsell_0.4.3        
## [46] markdown_0.8          crayon_1.3.4
```

```r
Sys.time()
```

```
## [1] "2018-01-17 20:13:40 CST"
```

