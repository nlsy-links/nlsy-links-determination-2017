



This report was automatically generated with the R package **knitr**
(version 1.18).


```r
# knitr::stitch_rmd(script="./dal/import-79-metadata.R", output="./stitched-output/dal/import-metadata.md") # dir.create(output="./stitched-output/dal/", recursive=T)
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
requireNamespace("odbc"                   ) #For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.
```

```r
# Constant values that won't change.
directory_in              <- "data-public/metadata/tables-79"

col_types_minimal <- readr::cols_only(
  ID                                  = readr::col_integer(),
  Label                               = readr::col_character(),
  Active                              = readr::col_logical(),
  Notes                               = readr::col_character()
)

# The order of this list matters.
#   - Tables are WRITTEN from top to bottom.
#   - Tables are DELETED from bottom to top.
lst_col_types <- list(
  item = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character(),
    MinValue                            = readr::col_integer(),
    MinNonnegative                      = readr::col_integer(),
    MaxValue                            = readr::col_integer(),
    Active                              = readr::col_logical(),
    Notes                               = readr::col_character()
  ),
  # item_97 = readr::cols_only(
  #   ID                                  = readr::col_integer(),
  #   Label                               = readr::col_character(),
  #   MinValue                            = readr::col_integer(),
  #   MinNonnegative                      = readr::col_integer(),
  #   MaxValue                            = readr::col_integer(),
  #   Active                              = readr::col_logical(),
  #   Notes                               = readr::col_character()
  # ),
  LUExtractSource = col_types_minimal,
  LUMarkerEvidence = col_types_minimal,
  LUGender = col_types_minimal,
  LUMarkerType = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character(),
    Explicit                            = readr::col_integer(),
    Active                              = readr::col_logical(),
    Notes                               = readr::col_character()
  ),
  LUMultipleBirth = col_types_minimal,
  LURaceCohort = col_types_minimal,
  LURelationshipPath = col_types_minimal,
  LURosterGen1 = col_types_minimal,
  LUSurveySource = col_types_minimal,
  LUTristate = col_types_minimal,
  LUYesNo = col_types_minimal,
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
  #   ID                                = readr::col_integer(),
  #   AlgorithmVersion                  = readr::col_integer(),
  #   SubjectTag_S1                     = readr::col_integer(),
  #   SubjectTag_S2                     = readr::col_integer(),
  #   MultipleBirthIfSameSex            = readr::col_integer(),
  #   IsMz                              = readr::col_integer(),
  #   SameGeneration                    = readr::col_character(),
  #   RosterAssignmentID                = readr::col_character(),
  #   RRoster                           = readr::col_character(),
  #   LastSurvey_S1                     = readr::col_integer(),
  #   LastSurvey_S2                     = readr::col_integer(),
  #   RImplicitPass1                    = readr::col_double(),
  #   RImplicit                         = readr::col_double(),
  #   RImplicitSubject                  = readr::col_double(),
  #   RImplicitMother                   = readr::col_double(),
  #   RImplicit2004                     = readr::col_double(),
  #   RExplicitOldestSibVersion         = readr::col_double(),
  #   RExplicitYoungestSibVersion       = readr::col_double(),
  #   RExplicitPass1                    = readr::col_double(),
  #   RExplicit                         = readr::col_double(),
  #   RPass1                            = readr::col_double(),
  #   R                                 = readr::col_double(),
  #   RFull                             = readr::col_double(),
  #   RPeek                             = readr::col_character()
  # ),
  RosterGen1Assignment    = readr::cols_only(
    ID                                  = readr::col_integer(),
    ResponseLower                       = readr::col_integer(),
    ResponseUpper                       = readr::col_integer(),
    Freq                                = readr::col_integer(),
    Resolved                            = readr::col_integer(),
    R                                   = readr::col_double(),
    RBoundLower                         = readr::col_double(),
    RBoundUpper                         = readr::col_double(),
    SameGeneration                      = readr::col_integer(),
    ShareBiodad                         = readr::col_integer(),
    ShareBiomom                         = readr::col_integer(),
    ShareBiograndparent                 = readr::col_integer(),
    Inconsistent                        = readr::col_integer(),
    Notes                               = readr::col_character(),
    ResponseLowerLabel                  = readr::col_character(),
    ResponseUpperLabel                  = readr::col_character()
  ),
  variable = readr::cols_only(
    # ID                                  = readr::col_integer(),
    VariableCode                        = readr::col_character(),
    Item                                = readr::col_integer(),
    Generation                          = readr::col_integer(),
    ExtractSource                       = readr::col_integer(),
    SurveySource                        = readr::col_integer(),
    SurveyYear                          = readr::col_integer(),
    LoopIndex                           = readr::col_integer(),
    Translate                           = readr::col_integer(),
    Notes                               = readr::col_character(),
    Active                              = readr::col_integer(),
    Notes                               = readr::col_character()
  )
  # variable_97 = readr::cols_only(
  #   # ID                                  = readr::col_integer(),
  #   VariableCode                        = readr::col_character(),
  #   Item                                = readr::col_integer(),
  #   Generation                          = readr::col_integer(),
  #   ExtractSource                       = readr::col_integer(),
  #   SurveySource                        = readr::col_integer(),
  #   SurveyYear                          = readr::col_integer(),
  #   LoopIndex                           = readr::col_integer(),
  #   Translate                           = readr::col_integer(),
  #   Active                              = readr::col_integer(),
  #   Notes                               = readr::col_character()
  # )
)

col_types_mapping <- readr::cols_only(
  table_name          = readr::col_character(),
  schema_name         = readr::col_character(),
  enum_name           = readr::col_character(),
  # enum_file         = readr::col_character(),
  c_sharp_type        = readr::col_character(),
  convert_to_enum     = readr::col_logical()
)
```

```r
start_time <- Sys.time()

ds_mapping <- readr::read_csv(file.path(directory_in, "_mapping.csv"), col_types=col_types_mapping)
ds_mapping
```

```
## # A tibble: 17 x 5
##    table_name           schema_name enum_name        c_sharp_type convert~
##    <chr>                <chr>       <chr>            <chr>        <lgl>   
##  1 item                 Metadata    Item             short        T       
##  2 #item_97             Metadata    item_97          short        T       
##  3 LUExtractSource      Enum        ExtractSource    byte         T       
##  4 LUGender             Enum        Gender           byte         T       
##  5 LUMarkerEvidence     Enum        MarkerEvidence   byte         T       
##  6 LUMarkerType         Enum        MarkerType       byte         T       
##  7 LUMultipleBirth      Enum        MultipleBirth    byte         T       
##  8 LURaceCohort         Enum        RaceCohort       byte         T       
##  9 LURelationshipPath   Enum        RelationshipPath byte         T       
## 10 LURosterGen1         Enum        RosterGen1       short        T       
## 11 LUSurveySource       Enum        SurveySource     byte         T       
## 12 LUTristate           Enum        Tristate         byte         T       
## 13 LUYesNo              Enum        YesNo            short        T       
## 14 MzManual             Metadata    NA_character     NA_character F       
## 15 RosterGen1Assignment Metadata    NA_character     NA_character F       
## 16 variable             Metadata    NA_character     NA_character F       
## 17 #variable_97         Metadata    NA_character     NA_character F
```

```r
ds_file <- lst_col_types %>%
  tibble::enframe(value = "col_types") %>%
  dplyr::mutate(
    path     = file.path(directory_in, paste0(name, ".csv")),
    # col_types = purrr::map(name, function(x) lst_col_types[[x]]),
    exists    = purrr::map_lgl(path, file.exists)
  ) %>%
  dplyr::select(name, path, dplyr::everything())
ds_file
```

```
## # A tibble: 15 x 4
##    name                 path                              col_types  exis~
##    <chr>                <chr>                             <list>     <lgl>
##  1 item                 data-public/metadata/tables-79/i~ <S3: col_~ T    
##  2 LUExtractSource      data-public/metadata/tables-79/L~ <S3: col_~ T    
##  3 LUMarkerEvidence     data-public/metadata/tables-79/L~ <S3: col_~ T    
##  4 LUGender             data-public/metadata/tables-79/L~ <S3: col_~ T    
##  5 LUMarkerType         data-public/metadata/tables-79/L~ <S3: col_~ T    
##  6 LUMultipleBirth      data-public/metadata/tables-79/L~ <S3: col_~ T    
##  7 LURaceCohort         data-public/metadata/tables-79/L~ <S3: col_~ T    
##  8 LURelationshipPath   data-public/metadata/tables-79/L~ <S3: col_~ T    
##  9 LURosterGen1         data-public/metadata/tables-79/L~ <S3: col_~ T    
## 10 LUSurveySource       data-public/metadata/tables-79/L~ <S3: col_~ T    
## 11 LUTristate           data-public/metadata/tables-79/L~ <S3: col_~ T    
## 12 LUYesNo              data-public/metadata/tables-79/L~ <S3: col_~ T    
## 13 MzManual             data-public/metadata/tables-79/M~ <S3: col_~ T    
## 14 RosterGen1Assignment data-public/metadata/tables-79/R~ <S3: col_~ T    
## 15 variable             data-public/metadata/tables-79/v~ <S3: col_~ T
```

```r
testit::assert("All metadata files must exist.", all(ds_file$exists))

ds_entries <- ds_file %>%
  # dplyr::slice(15) %>%
  dplyr::select(name, path, col_types) %>%
  dplyr::mutate(
    entries = purrr::pmap(list(file=.$path, col_types=.$col_types), readr::read_csv, comment = "#")
  )
ds_entries
```

```
## # A tibble: 15 x 4
##    name                 path                        col_types entries     
##    <chr>                <chr>                       <list>    <list>      
##  1 item                 data-public/metadata/table~ <S3: col~ <tibble [11~
##  2 LUExtractSource      data-public/metadata/table~ <S3: col~ <tibble [11~
##  3 LUMarkerEvidence     data-public/metadata/table~ <S3: col~ <tibble [8 ~
##  4 LUGender             data-public/metadata/table~ <S3: col~ <tibble [3 ~
##  5 LUMarkerType         data-public/metadata/table~ <S3: col~ <tibble [28~
##  6 LUMultipleBirth      data-public/metadata/table~ <S3: col~ <tibble [5 ~
##  7 LURaceCohort         data-public/metadata/table~ <S3: col~ <tibble [3 ~
##  8 LURelationshipPath   data-public/metadata/table~ <S3: col~ <tibble [5 ~
##  9 LURosterGen1         data-public/metadata/table~ <S3: col~ <tibble [67~
## 10 LUSurveySource       data-public/metadata/table~ <S3: col~ <tibble [5 ~
## 11 LUTristate           data-public/metadata/table~ <S3: col~ <tibble [3 ~
## 12 LUYesNo              data-public/metadata/table~ <S3: col~ <tibble [6 ~
## 13 MzManual             data-public/metadata/table~ <S3: col~ <tibble [20~
## 14 RosterGen1Assignment data-public/metadata/table~ <S3: col~ <tibble [50~
## 15 variable             data-public/metadata/table~ <S3: col~ <tibble [1,~
```

```r
# d <- readr::read_csv("data-public/metadata/tables/variable_79.csv", col_types=lst_col_types$variable_79, comment = "#")
# readr::problems(d)
# ds_entries$entries[15]

ds_table <- database_inventory()
ds_table
```

```
##    schema_name                 table_name row_count column_count
## 1      Archive      tblArchiveDescription        56            4
## 2      Archive    tblRelatedValuesArchive    714116           24
## 3          dbo                sysdiagrams         4            5
## 4         Enum    tblLUBioparent-not-used         0            2
## 5         Enum         tblLUExtractSource        11            4
## 6         Enum                tblLUGender         3            4
## 7         Enum        tblLUMarkerEvidence         8            4
## 8         Enum            tblLUMarkerType        28            5
## 9         Enum         tblLUMultipleBirth         5            4
## 10        Enum            tblLURaceCohort         3            4
## 11        Enum      tblLURelationshipPath         5            4
## 12        Enum            tblLURosterGen1        67            4
## 13        Enum          tblLUSurveySource         5            4
## 14        Enum              tblLUTristate         3            4
## 15        Enum                 tblLUYesNo         6            4
## 16     Extract                tbl97Roster         0          416
## 17     Extract            tblGen1Explicit     12686           96
## 18     Extract    tblGen1GeocodeSanitized      5302           29
## 19     Extract            tblGen1Implicit     12686          102
## 20     Extract               tblGen1Links     12686          117
## 21     Extract tblGen1MzDzDistinction2010         0            7
## 22     Extract            tblGen1Outcomes     12686           22
## 23     Extract      tblGen2FatherFromGen1     12686          959
## 24     Extract      tblGen2ImplicitFather     11521          111
## 25     Extract               tblGen2Links     11521          207
## 26     Extract       tblGen2LinksFromGen1     12686          123
## 27     Extract      tblGen2OutcomesHeight     11521           46
## 28     Extract        tblGen2OutcomesMath     11521           44
## 29     Extract      tblGen2OutcomesWeight     11521           31
## 30     Extract           tblLinks2004Gen1      3890            9
## 31     Extract           tblLinks2004Gen2     12855            5
## 32    Metadata                    tblItem       110            7
## 33    Metadata                 tblItem_97         7            7
## 34    Metadata                tblMzManual       208            9
## 35    Metadata    tblRosterGen1Assignment        50           16
## 36    Metadata                tblVariable      1642           10
## 37    Metadata             tblVariable_97        37           10
## 38     Process               tblBabyDaddy         0           11
## 39     Process            tblFatherOfGen2         0            7
## 40     Process                 tblIRDemo1         0            5
## 41     Process              tblMarkerGen1         0           10
## 42     Process              tblMarkerGen2         0            8
## 43     Process                 tblOutcome         0            5
## 44     Process    tblParentsOfGen1Current         0           19
## 45     Process      tblParentsOfGen1Retro         0            7
## 46     Process        tblRelatedStructure         0            6
## 47     Process           tblRelatedValues         0           24
## 48     Process                tblResponse         0            9
## 49     Process              tblRosterGen1         0           13
## 50     Process                 tblSubject         0            5
## 51     Process          tblSubjectDetails         0           15
## 52     Process              tblSurveyTime         0            7
```

```r
rm(directory_in) # rm(col_types_tulsa)
```

```r
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call ato `dplyr::rename()`.

ds_file <- ds_file %>%
  dplyr::left_join( ds_mapping, by=c("name"="table_name")) %>%
  dplyr::mutate(
    table_name    = paste0("tbl", name),
    sql_delete    = glue::glue("DELETE FROM {schema_name}.{table_name};")
    # table_name    = paste0(schema_name, ".tbl", name),
    # sql_delete    = paste0("DELETE FROM ", table_name)
  ) %>%
  dplyr::left_join(
    ds_entries %>%
      dplyr::select(name, entries)
    , by="name"
  )
rm(ds_entries)

ds_file$entries %>%
  purrr::walk(print)
```

```
## # A tibble: 110 x 7
##       ID Label                              MinVa~ MinN~ MaxV~ Acti~ Notes
##    <int> <chr>                               <int> <int> <int> <lgl> <chr>
##  1     1 IDOfOther1979RosterGen1                -4     1 12557 T     <NA> 
##  2     2 RosterGen1979                          -4     1    66 T     <NA> 
##  3     3 SiblingNumberFrom1993SiblingRoster     -4     1    99 T     <NA> 
##  4     4 IDCodeOfOtherSiblingGen1               -5     3 12518 T     <NA> 
##  5     5 ShareBiomomGen1                        -5     0     2 T     <NA> 
##  6     6 ShareBiodadGen1                        -5     0     2 T     <NA> 
##  7     9 IDCodeOfOtherInterviewedBiodadGen2     -7     1    11 T     <NA> 
##  8    10 ShareBiodadGen2                        -7     0     3 T     <NA> 
##  9    11 Gen1MomOfGen2Subject                    2     2 12675 T     <NA> 
## 10    13 DateOfBirthMonth                       -5     1    12 T     <NA> 
## # ... with 100 more rows
## # A tibble: 11 x 4
##       ID Label              Active Notes
##    <int> <chr>              <lgl>  <chr>
##  1     3 Gen1Links          T      <NA> 
##  2     4 Gen2Links          T      <NA> 
##  3     5 Gen2LinksFromGen1  T      <NA> 
##  4     6 Gen2ImplicitFather T      <NA> 
##  5     7 Gen2FatherFromGen1 T      <NA> 
##  6     8 Gen1Outcomes       T      <NA> 
##  7     9 Gen2OutcomesHeight T      <NA> 
##  8    10 Gen1Explicit       T      <NA> 
##  9    11 Gen1Implicit       T      <NA> 
## 10    12 Gen2OutcomesWeight T      <NA> 
## 11    13 Gen2OutcomesMath   T      <NA> 
## # A tibble: 8 x 4
##      ID Label            Active Notes
##   <int> <chr>            <lgl>  <chr>
## 1     0 Irrelevant       T      <NA> 
## 2     1 StronglySupports T      <NA> 
## 3     2 Supports         T      <NA> 
## 4     3 Consistent       T      <NA> 
## 5     4 Ambiguous        T      <NA> 
## 6     5 Missing          T      <NA> 
## 7     6 Unlikely         T      <NA> 
## 8     7 Disconfirms      T      <NA> 
## # A tibble: 3 x 4
##      ID Label           Active Notes
##   <int> <chr>           <lgl>  <chr>
## 1     1 Male            T      <NA> 
## 2     2 Female          T      <NA> 
## 3   255 InvalidSkipGen2 T      <NA> 
## # A tibble: 28 x 5
##       ID Label               Explicit Active Notes
##    <int> <chr>                  <int> <lgl>  <chr>
##  1     1 RosterGen1                 1 T      <NA> 
##  2     2 ShareBiomom                1 T      <NA> 
##  3     3 ShareBiodad                1 T      <NA> 
##  4     5 DobSeparation              0 T      <NA> 
##  5     6 GenderAgreement            0 T      <NA> 
##  6    10 FatherAsthma               0 T      <NA> 
##  7    11 BabyDaddyAsthma            0 T      <NA> 
##  8    12 BabyDaddyLeftHHDate        0 T      <NA> 
##  9    13 BabyDaddyDeathDate         0 T      <NA> 
## 10    14 BabyDaddyAlive             0 T      <NA> 
## # ... with 18 more rows
## # A tibble: 5 x 4
##      ID Label      Active Notes                                           
##   <int> <chr>      <lgl>  <chr>                                           
## 1     0 No         T      <NA>                                            
## 2     2 Twin       T      <NA>                                            
## 3     3 Trip       T      <NA>                                            
## 4     4 TwinOrTrip T      Currently Then Gen1 algorithm doesn't distingui~
## 5   255 DoNotKnow  T      <NA>                                            
## # A tibble: 3 x 4
##      ID Label    Active Notes
##   <int> <chr>    <lgl>  <chr>
## 1     1 Hispanic T      <NA> 
## 2     2 Black    T      <NA> 
## 3     3 Nbnh     T      <NA> 
## # A tibble: 5 x 4
##      ID Label          Active Notes                               
##   <int> <chr>          <lgl>  <chr>                               
## 1     1 Gen1Housemates T      <NA>                                
## 2     2 Gen2Siblings   T      <NA>                                
## 3     3 Gen2Cousins    T      <NA>                                
## 4     4 ParentChild    T      <NA>                                
## 5     5 AuntNiece      T      Actually (Uncle|Aunt)-(Nephew|Niece)
## # A tibble: 67 x 4
##       ID Label       Active Notes
##    <int> <chr>       <lgl>  <chr>
##  1    -4 ValidSkip   T      <NA> 
##  2    -3 InvalidSkip T      <NA> 
##  3    -1 Refusal     T      <NA> 
##  4     0 Respondent  T      <NA> 
##  5     1 Spouse      T      <NA> 
##  6     2 Son         T      <NA> 
##  7     3 Daughter    T      <NA> 
##  8     4 Father      T      <NA> 
##  9     5 Mother      T      <NA> 
## 10     6 Brother     T      <NA> 
## # ... with 57 more rows
## # A tibble: 5 x 4
##      ID Label       Active Notes
##   <int> <chr>       <lgl>  <chr>
## 1     0 NoInterview T      <NA> 
## 2     1 Gen1        T      <NA> 
## 3     2 Gen2C       T      <NA> 
## 4     3 Gen2YA      T      <NA> 
## 5     4 97          T      <NA> 
## # A tibble: 3 x 4
##      ID Label     Active Notes
##   <int> <chr>     <lgl>  <chr>
## 1     0 No        T      <NA> 
## 2     1 Yes       T      <NA> 
## 3   255 DoNotKnow T      <NA> 
## # A tibble: 6 x 4
##      ID Label                               Active Notes
##   <int> <chr>                               <lgl>  <chr>
## 1    -6 ValidSkipOrNoInterviewOrNotInSurvey T      <NA> 
## 2    -3 InvalidSkip                         T      <NA> 
## 3    -2 DoNotKnow                           T      <NA> 
## 4    -1 Refusal                             T      <NA> 
## 5     0 No                                  T      <NA> 
## 6     1 Yes                                 T      <NA> 
## # A tibble: 208 x 9
##       ID SubjectTag_S1 SubjectTag_S2 Gener~ Multi~  IsMz Unde~ Rela~ Notes
##    <int>         <int>         <int>  <int>  <int> <int> <int> <int> <chr>
##  1     1          5003          5004      2      2     0     0     1 Very~
##  2     3         14303         14304      2      2     0     0     1 Diff~
##  3     5         15904         15905      2      2     0     0     1 <NA> 
##  4     6         28805         28806      2      2     0     0     1 Diff~
##  5     8         36504         36505      2      2     1     0     1 Twic~
##  6     9         67703         67704      2      2     0     0     1 1994~
##  7    10         73301         73302      2      2     1     0     1 Most~
##  8    12         74301         74302      2      2     0     0     1 Diff~
##  9    13         77502         77503      2      2     1     0     1 1994~
## 10    14         93001         93002      2      2     1     0     1 1994~
## # ... with 198 more rows
## # A tibble: 50 x 16
##       ID Respo~ Respo~  Freq Resol~      R RBoun~ RBoun~ Same~ Shar~ Shar~
##    <int>  <int>  <int> <int>  <int>  <dbl>  <dbl>  <dbl> <int> <int> <int>
##  1     1     -3    - 3    67      0 NA      0      1.00    255   255   255
##  2     2     -3    - 1    11      0 NA      0      1.00    255   255   255
##  3     3     -3     33     6      1  0      0      0         1     0     0
##  4     4     -3     36    35      1  0      0      0       255     0     0
##  5     5     -1     18     1      1  0.125  0.125  0.125     0     0     0
##  6     6     -1     36     3      1  0      0      0       255     0     0
##  7     7      1      1   167      1  0      0      0         1     0     0
##  8     8      1     33     1      1  0      0      0         1     0     0
##  9     9      1     36     1      1  0      0      0         1     0     0
## 10    10      1     57     1      1  0      0      0         1     0     0
## # ... with 40 more rows, and 5 more variables: ShareBiograndparent <int>,
## #   Inconsistent <int>, Notes <chr>, ResponseLowerLabel <chr>,
## #   ResponseUpperLabel <chr>
## # A tibble: 1,642 x 10
##    VariableCode  Item Generation Extr~ Surv~ Surv~ Loop~ Tran~ Acti~ Notes
##    <chr>        <int>      <int> <int> <int> <int> <int> <int> <int> <chr>
##  1 R0000100       100          1     3     1  1979     0     0     1 Is r~
##  2 C0000100       100          2     6     2     0     0     0     1 Is r~
##  3 R0000149       101          1     3     1  1979     0     0     1 Is r~
##  4 R0214800       102          1     3     1  1979     0     0     1 Is r~
##  5 C0005400       102          2     4     2     0     0     0     1 Is r~
##  6 R0214700       103          1     3     1  1979     0     1     1 No m~
##  7 C0005300       103          2     4     2     0     0     1     1 no m~
##  8 R0000150         1          1    10     1  1979     1     1     1 <NA> 
##  9 R0000152         1          1    10     1  1979     2     1     1 <NA> 
## 10 R0000154         1          1    10     1  1979     3     1     1 <NA> 
## # ... with 1,632 more rows
```

```r
# ds_file %>%
#   dplyr::group_by(name) %>%
#   dplyr::mutate(
#     a = purrr::map_int(entries, ~max(nchar(.), na.rm=T))
#   ) %>%
#   dplyr::ungroup() %>%
#   dplyr::pull(a)


# ds_file %>%
#   dplyr::select(name, entries) %>%
#   tibble::deframe() %>%
#   purrr::map(~max(nchar(.), na.rm=T))

# lst_ds %>%
#   purrr::map(nrow)
# lst_ds %>%
#   purrr::map(readr::spec)

ds_file$table_name
```

```
##  [1] "tblitem"                 "tblLUExtractSource"     
##  [3] "tblLUMarkerEvidence"     "tblLUGender"            
##  [5] "tblLUMarkerType"         "tblLUMultipleBirth"     
##  [7] "tblLURaceCohort"         "tblLURelationshipPath"  
##  [9] "tblLURosterGen1"         "tblLUSurveySource"      
## [11] "tblLUTristate"           "tblLUYesNo"             
## [13] "tblMzManual"             "tblRosterGen1Assignment"
## [15] "tblvariable"
```

```r
ds_file
```

```
## # A tibble: 15 x 11
##    name   path    col_t~ exists sche~ enum~ c_sh~ conv~ tabl~ sql_d~ entr~
##    <chr>  <chr>   <list> <lgl>  <chr> <chr> <chr> <lgl> <chr> <chr>  <lis>
##  1 item   data-p~ <S3: ~ T      Meta~ Item  short T     tbli~ DELET~ <tib~
##  2 LUExt~ data-p~ <S3: ~ T      Enum  Extr~ byte  T     tblL~ DELET~ <tib~
##  3 LUMar~ data-p~ <S3: ~ T      Enum  Mark~ byte  T     tblL~ DELET~ <tib~
##  4 LUGen~ data-p~ <S3: ~ T      Enum  Gend~ byte  T     tblL~ DELET~ <tib~
##  5 LUMar~ data-p~ <S3: ~ T      Enum  Mark~ byte  T     tblL~ DELET~ <tib~
##  6 LUMul~ data-p~ <S3: ~ T      Enum  Mult~ byte  T     tblL~ DELET~ <tib~
##  7 LURac~ data-p~ <S3: ~ T      Enum  Race~ byte  T     tblL~ DELET~ <tib~
##  8 LURel~ data-p~ <S3: ~ T      Enum  Rela~ byte  T     tblL~ DELET~ <tib~
##  9 LURos~ data-p~ <S3: ~ T      Enum  Rost~ short T     tblL~ DELET~ <tib~
## 10 LUSur~ data-p~ <S3: ~ T      Enum  Surv~ byte  T     tblL~ DELET~ <tib~
## 11 LUTri~ data-p~ <S3: ~ T      Enum  Tris~ byte  T     tblL~ DELET~ <tib~
## 12 LUYes~ data-p~ <S3: ~ T      Enum  YesNo short T     tblL~ DELET~ <tib~
## 13 MzMan~ data-p~ <S3: ~ T      Meta~ NA_c~ NA_c~ F     tblM~ DELET~ <tib~
## 14 Roste~ data-p~ <S3: ~ T      Meta~ NA_c~ NA_c~ F     tblR~ DELET~ <tib~
## 15 varia~ data-p~ <S3: ~ T      Meta~ NA_c~ NA_c~ F     tblv~ DELET~ <tib~
```

```r
create_enum_body <- function( d ) {
  tab_spaces <- "    "
  labels   <- dplyr::if_else(      d$Active , d$Label, paste("//", d$Label))
  comments <- dplyr::if_else(is.na(d$Notes ), ""     , paste("//", d$Notes))

  paste0(sprintf("%s%-60s = %5s, %s\n", tab_spaces, labels, d$ID, comments), collapse="")
}

# ds_file %>%
#   dplyr::filter(name=="LURelationshipPath") %>%
#   dplyr::pull(entries)

ds_enum <- ds_file  %>%
  dplyr::filter(convert_to_enum) %>%
  dplyr::select(enum_name, entries, c_sharp_type) %>%
  dplyr::mutate(
    enum_header = paste0("\npublic enum ", .$enum_name, " {\n"),
    enum_body   = purrr::map_chr(.$entries, create_enum_body),
    enum_footer = "}\n",
    enum_cs     = paste0(enum_header, enum_body, enum_footer)
  ) %>%
  dplyr::select(-enum_header, -enum_body, -enum_footer)

ds_enum %>%
  dplyr::pull(enum_cs) %>%
  cat()
```

```
## 
## public enum Item {
##     IDOfOther1979RosterGen1                                      =     1, 
##     RosterGen1979                                                =     2, 
##     SiblingNumberFrom1993SiblingRoster                           =     3, 
##     IDCodeOfOtherSiblingGen1                                     =     4, 
##     ShareBiomomGen1                                              =     5, 
##     ShareBiodadGen1                                              =     6, 
##     IDCodeOfOtherInterviewedBiodadGen2                           =     9, 
##     ShareBiodadGen2                                              =    10, 
##     Gen1MomOfGen2Subject                                         =    11, 
##     DateOfBirthMonth                                             =    13, 
##     DateOfBirthYearGen1                                          =    14, 
##     DateOfBirthYearGen2                                          =    15, 
##     AgeAtInterviewDateYears                                      =    16, 
##     AgeAtInterviewDateMonths                                     =    17, 
##     InterviewDateDay                                             =    20, 
##     InterviewDateMonth                                           =    21, 
##     InterviewDateYear                                            =    22, 
##     Gen1SiblingIsATwinOrTrip1994                                 =    25, 
##     Gen1MultipleSiblingType1994                                  =    26, 
##     Gen1ListedTwinCorrect1994                                    =    27, 
##     Gen1TwinIsMzOrDz1994                                         =    28, 
##     Gen1ListedTripCorrect1994                                    =    29, 
##     Gen1TripIsMzOrDz1994                                         =    30, 
##     MotherOrBothInHHGen2                                         =    37, 
##     FatherHasAsthmaGen2                                          =    40, 
##     BioKidCountGen1                                              =    48, 
##     Gen1ChildsIDByBirthOrder                                     =    49, 
##     HerTwinsTripsAreListed                                       =    50, 
##     HerTwinsAreMz                                                =    52, 
##     HerTripsAreMz                                                =    53, 
##     HerTwinsMistakenForEachOther                                 =    54, 
##     HerTripsMistakenForEachOther                                 =    55, 
##     BirthOrderInNlsGen2                                          =    60, 
##     SiblingCountTotalFen1                                        =    63, 
##     BioKidCountGen2                                              =    64, 
##     OlderSiblingsTotalCountGen1                                  =    66, 
##     Gen1HairColor                                                =    70, 
##     Gen1EyeColor                                                 =    71, 
##     Gen2HairColor_NOTUSED                                        =    72, 
##     Gen2EyeColor_NOTUSED                                         =    73, 
##     BabyDaddyInHH                                                =    81, 
##     BabyDaddyAlive                                               =    82, 
##     BabyDaddyEverLiveInHH                                        =    83, 
##     BabyDaddyLeftHHMonth                                         =    84, 
##     BabyDaddyLeftHHYearFourDigit                                 =    85, 
##     BabyDaddyDeathMonth                                          =    86, 
##     BabyDaddyDeathYearTwoDigit                                   =    87, 
##     BabyDaddyDeathYearFourDigit                                  =    88, 
##     BabyDaddyDistanceFromMotherFuzzyCeiling                      =    89, 
##     BabyDaddyHasAsthma                                           =    90, 
##     BabyDaddyLeftHHMonthOrNeverInHH                              =    91, 
##     BabyDaddyLeftHHYearTwoDigit                                  =    92, 
##     SubjectID                                                    =   100, 
##     ExtendedFamilyID                                             =   101, 
##     Gender                                                       =   102, 
##     RaceCohort                                                   =   103, 
##     Gen2CFatherLivingInHH                                        =   121, 
##     Gen2CFatherAlive                                             =   122, 
##     Gen2CFatherDistanceFromMotherFuzzyCeiling                    =   123, 
##     Gen2CFatherAsthma_NOTUSED                                    =   125, 
##     Gen2YAFatherInHH_NOTUSED                                     =   141, 
##     Gen2YAFatherAlive_NOTUSED                                    =   142, 
##     Gen2YADeathMonth                                             =   143, 
##     Gen2YADeathYearFourDigit                                     =   144, 
##     Gen2FromMomDeathMonth                                        =   145, 
##     Gen2FromMomDeathYearFourDigit                                =   146, 
##     Gen1HeightInches                                             =   200, 
##     Gen1WeightPounds                                             =   201, 
##     Gen1AfqtScaled0Decimals_NOTUSED                              =   202, 
##     Gen1AfqtScaled3Decimals                                      =   203, 
##     Gen1HeightFeetInchesMashed                                   =   204, 
##     Gen1FatherAlive                                              =   300, 
##     Gen1FatherDeathCause                                         =   301, 
##     Gen1FatherDeathAge                                           =   302, 
##     Gen1FatherHasHealthProblems                                  =   303, 
##     Gen1FatherHealthProblem                                      =   304, 
##     Gen1FatherBirthCountry                                       =   305, 
##     Gen1LivedWithFatherAtAgeX                                    =   306, 
##     Gen1FatherHighestGrade                                       =   307, 
##     Gen1GrandfatherBirthCountry                                  =   308, 
##     Gen1FatherBirthMonth                                         =   309, 
##     Gen1FatherBirthYear                                          =   310, 
##     Gen1FatherAge                                                =   311, 
##     Gen1MotherAlive                                              =   320, 
##     Gen1MotherDeathCause                                         =   321, 
##     Gen1MotherDeathAge                                           =   322, 
##     Gen1MotherHasHealthProblems                                  =   323, 
##     Gen1MotherHealthProblem                                      =   324, 
##     Gen1MotherBirthCountry                                       =   325, 
##     Gen1LivedWithMotherAtAgeX                                    =   326, 
##     Gen1MotherHighestGrade                                       =   327, 
##     Gen1MotherBirthMonth                                         =   329, 
##     Gen1MotherBirthYear                                          =   330, 
##     Gen1MotherAge                                                =   331, 
##     Gen1AlwaysLivedWithBothParents                               =   340, 
##     Gen2HeightInchesTotal                                        =   500, 
##     Gen2HeightFeetOnly                                           =   501, 
##     Gen2HeightInchesRemainder                                    =   502, 
##     Gen2HeightInchesTotalMotherSupplement                        =   503, 
##     Gen2WeightPoundsYA                                           =   504, 
##     Gen2PiatMathRaw                                              =   511, 
##     Gen2PiatMathPercentile                                       =   512, 
##     Gen2PiatMathStandard                                         =   513, 
##     Gen1ListIncorrectGen2TwinTrips_NOTINTAGCURRENTLY             =  9993, 
##     Gen1VerifyFirstGen2TwinsTrips_NOTINTAGSETCURRENTLY           =  9994, 
##     Gen1FirstIncorrectTwinTripYoungerOrOlder_NOTUSED             =  9995, 
##     Gen1FirstIncorrectTwinTripAgeDifference_NOTUSED              =  9996, 
##     Gen1SecondIncorrectTwinTripYoungerOrOlder_NOTUSED            =  9997, 
##     Gen1SecondIncorrectTwinTripAgeDifference_NOTUSED             =  9998, 
##     NotTranslated                                                =  9999, 
## }
##  
## public enum ExtractSource {
##     Gen1Links                                                    =     3, 
##     Gen2Links                                                    =     4, 
##     Gen2LinksFromGen1                                            =     5, 
##     Gen2ImplicitFather                                           =     6, 
##     Gen2FatherFromGen1                                           =     7, 
##     Gen1Outcomes                                                 =     8, 
##     Gen2OutcomesHeight                                           =     9, 
##     Gen1Explicit                                                 =    10, 
##     Gen1Implicit                                                 =    11, 
##     Gen2OutcomesWeight                                           =    12, 
##     Gen2OutcomesMath                                             =    13, 
## }
##  
## public enum MarkerEvidence {
##     Irrelevant                                                   =     0, 
##     StronglySupports                                             =     1, 
##     Supports                                                     =     2, 
##     Consistent                                                   =     3, 
##     Ambiguous                                                    =     4, 
##     Missing                                                      =     5, 
##     Unlikely                                                     =     6, 
##     Disconfirms                                                  =     7, 
## }
##  
## public enum Gender {
##     Male                                                         =     1, 
##     Female                                                       =     2, 
##     InvalidSkipGen2                                              =   255, 
## }
##  
## public enum MarkerType {
##     RosterGen1                                                   =     1, 
##     ShareBiomom                                                  =     2, 
##     ShareBiodad                                                  =     3, 
##     DobSeparation                                                =     5, 
##     GenderAgreement                                              =     6, 
##     FatherAsthma                                                 =    10, 
##     BabyDaddyAsthma                                              =    11, 
##     BabyDaddyLeftHHDate                                          =    12, 
##     BabyDaddyDeathDate                                           =    13, 
##     BabyDaddyAlive                                               =    14, 
##     BabyDaddyInHH                                                =    15, 
##     BabyDaddyDistanceFromHH                                      =    16, 
##     Gen2CFatherAlive                                             =    17, 
##     Gen2CFatherInHH                                              =    18, 
##     Gen2CFatherDistanceFromHH                                    =    19, 
##     Gen1BiodadInHH                                               =    30, 
##     Gen1BiodadDeathAge                                           =    31, 
##     Gen1BiodadBirthYear                                          =    32, 
##     // Gen1BiodadInHH1979                                        =    33, 
##     Gen1BiodadBirthCountry                                       =    34, 
##     Gen1BiodadBirthState                                         =    35, 
##     Gen1BiomomInHH                                               =    40, 
##     Gen1BiomomDeathAge                                           =    41, 
##     Gen1BiomomBirthYear                                          =    42, 
##     // Gen1BiomomInHH1979                                        =    43, 
##     Gen1BiomomBirthCountry                                       =    44, 
##     Gen1BiomomBirthState                                         =    45, 
##     Gen1AlwaysLivedWithBothBioparents                            =    50, 
## }
##  
## public enum MultipleBirth {
##     No                                                           =     0, 
##     Twin                                                         =     2, 
##     Trip                                                         =     3, 
##     TwinOrTrip                                                   =     4, // Currently Then Gen1 algorithm doesn't distinguish.
##     DoNotKnow                                                    =   255, 
## }
##  
## public enum RaceCohort {
##     Hispanic                                                     =     1, 
##     Black                                                        =     2, 
##     Nbnh                                                         =     3, 
## }
##  
## public enum RelationshipPath {
##     Gen1Housemates                                               =     1, 
##     Gen2Siblings                                                 =     2, 
##     Gen2Cousins                                                  =     3, 
##     ParentChild                                                  =     4, 
##     AuntNiece                                                    =     5, // Actually (Uncle|Aunt)-(Nephew|Niece)
## }
##  
## public enum RosterGen1 {
##     ValidSkip                                                    =    -4, 
##     InvalidSkip                                                  =    -3, 
##     Refusal                                                      =    -1, 
##     Respondent                                                   =     0, 
##     Spouse                                                       =     1, 
##     Son                                                          =     2, 
##     Daughter                                                     =     3, 
##     Father                                                       =     4, 
##     Mother                                                       =     5, 
##     Brother                                                      =     6, 
##     Sister                                                       =     7, 
##     Grandfather                                                  =     8, 
##     Grandmother                                                  =     9, 
##     Grandson                                                     =    10, 
##     Granddaughter                                                =    11, 
##     UncleEtc                                                     =    12, 
##     AuntEtc                                                      =    13, 
##     GreatUncle                                                   =    14, 
##     GreatAunt                                                    =    15, 
##     Cousin                                                       =    16, 
##     Nephew                                                       =    17, 
##     Niece                                                        =    18, 
##     OtherBloodRelative                                           =    19, 
##     AdoptedOrStepson                                             =    20, 
##     AdoptedOrStepdaughter                                        =    21, 
##     SonInLaw                                                     =    22, 
##     DaughterInLaw                                                =    23, 
##     FatherInLaw                                                  =    24, 
##     MotherInLaw                                                  =    25, 
##     BrotherInLaw                                                 =    26, 
##     SisterInLaw                                                  =    27, 
##     GrandfatherInLaw                                             =    28, 
##     GrandmotherInLaw                                             =    29, 
##     GrandsonInLaw                                                =    30, 
##     GranddaughterInLaw                                           =    31, 
##     OtherInLawRelative                                           =    32, 
##     Partner                                                      =    33, 
##     Boarder                                                      =    34, 
##     FosterChild                                                  =    35, 
##     OtherNonRelative                                             =    36, 
##     Stepfather                                                   =    37, 
##     Stepmother                                                   =    38, 
##     Stepbrother                                                  =    39, 
##     Stepsister                                                   =    40, 
##     GreatGrandson                                                =    41, 
##     GreatGranddaughter                                           =    42, 
##     StepGrandson                                                 =    43, 
##     StepGranddaughter                                            =    44, 
##     FosterSon                                                    =    45, 
##     FosterDaughter                                               =    46, 
##     Parents                                                      =    47, 
##     Grandparents                                                 =    48, 
##     AuntOrUncle                                                  =    49, 
##     FosterFather                                                 =    50, 
##     FosterMother                                                 =    51, 
##     FosterBrother                                                =    52, 
##     FosterSister                                                 =    53, 
##     Guardian                                                     =    54, 
##     HusbandOrBrotherInLaw                                        =    57, 
##     WifeOrSisterInLaw                                            =    58, 
##     AdoptedOrStepbrother                                         =    59, 
##     AdoptedOrStepsister                                          =    60, // Watch out, this is listed at the bottom of the NlsInvestigator
##     BrotherOrCousin                                              =    62, 
##     SisterOrCousin                                               =    63, 
##     BrotherNaturalStepOrAdopted                                  =    64, 
##     SisterNaturalStepOrAdopted                                   =    65, 
##     SiblingOrSpouseOfInLaws                                      =    66, 
## }
##  
## public enum SurveySource {
##     NoInterview                                                  =     0, 
##     Gen1                                                         =     1, 
##     Gen2C                                                        =     2, 
##     Gen2YA                                                       =     3, 
##     97                                                           =     4, 
## }
##  
## public enum Tristate {
##     No                                                           =     0, 
##     Yes                                                          =     1, 
##     DoNotKnow                                                    =   255, 
## }
##  
## public enum YesNo {
##     ValidSkipOrNoInterviewOrNotInSurvey                          =    -6, 
##     InvalidSkip                                                  =    -3, 
##     DoNotKnow                                                    =    -2, 
##     Refusal                                                      =    -1, 
##     No                                                           =     0, 
##     Yes                                                          =     1, 
## }
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

ds_table_process <- ds_table %>%
  dplyr::filter(schema_name == "Process") %>%
  dplyr::mutate(
    # sql_truncate  = glue::glue("TRUNCATE TABLE {schema_name}.{table_name};")
    sql_truncate  = glue::glue("DELETE FROM {schema_name}.{table_name};")
  )

# Open channel
channel <- open_dsn_channel_odbc()
DBI::dbGetInfo(channel)
```

```
## $dbname
## [1] "NlsyLinks79"
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
## [1] "local-nlsy-links-79"
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
channel_rodbc <- open_dsn_channel_rodbc()
RODBC::odbcGetInfo(channel_rodbc)
```

```
##              DBMS_Name               DBMS_Ver        Driver_ODBC_Ver 
## "Microsoft SQL Server"           "13.00.4206"                "03.80" 
##       Data_Source_Name            Driver_Name             Driver_Ver 
##  "local-nlsy-links-79"      "msodbcsql13.dll"           "14.00.0500" 
##               ODBC_Ver            Server_Name 
##           "03.80.0000" "GIMBLE\\EXPRESS_2016"
```

```r
# Clear process tables
delete_results_process <- ds_table_process$sql_truncate %>%
  purrr::set_names(ds_table_process$table_name) %>%
  rev() %>%
  purrr::map(DBI::dbGetQuery, conn=channel)
delete_results_process
```

```
## $tblSurveyTime
## data frame with 0 columns and 0 rows
## 
## $tblSubjectDetails
## data frame with 0 columns and 0 rows
## 
## $tblSubject
## data frame with 0 columns and 0 rows
## 
## $tblRosterGen1
## data frame with 0 columns and 0 rows
## 
## $tblResponse
## data frame with 0 columns and 0 rows
## 
## $tblRelatedValues
## data frame with 0 columns and 0 rows
## 
## $tblRelatedStructure
## data frame with 0 columns and 0 rows
## 
## $tblParentsOfGen1Retro
## data frame with 0 columns and 0 rows
## 
## $tblParentsOfGen1Current
## data frame with 0 columns and 0 rows
## 
## $tblOutcome
## data frame with 0 columns and 0 rows
## 
## $tblMarkerGen2
## data frame with 0 columns and 0 rows
## 
## $tblMarkerGen1
## data frame with 0 columns and 0 rows
## 
## $tblIRDemo1
## data frame with 0 columns and 0 rows
## 
## $tblFatherOfGen2
## data frame with 0 columns and 0 rows
## 
## $tblBabyDaddy
## data frame with 0 columns and 0 rows
```

```r
# Delete metadata tables
# delete_result <- RODBC::sqlQuery(channel, "DELETE FROM [NlsLinks].[Metadata].[tblVariable]", errors=FALSE)
delete_results_metadata <- ds_file$sql_delete %>%
  purrr::set_names(ds_file$table_name) %>%
  rev() %>%
  purrr::map(DBI::dbGetQuery, conn=channel)

# DBI::dbGetQuery(conn=channel, ds_file$sql_delete[15])
delete_results_metadata
```

```
## $tblvariable
## data frame with 0 columns and 0 rows
## 
## $tblRosterGen1Assignment
## data frame with 0 columns and 0 rows
## 
## $tblMzManual
## data frame with 0 columns and 0 rows
## 
## $tblLUYesNo
## data frame with 0 columns and 0 rows
## 
## $tblLUTristate
## data frame with 0 columns and 0 rows
## 
## $tblLUSurveySource
## data frame with 0 columns and 0 rows
## 
## $tblLURosterGen1
## data frame with 0 columns and 0 rows
## 
## $tblLURelationshipPath
## data frame with 0 columns and 0 rows
## 
## $tblLURaceCohort
## data frame with 0 columns and 0 rows
## 
## $tblLUMultipleBirth
## data frame with 0 columns and 0 rows
## 
## $tblLUMarkerType
## data frame with 0 columns and 0 rows
## 
## $tblLUGender
## data frame with 0 columns and 0 rows
## 
## $tblLUMarkerEvidence
## data frame with 0 columns and 0 rows
## 
## $tblLUExtractSource
## data frame with 0 columns and 0 rows
## 
## $tblitem
## data frame with 0 columns and 0 rows
```

```r
# d <- ds_file %>%
#   dplyr::select(table_name, entries) %>%
#   dplyr::filter(table_name=="Enum.tblLURosterGen1") %>%
#   tibble::deframe() %>%
#   .[[1]]

# d2 <- d[, 1:16]
# RODBC::sqlSave(channel, dat=d, tablename="Enum.tblLURosterGen1", safer=TRUE, rownames=FALSE, append=TRUE)

# Upload metadata tables
purrr::pmap_int(
  list(
    ds_file$entries,
    ds_file$table_name,
    ds_file$schema_name
  ),
  function( d, table_name, schema_name ) {
    # browser()
    # DBI::dbWriteTable(
    #   conn    = channel,
    #   name    = table_name,
    #   schema  = schema_name,
    #   value   = d,
    #   append  = F
    # )
    RODBC::sqlSave(
      channel     = channel_rodbc,
      dat         = d,
      # tablename   = table_name,
      tablename   = paste0(schema_name, ".", table_name),
      safer       = TRUE,       # Don't keep the existing table.
      rownames    = FALSE,
      append      = TRUE
    )
  }
) #%>%
```

```
##  [1] 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1
```

```r
# purrr::set_names(ds_file$table_name)
# a <- ds_file$entries[[15]]
# table(a$ID)

# odbc::dbWriteTable(
#   conn    = channel,
#   name    = DBI::SQL("Metadata.tblvariable_97"),
#   # name    = "tblvariable_97",
#   # schema  = "Metadata",
#   value   = ds_file$entries[[16]],
#   append  = T
# )

# for( i in seq_len(nrow(ds_file)) ) {
#   message(glue::glue("Uploading from `{ basename(ds_file$path)[i]}` to `{ds_file$table_name[i]}`."))
#
#   d <- ds_file$entries[[i]]
#   print(d)
#
#   # RODBC::sqlQuery(channel, ds_extract$sql_truncate[i], errors=FALSE)
#
#   # d_peek <- RODBC::sqlQuery(channel, ds_extract$sql_select[i], errors=FALSE)
#   #
#   # missing_in_extract    <- setdiff(colnames(d_peek), colnames(d))
#   # missing_in_database   <- setdiff(colnames(d), colnames(d_peek))
#   #
#   # d_column <- tibble::tibble(
#   #   db        = colnames(d),
#   #   extract   = colnames(d_peek)
#   # ) %>%
#   #   dplyr::filter(db != extract)
#   #
#   # RODBC::sqlSave(
#   #   channel     = channel,
#   #   dat         = d,
#   #   tablename   = ds_extract$table_name[i],
#   #   safer       = TRUE,       # Don't keep the existing table.
#   #   rownames    = FALSE,
#   #   append      = TRUE
#   # ) %>%
#   #   print()
#
#   OuhscMunge::upload_sqls_rodbc(
#     d               = d,
#     table_name      = ds_file$table_name[i] ,
#     dsn_name        = "local-nlsy-links",
#     clear_table     = T,
#     create_table    = F
#   )
#
#
#   message(glue::glue("{format(object.size(d), units='MB')}"))
# }

# Close channel
DBI::dbDisconnect(channel); rm(channel)
RODBC::odbcClose(channel_rodbc); rm(channel_rodbc)

duration_in_seconds <- round(as.numeric(difftime(Sys.time(), start_time, units="secs")))
cat("File completed by `", Sys.info()["user"], "` at ", strftime(Sys.time(), "%Y-%m-%d, %H:%M %z"), " in ",  duration_in_seconds, " seconds.", sep="")
```

```
## File completed by `Will` at 2018-01-06, 17:56 -0600 in 3 seconds.
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
## [28] tidyselect_0.2.3      glue_1.2.0            OuhscMunge_0.1.8.9005
## [31] R6_2.2.2              tidyr_0.7.2           readr_1.1.1          
## [34] purrr_0.2.4           blob_1.1.0            RODBC_1.3-15         
## [37] backports_1.1.2       scales_0.5.0.9000     assertthat_0.2.0     
## [40] testit_0.7.1          colorspace_1.3-2      config_0.2           
## [43] utf8_1.1.3            stringi_1.1.6         munsell_0.4.3        
## [46] markdown_0.8          crayon_1.3.4
```

```r
Sys.time()
```

```
## [1] "2018-01-06 17:56:35 CST"
```

