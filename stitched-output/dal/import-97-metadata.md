



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
requireNamespace("odbc"                   ) #For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.
```

```r
# Constant values that won't change.
config                    <- config::get()
directory_in              <- "data-public/metadata/tables-97"
study                     <- "97"
shallow_only              <- F   # If TRUE, update only the metadata tables that won't delete any other database tables.

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
  LUExtractSource         = col_types_minimal,
  LUMarkerEvidence        = col_types_minimal,
  LUGender                = col_types_minimal,
  LUMarkerType = readr::cols_only(
    ID                                  = readr::col_integer(),
    Label                               = readr::col_character(),
    Explicit                            = readr::col_integer(),
    Active                              = readr::col_logical(),
    Notes                               = readr::col_character()
  ),
  LUMultipleBirth         = col_types_minimal,
  LURaceCohort            = col_types_minimal,
  LURoster                = col_types_minimal,
  LUTristate              = col_types_minimal,
  LUYesNo                 = col_types_minimal,
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
  RosterAssignment    = readr::cols_only(
    ID                                  = readr::col_integer(),
    ResponseLower                       = readr::col_integer(),
    ResponseUpper                       = readr::col_integer(),
    Freq                                = readr::col_integer(),
    Resolved                            = readr::col_integer(),
    R                                   = readr::col_double(),
    RBoundLower                         = readr::col_double(),
    RBoundUpper                         = readr::col_double(),
    SameGeneration                      = readr::col_double(),
    ShareBiodad                         = readr::col_integer(),
    ShareBiomom                         = readr::col_integer(),
    ShareBiograndparent                 = readr::col_integer(),
    Inconsistent                        = readr::col_integer(),
    Notes                               = readr::col_character(),
    ResponseLowerLabel                  = readr::col_character(),
    ResponseUpperLabel                  = readr::col_character()
  ),
  variable = readr::cols_only(
    # ID                                = readr::col_integer(),
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
  convert_to_enum     = readr::col_logical(),
  shallow             = readr::col_logical()
)

ds_file <- lst_col_types %>%
  tibble::enframe(value = "col_types") %>%
  dplyr::mutate(
    path        = file.path(directory_in, paste0(name, ".csv")),
    exists      = purrr::map_lgl(path, file.exists)
    # col_types = purrr::map(name, function(x) lst_col_types[[x]]),
  ) %>%
  dplyr::select(name, path, dplyr::everything())
ds_file
```

```
## # A tibble: 14 x 4
##    name               path                              col_types   exists
##    <chr>              <chr>                             <list>      <lgl> 
##  1 ArchiveDescription data-public/metadata/tables-97/A~ <S3: col_s~ TRUE  
##  2 item               data-public/metadata/tables-97/i~ <S3: col_s~ TRUE  
##  3 LUExtractSource    data-public/metadata/tables-97/L~ <S3: col_s~ TRUE  
##  4 LUMarkerEvidence   data-public/metadata/tables-97/L~ <S3: col_s~ TRUE  
##  5 LUGender           data-public/metadata/tables-97/L~ <S3: col_s~ TRUE  
##  6 LUMarkerType       data-public/metadata/tables-97/L~ <S3: col_s~ TRUE  
##  7 LUMultipleBirth    data-public/metadata/tables-97/L~ <S3: col_s~ TRUE  
##  8 LURaceCohort       data-public/metadata/tables-97/L~ <S3: col_s~ TRUE  
##  9 LURoster           data-public/metadata/tables-97/L~ <S3: col_s~ TRUE  
## 10 LUTristate         data-public/metadata/tables-97/L~ <S3: col_s~ TRUE  
## 11 LUYesNo            data-public/metadata/tables-97/L~ <S3: col_s~ TRUE  
## 12 MzManual           data-public/metadata/tables-97/M~ <S3: col_s~ TRUE  
## 13 RosterAssignment   data-public/metadata/tables-97/R~ <S3: col_s~ TRUE  
## 14 variable           data-public/metadata/tables-97/v~ <S3: col_s~ TRUE
```

```r
start_time <- Sys.time()

ds_mapping <- readr::read_csv(file.path(directory_in, "_mapping.csv"), col_types=col_types_mapping)



testit::assert("All metadata files must exist.", all(ds_file$exists))

ds_entries <- ds_file %>%
  # dplyr::slice(14) %>%
  dplyr::select(name, path, col_types) %>%
  dplyr::mutate(
    entries = purrr::pmap(list(file=.$path, col_types=.$col_types), readr::read_csv, comment = "#")
  )
ds_entries
```

```
## # A tibble: 14 x 4
##    name               path                        col_types   entries     
##    <chr>              <chr>                       <list>      <list>      
##  1 ArchiveDescription data-public/metadata/table~ <S3: col_s~ <tibble [7 ~
##  2 item               data-public/metadata/table~ <S3: col_s~ <tibble [26~
##  3 LUExtractSource    data-public/metadata/table~ <S3: col_s~ <tibble [6 ~
##  4 LUMarkerEvidence   data-public/metadata/table~ <S3: col_s~ <tibble [8 ~
##  5 LUGender           data-public/metadata/table~ <S3: col_s~ <tibble [3 ~
##  6 LUMarkerType       data-public/metadata/table~ <S3: col_s~ <tibble [28~
##  7 LUMultipleBirth    data-public/metadata/table~ <S3: col_s~ <tibble [5 ~
##  8 LURaceCohort       data-public/metadata/table~ <S3: col_s~ <tibble [4 ~
##  9 LURoster           data-public/metadata/table~ <S3: col_s~ <tibble [92~
## 10 LUTristate         data-public/metadata/table~ <S3: col_s~ <tibble [3 ~
## 11 LUYesNo            data-public/metadata/table~ <S3: col_s~ <tibble [6 ~
## 12 MzManual           data-public/metadata/table~ <S3: col_s~ <tibble [90~
## 13 RosterAssignment   data-public/metadata/table~ <S3: col_s~ <tibble [31~
## 14 variable           data-public/metadata/table~ <S3: col_s~ <tibble [55~
```

```r
# d <- readr::read_csv("data-public/metadata/tables/variable_97.csv", col_types=lst_col_types$variable_97, comment = "#")
# readr::problems(d)
# ds_entries$entries[15]

ds_table <- database_inventory(study)
ds_table
```

```
## # A tibble: 33 x 6
##    schema_name table_name            row_count column_count space_total_kb
##  * <chr>       <chr>                     <int>        <int>          <int>
##  1 Archive     tblArchiveDescription         7            3             72
##  2 Archive     tblRelatedValuesArch~     25190           23           2264
##  3 dbo         sysdiagrams                   0            5              0
##  4 Enum        tblLUExtractSource            6            4             72
##  5 Enum        tblLUGender                   3            4             72
##  6 Enum        tblLUMarkerEvidence           8            4             72
##  7 Enum        tblLUMarkerType              28            5             72
##  8 Enum        tblLUMultipleBirth            5            4             72
##  9 Enum        tblLURaceCohort               4            4             72
## 10 Enum        tblLURoster                  92            4             72
## # ... with 23 more rows, and 1 more variable: space_used_kb <int>
```

```r
rm(directory_in) # rm(col_types_tulsa)
```

```r
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call to `dplyr::rename()`.

if( shallow_only ) {
  ds_mapping <- ds_mapping %>%
    dplyr::filter(.data$shallow)
}
ds_mapping
```

```
## # A tibble: 14 x 6
##    table_name   schema_name enum_name c_sharp_type convert_to_enum shallow
##    <chr>        <chr>       <chr>     <chr>        <lgl>           <lgl>  
##  1 ArchiveDesc~ Archive     NA_chara~ NA_character FALSE           TRUE   
##  2 item         Metadata    Item      short        TRUE            FALSE  
##  3 LUExtractSo~ Enum        ExtractS~ byte         TRUE            FALSE  
##  4 LUGender     Enum        Gender    byte         TRUE            FALSE  
##  5 LUMarkerEvi~ Enum        MarkerEv~ byte         TRUE            FALSE  
##  6 LUMarkerType Enum        MarkerTy~ byte         TRUE            FALSE  
##  7 LUMultipleB~ Enum        Multiple~ byte         TRUE            FALSE  
##  8 LURaceCohort Enum        RaceCoho~ byte         TRUE            FALSE  
##  9 LURoster     Enum        RosterGe~ short        TRUE            FALSE  
## 10 LUTristate   Enum        Tristate  byte         TRUE            FALSE  
## 11 LUYesNo      Enum        YesNo     short        TRUE            FALSE  
## 12 MzManual     Metadata    NA_chara~ NA_character FALSE           TRUE   
## 13 RosterAssig~ Metadata    NA_chara~ NA_character FALSE           FALSE  
## 14 variable     Metadata    NA_chara~ NA_character FALSE           FALSE
```

```r
ds_file <- ds_file %>%
  dplyr::inner_join(ds_mapping, by=c("name"="table_name")) %>%
  dplyr::mutate(
    table_name    = paste0("tbl", name),
    sql_delete    = glue::glue("DELETE FROM {schema_name}.{table_name};")
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
## # A tibble: 7 x 3
##   AlgorithmVersion Description                                  Date      
##              <int> <chr>                                        <date>    
## 1                1 naive roster                                 2018-01-17
## 2                2 account for twins                            2018-01-18
## 3                3 same sib full twins are R=.5 by default, an~ 2018-02-14
## 4                7 allows nulls for RFull                       2018-06-19
## 5                8 recover different-sex full sibs              2018-06-19
## 6                9 recover same-sex full sibs                   2018-06-19
## 7               10 allow nonsibs to still be r>0                2018-06-19
## # A tibble: 26 x 7
##       ID Label           MinValue MinNonnegative MaxValue Active Notes    
##    <int> <chr>              <int>          <int>    <int> <lgl>  <chr>    
##  1     1 subject_id             1              1     9022 TRUE   <NA>     
##  2     2 extended_famil~        1              1     9022 TRUE   <NA>     
##  3     3 hh_internal_id         1              1        5 TRUE   <NA>     
##  4    10 gender                 1              1        2 TRUE   <NA>     
##  5    11 DateOfBirthMon~        1              1       12 TRUE   <NA>     
##  6    12 DateOfBirthYear     1980           1980     1984 TRUE   <NA>     
##  7    13 cross_sectiona~        0              0        1 TRUE   <NA>     
##  8    14 race_cohort            1              1        4 TRUE   race-eth~
##  9    20 InterviewDateD~       -7              1       31 TRUE   <NA>     
## 10    21 InterviewDateM~       -7              1       12 TRUE   <NA>     
## # ... with 16 more rows
## # A tibble: 6 x 4
##      ID Label             Active Notes
##   <int> <chr>             <lgl>  <chr>
## 1     1 97-demographics   TRUE   <NA> 
## 2     2 97-roster         TRUE   <NA> 
## 3     3 97-survey-time    TRUE   <NA> 
## 4     4 97-links-explicit TRUE   <NA> 
## 5     5 97-links-implicit TRUE   <NA> 
## 6     6 97-twins          TRUE   <NA> 
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
##  1     1 Roster                     1 TRUE   <NA> 
##  2     2 ShareBiomom                1 TRUE   <NA> 
##  3     3 ShareBiodad                1 TRUE   <NA> 
##  4     5 DobSeparation              0 FALSE  <NA> 
##  5     6 GenderAgreement            0 FALSE  <NA> 
##  6    10 FatherAsthma               0 FALSE  <NA> 
##  7    11 BabyDaddyAsthma            0 FALSE  <NA> 
##  8    12 BabyDaddyLeftHHDate        0 FALSE  <NA> 
##  9    13 BabyDaddyDeathDate         0 FALSE  <NA> 
## 10    14 BabyDaddyAlive             0 FALSE  <NA> 
## # ... with 18 more rows
## # A tibble: 5 x 4
##      ID Label      Active Notes                                           
##   <int> <chr>      <lgl>  <chr>                                           
## 1     0 No         TRUE   <NA>                                            
## 2     2 Twin       TRUE   <NA>                                            
## 3     3 Trip       TRUE   <NA>                                            
## 4     4 TwinOrTrip FALSE  Currently Then Gen1 algorithm doesn't distingui~
## 5   255 DoNotKnow  TRUE   <NA>                                            
## # A tibble: 4 x 4
##      ID Label    Active Notes
##   <int> <chr>    <lgl>  <chr>
## 1     1 Black    TRUE   <NA> 
## 2     2 Hispanic TRUE   <NA> 
## 3     3 Mixed    TRUE   <NA> 
## 4     4 Nbnh     TRUE   <NA> 
## # A tibble: 92 x 4
##       ID Label       Active Notes
##    <int> <chr>       <lgl>  <chr>
##  1    -4 valid_skip  TRUE   <NA> 
##  2    -2 do_not_know TRUE   <NA> 
##  3    -1 refusal     TRUE   <NA> 
##  4     0 self        TRUE   <NA> 
##  5     1 wife        TRUE   <NA> 
##  6     2 husband     TRUE   <NA> 
##  7     3 mother      TRUE   <NA> 
##  8     4 father      TRUE   <NA> 
##  9     7 mother_step TRUE   <NA> 
## 10     8 father_step TRUE   <NA> 
## # ... with 82 more rows
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
## # A tibble: 90 x 8
##       ID SubjectTag_S1 SubjectTag_S2 MultipleBirthIfSameS~  IsMz Undecided
##    <int>         <int>         <int>                 <int> <int>     <int>
##  1     1            66            67                     0     0         0
##  2     2            75            76                     2     1         0
##  3     3           116           117                     2     1         0
##  4     4           222           223                     2     0         0
##  5     5           343           344                     3   255         1
##  6     6           343           345                     3   255         1
##  7     7           344           345                     3   255         1
##  8     8           351           352                     2     1         0
##  9     9           447           448                     2     1         0
## 10    10           588           589                     2     0         0
## # ... with 80 more rows, and 2 more variables: Related <int>, Notes <chr>
## # A tibble: 31 x 16
##       ID ResponseLower ResponseUpper  Freq Resolved     R RBoundLower
##    <int>         <int>         <int> <int>    <int> <dbl>       <dbl>
##  1     1            -2            -1     2        0 NA           0   
##  2     2            -1            -1     2        0 NA           0   
##  3     3            13            13  1034        0  0.5         0.5 
##  4     4            13            14  2034        1  0.5         0.5 
##  5     5            14            14  1154        0  0.5         0.5 
##  6     6            15            15    48        1  0.25        0.25
##  7     7            15            18   132        1  0.25        0.25
##  8     8            16            19     2        1  0.25        0.25
##  9     9            18            18    62        1  0.25        0.25
## 10    10            19            19     8        1  0.25        0.25
## # ... with 21 more rows, and 9 more variables: RBoundUpper <dbl>,
## #   SameGeneration <dbl>, ShareBiodad <int>, ShareBiomom <int>,
## #   ShareBiograndparent <int>, Inconsistent <int>, Notes <chr>,
## #   ResponseLowerLabel <chr>, ResponseUpperLabel <chr>
## # A tibble: 551 x 11
##    VariableCode  Item ExtractSource SurveyYear LoopIndex1 LoopIndex2
##    <chr>        <int>         <int>      <int>      <int>      <int>
##  1 R0000100         1             1       1997          1          1
##  2 R1193000         2             1       1997          1          1
##  3 R0533400         3             1       1997          1          1
##  4 R0536300        10             1       1997          1          1
##  5 R0536401        11             1       1997          1          1
##  6 R0536402        12             1       1997          1          1
##  7 R1235800        13             1       1997          1          1
##  8 R1482600        14             1       1997          1          1
##  9 R1097800       101             2       1997          1          1
## 10 R1097900       101             2       1997          2          1
## # ... with 541 more rows, and 5 more variables: Translate <int>,
## #   Active <int>, Notes <chr>, QuestionName <chr>, VariableTitle <chr>
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
##  [1] "tblArchiveDescription" "tblitem"              
##  [3] "tblLUExtractSource"    "tblLUMarkerEvidence"  
##  [5] "tblLUGender"           "tblLUMarkerType"      
##  [7] "tblLUMultipleBirth"    "tblLURaceCohort"      
##  [9] "tblLURoster"           "tblLUTristate"        
## [11] "tblLUYesNo"            "tblMzManual"          
## [13] "tblRosterAssignment"   "tblvariable"
```

```r
ds_file
```

```
## # A tibble: 14 x 12
##    name    path        col_types exists schema_name enum_name c_sharp_type
##    <chr>   <chr>       <list>    <lgl>  <chr>       <chr>     <chr>       
##  1 Archiv~ data-publi~ <S3: col~ TRUE   Archive     NA_chara~ NA_character
##  2 item    data-publi~ <S3: col~ TRUE   Metadata    Item      short       
##  3 LUExtr~ data-publi~ <S3: col~ TRUE   Enum        ExtractS~ byte        
##  4 LUMark~ data-publi~ <S3: col~ TRUE   Enum        MarkerEv~ byte        
##  5 LUGend~ data-publi~ <S3: col~ TRUE   Enum        Gender    byte        
##  6 LUMark~ data-publi~ <S3: col~ TRUE   Enum        MarkerTy~ byte        
##  7 LUMult~ data-publi~ <S3: col~ TRUE   Enum        Multiple~ byte        
##  8 LURace~ data-publi~ <S3: col~ TRUE   Enum        RaceCoho~ byte        
##  9 LURost~ data-publi~ <S3: col~ TRUE   Enum        RosterGe~ short       
## 10 LUTris~ data-publi~ <S3: col~ TRUE   Enum        Tristate  byte        
## 11 LUYesNo data-publi~ <S3: col~ TRUE   Enum        YesNo     short       
## 12 MzManu~ data-publi~ <S3: col~ TRUE   Metadata    NA_chara~ NA_character
## 13 Roster~ data-publi~ <S3: col~ TRUE   Metadata    NA_chara~ NA_character
## 14 variab~ data-publi~ <S3: col~ TRUE   Metadata    NA_chara~ NA_character
## # ... with 5 more variables: convert_to_enum <lgl>, shallow <lgl>,
## #   table_name <chr>, sql_delete <chr>, entries <list>
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
##     hh_internal_id                                               =     3, 
##     gender                                                       =    10, 
##     DateOfBirthMonth                                             =    11, 
##     DateOfBirthYear                                              =    12, 
##     cross_sectional_cohort                                       =    13, 
##     race_cohort                                                  =    14, // race-ethnicity
##     InterviewDateDay                                             =    20, 
##     InterviewDateMonth                                           =    21, 
##     InterviewDateYear                                            =    22, 
##     AgeAtInterviewDateMonths                                     =    23, 
##     AgeAtInterviewDateYears                                      =    24, 
##     roster_crosswalk                                             =   101, 
##     hh_member_id                                                 =   102, 
##     hh_informant                                                 =   103, 
##     // roster_relationship_2_dim                                 =   104, // 16 x 16 square
##     roster_relationship_1_dim                                    =   105, // 1 x 16 vector
##     hh_unique_id                                                 =   106, // HHI2: People living in the Household - sorted, UID; HH member's unique ID
##     pair_multiple_birth                                          =   121, 
##     pair_twins_mz                                                =   122, 
##     pair_sister_same_bioparent                                   =   123, 
##     pair_brother_same_bioparent                                  =   124, 
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
##     97-twins                                                     =     6, 
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
##     Roster                                                       =     1, 
##     ShareBiomom                                                  =     2, 
##     ShareBiodad                                                  =     3, 
##     // DobSeparation                                             =     5, 
##     // GenderAgreement                                           =     6, 
##     // FatherAsthma                                              =    10, 
##     // BabyDaddyAsthma                                           =    11, 
##     // BabyDaddyLeftHHDate                                       =    12, 
##     // BabyDaddyDeathDate                                        =    13, 
##     // BabyDaddyAlive                                            =    14, 
##     // BabyDaddyInHH                                             =    15, 
##     // BabyDaddyDistanceFromHH                                   =    16, 
##     // Gen2CFatherAlive                                          =    17, 
##     // Gen2CFatherInHH                                           =    18, 
##     // Gen2CFatherDistanceFromHH                                 =    19, 
##     // Gen1BiodadInHH                                            =    30, 
##     // Gen1BiodadDeathAge                                        =    31, 
##     // Gen1BiodadBirthYear                                       =    32, 
##     // Gen1BiodadInHH1979                                        =    33, 
##     // Gen1BiodadBirthCountry                                    =    34, 
##     // Gen1BiodadBirthState                                      =    35, 
##     // Gen1BiomomInHH                                            =    40, 
##     // Gen1BiomomDeathAge                                        =    41, 
##     // Gen1BiomomBirthYear                                       =    42, 
##     // Gen1BiomomInHH1979                                        =    43, 
##     // Gen1BiomomBirthCountry                                    =    44, 
##     // Gen1BiomomBirthState                                      =    45, 
##     // Gen1AlwaysLivedWithBothBioparents                         =    50, 
## }
##  
## public enum MultipleBirth {
##     No                                                           =     0, 
##     Twin                                                         =     2, 
##     Trip                                                         =     3, 
##     // TwinOrTrip                                                =     4, // Currently Then Gen1 algorithm doesn't distinguish.
##     DoNotKnow                                                    =   255, 
## }
##  
## public enum RaceCohort {
##     Black                                                        =     1, 
##     Hispanic                                                     =     2, 
##     Mixed                                                        =     3, 
##     Nbnh                                                         =     4, 
## }
##  
## public enum RosterGen1 {
##     valid_skip                                                   =    -4, 
##     do_not_know                                                  =    -2, 
##     refusal                                                      =    -1, 
##     self                                                         =     0, 
##     wife                                                         =     1, 
##     husband                                                      =     2, 
##     mother                                                       =     3, 
##     father                                                       =     4, 
##     mother_step                                                  =     7, 
##     father_step                                                  =     8, 
##     mother_adoptive                                              =     5, 
##     father_adoptive                                              =     6, 
##     mother_foster                                                =     9, 
##     father_foster                                                =    10, 
##     mother_in_law                                                =    11, 
##     father_in_law                                                =    12, 
##     sister_full                                                  =    13, 
##     brother_full                                                 =    14, 
##     sister_half_same_mother                                      =    15, 
##     sister_half_same_father                                      =    16, 
##     sister_half_unsure                                           =    17, 
##     brother_half_same_mother                                     =    18, 
##     brother_half_same_father                                     =    19, 
##     brother_half_unsure                                          =    20, 
##     sister_step                                                  =    21, 
##     brother_step                                                 =    22, 
##     sister_adoptive                                              =    23, 
##     brother_adoptive                                             =    24, 
##     sister_foster                                                =    25, 
##     brother_foster                                               =    26, 
##     brother_in_law                                               =    27, 
##     sister_in_law                                                =    28, 
##     grandmother_maternal                                         =    29, 
##     grandmother_paternal                                         =    30, 
##     grandmother_social                                           =    31, 
##     grandmother_unsure                                           =    32, 
##     grandfather_maternal                                         =    33, 
##     grandfather_paternal                                         =    34, 
##     grandfather_social                                           =    35, 
##     grandfather_unsure                                           =    36, 
##     great_grandmother                                            =    37, 
##     great_grandfather                                            =    38, 
##     great_grandmother_social                                     =    39, 
##     great_grandmother_unsure                                     =    40, 
##     great_grandfather_maternal                                   =    41, 
##     great_grandfather_paternal                                   =    42, 
##     great_grandfather_social                                     =    43, 
##     great_grandfather_unsure                                     =    44, 
##     great_great_grandmother                                      =    45, 
##     great_great_grandfather                                      =    46, 
##     granddaughter                                                =    47, 
##     grandson                                                     =    48, 
##     daughter_bio                                                 =    49, 
##     son_bio                                                      =    50, 
##     daughter_step                                                =    51, 
##     son_step                                                     =    52, 
##     daughter_adoptive                                            =    53, 
##     son_adoptive                                                 =    54, 
##     daughter_foster                                              =    55, 
##     son_foster                                                   =    56, 
##     daughter_of_partner                                          =    57, 
##     son_of_partner                                               =    58, 
##     daughter_in_law                                              =    59, 
##     son_in_law                                                   =    60, 
##     grandmother_in_law                                           =    61, 
##     grandfather_in_law                                           =    62, 
##     aunt_in_law                                                  =    63, 
##     uncle_in_law                                                 =    64, 
##     cousin_in_law                                                =    65, 
##     great_grandmother_in_law                                     =    66, 
##     great_grandfather_in_law                                     =    67, 
##     roommate                                                     =    68, 
##     partner                                                      =    69, 
##     aunt_unsure                                                  =    70, 
##     great_aunt                                                   =    71, 
##     uncle_unsure                                                 =    72, 
##     great_uncle                                                  =    73, 
##     niece_unsure                                                 =    74, 
##     niece_step                                                   =    75, 
##     niece_foster                                                 =    76, 
##     niece_adoptive                                               =    77, 
##     nephew_unsure                                                =    78, 
##     nephew_step                                                  =    79, 
##     nephew_foster                                                =    80, 
##     nephew_adoptive                                              =    81, 
##     cousin_female_unsure                                         =    82, 
##     cousin_male_unsure                                           =    83, 
##     relative_other                                               =    84, 
##     nonrelative_other                                            =    85, 
##     great_grandson                                               =    86, 
##     great_granddaughter                                          =    87, 
##     relationship_missing                                         =    99, 
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
if( !shallow_only ) {
  d_extract_source <- ds_file  %>%
    dplyr::filter(name=="LUExtractSource") %>%
    dplyr::pull(entries) %>%
    purrr::flatten_df()

  d_item <- ds_file  %>%
    dplyr::filter(name=="item") %>%
    dplyr::pull(entries) %>%
    purrr::flatten_df()

  checkmate::assert_integer(  d_item$ID           , lower=1, upper=2^15   , any.missing=F, unique=T)
  checkmate::assert_character(d_item$Label        , pattern="^\\w+"       , any.missing=F, unique=T)


  d_variable <- ds_file  %>%
    dplyr::filter(name=="variable") %>%
    dplyr::pull(entries) %>%
    purrr::flatten_df() %>%
    dplyr::mutate(
      item_found    = (ExtractSource %in% d_extract_source$ID),
      extract_found = (Item %in% d_item$ID),
      unique_index  = paste(Item, SurveyYear, LoopIndex1, LoopIndex2)
    ) %>%
    dplyr::group_by(unique_index) %>%
    dplyr::mutate(
      unique_index_violation  = (1L < n())
    ) %>%
    dplyr::ungroup()


  pattern_unique_index <- "^\\d{1,5} \\d{4} \\d{1,2} \\d{1,2}$"
  checkmate::assert_character(d_variable$VariableCode                     , pattern="^[A-Z]\\d{7}$"            , any.missing=F, unique=T)
  checkmate::assert_integer(  d_variable$Item                             , lower=0    , any.missing=F)
  checkmate::assert_logical(  d_variable$item_found                                    , any.missing=F)
  testit::assert("All items referenced from the variables should be in the item table.", all(d_variable$item_found))
  testit::assert("All extract sources referenced from the variables should be in the item table.", all(d_variable$extract_found))
  checkmate::assert_character(d_variable$unique_index   , pattern=pattern_unique_index  , any.missing=F, unique=T)

  rm(d_item, d_variable)
}
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
## "Microsoft SQL Server"           "13.00.4206"                "03.80" 
##       Data_Source_Name            Driver_Name             Driver_Ver 
##  "local-nlsy-links-97"      "msodbcsql17.dll"           "17.01.0000" 
##               ODBC_Ver            Server_Name 
##           "03.80.0000" "GIMBLE\\EXPRESS_2016"
```

```r
if( !shallow_only ){
# Clear process tables
  delete_results_process <- ds_table_process$sql_truncate %>%
    purrr::set_names(ds_table_process$table_name) %>%
    rev() %>%
    purrr::map(DBI::dbGetQuery, conn=channel)
  delete_results_process
}
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
## $tblParentsOfGen1Current
## data frame with 0 columns and 0 rows
## 
## $tblOutcome
## data frame with 0 columns and 0 rows
## 
## $tblMarker
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
## $tblRosterAssignment
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

# ds_file <- ds_file %>%
#   dplyr::slice(1)
# Upload metadata tables

# i <- 2L
# OuhscMunge::upload_sqls_odbc(
#   d             = ds_file$entries[[i]] %>%
#     dplyr::mutate_if(is.logical, as.character),
#   schema_name   = ds_file$schema_name[[i]],
#   table_name    = ds_file$table_name[[i]],
#   dsn_name      = dsn_name(study),
#   clear_table   = F,
#   create_table  = FALSE,
#   convert_logical_to_integer = F
# )

purrr::pmap_int(
  list(
    ds_file$entries,
    ds_file$table_name,
    ds_file$schema_name
    # seq_len(nrow(ds_file))
  ),
  function( d, table_name, schema_name ) {
    message("Writing to table ", table_name)
    # OuhscMunge::upload_sqls_odbc(
    #   d             = d,
    #   schema_name   = schema_name,
    #   table_name    = table_name,
    #   dsn_name      = dsn_name(study),
    #   clear_table   = TRUE,
    #   create_table  = FALSE,
    #   convert_logical_to_integer = TRUE
    # )
    # browser()
    # DBI::dbWriteTable(
    #   conn    = channel,
    #   name    = table_name,
    #   schema  = schema_name,
    #   value   = d,
    #
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
## Writing to table tblArchiveDescription
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
## Writing to table tblRosterAssignment
```

```
## Writing to table tblvariable
```

```
##  [1] 1 1 1 1 1 1 1 1 1 1 1 1 1 1
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

# Close channels
DBI::dbDisconnect(channel); rm(channel)
RODBC::odbcClose(channel_rodbc); rm(channel_rodbc)

duration_in_seconds <- round(as.numeric(difftime(Sys.time(), start_time, units="secs")))
cat("`import-97-metadata.R` file completed by `", Sys.info()["user"], "` at ", strftime(Sys.time(), "%Y-%m-%d, %H:%M %z"), " in ",  duration_in_seconds, " seconds.", sep="")
```

```
## `import-97-metadata.R` file completed by `Will` at 2018-06-20, 11:58 -0500 in 2 seconds.
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
## [1] knitr_1.20     bindrcpp_0.2.2 magrittr_1.5  
## 
## loaded via a namespace (and not attached):
##  [1] Rcpp_0.12.17     highr_0.7        plyr_1.8.4       pillar_1.2.3    
##  [5] compiler_3.5.0   bindr_0.1.1      tools_3.5.0      odbc_1.1.5      
##  [9] digest_0.6.15    bit_1.1-14       evaluate_0.10.1  tibble_1.4.2    
## [13] checkmate_1.8.6  pkgconfig_2.0.1  rlang_0.2.1      DBI_1.0.0       
## [17] cli_1.0.0        yaml_2.1.19      dplyr_0.7.5      stringr_1.3.1   
## [21] hms_0.4.2.9000   bit64_0.9-7      rprojroot_1.3-2  tidyselect_0.2.4
## [25] glue_1.2.0       R6_2.2.2         rmarkdown_1.10   tidyr_0.8.1     
## [29] readr_1.2.0      purrr_0.2.5      blob_1.1.1       backports_1.1.2 
## [33] scales_0.5.0     RODBC_1.3-15     htmltools_0.3.6  rsconnect_0.8.8 
## [37] assertthat_0.2.0 testit_0.8       colorspace_1.3-2 config_0.3      
## [41] utf8_1.1.4       stringi_1.2.3    munsell_0.5.0    markdown_0.8    
## [45] crayon_1.3.4
```

```r
Sys.time()
```

```
## [1] "2018-06-20 11:58:49 CDT"
```

