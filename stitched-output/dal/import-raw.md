



This report was automatically generated with the R package **knitr**
(version 1.20).


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
study                     <- "79"
directory_in              <- "data-unshared/raw"
columns_to_drop           <- c("A0002600", "Y2267000")

ds_extract <- tibble::tribble(
  ~schema      , ~table_name                       , ~file_name
  ,"Extract"   , "tblGen1Explicit"         , "nlsy79-gen1/Gen1Explicit.csv"
  ,"Extract"   , "tblGen1Implicit"         , "nlsy79-gen1/Gen1Implicit.csv"
  ,"Extract"   , "tblGen1Links"            , "nlsy79-gen1/Gen1Links.csv"
  ,"Extract"   , "tblGen1Outcomes"         , "nlsy79-gen1/Gen1Outcomes.csv"
  ,"Extract"   , "tblGen1GeocodeSanitized" , "nlsy79-gen1/Gen1GeocodeSanitized.csv"
  # "Process.tblLURosterGen1"         , "nlsy79-gen1/RosterGen1.csv"
  # tblGen1MzDzDistinction2010
  #
  ,"Extract"   , "tblGen2FatherFromGen1"   , "nlsy79-gen2/Gen2FatherFromGen1.csv"
  ,"Extract"   , "tblGen2ImplicitFather"   , "nlsy79-gen2/Gen2ImplicitFather.csv"
  ,"Extract"   , "tblGen2Links"            , "nlsy79-gen2/Gen2Links.csv"
  ,"Extract"   , "tblGen2LinksFromGen1"    , "nlsy79-gen2/Gen2LinksFromGen1.csv"
  ,"Extract"   , "tblGen2OutcomesHeight"   , "nlsy79-gen2/Gen2OutcomesHeight.csv"
  ,"Extract"   , "tblGen2OutcomesMath"     , "nlsy79-gen2/Gen2OutcomesMath.csv"
  ,"Extract"   , "tblGen2OutcomesWeight"   , "nlsy79-gen2/Gen2OutcomesWeight.csv"


  ,"Extract"   , "tblGen2FatherFromGen1Death"   , "nlsy79-gen2/Gen2FatherFromGen1Death.csv"

  # "Extract"  , "tbl97Roster"             , "nlsy97/97-roster.csv"
)

col_types_default <- readr::cols(
  .default    = readr::col_integer()
)

checkmate::assert_character(ds_extract$table_name       , min.chars=10, any.missing=F, unique=T)
checkmate::assert_character(ds_extract$file_name        , min.chars=10, any.missing=F, unique=T)
```

```r
start_time <- Sys.time()

ds_extract <- ds_extract %>%
  dplyr::mutate(
    path                    = file.path(directory_in, file_name),
    extract_exist           = file.exists(path),
    table_name_qualified    = paste0(schema, ".", table_name),
    sql_select              = glue::glue("SELECT TOP(100) * FROM {table_name_qualified}"),
    sql_truncate            = glue::glue("TRUNCATE TABLE {table_name_qualified}")
  )
testit::assert("All files should be found.", all(ds_extract$extract_exist))

print(ds_extract, n=20)
```

```
## # A tibble: 13 x 8
##    schema table_name file_name path  extract_exist table_name_qual~
##    <chr>  <chr>      <chr>     <chr> <lgl>         <chr>           
##  1 Extra~ tblGen1Ex~ nlsy79-g~ data~ TRUE          Extract.tblGen1~
##  2 Extra~ tblGen1Im~ nlsy79-g~ data~ TRUE          Extract.tblGen1~
##  3 Extra~ tblGen1Li~ nlsy79-g~ data~ TRUE          Extract.tblGen1~
##  4 Extra~ tblGen1Ou~ nlsy79-g~ data~ TRUE          Extract.tblGen1~
##  5 Extra~ tblGen1Ge~ nlsy79-g~ data~ TRUE          Extract.tblGen1~
##  6 Extra~ tblGen2Fa~ nlsy79-g~ data~ TRUE          Extract.tblGen2~
##  7 Extra~ tblGen2Im~ nlsy79-g~ data~ TRUE          Extract.tblGen2~
##  8 Extra~ tblGen2Li~ nlsy79-g~ data~ TRUE          Extract.tblGen2~
##  9 Extra~ tblGen2Li~ nlsy79-g~ data~ TRUE          Extract.tblGen2~
## 10 Extra~ tblGen2Ou~ nlsy79-g~ data~ TRUE          Extract.tblGen2~
## 11 Extra~ tblGen2Ou~ nlsy79-g~ data~ TRUE          Extract.tblGen2~
## 12 Extra~ tblGen2Ou~ nlsy79-g~ data~ TRUE          Extract.tblGen2~
## 13 Extra~ tblGen2Fa~ nlsy79-g~ data~ TRUE          Extract.tblGen2~
## # ... with 2 more variables: sql_select <S3: glue>, sql_truncate <S3:
## #   glue>
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
## [1] "NlsyLinks79"
## 
## $dbms.name
## [1] "Microsoft SQL Server"
## 
## $db.version
## [1] "13.00.5081"
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
## [1] "local-nlsy-links-79"
## 
## $servername
## [1] "GIMBLE\\EXPRESS_2016"
## 
## $drivername
## [1] "msodbcsql17.dll"
## 
## $odbc.version
## [1] "03.80.0000"
## 
## $driver.version
## [1] "17.01.0000"
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

for( i in seq_len(nrow(ds_extract)) ) { # i <- 13L
  message(glue::glue("Uploading from `{ds_extract$file_name[i]}` to `{ds_extract$table_name_qualified[i]}`."))

  d <- readr::read_csv(ds_extract$path[i], col_types=col_types_default)

  columns_to_drop_specific <- colnames(d) %>%
    intersect(columns_to_drop)
  # %>%
    # glue::glue("{.}")

  if( length(columns_to_drop_specific) >= 1L ) {
    d <- d %>%
      dplyr::select_(.dots=paste0("-", columns_to_drop_specific))
  }

  # print(dim(d))
  # purrr::map_chr(d, class)
  print(d, n=20)

  #RODBC::sqlQuery(channel_odbc, ds_extract$sql_truncate[i], errors=FALSE)
  # d_peek <- RODBC::sqlQuery(channel_odbc, ds_extract$sql_select[i], errors=FALSE)

  DBI::dbGetQuery(channel_odbc, ds_extract$sql_truncate[i])

  d_peek <- DBI::dbGetQuery(channel_odbc, ds_extract$sql_select[i])
  peek <- colnames(d_peek)
  # peek <- DBI::dbListFields(channel_odbc, ds_extract$table_name_qualified[i])

  missing_in_extract    <- setdiff(peek       , colnames(d))
  missing_in_database   <- setdiff(colnames(d), peek       )

  # d_column <- tibble::tibble(
  #   db        = colnames(d),
  #   extract   = peek
  # ) %>%
  #   dplyr::filter(db != extract)

  # system.time({
  # DBI::dbWriteTable(
  #   conn    = channel_odbc,
  #   name    = DBI::SQL(ds_extract$table_name[i]),
  #   value   = d, #[, 1:10],
  #   # append  = T,
  #   overwrite = T
  # )
  # })

  system.time({
  RODBC::sqlSave(
    channel     = channel_rodbc,
    dat         = d,
    tablename   = ds_extract$table_name_qualified[i],
    safer       = FALSE,       # Don't keep the existing table.
    rownames    = FALSE,
    append      = TRUE
  ) %>%
  print()
  })

  # system.time({
  # DBI::dbWriteTable(
  #   conn        = channel_odbc,
  #   name        = DBI::Id(schema=ds_extract$schema[i], table=ds_extract$table_name[i]),
  #   value       = d,
  #   overwrite   = FALSE,
  #   append      = TRUE
  # ) %>%
  #     print()
  # })

  # OuhscMunge::upload_sqls_rodbc(
  #   d               = d[1:100, ],
  #   table_name      = ds_extract$table_name_qualified[i] ,
  #   dsn_name        = "local-nlsy-links-79",
  #   clear_table     = F,
  #   create_table    = T
  # )


  message(glue::glue("Tibble size: {format(object.size(d), units='MB')}"))
}
```

```
## Uploading from `nlsy79-gen1/Gen1Explicit.csv` to `Extract.tblGen1Explicit`.
```

```
## # A tibble: 12,686 x 96
##    R0000100 R0000149 R0000150 R0000151 R0000152 R0000153 R0000154 R0000155
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1        1        1       -4       -4       -4       -4       -4       -4
##  2        2        2       -4       -4       -4       -4       -4       -4
##  3        3        3        4        7       -4       -4       -4       -4
##  4        4        3        3        7       -4       -4       -4       -4
##  5        5        5        6        6       -4       -4       -4       -4
##  6        6        5        5        6       -4       -4       -4       -4
##  7        7        7       -4       -4       -4       -4       -4       -4
##  8        8        8       -4       -4       -4       -4       -4       -4
##  9        9        9       -4       -4       -4       -4       -4       -4
## 10       10       10       -4       -4       -4       -4       -4       -4
## 11       11       11       -4       -4       -4       -4       -4       -4
## 12       12       12       -4       -4       -4       -4       -4       -4
## 13       13       13       14        7       -4       -4       -4       -4
## 14       14       13       13        6       -4       -4       -4       -4
## 15       15       15       -4       -4       -4       -4       -4       -4
## 16       16       16       -4       -4       -4       -4       -4       -4
## 17       17       17       18        6       -4       -4       -4       -4
## 18       18       17       17        6       -4       -4       -4       -4
## 19       19       19       -4       -4       -4       -4       -4       -4
## 20       20       20       21        7       -4       -4       -4       -4
## # ... with 1.267e+04 more rows, and 88 more variables: R0000156 <int>,
## #   R0000157 <int>, R0000158 <int>, R0000159 <int>, R0000162 <int>,
## #   R0000163 <int>, R0000164 <int>, R0000165 <int>, R0000166 <int>,
## #   R0173600 <int>, R0214700 <int>, R0214800 <int>, R4125101 <int>,
## #   R4125801 <int>, R4126501 <int>, R4127201 <int>, R4127901 <int>,
## #   R4128601 <int>, R4129301 <int>, R4130001 <int>, R4130701 <int>,
## #   R4131401 <int>, R4132101 <int>, R4132801 <int>, R4133701 <int>,
## #   R4521500 <int>, R4521700 <int>, R4521800 <int>, R4521900 <int>,
## #   R4522000 <int>, R4522100 <int>, R4522200 <int>, R4522300 <int>,
## #   R4522400 <int>, R4522500 <int>, R4522600 <int>, R4522700 <int>,
## #   R4522800 <int>, T0002000 <int>, T0002100 <int>, T0002200 <int>,
## #   T0002300 <int>, T0002400 <int>, T0002500 <int>, T0002600 <int>,
## #   T0002700 <int>, T0002800 <int>, T0002900 <int>, T0003000 <int>,
## #   T0003100 <int>, T0003200 <int>, T0003300 <int>, T0003400 <int>,
## #   T0003500 <int>, T0003600 <int>, T0003700 <int>, T0003800 <int>,
## #   T0003900 <int>, T0004000 <int>, T0004100 <int>, T0004200 <int>,
## #   T0004300 <int>, T0004400 <int>, T0004500 <int>, T2261500 <int>,
## #   T2261600 <int>, T2261700 <int>, T2261800 <int>, T2261900 <int>,
## #   T2262000 <int>, T2262100 <int>, T2262200 <int>, T2262300 <int>,
## #   T2262400 <int>, T2262500 <int>, T2262600 <int>, T2262700 <int>,
## #   T2262800 <int>, T2262900 <int>, T2263000 <int>, T2263100 <int>,
## #   T2263200 <int>, T2263300 <int>, T2263400 <int>, T2263500 <int>,
## #   T2263600 <int>, T2263700 <int>, T2263800 <int>
## [1] 1
```

```
## Tibble size: 4.7 Mb
```

```
## Uploading from `nlsy79-gen1/Gen1Implicit.csv` to `Extract.tblGen1Implicit`.
```

```
## # A tibble: 12,686 x 102
##    H0001600 H0001700 H0001800 H0001900 H0002000 H0002100 H0002200 H0002300
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1       -4       -4       -4       -4       -4       -4       -4       -4
##  2        0        1       55        0       -4       -4       -4       -4
##  3        1       -4       -4        1     4920       -4       -4       -4
##  4       -4       -4       -4       -4       -4       -4       -4       -4
##  5       -4       -4       -4       -4       -4       -4       -4       -4
##  6        0        4       54        1     3039       -4       -4       -4
##  7       -2       -4       -4       -4       -4       -4       -4       -4
##  8        1       -4       -4        1     3625     5860     7851       -4
##  9        1       -4       -4        0       -4       -4       -4       -4
## 10       -4       -4       -4       -4       -4       -4       -4       -4
## 11        0        3       62        0       -4       -4       -4       -4
## 12       -4       -4       -4       -4       -4       -4       -4       -4
## 13        1       -4       -4        1       -4       -4       -4       -4
## 14        1       -4       -4        0       -4       -4       -4       -4
## 15        0        1       58        1     4292       -4       -4       -4
## 16        1       -4       -4        1     3310     3310       -4       -4
## 17        0        5       56        1     4292     4920       -4       -4
## 18        0        1       59        1     4920       -4       -4       -4
## 19        1       -4       -4        0       -4       -4       -4       -4
## 20        1       -4       -4        0       -4       -4       -4       -4
## # ... with 1.267e+04 more rows, and 94 more variables: H0002400 <int>,
## #   H0002500 <int>, H0002600 <int>, H0002700 <int>, H0002800 <int>,
## #   H0002900 <int>, H0003000 <int>, H0003100 <int>, H0013600 <int>,
## #   H0013700 <int>, H0013800 <int>, H0013900 <int>, H0014000 <int>,
## #   H0014100 <int>, H0014200 <int>, H0014300 <int>, H0014400 <int>,
## #   H0014500 <int>, H0014700 <int>, H0014800 <int>, H0014900 <int>,
## #   H0015000 <int>, H0015100 <int>, H0015200 <int>, H0015300 <int>,
## #   H0015400 <int>, H0015500 <int>, H0015600 <int>, H0015700 <int>,
## #   H0015800 <int>, H0015803 <int>, H0015804 <int>, R0000100 <int>,
## #   R0006100 <int>, R0006500 <int>, R0007300 <int>, R0007700 <int>,
## #   R0007900 <int>, R0173600 <int>, R0214700 <int>, R0214800 <int>,
## #   R2302900 <int>, R2303100 <int>, R2303200 <int>, R2303300 <int>,
## #   R2303500 <int>, R2303600 <int>, R2505100 <int>, R2505300 <int>,
## #   R2505400 <int>, R2505500 <int>, R2505700 <int>, R2505800 <int>,
## #   R2737900 <int>, R2837200 <int>, R2837300 <int>, R2837400 <int>,
## #   R2837500 <int>, R2837600 <int>, R2837700 <int>, R2837800 <int>,
## #   R2837900 <int>, R2838000 <int>, R2838100 <int>, R2838200 <int>,
## #   R2838300 <int>, R2838400 <int>, R2838500 <int>, R2838600 <int>,
## #   R2838700 <int>, R2838800 <int>, R2838900 <int>, R2839000 <int>,
## #   R2839100 <int>, R2839200 <int>, R2839300 <int>, R2839400 <int>,
## #   R2839500 <int>, R2839600 <int>, R2839700 <int>, R2839800 <int>,
## #   R2839900 <int>, R2840000 <int>, R2840100 <int>, R2840200 <int>,
## #   R2840300 <int>, R2840400 <int>, R2840500 <int>, R2840600 <int>,
## #   R2840700 <int>, R2840800 <int>, R2840900 <int>, R2841000 <int>,
## #   R2841100 <int>
## [1] 1
```

```
## Tibble size: 5 Mb
```

```
## Uploading from `nlsy79-gen1/Gen1Links.csv` to `Extract.tblGen1Links`.
```

```
## # A tibble: 12,686 x 117
##    R0000100 R0000149 R0000300 R0000500 R0009100 R0009300 R0172500 R0172600
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1        1        1        9       58        1        1        3        3
##  2        2        2        1       59        8        7        2       28
##  3        3        3        8       61        3        1        2        8
##  4        4        3        8       62        3        2        2        8
##  5        5        5        7       59        1        0        4       19
##  6        6        5       10       60        1        1        4       21
##  7        7        7        6       64        1        0        2       13
##  8        8        8        7       58        7        7        2       15
##  9        9        9        7       63        4        0        2       13
## 10       10       10       10       60        3        2        2        7
## 11       11       11        7       59        1        1        4       24
## 12       12       12        9       59        3        3        3        9
## 13       13       13        8       58        2        1        3        4
## 14       14       13       10       63        2        2        3        3
## 15       15       15        2       64        1        0        2       13
## 16       16       16       10       58        3        3        2       11
## 17       17       17        1       57        2        1        3        3
## 18       18       17        3       58        2        2        3       24
## 19       19       19       12       57        3        3        2       28
## 20       20       20       11       59        2        0        3       14
## # ... with 1.267e+04 more rows, and 109 more variables: R0173600 <int>,
## #   R0214700 <int>, R0214800 <int>, R0216500 <int>, R0329200 <int>,
## #   R0329210 <int>, R0406510 <int>, R0410100 <int>, R0410300 <int>,
## #   R0530700 <int>, R0530800 <int>, R0619010 <int>, R0809900 <int>,
## #   R0810000 <int>, R0898310 <int>, R1045700 <int>, R1045800 <int>,
## #   R1145110 <int>, R1427500 <int>, R1427600 <int>, R1520310 <int>,
## #   R1774100 <int>, R1774200 <int>, R1794600 <int>, R1794700 <int>,
## #   R1891010 <int>, R2156200 <int>, R2156300 <int>, R2258110 <int>,
## #   R2365700 <int>, R2365800 <int>, R2445510 <int>, R2742500 <int>,
## #   R2742600 <int>, R2871300 <int>, R2986100 <int>, R2986200 <int>,
## #   R3075000 <int>, R3302500 <int>, R3302600 <int>, R3401700 <int>,
## #   R3573400 <int>, R3573500 <int>, R3657100 <int>, R3917600 <int>,
## #   R3917700 <int>, R4007600 <int>, R4100200 <int>, R4100201 <int>,
## #   R4100202 <int>, R4418700 <int>, R4500200 <int>, R4500201 <int>,
## #   R4500202 <int>, R5081700 <int>, R5167000 <int>, R5200200 <int>,
## #   R5200201 <int>, R5200202 <int>, R6435300 <int>, R6435301 <int>,
## #   R6435302 <int>, R6479800 <int>, R6963300 <int>, R6963301 <int>,
## #   R6963302 <int>, R7007500 <int>, R7656300 <int>, R7656301 <int>,
## #   R7656302 <int>, R7704800 <int>, R7800500 <int>, R7800501 <int>,
## #   R7800502 <int>, R8497200 <int>, R9908000 <int>, T0000900 <int>,
## #   T0000901 <int>, T0000902 <int>, T0989000 <int>, T1200700 <int>,
## #   T1200701 <int>, T1200702 <int>, T2210800 <int>, T2260600 <int>,
## #   T2260601 <int>, T2260602 <int>, T2763400 <int>, T2763500 <int>,
## #   T2763600 <int>, T2763700 <int>, T2763800 <int>, T2763900 <int>,
## #   T2764000 <int>, T3108700 <int>, T3195600 <int>, T3195601 <int>,
## #   T3195602 <int>, T3729600 <int>, T3729700 <int>, ...
## [1] 1
```

```
## Tibble size: 5.7 Mb
```

```
## Uploading from `nlsy79-gen1/Gen1Outcomes.csv` to `Extract.tblGen1Outcomes`.
```

```
## # A tibble: 12,686 x 22
##    R0000100 R0173600 R0214700 R0214800 R0481600 R0481700 R0618200 R0618300
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1        1        5        3        2      505       -1       -4       -4
##  2        2        5        3        2      502      120       12        9
##  3        3        5        3        2       -5       -5       51       46
##  4        4        5        3        2      507      110       62       48
##  5        5        1        3        1      503      130       90       99
##  6        6        1        3        1      504      200       99       99
##  7        7        1        3        1      505      131       33       35
##  8        8        6        3        2      505      179       43       42
##  9        9        1        3        1      506      145       55       51
## 10       10        6        3        2      506      115       27       21
## 11       11        1        3        1      511      155       71       81
## 12       12        5        3        2      506      118       94       89
## 13       13        1        3        1      511      180       78       85
## 14       14        5        3        2      507      135       88       92
## 15       15        1        3        1      601      185       83       69
## 16       16        5        3        2      503      130       63       57
## 17       17        1        3        1      509      160       84       88
## 18       18        1        3        1      509      155       99       98
## 19       19        5        3        2      504      120       -4       -4
## 20       20        5        3        2      504      120       54       63
## # ... with 1.267e+04 more rows, and 14 more variables: R0618301 <int>,
## #   R0779800 <int>, R0779900 <int>, R1773900 <int>, R1774000 <int>,
## #   T0897300 <int>, T0897400 <int>, T0897500 <int>, T2053800 <int>,
## #   T2053900 <int>, T2054000 <int>, T3024700 <int>, T3024800 <int>,
## #   T3024900 <int>
## [1] 1
```

```
## Tibble size: 1.1 Mb
```

```
## Uploading from `nlsy79-gen1/Gen1GeocodeSanitized.csv` to `Extract.tblGen1GeocodeSanitized`.
```

```
## # A tibble: 5,302 x 29
##    SubjectTag_S1 SubjectTag_S2 DobDifferenceIn~ DobDifferenceIn~
##            <int>         <int>            <int>            <int>
##  1           300           400             -338             -338
##  2           500           600             -461             -462
##  3          1300          1400            -1868            -1868
##  4          1700          1800             -413             -413
##  5          2000          2100             -591             -591
##  6          2300          2400             -828             -828
##  7          2700          2800            -1356            -1356
##  8          2900          3000              698              698
##  9          3200          3300            -1640               NA
## 10          3400          3500            -1730            -1732
## 11          3700          3800            -1406            -1409
## 12          4000          4100             -772             -772
## 13          5000          5100            -1596               NA
## 14          5500          5600            -1998            -1998
## 15          5800          5900             -738            -1042
## 16          5800          6000            -1680            -1680
## 17          5900          6000             -942             -942
## 18          6100          6200             -677             -678
## 19          6300          6400             -603             -604
## 20          6300          6500            -1343            -1343
## # ... with 5,282 more rows, and 25 more variables:
## #   DobDifferenceInDays1981V1979 <int>,
## #   DobDifferenceInDays1981V1981 <int>, DobDayIsMissing1979_1 <int>,
## #   DobDayIsMissing1979_2 <int>, BirthSubjectCountyMissing_1 <int>,
## #   BirthSubjectCountyMissing_2 <int>, BirthSubjectCountyEqual <int>,
## #   BirthSubjectStateMissing_1 <int>, BirthSubjectStateMissing_2 <int>,
## #   BirthSubjectStateEqual <int>, BirthSubjectCountryMissing_1 <int>,
## #   BirthSubjectCountryMissing_2 <int>, BirthSubjectCountryEqual <int>,
## #   BirthMotherStateMissing_1 <int>, BirthMotherStateMissing_2 <int>,
## #   BirthMotherStateEqual <int>, BirthMotherCountryMissing_1 <int>,
## #   BirthMotherCountryMissing_2 <int>, BirthMotherCountryEqual <int>,
## #   BirthFatherStateMissing_1 <int>, BirthFatherStateMissing_2 <int>,
## #   BirthFatherStateEqual <int>, BirthFatherCountryMissing_1 <int>,
## #   BirthFatherCountryMissing_2 <int>, BirthFatherCountryEqual <int>
## [1] 1
```

```
## Tibble size: 0.6 Mb
```

```
## Uploading from `nlsy79-gen2/Gen2FatherFromGen1.csv` to `Extract.tblGen2FatherFromGen1`.
```

```
## # A tibble: 12,686 x 797
##    R0000100 R0173600 R0214700 R0214800 R1373300 R1373500 R1374000 R1374200
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1        1        5        3        2       -5       -5       -5       -5
##  2        2        5        3        2       -4       -4       -4       -4
##  3        3        5        3        2        1       -4        1       -4
##  4        4        5        3        2        0        4       -4       -4
##  5        5        1        3        1       -4       -4       -4       -4
##  6        6        1        3        1       -4       -4       -4       -4
##  7        7        1        3        1       -4       -4       -4       -4
##  8        8        6        3        2        1       -4        1       -4
##  9        9        1        3        1       -4       -4       -4       -4
## 10       10        6        3        2        1       -4       -4       -4
## 11       11        1        3        1       -4       -4       -4       -4
## 12       12        5        3        2       -4       -4       -4       -4
## 13       13        1        3        1       -4       -4       -4       -4
## 14       14        5        3        2       -4       -4       -4       -4
## 15       15        1        3        1       -4       -4       -4       -4
## 16       16        5        3        2       -4       -4       -4       -4
## 17       17        1        3        1       -4       -4       -4       -4
## 18       18        1        3        1       -4       -4       -4       -4
## 19       19        5        3        2       -4       -4       -4       -4
## 20       20        5        3        2       -4       -4       -4       -4
## # ... with 1.267e+04 more rows, and 789 more variables: R1374700 <int>,
## #   R1374900 <int>, R1375400 <int>, R1375600 <int>, R1376100 <int>,
## #   R1376300 <int>, R1376800 <int>, R1377000 <int>, R1377500 <int>,
## #   R1377700 <int>, R1753700 <int>, R1753900 <int>, R1754400 <int>,
## #   R1754600 <int>, R1755100 <int>, R1755300 <int>, R1755800 <int>,
## #   R1756000 <int>, R1756500 <int>, R1756700 <int>, R1757200 <int>,
## #   R1757400 <int>, R1757900 <int>, R1758100 <int>, R2095700 <int>,
## #   R2095900 <int>, R2096400 <int>, R2096600 <int>, R2097100 <int>,
## #   R2097300 <int>, R2097800 <int>, R2098000 <int>, R2098500 <int>,
## #   R2098700 <int>, R2099200 <int>, R2099400 <int>, R2099900 <int>,
## #   R2100100 <int>, R2345900 <int>, R2346200 <int>, R2346500 <int>,
## #   R2346800 <int>, R2347100 <int>, R2347400 <int>, R2347700 <int>,
## #   R2648000 <int>, R2648200 <int>, R2648700 <int>, R2648900 <int>,
## #   R2649400 <int>, R2649600 <int>, R2650100 <int>, R2650300 <int>,
## #   R2650800 <int>, R2651000 <int>, R2651500 <int>, R2651700 <int>,
## #   R2652200 <int>, R2652400 <int>, R2955900 <int>, R2956200 <int>,
## #   R2956500 <int>, R2956800 <int>, R2957100 <int>, R2957400 <int>,
## #   R2957700 <int>, R3255900 <int>, R3256100 <int>, R3257700 <int>,
## #   R3257900 <int>, R3259500 <int>, R3259700 <int>, R3261300 <int>,
## #   R3261500 <int>, R3263100 <int>, R3263300 <int>, R3264900 <int>,
## #   R3265100 <int>, R3266700 <int>, R3266900 <int>, R3268500 <int>,
## #   R3556400 <int>, R3557000 <int>, R3557600 <int>, R3771800 <int>,
## #   R3772000 <int>, R3772100 <int>, R3772400 <int>, R3773900 <int>,
## #   R3774100 <int>, R3774200 <int>, R3774500 <int>, R3776000 <int>,
## #   R3776200 <int>, R3776300 <int>, R3776600 <int>, R3778100 <int>,
## #   R3778300 <int>, R3778400 <int>, R3778700 <int>, ...
## [1] 1
```

```
## Tibble size: 39 Mb
```

```
## Uploading from `nlsy79-gen2/Gen2ImplicitFather.csv` to `Extract.tblGen2ImplicitFather`.
```

```
## # A tibble: 11,521 x 132
##    C0000100 C0000200 C0005300 C0005400 C0005700 C0008100 C0008200 C0008300
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1      201        2        3        2     1993       -7       -7       -7
##  2      202        2        3        2     1994       -7       -7       -7
##  3      301        3        3        2     1981        1       -7       -7
##  4      302        3        3        2     1983        1       -7       -7
##  5      303        3        3        2     1986       -7       -7       -7
##  6      401        4        3        1     1980        0        1        4
##  7      403        4        3        2     1997       -7       -7       -7
##  8      801        8        3        2     1976        1       -7       -7
##  9      802        8        3        1     1979        1       -7       -7
## 10      803        8        3        2     1982        1       -7       -7
## 11     1001       10        3        2     1983        1       -7       -7
## 12     1002       10        3        2     1988       -7       -7       -7
## 13     1201       12        3        1     1989       -7       -7       -7
## 14     1202       12        3        1     1993       -7       -7       -7
## 15     1601       16        3        1     1990       -7       -7       -7
## 16     1602       16        3        1     1993       -7       -7       -7
## 17     1603       16        3        2     1996       -7       -7       -7
## 18     1901       19        3        2     1987       -7       -7       -7
## 19     2001       20        3        2     1990       -7       -7       -7
## 20     2501       25        3        1     1990       -7       -7       -7
## # ... with 1.15e+04 more rows, and 124 more variables: C0008600 <int>,
## #   C0008700 <int>, C0008800 <int>, C0009100 <int>, C0009200 <int>,
## #   C0009300 <int>, C0009600 <int>, C0009700 <int>, C0009800 <int>,
## #   C0009900 <int>, C0010110 <int>, C0010200 <int>, C0010300 <int>,
## #   C0010400 <int>, C0010700 <int>, C0010800 <int>, C0010900 <int>,
## #   C0011110 <int>, C0011111 <int>, C0011112 <int>, C0011113 <int>,
## #   C0011114 <int>, C0011117 <int>, C0011118 <int>, C0011119 <int>,
## #   C0011122 <int>, C0011123 <int>, C0011124 <int>, C0011127 <int>,
## #   C0011128 <int>, C0011129 <int>, C0011132 <int>, C0011133 <int>,
## #   C0011134 <int>, C0011137 <int>, C0011138 <int>, C0011139 <int>,
## #   C0011142 <int>, C0011143 <int>, C0011144 <int>, C3070500 <int>,
## #   C3423600 <int>, C3601100 <int>, C3601700 <int>, C3601800 <int>,
## #   C3601900 <int>, C3605900 <int>, C3981100 <int>, C3981700 <int>,
## #   C3981800 <int>, C3981900 <int>, C5156100 <int>, C5525500 <int>,
## #   C5525600 <int>, C5525700 <int>, C5733200 <int>, C5801800 <int>,
## #   C5801900 <int>, C5802000 <int>, C6006200 <int>, Y0003200 <int>,
## #   Y0007300 <int>, Y0007400 <int>, Y0007600 <int>, Y0007601 <int>,
## #   Y0008000 <int>, Y0008500 <int>, Y0008600 <int>, Y0009400 <int>,
## #   Y0394100 <int>, Y0394300 <int>, Y0394500 <int>, Y0394501 <int>,
## #   Y0394900 <int>, Y0651000 <int>, Y0682500 <int>, Y0683800 <int>,
## #   Y0683900 <int>, Y0684100 <int>, Y0684101 <int>, Y0684500 <int>,
## #   Y0947100 <int>, Y0986200 <int>, Y0986700 <int>, Y0988800 <int>,
## #   Y0988900 <int>, Y0989400 <int>, Y0989401 <int>, Y0989900 <int>,
## #   Y1229100 <int>, Y1229200 <int>, Y1229700 <int>, Y1229701 <int>,
## #   Y1264300 <int>, Y1265300 <int>, Y1458900 <int>, Y1459400 <int>,
## #   Y1459401 <int>, Y1629500 <int>, Y1704000 <int>, ...
## [1] 1
```

```
## Tibble size: 5.9 Mb
```

```
## Uploading from `nlsy79-gen2/Gen2Links.csv` to `Extract.tblGen2Links`.
```

```
## # A tibble: 11,521 x 207
##    C0000100 C0000200 C0005300 C0005400 C0005500 C0005700 C0005800 C0006500
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1      201        2        3        2        3     1993        1       -7
##  2      202        2        3        2       11     1994        2       -7
##  3      301        3        3        2        6     1981        1       56
##  4      302        3        3        2       10     1983        2       28
##  5      303        3        3        2        4     1986        3       -7
##  6      401        4        3        1        8     1980        1       -7
##  7      403        4        3        2        3     1997        2       -7
##  8      801        8        3        2        3     1976        1      120
##  9      802        8        3        1        5     1979        2       81
## 10      803        8        3        2        9     1982        3       41
## 11     1001       10        3        2        1     1983        1       -7
## 12     1002       10        3        2        7     1988        2       -7
## 13     1201       12        3        1       10     1989        1       -7
## 14     1202       12        3        1        8     1993        2       -7
## 15     1601       16        3        1        2     1990        1       -7
## 16     1602       16        3        1        8     1993        2       -7
## 17     1603       16        3        2        9     1996        3       -7
## 18     1901       19        3        2       11     1987        1       -7
## 19     2001       20        3        2        8     1990        1       -7
## 20     2501       25        3        1       10     1990        1       -7
## # ... with 1.15e+04 more rows, and 199 more variables: C0006800 <int>,
## #   C0007010 <int>, C0007030 <int>, C0007041 <int>, C0007043 <int>,
## #   C0007045 <int>, C0007047 <int>, C0007049 <int>, C0007052 <int>,
## #   C0007055 <int>, C0402400 <int>, C0402500 <int>, C0402600 <int>,
## #   C0404100 <int>, C0404200 <int>, C0737000 <int>, C0737100 <int>,
## #   C0737200 <int>, C0948700 <int>, C0948800 <int>, C0948900 <int>,
## #   C1146600 <int>, C1146700 <int>, C1146800 <int>, C1230100 <int>,
## #   C1230200 <int>, C1230300 <int>, C1548100 <int>, C1548101 <int>,
## #   C1548102 <int>, C1989400 <int>, C1989401 <int>, C1989402 <int>,
## #   C2305100 <int>, C2305101 <int>, C2305102 <int>, C2544700 <int>,
## #   C2544701 <int>, C2544702 <int>, C2814500 <int>, C2814501 <int>,
## #   C2814502 <int>, C3123500 <int>, C3123501 <int>, C3123502 <int>,
## #   C3601100 <int>, C3627700 <int>, C3627701 <int>, C3627702 <int>,
## #   C3981100 <int>, C4006300 <int>, C4006301 <int>, C4006302 <int>,
## #   C5524800 <int>, C5550100 <int>, C5550101 <int>, C5550102 <int>,
## #   C5801100 <int>, Y0000200 <int>, Y0000201 <int>, Y0000202 <int>,
## #   Y0002100 <int>, Y0390100 <int>, Y0390101 <int>, Y0390102 <int>,
## #   Y0677600 <int>, Y0933700 <int>, Y0933701 <int>, Y0933702 <int>,
## #   Y0974800 <int>, Y1180500 <int>, Y1180501 <int>, Y1180502 <int>,
## #   Y1192400 <int>, Y1421100 <int>, Y1421101 <int>, Y1421102 <int>,
## #   Y1434300 <int>, Y1450200 <int>, Y1450201 <int>, Y1450202 <int>,
## #   Y1672700 <int>, Y1695600 <int>, Y1695601 <int>, Y1695602 <int>,
## #   Y1707300 <int>, Y1707400 <int>, Y1707500 <int>, Y1707600 <int>,
## #   Y1707700 <int>, Y1707800 <int>, Y1707900 <int>, Y1708000 <int>,
## #   Y1708100 <int>, Y1708200 <int>, Y1708300 <int>, Y1708400 <int>,
## #   Y1708500 <int>, Y1708600 <int>, Y1708700 <int>, ...
## [1] 1
```

```
## Tibble size: 9.2 Mb
```

```
## Uploading from `nlsy79-gen2/Gen2LinksFromGen1.csv` to `Extract.tblGen2LinksFromGen1`.
```

```
## # A tibble: 12,686 x 137
##    R0000100 R0214700 R0214800 R4825700 R4826000 R4826100 R4826300 R4826500
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1        1        3        2       -5       -5       -5       -5       -5
##  2        2        3        2        0       -4       -4       -4       -4
##  3        3        3        2        0       -4       -4       -4       -4
##  4        4        3        2       -5       -5       -5       -5       -5
##  5        5        3        1       -5       -5       -5       -5       -5
##  6        6        3        1        0       -4       -4       -4       -4
##  7        7        3        1        0       -4       -4       -4       -4
##  8        8        3        2        0       -4       -4       -4       -4
##  9        9        3        1        0       -4       -4       -4       -4
## 10       10        3        2        0       -4       -4       -4       -4
## 11       11        3        1        0       -4       -4       -4       -4
## 12       12        3        2        0       -4       -4       -4       -4
## 13       13        3        1        0       -4       -4       -4       -4
## 14       14        3        2        0       -4       -4       -4       -4
## 15       15        3        1        0       -4       -4       -4       -4
## 16       16        3        2        0       -4       -4       -4       -4
## 17       17        3        1        0       -4       -4       -4       -4
## 18       18        3        1        0       -4       -4       -4       -4
## 19       19        3        2        0       -4       -4       -4       -4
## 20       20        3        2        0       -4       -4       -4       -4
## # ... with 1.267e+04 more rows, and 129 more variables: R4826800 <int>,
## #   R5495900 <int>, R5496200 <int>, R5496300 <int>, R5496500 <int>,
## #   R5496700 <int>, R5497000 <int>, R5497200 <int>, R6210700 <int>,
## #   R6210800 <int>, R6210900 <int>, R6211500 <int>, R6211600 <int>,
## #   R6211700 <int>, R6211800 <int>, R6211900 <int>, R6212200 <int>,
## #   R6212300 <int>, R6764000 <int>, R6764100 <int>, R6764200 <int>,
## #   R6764900 <int>, R6765000 <int>, R6765100 <int>, R6765200 <int>,
## #   R6765600 <int>, R6765700 <int>, R6765800 <int>, R6839600 <int>,
## #   R7408300 <int>, R7408400 <int>, R7408500 <int>, R7409200 <int>,
## #   R7409300 <int>, R7409400 <int>, R7409500 <int>, R7409900 <int>,
## #   R7410000 <int>, R7410100 <int>, R7548600 <int>, R8106400 <int>,
## #   R8106500 <int>, R8106600 <int>, R8106700 <int>, R8106800 <int>,
## #   R8106900 <int>, R8107000 <int>, R8107100 <int>, R8107200 <int>,
## #   R8255400 <int>, R9900400 <int>, R9900600 <int>, R9900601 <int>,
## #   R9901200 <int>, R9901400 <int>, R9901401 <int>, R9902000 <int>,
## #   R9902200 <int>, R9902201 <int>, R9902800 <int>, R9903000 <int>,
## #   R9903001 <int>, R9903600 <int>, R9903800 <int>, R9903801 <int>,
## #   R9904400 <int>, R9904600 <int>, R9904601 <int>, R9905200 <int>,
## #   R9905400 <int>, R9905401 <int>, R9906000 <int>, R9906600 <int>,
## #   R9907200 <int>, R9907800 <int>, R9908000 <int>, R9911200 <int>,
## #   R9911201 <int>, T0337300 <int>, T0337400 <int>, T0337500 <int>,
## #   T0337600 <int>, T0337700 <int>, T0337800 <int>, T0337900 <int>,
## #   T0338000 <int>, T0338100 <int>, T0338200 <int>, T0338300 <int>,
## #   T0338400 <int>, T0338500 <int>, T0338600 <int>, T1486900 <int>,
## #   T1487000 <int>, T1487100 <int>, T1487200 <int>, T1487300 <int>,
## #   T1487400 <int>, T1487500 <int>, T1487600 <int>, ...
## [1] 1
```

```
## Tibble size: 6.7 Mb
```

```
## Uploading from `nlsy79-gen2/Gen2OutcomesHeight.csv` to `Extract.tblGen2OutcomesHeight`.
```

```
## # A tibble: 11,521 x 46
##    C0000100 C0000200 C0005300 C0005400 C0005700 C0577600 C0606300 C0606400
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1      201        2        3        2     1993       -7       -7       -7
##  2      202        2        3        2     1994       -7       -7       -7
##  3      301        3        3        2     1981       42        4        2
##  4      302        3        3        2     1983       37        3        9
##  5      303        3        3        2     1986       -7        2       10
##  6      401        4        3        1     1980       -7        4        7
##  7      403        4        3        2     1997       -7       -7       -7
##  8      801        8        3        2     1976       58        5        5
##  9      802        8        3        1     1979       50        4       10
## 10      803        8        3        2     1982       41        4        4
## 11     1001       10        3        2     1983       -7        3        9
## 12     1002       10        3        2     1988       -7        1       11
## 13     1201       12        3        1     1989       -7       -7       -7
## 14     1202       12        3        1     1993       -7       -7       -7
## 15     1601       16        3        1     1990       -7       -7       -7
## 16     1602       16        3        1     1993       -7       -7       -7
## 17     1603       16        3        2     1996       -7       -7       -7
## 18     1901       19        3        2     1987       -7        2        1
## 19     2001       20        3        2     1990       -7       -7       -7
## 20     2501       25        3        1     1990       -7       -7       -7
## # ... with 1.15e+04 more rows, and 38 more variables: C0826400 <int>,
## #   C0826500 <int>, C1016700 <int>, C1016800 <int>, C1220200 <int>,
## #   C1220300 <int>, C1532700 <int>, C1532800 <int>, C1779300 <int>,
## #   C1779400 <int>, C2288500 <int>, C2288600 <int>, C2552300 <int>,
## #   C2820900 <int>, C3130400 <int>, C3553400 <int>, C3634500 <int>,
## #   C3898000 <int>, C4013000 <int>, C5147900 <int>, Y0308300 <int>,
## #   Y0308400 <int>, Y0609600 <int>, Y0609700 <int>, Y0903900 <int>,
## #   Y0904000 <int>, Y1150800 <int>, Y1150900 <int>, Y1385800 <int>,
## #   Y1385900 <int>, Y1637500 <int>, Y1637600 <int>, Y1891100 <int>,
## #   Y1891200 <int>, Y2207000 <int>, Y2207100 <int>, Y2544700 <int>,
## #   Y2544800 <int>
## [1] 1
```

```
## Tibble size: 2 Mb
```

```
## Uploading from `nlsy79-gen2/Gen2OutcomesMath.csv` to `Extract.tblGen2OutcomesMath`.
```

```
## # A tibble: 11,521 x 44
##    C0000100 C0000200 C0005300 C0005400 C0005700 C0579900 C0580000 C0580100
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1      201        2        3        2     1993       -7       -7       -7
##  2      202        2        3        2     1994       -7       -7       -7
##  3      301        3        3        2     1981       -7       -7       -7
##  4      302        3        3        2     1983       -7       -7       -7
##  5      303        3        3        2     1986       -7       -7       -7
##  6      401        4        3        1     1980       -7       -7       -7
##  7      403        4        3        2     1997       -7       -7       -7
##  8      801        8        3        2     1976       48       71      108
##  9      802        8        3        1     1979       41       95      125
## 10      803        8        3        2     1982       -7       -7       -7
## 11     1001       10        3        2     1983       -7       -7       -7
## 12     1002       10        3        2     1988       -7       -7       -7
## 13     1201       12        3        1     1989       -7       -7       -7
## 14     1202       12        3        1     1993       -7       -7       -7
## 15     1601       16        3        1     1990       -7       -7       -7
## 16     1602       16        3        1     1993       -7       -7       -7
## 17     1603       16        3        2     1996       -7       -7       -7
## 18     1901       19        3        2     1987       -7       -7       -7
## 19     2001       20        3        2     1990       -7       -7       -7
## 20     2501       25        3        1     1990       -7       -7       -7
## # ... with 1.15e+04 more rows, and 36 more variables: C0799400 <int>,
## #   C0799500 <int>, C0799600 <int>, C0998600 <int>, C0998700 <int>,
## #   C0998800 <int>, C1198600 <int>, C1198700 <int>, C1198800 <int>,
## #   C1507600 <int>, C1507700 <int>, C1507800 <int>, C1564500 <int>,
## #   C1564600 <int>, C1564700 <int>, C1799900 <int>, C1800000 <int>,
## #   C1800100 <int>, C2503500 <int>, C2503600 <int>, C2503700 <int>,
## #   C2532000 <int>, C2532100 <int>, C2532200 <int>, C2802800 <int>,
## #   C2802900 <int>, C2803000 <int>, C3111300 <int>, C3111400 <int>,
## #   C3111500 <int>, C3615000 <int>, C3615100 <int>, C3615200 <int>,
## #   C3993600 <int>, C3993700 <int>, C3993800 <int>
## [1] 1
```

```
## Tibble size: 2 Mb
```

```
## Uploading from `nlsy79-gen2/Gen2OutcomesWeight.csv` to `Extract.tblGen2OutcomesWeight`.
```

```
## # A tibble: 11,521 x 31
##    C0000100 C0000200 C0005300 C0005400 C0005700 Y0308500 Y0904100 Y1151000
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1      201        2        3        2     1993       -7       -7       -7
##  2      202        2        3        2     1994       -7       -7       -7
##  3      301        3        3        2     1981       -7      115       -7
##  4      302        3        3        2     1983       -7      129      135
##  5      303        3        3        2     1986       -7       -7       -7
##  6      401        4        3        1     1980       -7      153       -7
##  7      403        4        3        2     1997       -7       -7       -7
##  8      801        8        3        2     1976      187       -7      220
##  9      802        8        3        1     1979      130      178      190
## 10      803        8        3        2     1982       -7      155      155
## 11     1001       10        3        2     1983       -7       -7       -7
## 12     1002       10        3        2     1988       -7       -7       -7
## 13     1201       12        3        1     1989       -7       -7       -7
## 14     1202       12        3        1     1993       -7       -7       -7
## 15     1601       16        3        1     1990       -7       -7       -7
## 16     1602       16        3        1     1993       -7       -7       -7
## 17     1603       16        3        2     1996       -7       -7       -7
## 18     1901       19        3        2     1987       -7       -7       -7
## 19     2001       20        3        2     1990       -7       -7       -7
## 20     2501       25        3        1     1990       -7       -7       -7
## # ... with 1.15e+04 more rows, and 23 more variables: Y1386000 <int>,
## #   Y1637700 <int>, Y1891300 <int>, Y2207200 <int>, Y2544900 <int>,
## #   Y2623301 <int>, Y2623302 <int>, Y2623401 <int>, Y2623402 <int>,
## #   Y2623501 <int>, Y2623502 <int>, Y2623601 <int>, Y2623602 <int>,
## #   Y2623701 <int>, Y2623702 <int>, Y2623801 <int>, Y2623802 <int>,
## #   Y2623901 <int>, Y2623902 <int>, Y2624001 <int>, Y2624002 <int>,
## #   Y2624101 <int>, Y2624102 <int>
## [1] 1
```

```
## Tibble size: 1.4 Mb
```

```
## Uploading from `nlsy79-gen2/Gen2FatherFromGen1Death.csv` to `Extract.tblGen2FatherFromGen1Death`.
```

```
## # A tibble: 12,686 x 318
##    R0000100 R0214800 R1373400 R1374100 R1374800 R1375500 R1376200 R1376900
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1        1        2       -5       -5       -5       -5       -5       -5
##  2        2        2       -4       -4       -4       -4       -4       -4
##  3        3        2       -4       -4       -4       -4       -4       -4
##  4        4        2        1       -4       -4       -4       -4       -4
##  5        5        1       -4       -4       -4       -4       -4       -4
##  6        6        1       -4       -4       -4       -4       -4       -4
##  7        7        1       -4       -4       -4       -4       -4       -4
##  8        8        2       -4       -4       -4       -4       -4       -4
##  9        9        1       -4       -4       -4       -4       -4       -4
## 10       10        2       -4       -4       -4       -4       -4       -4
## 11       11        1       -4       -4       -4       -4       -4       -4
## 12       12        2       -4       -4       -4       -4       -4       -4
## 13       13        1       -4       -4       -4       -4       -4       -4
## 14       14        2       -4       -4       -4       -4       -4       -4
## 15       15        1       -4       -4       -4       -4       -4       -4
## 16       16        2       -4       -4       -4       -4       -4       -4
## 17       17        1       -4       -4       -4       -4       -4       -4
## 18       18        1       -4       -4       -4       -4       -4       -4
## 19       19        2       -4       -4       -4       -4       -4       -4
## 20       20        2       -4       -4       -4       -4       -4       -4
## # ... with 1.267e+04 more rows, and 310 more variables: R1377600 <int>,
## #   R1753800 <int>, R1754500 <int>, R1755200 <int>, R1755900 <int>,
## #   R1756600 <int>, R1757300 <int>, R1758000 <int>, R2095800 <int>,
## #   R2096500 <int>, R2097200 <int>, R2097900 <int>, R2098600 <int>,
## #   R2099300 <int>, R2100000 <int>, R2648100 <int>, R2648800 <int>,
## #   R2649500 <int>, R2650200 <int>, R2650900 <int>, R2651600 <int>,
## #   R2652300 <int>, R3256000 <int>, R3257800 <int>, R3259600 <int>,
## #   R3261400 <int>, R3263200 <int>, R3265000 <int>, R3266800 <int>,
## #   R3771900 <int>, R3772200 <int>, R3772300 <int>, R3774000 <int>,
## #   R3774300 <int>, R3774400 <int>, R3776100 <int>, R3776400 <int>,
## #   R3776500 <int>, R3778200 <int>, R3778500 <int>, R3778600 <int>,
## #   R3780300 <int>, R3780600 <int>, R3780700 <int>, R3782400 <int>,
## #   R3784500 <int>, R4276900 <int>, R4277200 <int>, R4277201 <int>,
## #   R4277800 <int>, R4278100 <int>, R4278101 <int>, R4278700 <int>,
## #   R4279000 <int>, R4279001 <int>, R4279600 <int>, R4279900 <int>,
## #   R4279901 <int>, R4280500 <int>, R4280800 <int>, R4280801 <int>,
## #   R4281400 <int>, R4282200 <int>, R4827700 <int>, R4828000 <int>,
## #   R4828001 <int>, R4830700 <int>, R4831000 <int>, R4831001 <int>,
## #   R4833700 <int>, R4834000 <int>, R4834001 <int>, R4836700 <int>,
## #   R4837000 <int>, R4837001 <int>, R4839700 <int>, R4840000 <int>,
## #   R4840001 <int>, R4842600 <int>, R4844900 <int>, R4847200 <int>,
## #   R5498100 <int>, R5498400 <int>, R5498401 <int>, R5501100 <int>,
## #   R5501400 <int>, R5501401 <int>, R5504100 <int>, R5504400 <int>,
## #   R5504401 <int>, R5507100 <int>, R5507400 <int>, R5507401 <int>,
## #   R5510100 <int>, R5510400 <int>, R5510401 <int>, R5513100 <int>,
## #   R5515400 <int>, R5517700 <int>, R5519900 <int>, ...
## [1] 1
```

```
## Tibble size: 15.6 Mb
```

```r
DBI::dbDisconnect(channel_odbc); rm(channel_odbc)
RODBC::odbcClose(channel_rodbc); rm(channel_rodbc)

duration_in_seconds <- round(as.numeric(difftime(Sys.time(), start_time, units="secs")))
cat("File completed by `", Sys.info()["user"], "` at ", strftime(Sys.time(), "%Y-%m-%d, %H:%M %z"), " in ",  duration_in_seconds, " seconds.", sep="")
```

```
## File completed by `Will` at 2018-10-14, 17:01 -0500 in 42 seconds.
```

The R session information (including the OS info, R version and all
packages used):


```r
sessionInfo()
```

```
## R version 3.5.1 Patched (2018-09-10 r75281)
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
## [1] knitr_1.20     bindrcpp_0.2.2 magrittr_1.5  
## 
## loaded via a namespace (and not attached):
##  [1] Rcpp_0.12.19          highr_0.7             pillar_1.3.0         
##  [4] compiler_3.5.1        bindr_0.1.1           tools_3.5.1          
##  [7] odbc_1.1.6            packrat_0.4.9-3       digest_0.6.18        
## [10] bit_1.1-14            memoise_1.1.0         evaluate_0.12        
## [13] tibble_1.4.2          checkmate_1.8.5       pkgconfig_2.0.2      
## [16] rlang_0.2.2           rstudioapi_0.8        DBI_1.0.0            
## [19] cli_1.0.1             yaml_2.2.0            withr_2.1.2          
## [22] dplyr_0.7.6           stringr_1.3.1         devtools_1.13.6      
## [25] hms_0.4.2.9001        rprojroot_1.3-2       bit64_0.9-7          
## [28] tidyselect_0.2.5      glue_1.3.0            OuhscMunge_0.1.9.9009
## [31] R6_2.3.0              fansi_0.4.0           rmarkdown_1.10       
## [34] tidyr_0.8.1           readr_1.2.0           purrr_0.2.5          
## [37] blob_1.1.1            scales_1.0.0          backports_1.1.2      
## [40] RODBC_1.3-15          htmltools_0.3.6       assertthat_0.2.0     
## [43] testit_0.8.1          colorspace_1.3-2      config_0.3           
## [46] utf8_1.1.4            stringi_1.2.4         munsell_0.5.0        
## [49] markdown_0.8          crayon_1.3.4
```

```r
Sys.time()
```

```
## [1] "2018-10-14 17:01:53 CDT"
```

