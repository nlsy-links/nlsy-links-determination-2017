



This report was automatically generated with the R package **knitr**
(version 1.20).


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
requireNamespace("OuhscMunge"             ) # remotes::install_github("OuhscBbmc/OuhscMunge")
```

```r
# Constant values that won't change.
directory_in              <- "data-public/metadata/tables-79"
study                     <- "79"

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
  ArchiveDescription = readr::cols_only(
    AlgorithmVersion                    = readr::col_integer(),
    Description                         = readr::col_character(),
    Date                                = readr::col_date()
  ),
  item = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character(),
    MinValue                            = readr::col_integer(),
    MinNonnegative                      = readr::col_integer(),
    MaxValue                            = readr::col_integer(),
    Active                              = readr::col_logical(),
    Notes                               = readr::col_character()
  ),
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
## # A tibble: 16 x 5
##    table_name       schema_name enum_name     c_sharp_type convert_to_enum
##    <chr>            <chr>       <chr>         <chr>        <lgl>          
##  1 ArchiveDescript~ Archive     ArchiveDescr~ short        FALSE          
##  2 item             Metadata    Item          short        TRUE           
##  3 LUExtractSource  Enum        ExtractSource byte         TRUE           
##  4 LUGender         Enum        Gender        byte         TRUE           
##  5 LUMarkerEvidence Enum        MarkerEviden~ byte         TRUE           
##  6 LUMarkerType     Enum        MarkerType    byte         TRUE           
##  7 LUMultipleBirth  Enum        MultipleBirth byte         TRUE           
##  8 LURaceCohort     Enum        RaceCohort    byte         TRUE           
##  9 LURelationshipP~ Enum        Relationship~ byte         TRUE           
## 10 LURosterGen1     Enum        RosterGen1    short        TRUE           
## 11 LUSurveySource   Enum        SurveySource  byte         TRUE           
## 12 LUTristate       Enum        Tristate      byte         TRUE           
## 13 LUYesNo          Enum        YesNo         short        TRUE           
## 14 MzManual         Metadata    NA_character  NA_character FALSE          
## 15 RosterGen1Assig~ Metadata    NA_character  NA_character FALSE          
## 16 variable         Metadata    NA_character  NA_character FALSE
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
## # A tibble: 16 x 4
##    name            path                                col_types    exists
##    <chr>           <chr>                               <list>       <lgl> 
##  1 ArchiveDescrip~ data-public/metadata/tables-79/Arc~ <S3: col_sp~ TRUE  
##  2 item            data-public/metadata/tables-79/ite~ <S3: col_sp~ TRUE  
##  3 LUExtractSource data-public/metadata/tables-79/LUE~ <S3: col_sp~ TRUE  
##  4 LUMarkerEviden~ data-public/metadata/tables-79/LUM~ <S3: col_sp~ TRUE  
##  5 LUGender        data-public/metadata/tables-79/LUG~ <S3: col_sp~ TRUE  
##  6 LUMarkerType    data-public/metadata/tables-79/LUM~ <S3: col_sp~ TRUE  
##  7 LUMultipleBirth data-public/metadata/tables-79/LUM~ <S3: col_sp~ TRUE  
##  8 LURaceCohort    data-public/metadata/tables-79/LUR~ <S3: col_sp~ TRUE  
##  9 LURelationship~ data-public/metadata/tables-79/LUR~ <S3: col_sp~ TRUE  
## 10 LURosterGen1    data-public/metadata/tables-79/LUR~ <S3: col_sp~ TRUE  
## 11 LUSurveySource  data-public/metadata/tables-79/LUS~ <S3: col_sp~ TRUE  
## 12 LUTristate      data-public/metadata/tables-79/LUT~ <S3: col_sp~ TRUE  
## 13 LUYesNo         data-public/metadata/tables-79/LUY~ <S3: col_sp~ TRUE  
## 14 MzManual        data-public/metadata/tables-79/MzM~ <S3: col_sp~ TRUE  
## 15 RosterGen1Assi~ data-public/metadata/tables-79/Ros~ <S3: col_sp~ TRUE  
## 16 variable        data-public/metadata/tables-79/var~ <S3: col_sp~ TRUE
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
## # A tibble: 16 x 4
##    name          path                           col_types   entries       
##    <chr>         <chr>                          <list>      <list>        
##  1 ArchiveDescr~ data-public/metadata/tables-7~ <S3: col_s~ <tibble [57 x~
##  2 item          data-public/metadata/tables-7~ <S3: col_s~ <tibble [110 ~
##  3 LUExtractSou~ data-public/metadata/tables-7~ <S3: col_s~ <tibble [12 x~
##  4 LUMarkerEvid~ data-public/metadata/tables-7~ <S3: col_s~ <tibble [8 x ~
##  5 LUGender      data-public/metadata/tables-7~ <S3: col_s~ <tibble [3 x ~
##  6 LUMarkerType  data-public/metadata/tables-7~ <S3: col_s~ <tibble [28 x~
##  7 LUMultipleBi~ data-public/metadata/tables-7~ <S3: col_s~ <tibble [5 x ~
##  8 LURaceCohort  data-public/metadata/tables-7~ <S3: col_s~ <tibble [3 x ~
##  9 LURelationsh~ data-public/metadata/tables-7~ <S3: col_s~ <tibble [5 x ~
## 10 LURosterGen1  data-public/metadata/tables-7~ <S3: col_s~ <tibble [67 x~
## 11 LUSurveySour~ data-public/metadata/tables-7~ <S3: col_s~ <tibble [5 x ~
## 12 LUTristate    data-public/metadata/tables-7~ <S3: col_s~ <tibble [3 x ~
## 13 LUYesNo       data-public/metadata/tables-7~ <S3: col_s~ <tibble [6 x ~
## 14 MzManual      data-public/metadata/tables-7~ <S3: col_s~ <tibble [208 ~
## 15 RosterGen1As~ data-public/metadata/tables-7~ <S3: col_s~ <tibble [50 x~
## 16 variable      data-public/metadata/tables-7~ <S3: col_s~ <tibble [1,82~
```

```r
# d <- readr::read_csv("data-public/metadata/tables/variable_79.csv", col_types=lst_col_types$variable_79, comment = "#")
# readr::problems(d)
# ds_entries$entries[15]

ds_table <- database_inventory(study)
ds_table
```

```
## # A tibble: 51 x 6
##    schema_name table_name row_count column_count space_total_kb
##  * <chr>       <chr>          <int>        <int>          <int>
##  1 Archive     tblArchiv~        57            3             72
##  2 Archive     tblRelate~    789184           24          32464
##  3 dbo         sysdiagra~         4            5            280
##  4 Enum        tblLUBiop~         0            2              0
##  5 Enum        tblLUExtr~        12            4             72
##  6 Enum        tblLUGend~         3            4             72
##  7 Enum        tblLUMark~         8            4             72
##  8 Enum        tblLUMark~        28            5             72
##  9 Enum        tblLUMult~         5            4             72
## 10 Enum        tblLURace~         3            4             72
## # ... with 41 more rows, and 1 more variable: space_used_kb <int>
```

```r
rm(directory_in) # rm(col_types_tulsa)
```

```r
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call to `dplyr::rename()`.

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
## # A tibble: 57 x 3
##    AlgorithmVersion Description                                 Date      
##               <int> <chr>                                       <date>    
##  1               32 Unites Gen2 V28 with Gen1 that uses only e~ NA        
##  2               26 One of the last Gen2 version, before focus~ NA        
##  3               33 Replaces all Gen2 NULLS with .375s          NA        
##  4               34 Gen2 RExplicits uses Twin determinations (~ NA        
##  5               35 Unites Gen1 Again                           NA        
##  6               36 Includes AuntNieces, ParentChildren & Gen2~ NA        
##  7               37 Updated Gen1Links tagset for 2010 wave      NA        
##  8               38 Updated Gen2FatherFromGen1 tagset for 2010~ NA        
##  9               39 Updated Gen2LinksFromGen1 tagset for 2010 ~ NA        
## 10               40 Updated Gen2Links and Gen2ImplicitFather t~ NA        
## # ... with 47 more rows
## # A tibble: 110 x 7
##       ID Label               MinValue MinNonnegative MaxValue Active Notes
##    <int> <chr>                  <int>          <int>    <int> <lgl>  <chr>
##  1     1 IDOfOther1979Roste~       -4              1    12557 TRUE   <NA> 
##  2     2 RosterGen1979             -4              1       66 TRUE   <NA> 
##  3     3 SiblingNumberFrom1~       -4              1       99 TRUE   <NA> 
##  4     4 IDCodeOfOtherSibli~       -5              3    12518 TRUE   <NA> 
##  5     5 ShareBiomomGen1           -5              0        2 TRUE   <NA> 
##  6     6 ShareBiodadGen1           -5              0        2 TRUE   <NA> 
##  7     9 IDCodeOfOtherInter~       -7              1       11 TRUE   <NA> 
##  8    10 ShareBiodadGen2           -7              0        3 TRUE   <NA> 
##  9    11 Gen1MomOfGen2Subje~        2              2    12675 TRUE   <NA> 
## 10    13 DateOfBirthMonth          -5              1       12 TRUE   <NA> 
## # ... with 100 more rows
## # A tibble: 12 x 4
##       ID Label                   Active Notes
##    <int> <chr>                   <lgl>  <chr>
##  1     3 Gen1Links               TRUE   <NA> 
##  2     4 Gen2Links               TRUE   <NA> 
##  3     5 Gen2LinksFromGen1       TRUE   <NA> 
##  4     6 Gen2ImplicitFather      TRUE   <NA> 
##  5     7 Gen2FatherFromGen1      TRUE   <NA> 
##  6     8 Gen1Outcomes            TRUE   <NA> 
##  7     9 Gen2OutcomesHeight      TRUE   <NA> 
##  8    10 Gen1Explicit            TRUE   <NA> 
##  9    11 Gen1Implicit            TRUE   <NA> 
## 10    12 Gen2OutcomesWeight      TRUE   <NA> 
## 11    13 Gen2OutcomesMath        TRUE   <NA> 
## 12    14 Gen2ImplicitFatherDeath TRUE   <NA> 
## # A tibble: 8 x 4
##      ID Label            Active Notes
##   <int> <chr>            <lgl>  <chr>
## 1     0 Irrelevant       TRUE   <NA> 
## 2     1 StronglySupports TRUE   <NA> 
## 3     2 Supports         TRUE   <NA> 
## 4     3 Consistent       TRUE   <NA> 
## 5     4 Ambiguous        TRUE   <NA> 
## 6     5 Missing          TRUE   <NA> 
## 7     6 Unlikely         TRUE   <NA> 
## 8     7 Disconfirms      TRUE   <NA> 
## # A tibble: 3 x 4
##      ID Label           Active Notes
##   <int> <chr>           <lgl>  <chr>
## 1     1 Male            TRUE   <NA> 
## 2     2 Female          TRUE   <NA> 
## 3   255 InvalidSkipGen2 TRUE   <NA> 
## # A tibble: 28 x 5
##       ID Label               Explicit Active Notes
##    <int> <chr>                  <int> <lgl>  <chr>
##  1     1 RosterGen1                 1 TRUE   <NA> 
##  2     2 ShareBiomom                1 TRUE   <NA> 
##  3     3 ShareBiodad                1 TRUE   <NA> 
##  4     5 DobSeparation              0 TRUE   <NA> 
##  5     6 GenderAgreement            0 TRUE   <NA> 
##  6    10 FatherAsthma               0 TRUE   <NA> 
##  7    11 BabyDaddyAsthma            0 TRUE   <NA> 
##  8    12 BabyDaddyLeftHHDate        0 TRUE   <NA> 
##  9    13 BabyDaddyDeathDate         0 TRUE   <NA> 
## 10    14 BabyDaddyAlive             0 TRUE   <NA> 
## # ... with 18 more rows
## # A tibble: 5 x 4
##      ID Label      Active Notes                                           
##   <int> <chr>      <lgl>  <chr>                                           
## 1     0 No         TRUE   <NA>                                            
## 2     2 Twin       TRUE   <NA>                                            
## 3     3 Trip       TRUE   <NA>                                            
## 4     4 TwinOrTrip TRUE   Currently Then Gen1 algorithm doesn't distingui~
## 5   255 DoNotKnow  TRUE   <NA>                                            
## # A tibble: 3 x 4
##      ID Label    Active Notes
##   <int> <chr>    <lgl>  <chr>
## 1     1 Hispanic TRUE   <NA> 
## 2     2 Black    TRUE   <NA> 
## 3     3 Nbnh     TRUE   <NA> 
## # A tibble: 5 x 4
##      ID Label          Active Notes                               
##   <int> <chr>          <lgl>  <chr>                               
## 1     1 Gen1Housemates TRUE   <NA>                                
## 2     2 Gen2Siblings   TRUE   <NA>                                
## 3     3 Gen2Cousins    TRUE   <NA>                                
## 4     4 ParentChild    TRUE   <NA>                                
## 5     5 AuntNiece      TRUE   Actually (Uncle|Aunt)-(Nephew|Niece)
## # A tibble: 67 x 4
##       ID Label       Active Notes
##    <int> <chr>       <lgl>  <chr>
##  1    -4 ValidSkip   TRUE   <NA> 
##  2    -3 InvalidSkip TRUE   <NA> 
##  3    -1 Refusal     TRUE   <NA> 
##  4     0 Respondent  TRUE   <NA> 
##  5     1 Spouse      TRUE   <NA> 
##  6     2 Son         TRUE   <NA> 
##  7     3 Daughter    TRUE   <NA> 
##  8     4 Father      TRUE   <NA> 
##  9     5 Mother      TRUE   <NA> 
## 10     6 Brother     TRUE   <NA> 
## # ... with 57 more rows
## # A tibble: 5 x 4
##      ID Label       Active Notes
##   <int> <chr>       <lgl>  <chr>
## 1     0 NoInterview TRUE   <NA> 
## 2     1 Gen1        TRUE   <NA> 
## 3     2 Gen2C       TRUE   <NA> 
## 4     3 Gen2YA      TRUE   <NA> 
## 5     4 97          TRUE   <NA> 
## # A tibble: 3 x 4
##      ID Label     Active Notes
##   <int> <chr>     <lgl>  <chr>
## 1     0 No        TRUE   <NA> 
## 2     1 Yes       TRUE   <NA> 
## 3   255 DoNotKnow TRUE   <NA> 
## # A tibble: 6 x 4
##      ID Label                               Active Notes
##   <int> <chr>                               <lgl>  <chr>
## 1    -6 ValidSkipOrNoInterviewOrNotInSurvey TRUE   <NA> 
## 2    -3 InvalidSkip                         TRUE   <NA> 
## 3    -2 DoNotKnow                           TRUE   <NA> 
## 4    -1 Refusal                             TRUE   <NA> 
## 5     0 No                                  TRUE   <NA> 
## 6     1 Yes                                 TRUE   <NA> 
## # A tibble: 208 x 9
##       ID SubjectTag_S1 SubjectTag_S2 Generation MultipleBirthIf~  IsMz
##    <int>         <int>         <int>      <int>            <int> <int>
##  1     1          5003          5004          2                2     0
##  2     3         14303         14304          2                2     0
##  3     5         15904         15905          2                2     0
##  4     6         28805         28806          2                2     0
##  5     8         36504         36505          2                2     1
##  6     9         67703         67704          2                2     0
##  7    10         73301         73302          2                2     1
##  8    12         74301         74302          2                2     0
##  9    13         77502         77503          2                2     1
## 10    14         93001         93002          2                2     1
## # ... with 198 more rows, and 3 more variables: Undecided <int>,
## #   Related <int>, Notes <chr>
## # A tibble: 50 x 16
##       ID ResponseLower ResponseUpper  Freq Resolved      R RBoundLower
##    <int>         <int>         <int> <int>    <int>  <dbl>       <dbl>
##  1     1            -3            -3    67        0 NA           0    
##  2     2            -3            -1    11        0 NA           0    
##  3     3            -3            33     6        1  0           0    
##  4     4            -3            36    35        1  0           0    
##  5     5            -1            18     1        1  0.125       0.125
##  6     6            -1            36     3        1  0           0    
##  7     7             1             1   167        1  0           0    
##  8     8             1            33     1        1  0           0    
##  9     9             1            36     1        1  0           0    
## 10    10             1            57     1        1  0           0    
## # ... with 40 more rows, and 9 more variables: RBoundUpper <dbl>,
## #   SameGeneration <int>, ShareBiodad <int>, ShareBiomom <int>,
## #   ShareBiograndparent <int>, Inconsistent <int>, Notes <chr>,
## #   ResponseLowerLabel <chr>, ResponseUpperLabel <chr>
## # A tibble: 1,820 x 10
##    VariableCode  Item Generation ExtractSource SurveySource SurveyYear
##    <chr>        <int>      <int>         <int>        <int>      <int>
##  1 R0000100       100          1             3            1       1979
##  2 C0000100       100          2             6            2          0
##  3 R0000149       101          1             3            1       1979
##  4 R0214800       102          1             3            1       1979
##  5 C0005400       102          2             4            2          0
##  6 R0214700       103          1             3            1       1979
##  7 C0005300       103          2             4            2          0
##  8 R0000150         1          1            10            1       1979
##  9 R0000152         1          1            10            1       1979
## 10 R0000154         1          1            10            1       1979
## # ... with 1,810 more rows, and 4 more variables: LoopIndex <int>,
## #   Translate <int>, Active <int>, Notes <chr>
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
##  [1] "tblArchiveDescription"   "tblitem"                
##  [3] "tblLUExtractSource"      "tblLUMarkerEvidence"    
##  [5] "tblLUGender"             "tblLUMarkerType"        
##  [7] "tblLUMultipleBirth"      "tblLURaceCohort"        
##  [9] "tblLURelationshipPath"   "tblLURosterGen1"        
## [11] "tblLUSurveySource"       "tblLUTristate"          
## [13] "tblLUYesNo"              "tblMzManual"            
## [15] "tblRosterGen1Assignment" "tblvariable"
```

```r
ds_file
```

```
## # A tibble: 16 x 11
##    name  path  col_types exists schema_name enum_name c_sharp_type
##    <chr> <chr> <list>    <lgl>  <chr>       <chr>     <chr>       
##  1 Arch~ data~ <S3: col~ TRUE   Archive     ArchiveD~ short       
##  2 item  data~ <S3: col~ TRUE   Metadata    Item      short       
##  3 LUEx~ data~ <S3: col~ TRUE   Enum        ExtractS~ byte        
##  4 LUMa~ data~ <S3: col~ TRUE   Enum        MarkerEv~ byte        
##  5 LUGe~ data~ <S3: col~ TRUE   Enum        Gender    byte        
##  6 LUMa~ data~ <S3: col~ TRUE   Enum        MarkerTy~ byte        
##  7 LUMu~ data~ <S3: col~ TRUE   Enum        Multiple~ byte        
##  8 LURa~ data~ <S3: col~ TRUE   Enum        RaceCoho~ byte        
##  9 LURe~ data~ <S3: col~ TRUE   Enum        Relation~ byte        
## 10 LURo~ data~ <S3: col~ TRUE   Enum        RosterGe~ short       
## 11 LUSu~ data~ <S3: col~ TRUE   Enum        SurveySo~ byte        
## 12 LUTr~ data~ <S3: col~ TRUE   Enum        Tristate  byte        
## 13 LUYe~ data~ <S3: col~ TRUE   Enum        YesNo     short       
## 14 MzMa~ data~ <S3: col~ TRUE   Metadata    NA_chara~ NA_character
## 15 Rost~ data~ <S3: col~ TRUE   Metadata    NA_chara~ NA_character
## 16 vari~ data~ <S3: col~ TRUE   Metadata    NA_chara~ NA_character
## # ... with 4 more variables: convert_to_enum <lgl>, table_name <chr>,
## #   sql_delete <S3: glue>, entries <list>
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
##     HerTwinsTripsAreListed                                       =    50, // Manually reviewed after 2010 wave, but not since
##     HerTwinsAreMz                                                =    52, // Manually reviewed after 2010 wave, but not since
##     HerTripsAreMz                                                =    53, // Manually reviewed after 2010 wave, but not since
##     HerTwinsMistakenForEachOther                                 =    54, // Manually reviewed after 2010 wave, but not since
##     HerTripsMistakenForEachOther                                 =    55, // Manually reviewed after 2010 wave, but not since
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
##     Gen2ImplicitFatherDeath                                      =    14, 
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
channel <- open_dsn_channel_odbc(study)
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
RODBC::odbcGetInfo(channel_rodbc)
```

```
##              DBMS_Name               DBMS_Ver        Driver_ODBC_Ver 
## "Microsoft SQL Server"           "13.00.5081"                "03.80" 
##       Data_Source_Name            Driver_Name             Driver_Ver 
##  "local-nlsy-links-79"      "msodbcsql17.dll"           "17.01.0000" 
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
## 
## $tblArchiveDescription
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
    #   conn        = channel,
    #   name        = DBI::Id(schema=schema_name, table=table_name),
    #   value       = d,
    #   overwrite   = FALSE,
    #   append      = TRUE
    # )
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
##  [1] 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1
```

```r
# purrr::set_names(ds_file$table_name)
# a <- ds_file$entries[[15]]
# table(a$ID)

# RODBC::sqlSave(
#   channel     = channel_rodbc,
#   dat         = ds_file$entries[[16]][, ],
#   tablename   = "Metadata.tblVariable",
#   safer       = TRUE,       # Don't keep the existing table.
#   rownames    = FALSE,
#   append      = TRUE
# )

# DBI::dbWriteTable(
#   conn        = channel,
#   name        = DBI::Id(catalog="NlsyLinks79", schema="Metadata", table="tblv"),
#   value       = ds_file$entries[[15]][1:10, 2],
#   overwrite   = FALSE,
#   append      = F
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
cat("`import-79-metadata.R` file completed by `", Sys.info()["user"], "` at ", strftime(Sys.time(), "%Y-%m-%d, %H:%M %z"), " in ",  duration_in_seconds, " seconds.", sep="")
```

```
## `import-79-metadata.R` file completed by `Will` at 2018-10-14, 17:01 -0500 in 9 seconds.
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
## [1] "2018-10-14 17:01:11 CDT"
```

