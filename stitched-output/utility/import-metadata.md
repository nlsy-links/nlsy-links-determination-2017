



This report was automatically generated with the R package **knitr**
(version 1.16).


```r
# knitr::stitch_rmd(script="./utility/import-metadata.R", output="./stitched-output/utility/import-metadata.md") # dir.create(output="./stitched-output/utility/", recursive=T)
rm(list=ls(all=TRUE))  #Clear the variables from previous runs.
```

```r
# Call `base::source()` on any repo file that defines functions needed below.  Ideally, no real operations are performed.
base::source("utility/connectivity.R")
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
schema_name               <- "Metadata"

lst_col_types <- list(
  Item = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character(),
    MinValue                            = readr::col_integer(),
    MinNonnegative                      = readr::col_integer(),
    MaxValue                            = readr::col_integer()
  ),
  LUExtractSource = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character()
  ),
  LUMarkerEvidence = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character()
  ),
  LUMarkerType = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character(),
    Explicit                            = readr::col_integer()
  ),
  LURelationshipPath = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character()
  ),
  LUSurveySource = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character()
  ),
  MzManual = readr::cols_only(
    ID                                  = readr::col_integer(),
    SubjectTag_S1                       = readr::col_integer(),
    SubjectTag_S2                       = readr::col_integer(),
    Generation                          = readr::col_integer(),
    MultipleBirthIfSameSex              = readr::col_integer(),
    IsMz                                = readr::col_integer(),
    Undecided                           = readr::col_integer(),
    Related                             = readr::col_integer(),
    Notes                               = readr::col_character()
  ),
  # RArchive = readr::cols_only(
  #   ID                                  = readr::col_integer(),
  #   AlgorithmVersion                    = readr::col_integer(),
  #   SubjectTag_S1                       = readr::col_integer(),
  #   SubjectTag_S2                       = readr::col_integer(),
  #   MultipleBirthIfSameSex              = readr::col_integer(),
  #   IsMz                                = readr::col_integer(),
  #   SameGeneration                      = readr::col_character(),
  #   RosterAssignmentID                  = readr::col_character(),
  #   RRoster                             = readr::col_character(),
  #   LastSurvey_S1                       = readr::col_integer(),
  #   LastSurvey_S2                       = readr::col_integer(),
  #   RImplicitPass1                      = readr::col_double(),
  #   RImplicit                           = readr::col_double(),
  #   RImplicitSubject                    = readr::col_double(),
  #   RImplicitMother                     = readr::col_double(),
  #   RImplicit2004                       = readr::col_double(),
  #   RExplicitOldestSibVersion           = readr::col_double(),
  #   RExplicitYoungestSibVersion         = readr::col_double(),
  #   RExplicitPass1                      = readr::col_double(),
  #   RExplicit                           = readr::col_double(),
  #   RPass1                              = readr::col_double(),
  #   R                                   = readr::col_double(),
  #   RFull                               = readr::col_double(),
  #   RPeek                               = readr::col_character()
  # ),
  Variable = readr::cols_only(
    ID                                  = readr::col_integer(),
    VariableCode                        = readr::col_character(),
    Item                                = readr::col_integer(),
    Generation                          = readr::col_integer(),
    ExtractSource                       = readr::col_integer(),
    SurveySource                        = readr::col_integer(),
    SurveyYear                          = readr::col_integer(),
    LoopIndex                           = readr::col_integer(),
    Translate                           = readr::col_integer(),
    Notes                               = readr::col_character()
  )
)
```

```r
ds_file <- names(lst_col_types) %>%
  tibble::tibble(
    name = .
  ) %>%
  dplyr::mutate(
    path     = file.path(directory_in, paste0(name, ".csv")),
    table_name = paste0(schema_name, ".tbl", name),
    # table_name = paste0("tbl", name),
    col_types = purrr::map(name, function(x) lst_col_types[[x]]),
    exists    = purrr::map_lgl(path, file.exists),
    sql_delete= paste0("DELETE FROM ", table_name)
  )

ds_file
```

```
## # A tibble: 8 x 6
##                 name                                               path
##                <chr>                                              <chr>
## 1               Item               data-public/metadata/tables/Item.csv
## 2    LUExtractSource    data-public/metadata/tables/LUExtractSource.csv
## 3   LUMarkerEvidence   data-public/metadata/tables/LUMarkerEvidence.csv
## 4       LUMarkerType       data-public/metadata/tables/LUMarkerType.csv
## 5 LURelationshipPath data-public/metadata/tables/LURelationshipPath.csv
## 6     LUSurveySource     data-public/metadata/tables/LUSurveySource.csv
## 7           MzManual           data-public/metadata/tables/MzManual.csv
## 8           Variable           data-public/metadata/tables/Variable.csv
## # ... with 4 more variables: table_name <chr>, col_types <list>,
## #   exists <lgl>, sql_delete <chr>
```

```r
testit::assert("All metadata files must exist.", all(ds_file$exists))

lst_ds <- ds_file %>%
  dplyr::select(
    file          = path,
    col_types
  ) %>%
  purrr::pmap(readr::read_csv) %>%
  purrr::set_names(nm=ds_file$table_name)

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
lst_ds$Metadata.tblVariable %>%
  purrr::map(~max(nchar(.), na.rm=T))
```

```
## $ID
## [1] 4
## 
## $VariableCode
## [1] 8
## 
## $Item
## [1] 4
## 
## $Generation
## [1] 1
## 
## $ExtractSource
## [1] 2
## 
## $SurveySource
## [1] 1
## 
## $SurveyYear
## [1] 4
## 
## $LoopIndex
## [1] 3
## 
## $Translate
## [1] 1
## 
## $Notes
## [1] 56
```

```r
# lst_ds %>%
#   purrr::map(nrow)
# lst_ds %>%
#   purrr::map(readr::spec)

names(lst_ds)
```

```
## [1] "Metadata.tblItem"               "Metadata.tblLUExtractSource"   
## [3] "Metadata.tblLUMarkerEvidence"   "Metadata.tblLUMarkerType"      
## [5] "Metadata.tblLURelationshipPath" "Metadata.tblLUSurveySource"    
## [7] "Metadata.tblMzManual"           "Metadata.tblVariable"
```

```r
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call ato `dplyr::rename()`.
```

```r
# Sniff out problems
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
# lst_ds %>%
#   purrr::map(function(x)paste(names(x)))
#

# channel <- RODBC::odbcDriverConnect("driver={SQL Server}; Server=Bee\\Bass; Database=NlsLinks; Uid=NlsyReadWrite; Pwd=nophi")
channel <- open_dsn_channel()
RODBC::odbcGetInfo(channel)
```

```
##              DBMS_Name               DBMS_Ver        Driver_ODBC_Ver 
## "Microsoft SQL Server"           "13.00.4001"                "03.80" 
##       Data_Source_Name            Driver_Name             Driver_Ver 
##     "local-nlsy-links"      "msodbcsql13.dll"           "14.00.0500" 
##               ODBC_Ver            Server_Name 
##           "03.80.0000" "GIMBLE\\EXPRESS_2016"
```

```r
# RODBC::sqlSave(channel, dat=lst_ds[[1]], tablename="Metadata.tblItem", safer=keepExistingTable, rownames=FALSE, append=F)

# delete_result <- RODBC::sqlQuery(channel, "DELETE FROM [NlsLinks].[Metadata].[tblVariable]", errors=FALSE)

delete_results <- ds_file$sql_delete %>%
  purrr::set_names(ds_file$table_name) %>%
  purrr::map_int(RODBC::sqlQuery, channel=channel, errors=FALSE)

delete_results
```

```
##               Metadata.tblItem    Metadata.tblLUExtractSource 
##                             -2                             -2 
##   Metadata.tblLUMarkerEvidence       Metadata.tblLUMarkerType 
##                             -2                             -2 
## Metadata.tblLURelationshipPath     Metadata.tblLUSurveySource 
##                             -2                             -2 
##           Metadata.tblMzManual           Metadata.tblVariable 
##                             -2                             -2
```

```r
# d <- lst_ds[["Metadata.tblMzManual"]] %>%
#   dplyr::slice(1:2)
# summary(d)

# RODBC::sqlSave(channel, dat=d, tablename="Metadata.tblMzManual", safer=FALSE, rownames=FALSE, append=T)


purrr::map2_int(
  lst_ds,
  # names(lst_ds),
  ds_file$table_name,
  function( d, table_name ) {
    RODBC::sqlSave(
      channel     = channel,
      dat         = d,
      tablename   = table_name,
      safer       = FALSE,       # Don't keep the existing table.
      rownames    = FALSE,
      append      = TRUE
    )
  }
)
```

```
##               Metadata.tblItem    Metadata.tblLUExtractSource 
##                              1                              1 
##   Metadata.tblLUMarkerEvidence       Metadata.tblLUMarkerType 
##                              1                              1 
## Metadata.tblLURelationshipPath     Metadata.tblLUSurveySource 
##                              1                              1 
##           Metadata.tblMzManual           Metadata.tblVariable 
##                              1                              1
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
## [1] bindrcpp_0.1 DBI_0.6-1    magrittr_1.5
## 
## loaded via a namespace (and not attached):
##  [1] Rcpp_0.12.11     bindr_0.1        knitr_1.16       hms_0.3         
##  [5] munsell_0.4.3    testit_0.7       colorspace_1.3-2 R6_2.2.1        
##  [9] rlang_0.1.1      stringr_1.2.0    highr_0.6        plyr_1.8.4      
## [13] dplyr_0.7.0      tools_3.4.0      assertthat_0.2.0 tibble_1.3.3    
## [17] purrr_0.2.2.2    readr_1.1.1      tidyr_0.6.3      RODBC_1.3-15    
## [21] glue_1.1.0       evaluate_0.10    stringi_1.1.5    compiler_3.4.0  
## [25] scales_0.4.1
```

```r
Sys.time()
```

```
## [1] "2017-06-21 10:10:24 CDT"
```

