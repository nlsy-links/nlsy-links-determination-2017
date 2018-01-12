



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
directory_in              <- "data-unshared/raw"
columns_to_drop           <- c("A0002600", "Y2267000")

ds_extract <- tibble::tribble(
  ~table_name                       , ~file_name
  ,"Extract.tblLinksExplicit"       , "nlsy97/97-links-explicit.csv"
  ,"Extract.tblLinksImplicit"       , "nlsy97/97-links-implicit.csv"
  ,"Extract.tblRoster"              , "nlsy97/97-roster.csv"
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
    path            = file.path(directory_in, file_name),
    extract_exist   = file.exists(path),
    sql_select      = glue::glue("SELECT TOP(100) * FROM {table_name}"),
    sql_truncate    = glue::glue("TRUNCATE TABLE {table_name}")
  )
testit::assert("All files should be found.", all(ds_extract$extract_exist))

print(ds_extract, n=20)
```

```
## # A tibble: 3 x 6
##   table_name               file_na~ path       extra~ sql_select sql_trun~
##   <chr>                    <chr>    <chr>      <lgl>  <chr>      <chr>    
## 1 Extract.tblLinksExplicit nlsy97/~ data-unsh~ T      SELECT TO~ TRUNCATE~
## 2 Extract.tblLinksImplicit nlsy97/~ data-unsh~ T      SELECT TO~ TRUNCATE~
## 3 Extract.tblRoster        nlsy97/~ data-unsh~ T      SELECT TO~ TRUNCATE~
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
## [1] "14.00.0500"
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
  message(glue::glue("Uploading from `{ds_extract$file_name[i]}` to `{ds_extract$table_name[i]}`."))

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
  # peek <- DBI::dbListFields(channel_odbc, ds_extract$table_name[i])

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
    tablename   = ds_extract$table_name[i],
    safer       = TRUE,       # Don't keep the existing table.
    rownames    = FALSE,
    append      = TRUE
  ) %>%
  print()
  })

  # OuhscMunge::upload_sqls_rodbc(
  #   d               = d[1:100, ],
  #   table_name      = ds_extract$table_name[i] ,
  #   dsn_name        = "local-nlsy-links-97",
  #   clear_table     = F,
  #   create_table    = T
  # )


  message(glue::glue("Tibble size: {format(object.size(d), units='MB')}"))
}
```

```
## Uploading from `nlsy97/97-links-explicit.csv` to `Extract.tblLinksExplicit`.
```

```
## # A tibble: 8,984 x 89
##    R000~ R053~ R053~ R053~ R082~ R082~ R082~ R082~ R082~ R082~ R082~ R082~
##    <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int>
##  1     1     2     9  1981    -4    -4    -4    -4    -4    -4    -4    -4
##  2     2     1     7  1982    -4    -4    -4    -4    -4    -4    -4    -4
##  3     3     2     9  1983    -4    -4    -4    -4    -4    -4    -4    -4
##  4     4     2     2  1981    -4    -4    -4    -4    -4    -4    -4    -4
##  5     5     1    10  1982    -4    -4    -4    -4    -4    -4    -4    -4
##  6     6     2     1  1982    -4    -4    -4    -4    -4    -4    -4    -4
##  7     7     1     4  1983    -4    -4    -4    -4    -4    -4    -4    -4
##  8     8     2     6  1981    -4    -4    -4    -4    -4    -4    -4    -4
##  9     9     1    10  1982    -4    -4    -4    -4    -4    -4    -4    -4
## 10    10     1     3  1984    -4    -4    -4    -4    -4    -4    -4    -4
## 11    11     2     6  1982    -4    -4    -4    -4    -4    -4    -4    -4
## 12    12     1    10  1981    -4    -4    -4    -4    -4    -4    -4    -4
## 13    13     1    11  1984    -4    -4    -4    -4    -4    -4    -4    -4
## 14    14     1     7  1980    -4    -4    -4    -4    -4    -4    -4    -4
## 15    15     2     1  1983    -4    -4    -4    -4    -4    -4    -4    -4
## 16    16     1     2  1982    -4    -4    -4    -4    -4    -4    -4    -4
## 17    17     2    11  1981    -4    -4    -4    -4    -4    -4    -4    -4
## 18    18     1     2  1982    -4    -4    -4    -4    -4    -4    -4    -4
## 19    19     1     4  1984    -4    -4    -4    -4    -4    -4    -4    -4
## 20    20     1    12  1980    -4    -4    -4    -4    -4    -4    -4    -4
## # ... with 8,964 more rows, and 77 more variables: R0823000 <int>,
## #   R0823100 <int>, R0823200 <int>, R0823300 <int>, R0823400 <int>,
## #   R0823500 <int>, R0823600 <int>, R0823700 <int>, R0823800 <int>,
## #   R0823900 <int>, R0824000 <int>, R0824100 <int>, R0824200 <int>,
## #   R0824300 <int>, R0824400 <int>, R0824500 <int>, R0824600 <int>,
## #   R0824700 <int>, R0824800 <int>, R0824900 <int>, R0825000 <int>,
## #   R0825100 <int>, R0825200 <int>, R0825300 <int>, R0825400 <int>,
## #   R1097800 <int>, R1097900 <int>, R1098000 <int>, R1098100 <int>,
## #   R1098200 <int>, R1098300 <int>, R1098400 <int>, R1098500 <int>,
## #   R1098600 <int>, R1098700 <int>, R1098800 <int>, R1098900 <int>,
## #   R1099000 <int>, R1099100 <int>, R1099200 <int>, R1099300 <int>,
## #   R1101000 <int>, R1101100 <int>, R1101200 <int>, R1101300 <int>,
## #   R1101400 <int>, R1101500 <int>, R1101600 <int>, R1101700 <int>,
## #   R1101800 <int>, R1101900 <int>, R1102000 <int>, R1102100 <int>,
## #   R1102200 <int>, R1102300 <int>, R1102400 <int>, R1102500 <int>,
## #   R1102501 <int>, R1117000 <int>, R1117100 <int>, R1117200 <int>,
## #   R1117300 <int>, R1117400 <int>, R1117500 <int>, R1117600 <int>,
## #   R1117700 <int>, R1117800 <int>, R1117900 <int>, R1118000 <int>,
## #   R1118100 <int>, R1118200 <int>, R1118300 <int>, R1118400 <int>,
## #   R1118500 <int>, R1193000 <int>, R1235800 <int>, R1482600 <int>
## [1] 1
```

```
## Tibble size: 3.1 Mb
```

```
## Uploading from `nlsy97/97-links-implicit.csv` to `Extract.tblLinksImplicit`.
```

```
## # A tibble: 8,984 x 45
##    R000~ R053~ R053~ R053~ R055~ R055~ R056~ R056~ R119~ R119~ R119~ R120~
##    <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int>
##  1     1     2     9  1981     1     3    -4    -4     1     1     1     6
##  2     2     1     7  1982   - 4    -4    -4    -4     2     1     1     4
##  3     3     2     9  1983     1    -4     8    -4     3     1     0     2
##  4     4     2     2  1981     4    -4     8    -4     4     1     1     2
##  5     5     1    10  1982    11     7    -4    -4     6     1     1     4
##  6     6     2     1  1982     3    -4    -4    -4     8     2     1     5
##  7     7     1     4  1983     3    -4    -4    -4     8     2     1     5
##  8     8     2     6  1981     4     4    -4    -4     9     3     2     5
##  9     9     1    10  1982     4     4    -4    -4     9     3     2     5
## 10    10     1     3  1984     4     4    -4    -4     9     3     2     5
## 11    11     2     6  1982     6    -4     5    -4    10     1     1     3
## 12    12     1    10  1981     2     5    -4    -4    11     1     1     4
## 13    13     1    11  1984   - 4    -4    -4    -4    12     1     0     4
## 14    14     1     7  1980     4     5    -4    -4    13     1     1     4
## 15    15     2     1  1983   - 4    -4    -4    -4    14     1     0     4
## 16    16     1     2  1982     5    -4    -4    -4    15     1     1     2
## 17    17     2    11  1981     6    -4    -4    -4    16     1     1     8
## 18    18     1     2  1982     3    -4    -4    -4    17     2     0     6
## 19    19     1     4  1984     3    -4    -4    -4    17     2     0     6
## 20    20     1    12  1980     4    -4     5    -4    18     1     1     4
## # ... with 8,964 more rows, and 33 more variables: R1211100 <int>,
## #   R1235800 <int>, R1302400 <int>, R1302500 <int>, R1482600 <int>,
## #   S0192900 <int>, S0193100 <int>, S0193500 <int>, S0193600 <int>,
## #   S0193800 <int>, S0193900 <int>, S5604900 <int>, S5605100 <int>,
## #   T3706800 <int>, T3706900 <int>, T4580500 <int>, T4580600 <int>,
## #   T4580700 <int>, T4580900 <int>, T4581100 <int>, Z0490900 <int>,
## #   Z0491000 <int>, Z0491100 <int>, Z0491200 <int>, Z0494800 <int>,
## #   Z0494900 <int>, Z0495000 <int>, Z0495100 <int>, Z0498500 <int>,
## #   Z0498700 <int>, Z0499200 <int>, Z0499400 <int>, Z0499500 <int>
## [1] 1
```

```
## Tibble size: 1.6 Mb
```

```
## Uploading from `nlsy97/97-roster.csv` to `Extract.tblRoster`.
```

```
## # A tibble: 8,984 x 416
##    R000~ R053~ R053~ R053~ R109~ R109~ R109~ R109~ R109~ R109~ R109~ R109~
##    <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int> <int>
##  1     1     2     9  1981     6     1     4     2     3     5    -4    -4
##  2     2     1     7  1982     3     2     1     4    -4    -4    -4    -4
##  3     3     2     9  1983     2     1    -4    -4    -4    -4    -4    -4
##  4     4     2     2  1981     2     1    -4    -4    -4    -4    -4    -4
##  5     5     1    10  1982     3     1     2     4    -4    -4    -4    -4
##  6     6     2     1  1982     3     4     1     2     5    -4    -4    -4
##  7     7     1     4  1983     3     4     1     2     5    -4    -4    -4
##  8     8     2     6  1981     3     4     5     2     1    -4    -4    -4
##  9     9     1    10  1982     3     4     5     2     1    -4    -4    -4
## 10    10     1     3  1984     3     4     5     2     1    -4    -4    -4
## 11    11     2     6  1982     2     1     3    -4    -4    -4    -4    -4
## 12    12     1    10  1981     4     1     3     2    -4    -4    -4    -4
## 13    13     1    11  1984     2     1     3     4     5    -4    -4    -4
## 14    14     1     7  1980     3     1     2     4    -4    -4    -4    -4
## 15    15     2     1  1983     3     1     2     4    -4    -4    -4    -4
## 16    16     1     2  1982     2     1    -4    -4    -4    -4    -4    -4
## 17    17     2    11  1981     4     1     2     3     5     6     7     8
## 18    18     1     2  1982     6     2     1     3     4     5    -4    -4
## 19    19     1     4  1984     6     2     1     3     4     5    -4    -4
## 20    20     1    12  1980     2     3     4     1    -4    -4    -4    -4
## # ... with 8,964 more rows, and 404 more variables: R1098600 <int>,
## #   R1098700 <int>, R1098800 <int>, R1098900 <int>, R1099000 <int>,
## #   R1099100 <int>, R1099200 <int>, R1099300 <int>, R1101000 <int>,
## #   R1101100 <int>, R1101200 <int>, R1101300 <int>, R1101400 <int>,
## #   R1101500 <int>, R1101600 <int>, R1101700 <int>, R1101800 <int>,
## #   R1101900 <int>, R1102000 <int>, R1102100 <int>, R1102200 <int>,
## #   R1102300 <int>, R1102400 <int>, R1102500 <int>, R1102501 <int>,
## #   R1102600 <int>, R1102700 <int>, R1102800 <int>, R1102900 <int>,
## #   R1103000 <int>, R1103100 <int>, R1103200 <int>, R1103300 <int>,
## #   R1103400 <int>, R1103500 <int>, R1103600 <int>, R1103700 <int>,
## #   R1103800 <int>, R1103900 <int>, R1104000 <int>, R1104100 <int>,
## #   R1117000 <int>, R1117100 <int>, R1117200 <int>, R1117300 <int>,
## #   R1117400 <int>, R1117500 <int>, R1117600 <int>, R1117700 <int>,
## #   R1117800 <int>, R1117900 <int>, R1118000 <int>, R1118100 <int>,
## #   R1118200 <int>, R1118300 <int>, R1118400 <int>, R1118500 <int>,
## #   R1118600 <int>, R1118700 <int>, R1118800 <int>, R1118900 <int>,
## #   R1119000 <int>, R1119100 <int>, R1119200 <int>, R1119300 <int>,
## #   R1119400 <int>, R1119500 <int>, R1119600 <int>, R1119700 <int>,
## #   R1119800 <int>, R1119900 <int>, R1120000 <int>, R1120100 <int>,
## #   R1120200 <int>, R1120300 <int>, R1120400 <int>, R1120500 <int>,
## #   R1120600 <int>, R1120700 <int>, R1120800 <int>, R1120900 <int>,
## #   R1121000 <int>, R1121100 <int>, R1121200 <int>, R1121300 <int>,
## #   R1121400 <int>, R1121500 <int>, R1121600 <int>, R1121700 <int>,
## #   R1121800 <int>, R1121900 <int>, R1122000 <int>, R1122100 <int>,
## #   R1122200 <int>, R1122300 <int>, R1122400 <int>, R1122500 <int>,
## #   R1122600 <int>, R1122700 <int>, R1122800 <int>, ...
## [1] 1
```

```
## Tibble size: 14.5 Mb
```

```r
DBI::dbDisconnect(channel_odbc); rm(channel_odbc)
RODBC::odbcClose(channel_rodbc); rm(channel_rodbc)

duration_in_seconds <- round(as.numeric(difftime(Sys.time(), start_time, units="secs")))
cat("File completed by `", Sys.info()["user"], "` at ", strftime(Sys.time(), "%Y-%m-%d, %H:%M %z"), " in ",  duration_in_seconds, " seconds.", sep="")
```

```
## File completed by `Will` at 2018-01-12, 17:04 -0600 in 19 seconds.
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
## [1] bindrcpp_0.2 magrittr_1.5
## 
## loaded via a namespace (and not attached):
##  [1] Rcpp_0.12.14          highr_0.6             plyr_1.8.4           
##  [4] pillar_1.0.1          compiler_3.4.3        bindr_0.1            
##  [7] tools_3.4.3           odbc_1.1.3            digest_0.6.13        
## [10] bit_1.1-12            memoise_1.1.0         evaluate_0.10.1      
## [13] tibble_1.4.1          checkmate_1.8.5       pkgconfig_2.0.1      
## [16] rlang_0.1.6           DBI_0.7               cli_1.0.0            
## [19] rstudioapi_0.7        yaml_2.1.16           withr_2.1.1.9000     
## [22] dplyr_0.7.4           stringr_1.2.0         knitr_1.18           
## [25] devtools_1.13.4       hms_0.4.0             bit64_0.9-7          
## [28] rprojroot_1.3-2       OuhscMunge_0.1.8.9005 glue_1.2.0           
## [31] R6_2.2.2              rmarkdown_1.8         tidyr_0.7.2          
## [34] readr_1.1.1           purrr_0.2.4           blob_1.1.0           
## [37] backports_1.1.2       scales_0.5.0.9000     RODBC_1.3-15         
## [40] htmltools_0.3.6       rsconnect_0.8.5       assertthat_0.2.0     
## [43] testit_0.7.1          colorspace_1.3-2      utf8_1.1.3           
## [46] stringi_1.1.6         munsell_0.4.3         markdown_0.8         
## [49] crayon_1.3.4
```

```r
Sys.time()
```

```
## [1] "2018-01-12 17:04:01 CST"
```

