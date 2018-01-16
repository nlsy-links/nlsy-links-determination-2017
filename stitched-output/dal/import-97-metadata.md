



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
directory_in              <- "data-public/metadata/tables-97"
study                     <- "97"

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
  LURoster = col_types_minimal,
  LUTristate = col_types_minimal,
  LUYesNo = col_types_minimal,
  MzManual = readr::cols_only(
    ID                                  = readr::col_integer(),
    SubjectTag_S1                       = readr::col_integer(),
    SubjectTag_S2                       = readr::col_integer(),
    MultipleBirthIfSameSex              = readr::col_integer(),
    IsMz                                = readr::col_integer(),
    Undecided                           = readr::col_integer(),
    Related                             = readr::col_integer(),
    Notes                               = readr::col_character()
  ),
  # RosterAssignment    = readr::cols_only(
  #   ID                                  = readr::col_integer(),
  #   ResponseLower                       = readr::col_integer(),
  #   ResponseUpper                       = readr::col_integer(),
  #   Freq                                = readr::col_integer(),
  #   Resolved                            = readr::col_integer(),
  #   R                                   = readr::col_double(),
  #   RBoundLower                         = readr::col_double(),
  #   RBoundUpper                         = readr::col_double(),
  #   ShareBiodad                         = readr::col_integer(),
  #   ShareBiomom                         = readr::col_integer(),
  #   ShareBiograndparent                 = readr::col_integer(),
  #   Inconsistent                        = readr::col_integer(),
  #   Notes                               = readr::col_character(),
  #   ResponseLowerLabel                  = readr::col_character(),
  #   ResponseUpperLabel                  = readr::col_character()
  # ),
  variable = readr::cols_only(
    # ID                                  = readr::col_integer(),
    VariableCode                        = readr::col_character(),
    Item                                = readr::col_integer(),
    ExtractSource                       = readr::col_integer(),
    SurveyYear                          = readr::col_integer(),
    LoopIndex1                          = readr::col_integer(),
    LoopIndex2                          = readr::col_integer(),
    Translate                           = readr::col_integer(),
    Active                              = readr::col_integer(),
    Notes                               = readr::col_character(),
    QuestionName                        = readr::col_character(),
    VariableTitle                       = readr::col_character()
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
## # A tibble: 14 x 5
##    table_name         schema_name enum_name        c_sharp_type convert_t~
##    <chr>              <chr>       <chr>            <chr>        <lgl>     
##  1 item               Metadata    Item             short        T         
##  2 LUExtractSource    Enum        ExtractSource    byte         T         
##  3 LUGender           Enum        Gender           byte         T         
##  4 LUMarkerEvidence   Enum        MarkerEvidence   byte         T         
##  5 LUMarkerType       Enum        MarkerType       byte         T         
##  6 LUMultipleBirth    Enum        MultipleBirth    byte         T         
##  7 LURaceCohort       Enum        RaceCohort       byte         T         
##  8 LURelationshipPath Enum        RelationshipPath byte         T         
##  9 LURoster           Enum        RosterGen1       short        T         
## 10 LUTristate         Enum        Tristate         byte         T         
## 11 LUYesNo            Enum        YesNo            short        T         
## 12 MzManual           Metadata    NA_character     NA_character F         
## 13 #RosterAssignment  Metadata    NA_character     NA_character F         
## 14 variable           Metadata    NA_character     NA_character F
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
## # A tibble: 13 x 4
##    name               path                               col_types   exis~
##    <chr>              <chr>                              <list>      <lgl>
##  1 item               data-public/metadata/tables-97/it~ <S3: col_s~ T    
##  2 LUExtractSource    data-public/metadata/tables-97/LU~ <S3: col_s~ T    
##  3 LUMarkerEvidence   data-public/metadata/tables-97/LU~ <S3: col_s~ T    
##  4 LUGender           data-public/metadata/tables-97/LU~ <S3: col_s~ T    
##  5 LUMarkerType       data-public/metadata/tables-97/LU~ <S3: col_s~ T    
##  6 LUMultipleBirth    data-public/metadata/tables-97/LU~ <S3: col_s~ T    
##  7 LURaceCohort       data-public/metadata/tables-97/LU~ <S3: col_s~ T    
##  8 LURelationshipPath data-public/metadata/tables-97/LU~ <S3: col_s~ T    
##  9 LURoster           data-public/metadata/tables-97/LU~ <S3: col_s~ T    
## 10 LUTristate         data-public/metadata/tables-97/LU~ <S3: col_s~ T    
## 11 LUYesNo            data-public/metadata/tables-97/LU~ <S3: col_s~ T    
## 12 MzManual           data-public/metadata/tables-97/Mz~ <S3: col_s~ T    
## 13 variable           data-public/metadata/tables-97/va~ <S3: col_s~ T
```

```r
testit::assert("All metadata files must exist.", all(ds_file$exists))

ds_entries <- ds_file %>%
  # dplyr::slice(1:9) %>%
  dplyr::select(name, path, col_types) %>%
  dplyr::mutate(
    entries = purrr::pmap(list(file=.$path, col_types=.$col_types), readr::read_csv, comment = "#")
  )
ds_entries
```

```
## # A tibble: 13 x 4
##    name               path                          col_types  entries    
##    <chr>              <chr>                         <list>     <list>     
##  1 item               data-public/metadata/tables-~ <S3: col_~ <tibble [1~
##  2 LUExtractSource    data-public/metadata/tables-~ <S3: col_~ <tibble [5~
##  3 LUMarkerEvidence   data-public/metadata/tables-~ <S3: col_~ <tibble [8~
##  4 LUGender           data-public/metadata/tables-~ <S3: col_~ <tibble [3~
##  5 LUMarkerType       data-public/metadata/tables-~ <S3: col_~ <tibble [2~
##  6 LUMultipleBirth    data-public/metadata/tables-~ <S3: col_~ <tibble [5~
##  7 LURaceCohort       data-public/metadata/tables-~ <S3: col_~ <tibble [3~
##  8 LURelationshipPath data-public/metadata/tables-~ <S3: col_~ <tibble [5~
##  9 LURoster           data-public/metadata/tables-~ <S3: col_~ <tibble [9~
## 10 LUTristate         data-public/metadata/tables-~ <S3: col_~ <tibble [3~
## 11 LUYesNo            data-public/metadata/tables-~ <S3: col_~ <tibble [6~
## 12 MzManual           data-public/metadata/tables-~ <S3: col_~ <tibble [2~
## 13 variable           data-public/metadata/tables-~ <S3: col_~ <tibble [1~
```

```r
# d <- readr::read_csv("data-public/metadata/tables/variable_97.csv", col_types=lst_col_types$variable_97, comment = "#")
# readr::problems(d)
# ds_entries$entries[15]

ds_table <- database_inventory(study)
ds_table
```

```
## # A tibble: 28 x 6
##    schema_name table_name              row_count column_count space~ spac~
##  * <chr>       <chr>                       <int>        <int>  <int> <int>
##  1 Archive     tblArchiveDescription           0            4      0     0
##  2 Archive     tblRelatedValuesArchive         0           22     72    16
##  3 Enum        tblLUExtractSource              5            4     72    16
##  4 Enum        tblLUGender                     3            4     72    16
##  5 Enum        tblLUMarkerEvidence             8            4     72    16
##  6 Enum        tblLUMarkerType                28            5     72    16
##  7 Enum        tblLUMultipleBirth              5            4     72    16
##  8 Enum        tblLURaceCohort                 3            4     72    16
##  9 Enum        tblLURelationshipPath           5            4     72    16
## 10 Enum        tblLURoster                    90            4     72    16
## # ... with 18 more rows
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
## # A tibble: 16 x 7
##       ID Label                            MinValue MinN~ MaxV~ Acti~ Notes
##    <int> <chr>                               <int> <int> <int> <lgl> <chr>
##  1     1 subject_id                              1     1  9022 T     <NA> 
##  2     2 extended_family_id                      1     1  9022 T     <NA> 
##  3    10 gender                                  1     1     2 T     <NA> 
##  4    11 DateOfBirthMonth                        1     1    12 T     <NA> 
##  5    12 DateOfBirthYear                      1980  1980  1984 T     <NA> 
##  6    13 sample_cohort                           0     0     1 T     <NA> 
##  7    20 InterviewDateDay                    -   7     1    31 T     <NA> 
##  8    21 InterviewDateMonth                  -   7     1    12 T     <NA> 
##  9    22 InterviewDateYear                   -   7    86  2016 T     <NA> 
## 10    23 AgeAtInterviewDateMonths            -   5   146   500 T     <NA> 
## 11    24 AgeAtInterviewDateYears             -   5    12    40 T     <NA> 
## 12   101 roster_crosswalk                    -   5     1    20 T     <NA> 
## 13   102 hh_member_id                        -   4     1    17 T     <NA> 
## 14  1020 InterviewDateDayParent_NOTUSED      -   4     1    31 F     <NA> 
## 15  1021 InterviewDateMonthParent_NOTUSED    -   4     1     9 F     <NA> 
## 16  1022 InterviewDateYearParent_NOTUSED     -   4  1997  1998 F     <NA> 
## # A tibble: 5 x 4
##      ID Label             Active Notes
##   <int> <chr>             <lgl>  <chr>
## 1     1 97-demographics   T      <NA> 
## 2     2 97-roster         T      <NA> 
## 3     3 97-survey-time    T      <NA> 
## 4     4 97-links-explicit T      <NA> 
## 5     5 97-links-implicit T      <NA> 
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
## # A tibble: 90 x 4
##       ID Label           Active Notes
##    <int> <chr>           <lgl>  <chr>
##  1    -4 valid_skip      T      <NA> 
##  2     0 Identity        T      <NA> 
##  3     1 Wife            T      <NA> 
##  4     2 Husband         T      <NA> 
##  5     3 Mother          T      <NA> 
##  6     4 Father          T      <NA> 
##  7     7 Step-mother     T      <NA> 
##  8     8 Step-father     T      <NA> 
##  9     5 Adoptive mother T      <NA> 
## 10     6 Adoptive father T      <NA> 
## # ... with 80 more rows
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
## # A tibble: 208 x 8
##       ID SubjectTag_S1 SubjectTag_S2 Multiple~  IsMz Undec~ Rela~ Notes   
##    <int>         <int>         <int>     <int> <int>  <int> <int> <chr>   
##  1     1          5003          5004         2     0      0     1 Very Co~
##  2     3         14303         14304         2     0      0     1 Differe~
##  3     5         15904         15905         2     0      0     1 <NA>    
##  4     6         28805         28806         2     0      0     1 Differe~
##  5     8         36504         36505         2     1      0     1 Twice D~
##  6     9         67703         67704         2     0      0     1 1994-20~
##  7    10         73301         73302         2     1      0     1 Mostly ~
##  8    12         74301         74302         2     0      0     1 Differe~
##  9    13         77502         77503         2     1      0     1 1994-20~
## 10    14         93001         93002         2     1      0     1 1994-20~
## # ... with 198 more rows
## # A tibble: 127 x 11
##    Varia~  Item Extra~ Surve~ Loop~ Loop~ Tran~ Acti~ Notes Ques~ Variabl~
##    <chr>  <int>  <int>  <int> <int> <int> <int> <int> <chr> <chr> <chr>   
##  1 R0000~     1      1   1997     1     1     1     1 <NA>  PUBID YOUTH C~
##  2 R1193~     2      1   1997     1     1     1     1 <NA>  SIDC~ HOUSEHO~
##  3 R0536~    10      1   1997     1     1     1     1 <NA>  KEY!~ RS GEND~
##  4 R0536~    11      1   1997     1     1     1     1 <NA>  KEY!~ RS BIRT~
##  5 R0536~    12      1   1997     1     1     1     1 <NA>  KEY!~ RS BIRT~
##  6 R1235~    13      1   1997     1     1     1     1 <NA>  CV_S~ SAMPLE ~
##  7 R1097~   101      2   1997     1     1     1     1 <NA>  HHI2~ HHI2_HH~
##  8 R1097~   101      2   1997     2     1     1     1 <NA>  HHI2~ HHI2_HH~
##  9 R1098~   101      2   1997     3     1     1     1 <NA>  HHI2~ HHI2_HH~
## 10 R1098~   101      2   1997     4     1     1     1 <NA>  HHI2~ HHI2_HH~
## # ... with 117 more rows
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
##  [1] "tblitem"               "tblLUExtractSource"   
##  [3] "tblLUMarkerEvidence"   "tblLUGender"          
##  [5] "tblLUMarkerType"       "tblLUMultipleBirth"   
##  [7] "tblLURaceCohort"       "tblLURelationshipPath"
##  [9] "tblLURoster"           "tblLUTristate"        
## [11] "tblLUYesNo"            "tblMzManual"          
## [13] "tblvariable"
```

```r
ds_file
```

```
## # A tibble: 13 x 11
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
## 10 LUTri~ data-p~ <S3: ~ T      Enum  Tris~ byte  T     tblL~ DELET~ <tib~
## 11 LUYes~ data-p~ <S3: ~ T      Enum  YesNo short T     tblL~ DELET~ <tib~
## 12 MzMan~ data-p~ <S3: ~ T      Meta~ NA_c~ NA_c~ F     tblM~ DELET~ <tib~
## 13 varia~ data-p~ <S3: ~ T      Meta~ NA_c~ NA_c~ F     tblv~ DELET~ <tib~
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
##     subject_id                                                   =     1, 
##     extended_family_id                                           =     2, 
##     gender                                                       =    10, 
##     DateOfBirthMonth                                             =    11, 
##     DateOfBirthYear                                              =    12, 
##     sample_cohort                                                =    13, 
##     InterviewDateDay                                             =    20, 
##     InterviewDateMonth                                           =    21, 
##     InterviewDateYear                                            =    22, 
##     AgeAtInterviewDateMonths                                     =    23, 
##     AgeAtInterviewDateYears                                      =    24, 
##     roster_crosswalk                                             =   101, 
##     hh_member_id                                                 =   102, 
##     // InterviewDateDayParent_NOTUSED                            =  1020, 
##     // InterviewDateMonthParent_NOTUSED                          =  1021, 
##     // InterviewDateYearParent_NOTUSED                           =  1022, 
## }
##  
## public enum ExtractSource {
##     97-demographics                                              =     1, 
##     97-roster                                                    =     2, 
##     97-survey-time                                               =     3, 
##     97-links-explicit                                            =     4, 
##     97-links-implicit                                            =     5, 
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
##     valid_skip                                                   =    -4, 
##     Identity                                                     =     0, 
##     Wife                                                         =     1, 
##     Husband                                                      =     2, 
##     Mother                                                       =     3, 
##     Father                                                       =     4, 
##     Step-mother                                                  =     7, 
##     Step-father                                                  =     8, 
##     Adoptive mother                                              =     5, 
##     Adoptive father                                              =     6, 
##     Foster mother                                                =     9, 
##     Foster father                                                =    10, 
##     Mother-in-law                                                =    11, 
##     Father-in-law                                                =    12, 
##     Sister (FULL)                                                =    13, 
##     Brother (FULL)                                               =    14, 
##     Sister (HALF - Same mother)                                  =    15, 
##     Sister (HALF - Same father)                                  =    16, 
##     Sister (HALF - don't know)                                   =    17, 
##     Brother (HALF - Same mother)                                 =    18, 
##     Brother (HALF - Same father)                                 =    19, 
##     Brother (HALF - don't know)                                  =    20, 
##     Sister (STEP)                                                =    21, 
##     Brother (STEP)                                               =    22, 
##     Sister (ADOPTIVE)                                            =    23, 
##     Brother (ADOPTIVE)                                           =    24, 
##     Sister (FOSTER)                                              =    25, 
##     Brother (FOSTER)                                             =    26, 
##     Brother-in-law                                               =    27, 
##     Sister-in-law                                                =    28, 
##     Maternal Grandmother                                         =    29, 
##     Paternal Grandmother                                         =    30, 
##     Social Grandmother                                           =    31, 
##     Grandmother (don't know or refused)                          =    32, 
##     Maternal Grandfather                                         =    33, 
##     Paternal Grandfather                                         =    34, 
##     Social Grandfather                                           =    35, 
##     Grandfather (don't know or refused)                          =    36, 
##     Maternal Great-Grandmother                                   =    37, 
##     Paternal Great-Grandmother                                   =    38, 
##     Social Great-Grandmother                                     =    39, 
##     Great-Grandmother (don't know or refused)                    =    40, 
##     Maternal Great-Grandfather                                   =    41, 
##     Paternal Great-Grandfather                                   =    42, 
##     Social Great-Grandfather                                     =    43, 
##     Great-Grandfather (don't know or refused)                    =    44, 
##     Great Great Grandmother                                      =    45, 
##     Great Great Grandfather                                      =    46, 
##     Granddaughter (Biological or social)                         =    47, 
##     Grandson (Biological or social)                              =    48, 
##     Daughter (Biological)                                        =    49, 
##     Son (Biological)                                             =    50, 
##     Step-daughter                                                =    51, 
##     Step-son                                                     =    52, 
##     Adoptive daughter                                            =    53, 
##     Adoptive son                                                 =    54, 
##     Foster daughter                                              =    55, 
##     Foster son                                                   =    56, 
##     Daughter of lover/partner                                    =    57, 
##     Son of lover/partner                                         =    58, 
##     Daughter-in-law                                              =    59, 
##     Son-in-law                                                   =    60, 
##     Grandmother-in-law                                           =    61, 
##     Grandfather-in-law                                           =    62, 
##     Aunt-in-law                                                  =    63, 
##     Uncle-in-law                                                 =    64, 
##     Cousin-in-law                                                =    65, 
##     Great-Grandmother-in-law                                     =    66, 
##     Great-Grandfather-in-law                                     =    67, 
##     Roommate                                                     =    68, 
##     Lover/partner                                                =    69, 
##     Aunt (biological or social)                                  =    70, 
##     Great Aunt                                                   =    71, 
##     Uncle (biological or social)                                 =    72, 
##     Great Uncle                                                  =    73, 
##     Niece (biological or social)                                 =    74, 
##     Step Niece (biological or social)                            =    75, 
##     Foster Niece (biological or social)                          =    76, 
##     Adoptive Niece (biological or social)                        =    77, 
##     Nephew (biological or social)                                =    78, 
##     Step Nephew (biological or social)                           =    79, 
##     Foster Nephew (biological or social)                         =    80, 
##     Adoptive Nephew (biological or social)                       =    81, 
##     Female cousin (biological or social)                         =    82, 
##     Male cousin (biological or social)                           =    83, 
##     Other relative                                               =    84, 
##     Other non-relative                                           =    85, 
##     Great Grandson                                               =    86, 
##     Great Granddaughter                                          =    87, 
##     RELATIONSHIP MISSING                                         =    99, 
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
RODBC::odbcGetInfo(channel_rodbc)
```

```
##              DBMS_Name               DBMS_Ver        Driver_ODBC_Ver 
## "Microsoft SQL Server"           "13.00.4206"                "03.80" 
##       Data_Source_Name            Driver_Name             Driver_Ver 
##  "local-nlsy-links-97"      "msodbcsql13.dll"           "14.00.1000" 
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
## $tblRoster
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
## $tblOutcome
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
## $tblMzManual
## data frame with 0 columns and 0 rows
## 
## $tblLUYesNo
## data frame with 0 columns and 0 rows
## 
## $tblLUTristate
## data frame with 0 columns and 0 rows
## 
## $tblLURoster
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
    # seq_len(nrow(ds_file))
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
    message("Writing to table ", table_name)
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
## Writing to table tblitem
```

```
## Writing to table tblLUExtractSource
```

```
## Writing to table tblLUMarkerEvidence
```

```
## Writing to table tblLUGender
```

```
## Writing to table tblLUMarkerType
```

```
## Writing to table tblLUMultipleBirth
```

```
## Writing to table tblLURaceCohort
```

```
## Writing to table tblLURelationshipPath
```

```
## Writing to table tblLURoster
```

```
## Writing to table tblLUTristate
```

```
## Writing to table tblLUYesNo
```

```
## Writing to table tblMzManual
```

```
## Writing to table tblvariable
```

```
##  [1] 1 1 1 1 1 1 1 1 1 1 1 1 1
```

```r
# purrr::set_names(ds_file$table_name)
# a <- ds_file$entries[[13]]
# table(a$ID)


# odbc::dbWriteTable(
#   conn    = channel,
#   name    = DBI::SQL("Metadata.tblRosterAssignment"),
#   # name    = "tblvariable_97",
#   # schema  = "Metadata",
#   value   = ds_file$entries[[13]],
#   append  = T
# )

# RODBC::sqlSave(
#   channel     = channel_rodbc,
#   dat         = ds_file$entries[[13]][1:1, ],
#   # tablename   = table_name,
#   tablename   = "Metadata.tblRosterAssignment",
#   safer       = TRUE,       # Don't keep the existing table.
#   rownames    = FALSE,
#   append      = TRUE
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
cat("`import-97-metadata.R` file completed by `", Sys.info()["user"], "` at ", strftime(Sys.time(), "%Y-%m-%d, %H:%M %z"), " in ",  duration_in_seconds, " seconds.", sep="")
```

```
## `import-97-metadata.R` file completed by `Will` at 2018-01-16, 17:24 -0600 in 2 seconds.
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
##  [1] Rcpp_0.12.14          highr_0.6             plyr_1.8.4           
##  [4] pillar_1.0.1          compiler_3.4.3        bindr_0.1            
##  [7] tools_3.4.3           odbc_1.1.3            digest_0.6.13        
## [10] bit_1.1-12            memoise_1.1.0         evaluate_0.10.1      
## [13] tibble_1.4.1          checkmate_1.8.5       pkgconfig_2.0.1      
## [16] rlang_0.1.6           DBI_0.7               cli_1.0.0            
## [19] rstudioapi_0.7        yaml_2.1.16           withr_2.1.1.9000     
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
## [1] "2018-01-16 17:24:02 CST"
```

