



This report was automatically generated with the R package **knitr**
(version 1.16).


```r
# knitr::stitch_rmd(script="./dal/import-raw.R", output="./stitched-output/dal/import-raw.md") # dir.create(output="./stitched-output/dal/", recursive=T)
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
requireNamespace("readr"                  )
requireNamespace("tidyr"                  )
requireNamespace("tibble"                 )
requireNamespace("purrr"                  )
requireNamespace("dplyr"                  ) #Avoid attaching dplyr, b/c its function names conflict with a lot of packages (esp base, stats, and plyr).
requireNamespace("testit"                 ) #For asserting conditions meet expected patterns.
requireNamespace("RODBC"                  ) #For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.
```

```r
# Constant values that won't change.
directory_in              <- "data-unshared/raw"
schema_name               <- "Extract"

col_types_extract <- readr::cols(
  .default            = readr::col_integer()
)
col_types_mapping <- readr::cols_only(
  name                = readr::col_character(),
  subdirectory        = readr::col_character()
)
```

```r
ds_mapping <- readr::read_csv(file.path(directory_in, "_mapping.csv"), col_types=col_types_mapping)
ds_mapping
```

```
## # A tibble: 4 x 2
##           name subdirectory
##          <chr>        <chr>
## 1 Gen1Outcomes  nlsy79-gen1
## 2 Gen1Explicit  nlsy79-gen1
## 3 Gen1Implicit  nlsy79-gen1
## 4    Gen1Links  nlsy79-gen1
```

```r
ds_file <- ds_mapping %>%
  dplyr::mutate(
    table_name    = paste0(schema_name, ".tbl", name),
    path          = file.path(directory_in, subdirectory, paste0(name, ".csv")),
    exists        = purrr::map_lgl(path, file.exists)
  ) %>%
  dplyr::select(name, exists, dplyr::everything())
ds_file
```

```
## # A tibble: 4 x 5
##           name exists subdirectory              table_name
##          <chr>  <lgl>        <chr>                   <chr>
## 1 Gen1Outcomes   TRUE  nlsy79-gen1 Extract.tblGen1Outcomes
## 2 Gen1Explicit   TRUE  nlsy79-gen1 Extract.tblGen1Explicit
## 3 Gen1Implicit   TRUE  nlsy79-gen1 Extract.tblGen1Implicit
## 4    Gen1Links   TRUE  nlsy79-gen1    Extract.tblGen1Links
## # ... with 1 more variables: path <chr>
```

```r
testit::assert("All metadata files must exist.", all(ds_file$exists))
rm(ds_mapping)

# ds_entries <- ds_file %>%
#   dplyr::select(name, path) %>%
#   dplyr::mutate(
#     entries = purrr::map(.$path, readr::read_csv, col_types = col_types_extract)
#   )
# ds_entries
# print(object.size(ds_entries), units="MB")


rm(directory_in) # rm(col_types_tulsa)
```

```r
# ds_entries %>%
#   purrr::walk(print)

ds_file$table_name
```

```
## [1] "Extract.tblGen1Outcomes" "Extract.tblGen1Explicit"
## [3] "Extract.tblGen1Implicit" "Extract.tblGen1Links"
```

```r
ds_file
```

```
## # A tibble: 4 x 5
##           name exists subdirectory              table_name
##          <chr>  <lgl>        <chr>                   <chr>
## 1 Gen1Outcomes   TRUE  nlsy79-gen1 Extract.tblGen1Outcomes
## 2 Gen1Explicit   TRUE  nlsy79-gen1 Extract.tblGen1Explicit
## 3 Gen1Implicit   TRUE  nlsy79-gen1 Extract.tblGen1Implicit
## 4    Gen1Links   TRUE  nlsy79-gen1    Extract.tblGen1Links
## # ... with 1 more variables: path <chr>
```

```r
# Sniff out problems
```


```r
channel <- open_dsn_channel()
RODBC::odbcGetInfo(channel)
```

```
##              DBMS_Name               DBMS_Ver        Driver_ODBC_Ver 
## "Microsoft SQL Server"           "13.00.4202"                "03.80" 
##       Data_Source_Name            Driver_Name             Driver_Ver 
##     "local-nlsy-links"      "msodbcsql13.dll"           "14.00.0500" 
##               ODBC_Ver            Server_Name 
##           "03.80.0000" "GIMBLE\\EXPRESS_2016"
```

```r
# d <- ds_file %>%
#   dplyr::select(table_name, entries) %>%
#   dplyr::filter(table_name=="Enum.tblLURosterGen1") %>%
#   tibble::deframe() %>%
#   .[[1]]

# d2 <- d[, 1:16]
# RODBC::sqlSave(channel, dat=d, tablename="Enum.tblLURosterGen1", safer=TRUE, rownames=FALSE, append=TRUE)

for( i in seq_len(nrow(ds_file)) ) {

  cat(ds_file$table_name[[i]], ":", ds_file$path[[i]], "\n")

  d <- readr::read_csv(ds_file$path[[i]], col_types = col_types_extract)
  print(object.size(d), units="MB")
  print(d)
  cat("\n")

  result_save <- RODBC::sqlSave(
    channel     = channel,
    dat         = d,
    tablename   = ds_file$table_name[[i]],
    safer       = FALSE,       # Don't keep the existing table.
    rownames    = FALSE,
    append      = TRUE
  )

  cat("Save result:", result_save)
  cat("\n---------------------------------------------------------------\n")
}
```

```
## Extract.tblGen1Outcomes : data-unshared/raw/nlsy79-gen1/Gen1Outcomes.csv 
## 1 Mb
## # A tibble: 12,686 x 21
##    R0000100 R0214700 R0214800 R0481600 R0481700 R0618200 R0618300 R0618301
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1        1        3        2      505       -1       -4       -4       -4
##  2        2        3        2      502      120       12        9     6841
##  3        3        3        2       -5       -5       51       46    49444
##  4        4        3        2      507      110       62       48    55761
##  5        5        3        1      503      130       90       99    96772
##  6        6        3        1      504      200       99       99    99393
##  7        7        3        1      505      131       33       35    47412
##  8        8        3        2      505      179       43       42    44022
##  9        9        3        1      506      145       55       51    59683
## 10       10        3        2      506      115       27       21    30039
## # ... with 12,676 more rows, and 13 more variables: R0779800 <int>,
## #   R0779900 <int>, R1773900 <int>, R1774000 <int>, T0897300 <int>,
## #   T0897400 <int>, T0897500 <int>, T2053800 <int>, T2053900 <int>,
## #   T2054000 <int>, T3024700 <int>, T3024800 <int>, T3024900 <int>
## 
## Save result: 1
## ---------------------------------------------------------------
## Extract.tblGen1Explicit : data-unshared/raw/nlsy79-gen1/Gen1Explicit.csv 
## 4.6 Mb
## # A tibble: 12,686 x 95
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
## # ... with 12,676 more rows, and 87 more variables: R0000156 <int>,
## #   R0000157 <int>, R0000158 <int>, R0000159 <int>, R0000162 <int>,
## #   R0000163 <int>, R0000164 <int>, R0000165 <int>, R0000166 <int>,
## #   R0214700 <int>, R0214800 <int>, R4125101 <int>, R4125801 <int>,
## #   R4126501 <int>, R4127201 <int>, R4127901 <int>, R4128601 <int>,
## #   R4129301 <int>, R4130001 <int>, R4130701 <int>, R4131401 <int>,
## #   R4132101 <int>, R4132801 <int>, R4133701 <int>, R4521500 <int>,
## #   R4521700 <int>, R4521800 <int>, R4521900 <int>, R4522000 <int>,
## #   R4522100 <int>, R4522200 <int>, R4522300 <int>, R4522400 <int>,
## #   R4522500 <int>, R4522600 <int>, R4522700 <int>, R4522800 <int>,
## #   T0002000 <int>, T0002100 <int>, T0002200 <int>, T0002300 <int>,
## #   T0002400 <int>, T0002500 <int>, T0002600 <int>, T0002700 <int>,
## #   T0002800 <int>, T0002900 <int>, T0003000 <int>, T0003100 <int>,
## #   T0003200 <int>, T0003300 <int>, T0003400 <int>, T0003500 <int>,
## #   T0003600 <int>, T0003700 <int>, T0003800 <int>, T0003900 <int>,
## #   T0004000 <int>, T0004100 <int>, T0004200 <int>, T0004300 <int>,
## #   T0004400 <int>, T0004500 <int>, T2261500 <int>, T2261600 <int>,
## #   T2261700 <int>, T2261800 <int>, T2261900 <int>, T2262000 <int>,
## #   T2262100 <int>, T2262200 <int>, T2262300 <int>, T2262400 <int>,
## #   T2262500 <int>, T2262600 <int>, T2262700 <int>, T2262800 <int>,
## #   T2262900 <int>, T2263000 <int>, T2263100 <int>, T2263200 <int>,
## #   T2263300 <int>, T2263400 <int>, T2263500 <int>, T2263600 <int>,
## #   T2263700 <int>, T2263800 <int>
## 
## Save result: 1
## ---------------------------------------------------------------
## Extract.tblGen1Implicit : data-unshared/raw/nlsy79-gen1/Gen1Implicit.csv 
## 4.9 Mb
## # A tibble: 12,686 x 101
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
## # ... with 12,676 more rows, and 93 more variables: H0002400 <int>,
## #   H0002500 <int>, H0002600 <int>, H0002700 <int>, H0002800 <int>,
## #   H0002900 <int>, H0003000 <int>, H0003100 <int>, H0013600 <int>,
## #   H0013700 <int>, H0013800 <int>, H0013900 <int>, H0014000 <int>,
## #   H0014100 <int>, H0014200 <int>, H0014300 <int>, H0014400 <int>,
## #   H0014500 <int>, H0014700 <int>, H0014800 <int>, H0014900 <int>,
## #   H0015000 <int>, H0015100 <int>, H0015200 <int>, H0015300 <int>,
## #   H0015400 <int>, H0015500 <int>, H0015600 <int>, H0015700 <int>,
## #   H0015800 <int>, H0015803 <int>, H0015804 <int>, R0000100 <int>,
## #   R0006100 <int>, R0006500 <int>, R0007300 <int>, R0007700 <int>,
## #   R0007900 <int>, R0214700 <int>, R0214800 <int>, R2302900 <int>,
## #   R2303100 <int>, R2303200 <int>, R2303300 <int>, R2303500 <int>,
## #   R2303600 <int>, R2505100 <int>, R2505300 <int>, R2505400 <int>,
## #   R2505500 <int>, R2505700 <int>, R2505800 <int>, R2737900 <int>,
## #   R2837200 <int>, R2837300 <int>, R2837400 <int>, R2837500 <int>,
## #   R2837600 <int>, R2837700 <int>, R2837800 <int>, R2837900 <int>,
## #   R2838000 <int>, R2838100 <int>, R2838200 <int>, R2838300 <int>,
## #   R2838400 <int>, R2838500 <int>, R2838600 <int>, R2838700 <int>,
## #   R2838800 <int>, R2838900 <int>, R2839000 <int>, R2839100 <int>,
## #   R2839200 <int>, R2839300 <int>, R2839400 <int>, R2839500 <int>,
## #   R2839600 <int>, R2839700 <int>, R2839800 <int>, R2839900 <int>,
## #   R2840000 <int>, R2840100 <int>, R2840200 <int>, R2840300 <int>,
## #   R2840400 <int>, R2840500 <int>, R2840600 <int>, R2840700 <int>,
## #   R2840800 <int>, R2840900 <int>, R2841000 <int>, R2841100 <int>
## 
## Save result: 1
## ---------------------------------------------------------------
## Extract.tblGen1Links : data-unshared/raw/nlsy79-gen1/Gen1Links.csv 
## 4.6 Mb
## # A tibble: 12,686 x 95
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
## # ... with 12,676 more rows, and 87 more variables: R0214700 <int>,
## #   R0214800 <int>, R0216500 <int>, R0329200 <int>, R0329210 <int>,
## #   R0406510 <int>, R0410100 <int>, R0410300 <int>, R0530700 <int>,
## #   R0530800 <int>, R0619010 <int>, R0809900 <int>, R0810000 <int>,
## #   R0898310 <int>, R1045700 <int>, R1045800 <int>, R1145110 <int>,
## #   R1427500 <int>, R1427600 <int>, R1520310 <int>, R1774100 <int>,
## #   R1774200 <int>, R1794600 <int>, R1794700 <int>, R1891010 <int>,
## #   R2156200 <int>, R2156300 <int>, R2258110 <int>, R2365700 <int>,
## #   R2365800 <int>, R2445510 <int>, R2742500 <int>, R2742600 <int>,
## #   R2871300 <int>, R2986100 <int>, R2986200 <int>, R3075000 <int>,
## #   R3302500 <int>, R3302600 <int>, R3401700 <int>, R3573400 <int>,
## #   R3573500 <int>, R3657100 <int>, R3917600 <int>, R3917700 <int>,
## #   R4007600 <int>, R4100200 <int>, R4100201 <int>, R4100202 <int>,
## #   R4418700 <int>, R4500200 <int>, R4500201 <int>, R4500202 <int>,
## #   R5081700 <int>, R5167000 <int>, R5200200 <int>, R5200201 <int>,
## #   R5200202 <int>, R6435300 <int>, R6435301 <int>, R6435302 <int>,
## #   R6479800 <int>, R6963300 <int>, R6963301 <int>, R6963302 <int>,
## #   R7007500 <int>, R7656300 <int>, R7656301 <int>, R7656302 <int>,
## #   R7704800 <int>, R8423200 <int>, R8423201 <int>, R8423202 <int>,
## #   R8497200 <int>, R9908000 <int>, T0967300 <int>, T0967301 <int>,
## #   T0967302 <int>, T0989000 <int>, T2190500 <int>, T2190501 <int>,
## #   T2190502 <int>, T2210800 <int>, T2260600 <int>, T2260601 <int>,
## #   T2260602 <int>, T3108700 <int>
## 
## Save result: 1
## ---------------------------------------------------------------
```

```r
RODBC::odbcClose(channel); rm(channel)
```

The R session information (including the OS info, R version and all
packages used):


```r
sessionInfo()
```

```
## R version 3.4.0 Patched (2017-05-16 r72684)
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
## [1] bindrcpp_0.1 magrittr_1.5
## 
## loaded via a namespace (and not attached):
##  [1] Rcpp_0.12.11     tidyr_0.6.3      dplyr_0.7.0      assertthat_0.2.0
##  [5] R6_2.2.1         evaluate_0.10    stringi_1.1.5    rlang_0.1.1     
##  [9] testit_0.7       RODBC_1.3-15     tools_3.4.0      stringr_1.2.0   
## [13] readr_1.1.1      glue_1.1.0       markdown_0.8     purrr_0.2.2.2   
## [17] hms_0.3          compiler_3.4.0   knitr_1.16       bindr_0.1       
## [21] tibble_1.3.3
```

```r
Sys.time()
```

```
## [1] "2017-06-21 23:37:07 CDT"
```

