



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
ds_mapping <- readr::read_csv("data-public/metadata/tables/_mapping-unshared.csv", col_types=col_types_mapping)
ds_mapping
```

```
## # A tibble: 13 x 2
##                     name subdirectory
##                    <chr>        <chr>
##  1          Gen1Outcomes  nlsy79-gen1
##  2          Gen1Explicit  nlsy79-gen1
##  3  Gen1GeocodeSanitized  nlsy79-gen1
##  4          Gen1Implicit  nlsy79-gen1
##  5             Gen1Links  nlsy79-gen1
##  6    Gen2OutcomesWeight  nlsy79-gen2
##  7 Gen2BirthDateFromGen1  nlsy79-gen2
##  8    Gen2FatherFromGen1  nlsy79-gen2
##  9    Gen2ImplicitFather  nlsy79-gen2
## 10             Gen2Links  nlsy79-gen2
## 11     Gen2LinksFromGen1  nlsy79-gen2
## 12    Gen2OutcomesHeight  nlsy79-gen2
## 13      Gen2OutcomesMath  nlsy79-gen2
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
## # A tibble: 13 x 5
##                     name exists subdirectory
##                    <chr>  <lgl>        <chr>
##  1          Gen1Outcomes   TRUE  nlsy79-gen1
##  2          Gen1Explicit   TRUE  nlsy79-gen1
##  3  Gen1GeocodeSanitized   TRUE  nlsy79-gen1
##  4          Gen1Implicit   TRUE  nlsy79-gen1
##  5             Gen1Links   TRUE  nlsy79-gen1
##  6    Gen2OutcomesWeight   TRUE  nlsy79-gen2
##  7 Gen2BirthDateFromGen1   TRUE  nlsy79-gen2
##  8    Gen2FatherFromGen1   TRUE  nlsy79-gen2
##  9    Gen2ImplicitFather   TRUE  nlsy79-gen2
## 10             Gen2Links   TRUE  nlsy79-gen2
## 11     Gen2LinksFromGen1   TRUE  nlsy79-gen2
## 12    Gen2OutcomesHeight   TRUE  nlsy79-gen2
## 13      Gen2OutcomesMath   TRUE  nlsy79-gen2
## # ... with 2 more variables: table_name <chr>, path <chr>
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
##  [1] "Extract.tblGen1Outcomes"          "Extract.tblGen1Explicit"         
##  [3] "Extract.tblGen1GeocodeSanitized"  "Extract.tblGen1Implicit"         
##  [5] "Extract.tblGen1Links"             "Extract.tblGen2OutcomesWeight"   
##  [7] "Extract.tblGen2BirthDateFromGen1" "Extract.tblGen2FatherFromGen1"   
##  [9] "Extract.tblGen2ImplicitFather"    "Extract.tblGen2Links"            
## [11] "Extract.tblGen2LinksFromGen1"     "Extract.tblGen2OutcomesHeight"   
## [13] "Extract.tblGen2OutcomesMath"
```

```r
ds_file
```

```
## # A tibble: 13 x 5
##                     name exists subdirectory
##                    <chr>  <lgl>        <chr>
##  1          Gen1Outcomes   TRUE  nlsy79-gen1
##  2          Gen1Explicit   TRUE  nlsy79-gen1
##  3  Gen1GeocodeSanitized   TRUE  nlsy79-gen1
##  4          Gen1Implicit   TRUE  nlsy79-gen1
##  5             Gen1Links   TRUE  nlsy79-gen1
##  6    Gen2OutcomesWeight   TRUE  nlsy79-gen2
##  7 Gen2BirthDateFromGen1   TRUE  nlsy79-gen2
##  8    Gen2FatherFromGen1   TRUE  nlsy79-gen2
##  9    Gen2ImplicitFather   TRUE  nlsy79-gen2
## 10             Gen2Links   TRUE  nlsy79-gen2
## 11     Gen2LinksFromGen1   TRUE  nlsy79-gen2
## 12    Gen2OutcomesHeight   TRUE  nlsy79-gen2
## 13      Gen2OutcomesMath   TRUE  nlsy79-gen2
## # ... with 2 more variables: table_name <chr>, path <chr>
```

```r
# Sniff out problems
```


```r
upload_start_time <- Sys.time()

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

  # summary(d)
  # d2 <- d[1:3, ]

  result_save <- RODBC::sqlSave(
    channel     = channel,
    dat         = d,
    tablename   = ds_file$table_name[[i]],
    safer       = FALSE,       # Don't keep the existing table.
    rownames    = FALSE,
    append      = FALSE        # Toggle this to 'TRUE' the first time a table is uploaded.
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
## Extract.tblGen1GeocodeSanitized : data-unshared/raw/nlsy79-gen1/Gen1GeocodeSanitized.csv 
## 0.6 Mb
## # A tibble: 5,302 x 29
##    SubjectTag_S1 SubjectTag_S2 DobDifferenceInDays1979V1979
##            <int>         <int>                        <int>
##  1           300           400                         -338
##  2           500           600                         -461
##  3          1300          1400                        -1868
##  4          1700          1800                         -413
##  5          2000          2100                         -591
##  6          2300          2400                         -828
##  7          2700          2800                        -1356
##  8          2900          3000                          698
##  9          3200          3300                        -1640
## 10          3400          3500                        -1730
## # ... with 5,292 more rows, and 26 more variables:
## #   DobDifferenceInDays1979V1981 <int>,
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
## Extract.tblGen2OutcomesWeight : data-unshared/raw/nlsy79-gen2/Gen2OutcomesWeight.csv 
## 0.6 Mb
## # A tibble: 11,504 x 13
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
## # ... with 11,494 more rows, and 5 more variables: Y1386000 <int>,
## #   Y1637700 <int>, Y1891300 <int>, Y2207200 <int>, Y2544900 <int>
## 
## Save result: 1
## ---------------------------------------------------------------
## Extract.tblGen2BirthDateFromGen1 : data-unshared/raw/nlsy79-gen2/Gen2BirthDateFromGen1.csv 
## 1.2 Mb
## # A tibble: 12,686 x 25
##    R0000100 `"R0214700"` `"R0214800"` `"R9900001"` `"R9900002"`
##       <int>        <int>        <int>        <int>        <int>
##  1        1            3            2           -4           -4
##  2        2            3            2            3         1993
##  3        3            3            2            6         1981
##  4        4            3            2            8         1980
##  5        5            3            1            5         1989
##  6        6            3            1            8         1991
##  7        7            3            1            3         1990
##  8        8            3            2            3         1976
##  9        9            3            1            3         1991
## 10       10            3            2            1         1983
## # ... with 12,676 more rows, and 20 more variables: `"R9900801"` <int>,
## #   `"R9900802"` <int>, `"R9901601"` <int>, `"R9901602"` <int>,
## #   `"R9902401"` <int>, `"R9902402"` <int>, `"R9903201"` <int>,
## #   `"R9903202"` <int>, `"R9904001"` <int>, `"R9904002"` <int>,
## #   `"R9904801"` <int>, `"R9904802"` <int>, `"R9905601"` <int>,
## #   `"R9905602"` <int>, `"R9906201"` <int>, `"R9906202"` <int>,
## #   `"R9906801"` <int>, `"R9906802"` <int>, `"R9907401"` <int>,
## #   `"R9907402"` <int>
## 
## Save result: 1
## ---------------------------------------------------------------
## Extract.tblGen2FatherFromGen1 : data-unshared/raw/nlsy79-gen2/Gen2FatherFromGen1.csv 
## 46.5 Mb
## # A tibble: 12,686 x 952
##    R0000100 R0214700 R0214800 R1373300 R1373400 R1373500 R1374000 R1374100
##       <int>    <int>    <int>    <int>    <int>    <int>    <int>    <int>
##  1        1        3        2       -5       -5       -5       -5       -5
##  2        2        3        2       -4       -4       -4       -4       -4
##  3        3        3        2        1       -4       -4        1       -4
##  4        4        3        2        0        1        4       -4       -4
##  5        5        3        1       -4       -4       -4       -4       -4
##  6        6        3        1       -4       -4       -4       -4       -4
##  7        7        3        1       -4       -4       -4       -4       -4
##  8        8        3        2        1       -4       -4        1       -4
##  9        9        3        1       -4       -4       -4       -4       -4
## 10       10        3        2        1       -4       -4       -4       -4
## # ... with 12,676 more rows, and 944 more variables: R1374200 <int>,
## #   R1374700 <int>, R1374800 <int>, R1374900 <int>, R1375400 <int>,
## #   R1375500 <int>, R1375600 <int>, R1376100 <int>, R1376200 <int>,
## #   R1376300 <int>, R1376800 <int>, R1376900 <int>, R1377000 <int>,
## #   R1377500 <int>, R1377600 <int>, R1377700 <int>, R1753700 <int>,
## #   R1753800 <int>, R1753900 <int>, R1754400 <int>, R1754500 <int>,
## #   R1754600 <int>, R1755100 <int>, R1755200 <int>, R1755300 <int>,
## #   R1755800 <int>, R1755900 <int>, R1756000 <int>, R1756500 <int>,
## #   R1756600 <int>, R1756700 <int>, R1757200 <int>, R1757300 <int>,
## #   R1757400 <int>, R1757900 <int>, R1758000 <int>, R1758100 <int>,
## #   R2095700 <int>, R2095800 <int>, R2095900 <int>, R2096400 <int>,
## #   R2096500 <int>, R2096600 <int>, R2097100 <int>, R2097200 <int>,
## #   R2097300 <int>, R2097800 <int>, R2097900 <int>, R2098000 <int>,
## #   R2098500 <int>, R2098600 <int>, R2098700 <int>, R2099200 <int>,
## #   R2099300 <int>, R2099400 <int>, R2099900 <int>, R2100000 <int>,
## #   R2100100 <int>, R2345900 <int>, R2346200 <int>, R2346500 <int>,
## #   R2346800 <int>, R2347100 <int>, R2347400 <int>, R2347700 <int>,
## #   R2648000 <int>, R2648100 <int>, R2648200 <int>, R2648700 <int>,
## #   R2648800 <int>, R2648900 <int>, R2649400 <int>, R2649500 <int>,
## #   R2649600 <int>, R2650100 <int>, R2650200 <int>, R2650300 <int>,
## #   R2650800 <int>, R2650900 <int>, R2651000 <int>, R2651500 <int>,
## #   R2651600 <int>, R2651700 <int>, R2652200 <int>, R2652300 <int>,
## #   R2652400 <int>, R2955900 <int>, R2956200 <int>, R2956500 <int>,
## #   R2956800 <int>, R2957100 <int>, R2957400 <int>, R2957700 <int>,
## #   R3255900 <int>, R3256000 <int>, R3256100 <int>, R3257700 <int>,
## #   R3257800 <int>, R3257900 <int>, R3259500 <int>, ...
## 
## Save result: 1
## ---------------------------------------------------------------
## Extract.tblGen2ImplicitFather : data-unshared/raw/nlsy79-gen2/Gen2ImplicitFather.csv 
## 4.9 Mb
## # A tibble: 11,504 x 111
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
## # ... with 11,494 more rows, and 103 more variables: C0008600 <int>,
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
## #   C3981800 <int>, C3981900 <int>, Y0003200 <int>, Y0007300 <int>,
## #   Y0007400 <int>, Y0007600 <int>, Y0007601 <int>, Y0008000 <int>,
## #   Y0008500 <int>, Y0008600 <int>, Y0009400 <int>, Y0394100 <int>,
## #   Y0394300 <int>, Y0394500 <int>, Y0394501 <int>, Y0394900 <int>,
## #   Y0651000 <int>, Y0682500 <int>, Y0683800 <int>, Y0683900 <int>,
## #   Y0684100 <int>, Y0684101 <int>, Y0684500 <int>, Y0947100 <int>,
## #   Y0986200 <int>, Y0986700 <int>, Y0988800 <int>, Y0988900 <int>,
## #   Y0989400 <int>, Y0989401 <int>, Y0989900 <int>, Y1229100 <int>,
## #   Y1229200 <int>, Y1229700 <int>, Y1229701 <int>, Y1458900 <int>,
## #   Y1459400 <int>, Y1459401 <int>, Y1629500 <int>, Y1704000 <int>,
## #   Y1704500 <int>, Y1704501 <int>, Y1707300 <int>, Y1883300 <int>,
## #   Y1989500 <int>, Y1990000 <int>, Y1990001 <int>, Y1992900 <int>,
## #   Y2197500 <int>, Y2308300 <int>, Y2308800 <int>, ...
## 
## Save result: 1
## ---------------------------------------------------------------
## Extract.tblGen2Links : data-unshared/raw/nlsy79-gen2/Gen2Links.csv 
## 7.3 Mb
## # A tibble: 11,512 x 164
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
## # ... with 11,502 more rows, and 156 more variables: C0006800 <int>,
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
## #   Y0000200 <int>, Y0000201 <int>, Y0000202 <int>, Y0002100 <int>,
## #   Y0390100 <int>, Y0390101 <int>, Y0390102 <int>, Y0677600 <int>,
## #   Y0933700 <int>, Y0933701 <int>, Y0933702 <int>, Y0974800 <int>,
## #   Y1180500 <int>, Y1180501 <int>, Y1180502 <int>, Y1192400 <int>,
## #   Y1421100 <int>, Y1421101 <int>, Y1421102 <int>, Y1434300 <int>,
## #   Y1450200 <int>, Y1450201 <int>, Y1450202 <int>, Y1672700 <int>,
## #   Y1695600 <int>, Y1695601 <int>, Y1695602 <int>, Y1707300 <int>,
## #   Y1707400 <int>, Y1707500 <int>, Y1707600 <int>, Y1707700 <int>,
## #   Y1707800 <int>, Y1707900 <int>, Y1708000 <int>, Y1708100 <int>,
## #   Y1708200 <int>, Y1708300 <int>, Y1708400 <int>, Y1708500 <int>,
## #   Y1708600 <int>, Y1708700 <int>, Y1708800 <int>, Y1708900 <int>,
## #   Y1709000 <int>, Y1709100 <int>, Y1709200 <int>, ...
## 
## Save result: 1
## ---------------------------------------------------------------
## Extract.tblGen2LinksFromGen1 : data-unshared/raw/nlsy79-gen2/Gen2LinksFromGen1.csv 
## 5.2 Mb
## # A tibble: 12,686 x 106
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
## # ... with 12,676 more rows, and 98 more variables: R4826800 <int>,
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
## #   R8255400 <int>, R9900400 <int>, R9901200 <int>, R9902000 <int>,
## #   R9902800 <int>, R9903600 <int>, R9904400 <int>, R9905200 <int>,
## #   R9906000 <int>, R9906600 <int>, R9907200 <int>, R9907800 <int>,
## #   R9908000 <int>, T0337300 <int>, T0337400 <int>, T0337500 <int>,
## #   T0337600 <int>, T0337700 <int>, T0337800 <int>, T0337900 <int>,
## #   T0338000 <int>, T0338100 <int>, T0338200 <int>, T0338300 <int>,
## #   T0338400 <int>, T0338500 <int>, T0338600 <int>, T1486900 <int>,
## #   T1487000 <int>, T1487100 <int>, T1487200 <int>, T1487300 <int>,
## #   T1487400 <int>, T1487500 <int>, T1487600 <int>, T1487700 <int>,
## #   T1487800 <int>, T2217700 <int>, T2533500 <int>, T2533600 <int>,
## #   T2533700 <int>, T2533800 <int>, T2533900 <int>, T2534000 <int>,
## #   T2534100 <int>, T2534200 <int>, T2534300 <int>, T2534400 <int>,
## #   T2534500 <int>
## 
## Save result: 1
## ---------------------------------------------------------------
## Extract.tblGen2OutcomesHeight : data-unshared/raw/nlsy79-gen2/Gen2OutcomesHeight.csv 
## 2 Mb
## # A tibble: 11,504 x 46
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
## # ... with 11,494 more rows, and 38 more variables: C0826400 <int>,
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
## 
## Save result: 1
## ---------------------------------------------------------------
## Extract.tblGen2OutcomesMath : data-unshared/raw/nlsy79-gen2/Gen2OutcomesMath.csv 
## 2 Mb
## # A tibble: 11,504 x 44
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
## # ... with 11,494 more rows, and 36 more variables: C0799400 <int>,
## #   C0799500 <int>, C0799600 <int>, C0998600 <int>, C0998700 <int>,
## #   C0998800 <int>, C1198600 <int>, C1198700 <int>, C1198800 <int>,
## #   C1507600 <int>, C1507700 <int>, C1507800 <int>, C1564500 <int>,
## #   C1564600 <int>, C1564700 <int>, C1799900 <int>, C1800000 <int>,
## #   C1800100 <int>, C2503500 <int>, C2503600 <int>, C2503700 <int>,
## #   C2532000 <int>, C2532100 <int>, C2532200 <int>, C2802800 <int>,
## #   C2802900 <int>, C2803000 <int>, C3111300 <int>, C3111400 <int>,
## #   C3111500 <int>, C3615000 <int>, C3615100 <int>, C3615200 <int>,
## #   C3993600 <int>, C3993700 <int>, C3993800 <int>
## 
## Save result: 1
## ---------------------------------------------------------------
```

```r
RODBC::odbcClose(channel); rm(channel)
cat("upload_duration_in_seconds:", round(as.numeric(difftime(Sys.time(), upload_start_time, units="secs"))), "\n")
```

```
## upload_duration_in_seconds: 32
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
##  [1] Rcpp_0.12.11     bindr_0.1        knitr_1.16       hms_0.3         
##  [5] testit_0.7       R6_2.2.1         rlang_0.1.1      stringr_1.2.0   
##  [9] dplyr_0.7.0      tools_3.4.0      htmltools_0.3.6  yaml_2.1.14     
## [13] assertthat_0.2.0 digest_0.6.12    rprojroot_1.2    tibble_1.3.3    
## [17] purrr_0.2.2.2    readr_1.1.1      tidyr_0.6.3      RODBC_1.3-15    
## [21] rsconnect_0.8    glue_1.1.0       evaluate_0.10    rmarkdown_1.6   
## [25] stringi_1.1.5    compiler_3.4.0   backports_1.1.0  markdown_0.8    
## [29] pkgconfig_2.0.1
```

```r
Sys.time()
```

```
## [1] "2017-06-22 00:19:45 CDT"
```

