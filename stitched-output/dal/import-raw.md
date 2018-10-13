



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
directory_in              <- "data-unshared/raw"
columns_to_drop           <- c("A0002600", "Y2267000")

ds_extract <- tibble::tribble(
  ~table_name                       , ~file_name
  ,"Extract.tblGen1Explicit"         , "nlsy79-gen1/Gen1Explicit.csv"
  ,"Extract.tblGen1Implicit"         , "nlsy79-gen1/Gen1Implicit.csv"
  ,"Extract.tblGen1Links"            , "nlsy79-gen1/Gen1Links.csv"
  ,"Extract.tblGen1Outcomes"         , "nlsy79-gen1/Gen1Outcomes.csv"
  ,"Extract.tblGen1GeocodeSanitized" , "nlsy79-gen1/Gen1GeocodeSanitized.csv"
  # # "Process.tblLURosterGen1"         , "nlsy79-gen1/RosterGen1.csv"
  # # tblGen1MzDzDistinction2010
  # #
  ,"Extract.tblGen2FatherFromGen1"   , "nlsy79-gen2/Gen2FatherFromGen1.csv"
  ,"Extract.tblGen2ImplicitFather"   , "nlsy79-gen2/Gen2ImplicitFather.csv"
  ,"Extract.tblGen2Links"            , "nlsy79-gen2/Gen2Links.csv"
  ,"Extract.tblGen2LinksFromGen1"    , "nlsy79-gen2/Gen2LinksFromGen1.csv"
  ,"Extract.tblGen2OutcomesHeight"   , "nlsy79-gen2/Gen2OutcomesHeight.csv"
  ,"Extract.tblGen2OutcomesMath"     , "nlsy79-gen2/Gen2OutcomesMath.csv"
  ,"Extract.tblGen2OutcomesWeight"   , "nlsy79-gen2/Gen2OutcomesWeight.csv"

  # "Extract.tbl97Roster"             , "nlsy97/97-roster.csv"
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
## # A tibble: 12 x 6
##    table_name  file_name   path    extract_exist sql_select  sql_truncate 
##    <chr>       <chr>       <chr>   <lgl>         <chr>       <chr>        
##  1 Extract.tb~ nlsy79-gen~ data-u~ TRUE          SELECT TOP~ TRUNCATE TAB~
##  2 Extract.tb~ nlsy79-gen~ data-u~ TRUE          SELECT TOP~ TRUNCATE TAB~
##  3 Extract.tb~ nlsy79-gen~ data-u~ TRUE          SELECT TOP~ TRUNCATE TAB~
##  4 Extract.tb~ nlsy79-gen~ data-u~ TRUE          SELECT TOP~ TRUNCATE TAB~
##  5 Extract.tb~ nlsy79-gen~ data-u~ TRUE          SELECT TOP~ TRUNCATE TAB~
##  6 Extract.tb~ nlsy79-gen~ data-u~ TRUE          SELECT TOP~ TRUNCATE TAB~
##  7 Extract.tb~ nlsy79-gen~ data-u~ TRUE          SELECT TOP~ TRUNCATE TAB~
##  8 Extract.tb~ nlsy79-gen~ data-u~ TRUE          SELECT TOP~ TRUNCATE TAB~
##  9 Extract.tb~ nlsy79-gen~ data-u~ TRUE          SELECT TOP~ TRUNCATE TAB~
## 10 Extract.tb~ nlsy79-gen~ data-u~ TRUE          SELECT TOP~ TRUNCATE TAB~
## 11 Extract.tb~ nlsy79-gen~ data-u~ TRUE          SELECT TOP~ TRUNCATE TAB~
## 12 Extract.tb~ nlsy79-gen~ data-u~ TRUE          SELECT TOP~ TRUNCATE TAB~
```


```r
# Sniff out problems
```


```r
channel_odbc <- open_dsn_channel_odbc()
```

```
## Error in checkSubset(x, choices, empty.ok, fmatch): argument "study" is missing, with no default
```

```r
DBI::dbGetInfo(channel_odbc)
```

```
## Error in DBI::dbGetInfo(channel_odbc): object 'channel_odbc' not found
```

```r
channel_rodbc <- open_dsn_channel_rodbc()
```

```
## Error in checkSubset(x, choices, empty.ok, fmatch): argument "study" is missing, with no default
```

```r
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
```

```
## Error in DBI::dbGetQuery(channel_odbc, ds_extract$sql_truncate[i]): object 'channel_odbc' not found
```

```r
DBI::dbDisconnect(channel_odbc); rm(channel_odbc)
```

```
## Error in DBI::dbDisconnect(channel_odbc): object 'channel_odbc' not found
```

```
## Warning in rm(channel_odbc): object 'channel_odbc' not found
```

```r
RODBC::odbcClose(channel_rodbc); rm(channel_rodbc)
```

```
## Error in odbcValidChannel(channel): object 'channel_rodbc' not found
```

```
## Warning in rm(channel_rodbc): object 'channel_rodbc' not found
```

```r
duration_in_seconds <- round(as.numeric(difftime(Sys.time(), start_time, units="secs")))
cat("File completed by `", Sys.info()["user"], "` at ", strftime(Sys.time(), "%Y-%m-%d, %H:%M %z"), " in ",  duration_in_seconds, " seconds.", sep="")
```

```
## File completed by `Will` at 2018-06-20, 12:30 -0500 in 1 seconds.
```

The R session information (including the OS info, R version and all
packages used):


```r
sessionInfo()
```

```
## R version 3.5.0 Patched (2018-05-14 r74725)
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
## [1] bindrcpp_0.2.2 magrittr_1.5  
## 
## loaded via a namespace (and not attached):
##  [1] Rcpp_0.12.17     pillar_1.2.3     compiler_3.5.0   plyr_1.8.4      
##  [5] bindr_0.1.1      tools_3.5.0      odbc_1.1.5       digest_0.6.15   
##  [9] bit_1.1-14       evaluate_0.10.1  tibble_1.4.2     checkmate_1.8.6 
## [13] pkgconfig_2.0.1  rlang_0.2.1      DBI_1.0.0        cli_1.0.0       
## [17] rstudioapi_0.7   yaml_2.1.19      dplyr_0.7.5      stringr_1.3.1   
## [21] knitr_1.20       hms_0.4.2.9000   bit64_0.9-7      rprojroot_1.3-2 
## [25] tidyselect_0.2.4 glue_1.2.0       R6_2.2.2         rmarkdown_1.10  
## [29] tidyr_0.8.1      readr_1.2.0      purrr_0.2.5      blob_1.1.1      
## [33] scales_0.5.0     backports_1.1.2  RODBC_1.3-15     htmltools_0.3.6 
## [37] rsconnect_0.8.8  assertthat_0.2.0 testit_0.8       colorspace_1.3-2
## [41] config_0.3       utf8_1.1.4       stringi_1.2.3    munsell_0.5.0   
## [45] markdown_0.8     crayon_1.3.4
```

```r
Sys.time()
```

```
## [1] "2018-06-20 12:30:08 CDT"
```

