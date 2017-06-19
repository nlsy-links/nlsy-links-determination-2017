



This report was automatically generated with the R package **knitr**
(version 1.16).


```r
# knitr::stitch_rmd(script="./utility/import-metadata.R", output="./stitched-output/utility/import-metadata.md") # dir.create(output="./stitched-output/utility/", recursive=T)
rm(list=ls(all=TRUE))  #Clear the variables from previous runs.
```

```r
# Call `base::source()` on any repo file that defines functions needed below.  Ideally, no real operations are performed.
```

```r
# Attach these package(s) so their functions don't need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
library(magrittr            , quietly=TRUE)
library(DBI                 , quietly=TRUE)

# Verify these packages are available on the machine, but their functions need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
requireNamespace("readr"                  )
requireNamespace("tidyr"                  )
requireNamespace("dplyr"                  ) #Avoid attaching dplyr, b/c its function names conflict with a lot of packages (esp base, stats, and plyr).
requireNamespace("testit"                 ) #For asserting conditions meet expected patterns.
# requireNamespace("RODBC") #For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.
```

```r
# Constant values that won't change.
directory_in              <- "data-public/metadata/tables"
schamea_name              <- "Metadata"

# col_types_tulsa <- readr::cols_only(
#   Month       = readr::col_date("%m/%d/%Y"),
#   FteSum      = readr::col_double(),
#   FmlaSum     = readr::col_integer()
# )
```

```r
lst_ds <- directory_in %>%
  list.files(pattern="*.csv", full.names=T) %>%
  purrr::map(readr::read_csv)
```

```
## Parsed with column specification:
## cols(
##   ID = col_integer(),
##   Label = col_character(),
##   MinValue = col_integer(),
##   MinNonnegative = col_integer(),
##   MaxValue = col_integer()
## )
```

```
## Parsed with column specification:
## cols(
##   ID = col_integer(),
##   Label = col_character()
## )
## Parsed with column specification:
## cols(
##   ID = col_integer(),
##   Label = col_character()
## )
```

```
## Parsed with column specification:
## cols(
##   ID = col_integer(),
##   Label = col_character(),
##   Explicit = col_integer()
## )
```

```
## Parsed with column specification:
## cols(
##   ID = col_integer(),
##   Label = col_character()
## )
## Parsed with column specification:
## cols(
##   ID = col_integer(),
##   Label = col_character()
## )
```

```
## Parsed with column specification:
## cols(
##   ID = col_integer(),
##   SubjectTag_S1 = col_integer(),
##   SubjectTag_S2 = col_integer(),
##   Generation = col_integer(),
##   MultipleBirthIfSameSex = col_integer(),
##   IsMz = col_integer(),
##   Undecided = col_integer(),
##   Related = col_integer(),
##   Notes = col_character()
## )
```

```
## Parsed with column specification:
## cols(
##   .default = col_double(),
##   ID = col_integer(),
##   AlgorithmVersion = col_integer(),
##   SubjectTag_S1 = col_integer(),
##   SubjectTag_S2 = col_integer(),
##   MultipleBirthIfSameSex = col_integer(),
##   IsMz = col_integer(),
##   SameGeneration = col_character(),
##   RosterAssignmentID = col_character(),
##   RRoster = col_character(),
##   LastSurvey_S1 = col_integer(),
##   LastSurvey_S2 = col_integer(),
##   RPeek = col_character()
## )
```

```
## See spec(...) for full column specifications.
```

```
## Parsed with column specification:
## cols(
##   ID = col_integer(),
##   VariableCode = col_character(),
##   Item = col_integer(),
##   Generation = col_integer(),
##   ExtractSource = col_integer(),
##   SurveySource = col_integer(),
##   SurveyYear = col_integer(),
##   LoopIndex = col_integer(),
##   Translate = col_integer(),
##   Notes = col_character()
## )
```

```r
rm(directory_in) # rm(col_types_tulsa)

lst_ds %>%
  purrr::walk(print)
```

```
## # A tibble: 108 x 5
##       ID                              Label MinValue MinNonnegative
##    <int>                              <chr>    <int>          <int>
##  1     1            IDOfOther1979RosterGen1       -4              1
##  2     2                      RosterGen1979       -4              1
##  3     3 SiblingNumberFrom1993SiblingRoster       -4              1
##  4     4           IDCodeOfOtherSiblingGen1       -5              3
##  5     5                    ShareBiomomGen1       -5              0
##  6     6                    ShareBiodadGen1       -5              0
##  7     9 IDCodeOfOtherInterviewedBiodadGen2       -7              1
##  8    10                    ShareBiodadGen2       -7              0
##  9    11               Gen1MomOfGen2Subject        2              2
## 10    13                   DateOfBirthMonth       -5              1
## # ... with 98 more rows, and 1 more variables: MaxValue <int>
## # A tibble: 11 x 2
##       ID              Label
##    <int>              <chr>
##  1     3          Gen1Links
##  2     4          Gen2Links
##  3     5  Gen2LinksFromGen1
##  4     6 Gen2ImplicitFather
##  5     7 Gen2FatherFromGen1
##  6     8       Gen1Outcomes
##  7     9 Gen2OutcomesHeight
##  8    10       Gen1Explicit
##  9    11       Gen1Implicit
## 10    12 Gen2OutcomesWeight
## 11    13   Gen2OutcomesMath
## # A tibble: 8 x 2
##      ID            Label
##   <int>            <chr>
## 1     0       Irrelevant
## 2     1 StronglySupports
## 3     2         Supports
## 4     3       Consistent
## 5     4        Ambiguous
## 6     5          Missing
## 7     6         Unlikely
## 8     7      Disconfirms
## # A tibble: 28 x 3
##       ID               Label Explicit
##    <int>               <chr>    <int>
##  1     1          RosterGen1        1
##  2     2         ShareBiomom        1
##  3     3         ShareBiodad        1
##  4     5       DobSeparation        0
##  5     6     GenderAgreement        0
##  6    10        FatherAsthma        0
##  7    11     BabyDaddyAsthma        0
##  8    12 BabyDaddyLeftHHDate        0
##  9    13  BabyDaddyDeathDate        0
## 10    14      BabyDaddyAlive        0
## # ... with 18 more rows
## # A tibble: 5 x 2
##      ID          Label
##   <int>          <chr>
## 1     1 Gen1Housemates
## 2     2   Gen2Siblings
## 3     3    Gen2Cousins
## 4     4    ParentChild
## 5     5      AuntNiece
## # A tibble: 4 x 2
##      ID       Label
##   <int>       <chr>
## 1     0 NoInterview
## 2     1        Gen1
## 3     2       Gen2C
## 4     3      Gen2YA
## # A tibble: 206 x 9
##       ID SubjectTag_S1 SubjectTag_S2 Generation MultipleBirthIfSameSex
##    <int>         <int>         <int>      <int>                  <int>
##  1     1          5003          5004          2                      2
##  2     3         14303         14304          2                      2
##  3     5         15904         15905          2                      2
##  4     6         28805         28806          2                      2
##  5     8         36504         36505          2                      2
##  6     9         67703         67704          2                      2
##  7    10         73301         73302          2                      2
##  8    12         74301         74302          2                      2
##  9    13         77502         77503          2                      2
## 10    14         93001         93002          2                      2
## # ... with 196 more rows, and 4 more variables: IsMz <int>,
## #   Undecided <int>, Related <int>, Notes <chr>
## # A tibble: 909,870 x 24
##       ID AlgorithmVersion SubjectTag_S1 SubjectTag_S2
##    <int>            <int>         <int>         <int>
##  1     1               25           201           202
##  2     2               25           301           302
##  3     3               25           301           303
##  4     4               25           302           303
##  5     5               25           401           403
##  6     6               25           801           802
##  7     7               25           801           803
##  8     8               25           802           803
##  9     9               25          1001          1002
## 10    10               25          1201          1202
## # ... with 909,860 more rows, and 20 more variables:
## #   MultipleBirthIfSameSex <int>, IsMz <int>, SameGeneration <chr>,
## #   RosterAssignmentID <chr>, RRoster <chr>, LastSurvey_S1 <int>,
## #   LastSurvey_S2 <int>, RImplicitPass1 <dbl>, RImplicit <dbl>,
## #   RImplicitSubject <dbl>, RImplicitMother <dbl>, RImplicit2004 <dbl>,
## #   RExplicitOldestSibVersion <dbl>, RExplicitYoungestSibVersion <dbl>,
## #   RExplicitPass1 <dbl>, RExplicit <dbl>, RPass1 <dbl>, R <dbl>,
## #   RFull <dbl>, RPeek <chr>
## # A tibble: 1,559 x 10
##       ID VariableCode  Item Generation ExtractSource SurveySource
##    <int>        <chr> <int>      <int>         <int>        <int>
##  1     1     R0000149   101          1             3            1
##  2     2     R0216500    16          1             3            1
##  3     3     R0406510    16          1             3            1
##  4     4     R0619010    16          1             3            1
##  5     5     R0898310    16          1             3            1
##  6     6     R1145110    16          1             3            1
##  7     7     R1520310    16          1             3            1
##  8     8     R1891010    16          1             3            1
##  9     9     R2258110    16          1             3            1
## 10    10     R2445510    16          1             3            1
## # ... with 1,549 more rows, and 4 more variables: SurveyYear <int>,
## #   LoopIndex <int>, Translate <int>, Notes <chr>
```

```r
lst_ds %>%
  purrr::map("name")
```

```
## [[1]]
## NULL
## 
## [[2]]
## NULL
## 
## [[3]]
## NULL
## 
## [[4]]
## NULL
## 
## [[5]]
## NULL
## 
## [[6]]
## NULL
## 
## [[7]]
## NULL
## 
## [[8]]
## NULL
## 
## [[9]]
## NULL
```

```r
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call ato `dplyr::rename()`.
```

```r
# # Sniff out problems
# testit::assert("The month value must be nonmissing & since 2000", all(!is.na(ds$month) & (ds$month>="2012-01-01")))
# testit::assert("The county_id value must be nonmissing & positive.", all(!is.na(ds$county_id) & (ds$county_id>0)))
# testit::assert("The county_id value must be in [1, 77].", all(ds$county_id %in% seq_len(77L)))
# testit::assert("The region_id value must be nonmissing & positive.", all(!is.na(ds$region_id) & (ds$region_id>0)))
# testit::assert("The region_id value must be in [1, 20].", all(ds$region_id %in% seq_len(20L)))
# testit::assert("The `fte` value must be nonmissing & positive.", all(!is.na(ds$fte) & (ds$fte>=0)))
# # testit::assert("The `fmla_hours` value must be nonmissing & nonnegative", all(is.na(ds$fmla_hours) | (ds$fmla_hours>=0)))
#
# testit::assert("The County-month combination should be unique.", all(!duplicated(paste(ds$county_id, ds$month))))
# testit::assert("The Region-County-month combination should be unique.", all(!duplicated(paste(ds$region_id, ds$county_id, ds$month))))
# table(paste(ds$county_id, ds$month))[table(paste(ds$county_id, ds$month))>1]
```

```r
# dput(colnames(ds)) # Print colnames for line below.
# columns_to_write <- c("county_month_id", "county_id", "month", "fte", "fte_approximated", "region_id")
# ds_slim <- ds %>%
#   dplyr::select_(.dots=columns_to_write) %>%
#   dplyr::mutate(
#     fte_approximated <- as.integer(fte_approximated)
#   )
# ds_slim
#
# rm(columns_to_write)
```


The R session information (including the OS info, R version and all
packages used):


```r
sessionInfo()
```

```
## R version 3.4.0 (2017-04-21)
## Platform: x86_64-pc-linux-gnu (64-bit)
## Running under: Ubuntu 16.04.2 LTS
## 
## Matrix products: default
## BLAS: /usr/lib/atlas-base/atlas/libblas.so.3.0
## LAPACK: /usr/lib/atlas-base/atlas/liblapack.so.3.0
## 
## locale:
##  [1] LC_CTYPE=en_US.UTF-8       LC_NUMERIC=C              
##  [3] LC_TIME=en_US.UTF-8        LC_COLLATE=en_US.UTF-8    
##  [5] LC_MONETARY=en_US.UTF-8    LC_MESSAGES=en_US.UTF-8   
##  [7] LC_PAPER=en_US.UTF-8       LC_NAME=C                 
##  [9] LC_ADDRESS=C               LC_TELEPHONE=C            
## [11] LC_MEASUREMENT=en_US.UTF-8 LC_IDENTIFICATION=C       
## 
## attached base packages:
## [1] stats     graphics  grDevices utils     datasets  methods   base     
## 
## other attached packages:
## [1] DBI_0.6-1    magrittr_1.5
## 
## loaded via a namespace (and not attached):
##  [1] Rcpp_0.12.11          lattice_0.20-35       tidyr_0.6.3          
##  [4] zoo_1.8-0             digest_0.6.12         dplyr_0.7.0          
##  [7] assertthat_0.2.0      grid_3.4.0            R6_2.2.1             
## [10] evaluate_0.10         RSQLite_1.1-2         stringi_1.1.5        
## [13] rlang_0.1.1.9000      testit_0.7            tools_3.4.0          
## [16] stringr_1.2.0         readr_1.1.1           OuhscMunge_0.1.8.9001
## [19] glue_1.1.0            purrr_0.2.2.2         hms_0.3              
## [22] compiler_3.4.0        memoise_1.1.0         knitr_1.16           
## [25] tibble_1.3.3
```

```r
Sys.time()
```

```
## [1] "2017-06-19 08:52:03 CDT"
```

