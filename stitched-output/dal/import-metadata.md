



This report was automatically generated with the R package **knitr**
(version 1.16).


```r
# knitr::stitch_rmd(script="./dal/import-metadata.R", output="./stitched-output/dal/import-metadata.md") # dir.create(output="./stitched-output/dal/", recursive=T)
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
directory_in              <- "data-public/metadata/tables"

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
  Item = readr::cols_only(
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
    Notes                               = readr::col_character(),
    Active                              = readr::col_logical(),
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
ds_mapping <- readr::read_csv(file.path(directory_in, "_mapping.csv"), col_types=col_types_mapping)
ds_mapping
```

```
## # A tibble: 15 x 5
##              table_name schema_name        enum_name c_sharp_type
##                   <chr>       <chr>            <chr>        <chr>
##  1                 Item    Metadata             Item        short
##  2      LUExtractSource        Enum    ExtractSource         byte
##  3             LUGender        Enum           Gender         byte
##  4     LUMarkerEvidence        Enum   MarkerEvidence         byte
##  5         LUMarkerType        Enum       MarkerType         byte
##  6      LUMultipleBirth        Enum    MultipleBirth         byte
##  7         LURaceCohort        Enum       RaceCohort         byte
##  8   LURelationshipPath        Enum RelationshipPath         byte
##  9         LURosterGen1        Enum       RosterGen1        short
## 10       LUSurveySource        Enum     SurveySource         byte
## 11           LUTristate        Enum         Tristate         byte
## 12              LUYesNo        Enum            YesNo        short
## 13             MzManual    Metadata     NA_character NA_character
## 14 RosterGen1Assignment    Metadata     NA_character NA_character
## 15             Variable    Metadata     NA_character NA_character
## # ... with 1 more variables: convert_to_enum <lgl>
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
## # A tibble: 14 x 4
##                    name
##                   <chr>
##  1                 Item
##  2      LUExtractSource
##  3     LUMarkerEvidence
##  4         LUMarkerType
##  5      LUMultipleBirth
##  6         LURaceCohort
##  7   LURelationshipPath
##  8         LURosterGen1
##  9       LUSurveySource
## 10           LUTristate
## 11              LUYesNo
## 12             MzManual
## 13 RosterGen1Assignment
## 14             Variable
## # ... with 3 more variables: path <chr>, col_types <list>, exists <lgl>
```

```r
testit::assert("All metadata files must exist.", all(ds_file$exists))

ds_entries <- ds_file %>%
  dplyr::select(name, path, col_types) %>%
  dplyr::mutate(
    entries = purrr::pmap(list(file=.$path, col_types=.$col_types), readr::read_csv)
  )
ds_entries
```

```
## # A tibble: 14 x 4
##                    name
##                   <chr>
##  1                 Item
##  2      LUExtractSource
##  3     LUMarkerEvidence
##  4         LUMarkerType
##  5      LUMultipleBirth
##  6         LURaceCohort
##  7   LURelationshipPath
##  8         LURosterGen1
##  9       LUSurveySource
## 10           LUTristate
## 11              LUYesNo
## 12             MzManual
## 13 RosterGen1Assignment
## 14             Variable
## # ... with 3 more variables: path <chr>, col_types <list>, entries <list>
```

```r
# d <- readr::read_csv("data-public/metadata/tables/LURosterGen1.csv", col_types=lst_col_types$LURosterGen1)

rm(directory_in) # rm(col_types_tulsa)
```

```r
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call ato `dplyr::rename()`.

ds_file <- ds_file %>%
  dplyr::left_join( ds_mapping, by=c("name"="table_name")) %>%
  dplyr::mutate(
    table_name    = paste0(schema_name, ".tbl", name),
    sql_delete    = paste0("DELETE FROM ", table_name)
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
## # A tibble: 108 x 7
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
## # ... with 98 more rows, and 3 more variables: MaxValue <int>,
## #   Active <lgl>, Notes <chr>
## # A tibble: 11 x 4
##       ID              Label Active Notes
##    <int>              <chr>  <lgl> <chr>
##  1     3          Gen1Links   TRUE  <NA>
##  2     4          Gen2Links   TRUE  <NA>
##  3     5  Gen2LinksFromGen1   TRUE  <NA>
##  4     6 Gen2ImplicitFather   TRUE  <NA>
##  5     7 Gen2FatherFromGen1   TRUE  <NA>
##  6     8       Gen1Outcomes   TRUE  <NA>
##  7     9 Gen2OutcomesHeight   TRUE  <NA>
##  8    10       Gen1Explicit   TRUE  <NA>
##  9    11       Gen1Implicit   TRUE  <NA>
## 10    12 Gen2OutcomesWeight   TRUE  <NA>
## 11    13   Gen2OutcomesMath   TRUE  <NA>
## # A tibble: 8 x 4
##      ID            Label Active Notes
##   <int>            <chr>  <lgl> <chr>
## 1     0       Irrelevant   TRUE  <NA>
## 2     1 StronglySupports   TRUE  <NA>
## 3     2         Supports   TRUE  <NA>
## 4     3       Consistent   TRUE  <NA>
## 5     4        Ambiguous   TRUE  <NA>
## 6     5          Missing   TRUE  <NA>
## 7     6         Unlikely   TRUE  <NA>
## 8     7      Disconfirms   TRUE  <NA>
## # A tibble: 28 x 5
##       ID               Label Explicit Active Notes
##    <int>               <chr>    <int>  <lgl> <chr>
##  1     1          RosterGen1        1   TRUE  <NA>
##  2     2         ShareBiomom        1   TRUE  <NA>
##  3     3         ShareBiodad        1   TRUE  <NA>
##  4     5       DobSeparation        0   TRUE  <NA>
##  5     6     GenderAgreement        0   TRUE  <NA>
##  6    10        FatherAsthma        0   TRUE  <NA>
##  7    11     BabyDaddyAsthma        0   TRUE  <NA>
##  8    12 BabyDaddyLeftHHDate        0   TRUE  <NA>
##  9    13  BabyDaddyDeathDate        0   TRUE  <NA>
## 10    14      BabyDaddyAlive        0   TRUE  <NA>
## # ... with 18 more rows
## # A tibble: 5 x 4
##      ID      Label Active
##   <int>      <chr>  <lgl>
## 1     0         No   TRUE
## 2     2       Twin   TRUE
## 3     3       Trip   TRUE
## 4     4 TwinOrTrip   TRUE
## 5   255  DoNotKnow   TRUE
## # ... with 1 more variables: Notes <chr>
## # A tibble: 3 x 4
##      ID    Label Active Notes
##   <int>    <chr>  <lgl> <chr>
## 1     1 Hispanic   TRUE  <NA>
## 2     2    Black   TRUE  <NA>
## 3   255     Nbnh   TRUE  <NA>
## # A tibble: 5 x 4
##      ID          Label Active                                Notes
##   <int>          <chr>  <lgl>                                <chr>
## 1     1 Gen1Housemates   TRUE                                 <NA>
## 2     2   Gen2Siblings   TRUE                                 <NA>
## 3     3    Gen2Cousins   TRUE                                 <NA>
## 4     4    ParentChild   TRUE                                 <NA>
## 5     5      AuntNiece   TRUE Acutally (Uncle|Aunt)-(Nephew|Niece)
## # A tibble: 67 x 4
##       ID       Label Active Notes
##    <int>       <chr>  <lgl> <chr>
##  1    -4   ValidSkip   TRUE  <NA>
##  2    -3 InvalidSkip   TRUE  <NA>
##  3    -1     Refusal   TRUE  <NA>
##  4     0  Respondent   TRUE  <NA>
##  5     1      Spouse   TRUE  <NA>
##  6     2         Son   TRUE  <NA>
##  7     3    Daughter   TRUE  <NA>
##  8     4      Father   TRUE  <NA>
##  9     5      Mother   TRUE  <NA>
## 10     6     Brother   TRUE  <NA>
## # ... with 57 more rows
## # A tibble: 4 x 4
##      ID       Label Active Notes
##   <int>       <chr>  <lgl> <chr>
## 1     0 NoInterview   TRUE  <NA>
## 2     1        Gen1   TRUE  <NA>
## 3     2       Gen2C   TRUE  <NA>
## 4     3      Gen2YA   TRUE  <NA>
## # A tibble: 3 x 4
##      ID     Label Active Notes
##   <int>     <chr>  <lgl> <chr>
## 1     0        No   TRUE  <NA>
## 2     1       Yes   TRUE  <NA>
## 3   255 DoNotKnow   TRUE  <NA>
## # A tibble: 6 x 4
##      ID                               Label Active Notes
##   <int>                               <chr>  <lgl> <chr>
## 1    -6 ValidSkipOrNoInterviewOrNotInSurvey   TRUE  <NA>
## 2    -3                         InvalidSkip   TRUE  <NA>
## 3    -2                           DoNotKnow   TRUE  <NA>
## 4    -1                             Refusal   TRUE  <NA>
## 5     0                                  No   TRUE  <NA>
## 6     1                                 Yes   TRUE  <NA>
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
## # A tibble: 50 x 16
##       ID ResponseLower ResponseUpper  Freq Resolved     R RBoundLower
##    <int>         <int>         <int> <int>    <int> <dbl>       <dbl>
##  1     1            -3            -3    67        0    NA       0.000
##  2     2            -3            -1    11        0    NA       0.000
##  3     3            -3            33     6        1 0.000       0.000
##  4     4            -3            36    35        1 0.000       0.000
##  5     5            -1            18     1        1 0.125       0.125
##  6     6            -1            36     3        1 0.000       0.000
##  7     7             1             1   167        1 0.000       0.000
##  8     8             1            33     1        1 0.000       0.000
##  9     9             1            36     1        1 0.000       0.000
## 10    10             1            57     1        1 0.000       0.000
## # ... with 40 more rows, and 9 more variables: RBoundUpper <dbl>,
## #   SameGeneration <int>, ShareBiodad <int>, ShareBiomom <int>,
## #   ShareBiograndparent <int>, Inconsistent <int>, Notes <chr>,
## #   ResponseLowerLabel <chr>, ResponseUpperLabel <chr>
## # A tibble: 1,559 x 11
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
## # ... with 1,549 more rows, and 5 more variables: SurveyYear <int>,
## #   LoopIndex <int>, Translate <int>, Active <lgl>, Notes <chr>
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
##  [1] "Metadata.tblItem"                 "Enum.tblLUExtractSource"         
##  [3] "Enum.tblLUMarkerEvidence"         "Enum.tblLUMarkerType"            
##  [5] "Enum.tblLUMultipleBirth"          "Enum.tblLURaceCohort"            
##  [7] "Enum.tblLURelationshipPath"       "Enum.tblLURosterGen1"            
##  [9] "Enum.tblLUSurveySource"           "Enum.tblLUTristate"              
## [11] "Enum.tblLUYesNo"                  "Metadata.tblMzManual"            
## [13] "Metadata.tblRosterGen1Assignment" "Metadata.tblVariable"
```

```r
ds_file
```

```
## # A tibble: 14 x 11
##                    name
##                   <chr>
##  1                 Item
##  2      LUExtractSource
##  3     LUMarkerEvidence
##  4         LUMarkerType
##  5      LUMultipleBirth
##  6         LURaceCohort
##  7   LURelationshipPath
##  8         LURosterGen1
##  9       LUSurveySource
## 10           LUTristate
## 11              LUYesNo
## 12             MzManual
## 13 RosterGen1Assignment
## 14             Variable
## # ... with 10 more variables: path <chr>, col_types <list>, exists <lgl>,
## #   schema_name <chr>, enum_name <chr>, c_sharp_type <chr>,
## #   convert_to_enum <lgl>, table_name <chr>, sql_delete <chr>,
## #   entries <list>
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
##     // BabyDaddyInHH                                             =    81, 
##     // BabyDaddyAlive                                            =    82, 
##     // BabyDaddyEverLiveInHH                                     =    83, 
##     // BabyDaddyLeftHHMonth                                      =    84, 
##     // BabyDaddyLeftHHYearFourDigit                              =    85, 
##     // BabyDaddyDeathMonth                                       =    86, 
##     // BabyDaddyDeathYearTwoDigit                                =    87, 
##     // BabyDaddyDeathYearFourDigit                               =    88, 
##     // BabyDaddyDistanceFromMotherFuzzyCeiling                   =    89, 
##     // BabyDaddyHasAsthma                                        =    90, 
##     // BabyDaddyLeftHHMonthOrNeverInHH                           =    91, 
##     // BabyDaddyLeftHHYearTwoDigit                               =    92, 
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
##     Gen1BiodadBrithCountry                                       =    34, 
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
##     Nbnh                                                         =   255, 
## }
##  
## public enum RelationshipPath {
##     Gen1Housemates                                               =     1, 
##     Gen2Siblings                                                 =     2, 
##     Gen2Cousins                                                  =     3, 
##     ParentChild                                                  =     4, 
##     AuntNiece                                                    =     5, // Acutally (Uncle|Aunt)-(Nephew|Niece)
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
##     AdoptedOrStepsister                                          =    60, 
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
# delete_result <- RODBC::sqlQuery(channel, "DELETE FROM [NlsLinks].[Metadata].[tblVariable]", errors=FALSE)
delete_results <- ds_file$sql_delete %>%
  rev() %>%
  purrr::set_names(ds_file$table_name) %>%
  purrr::map_int(RODBC::sqlQuery, channel=channel, errors=FALSE)

delete_results
```

```
##                 Metadata.tblItem          Enum.tblLUExtractSource 
##                               -2                               -2 
##         Enum.tblLUMarkerEvidence             Enum.tblLUMarkerType 
##                               -2                               -2 
##          Enum.tblLUMultipleBirth             Enum.tblLURaceCohort 
##                               -2                               -2 
##       Enum.tblLURelationshipPath             Enum.tblLURosterGen1 
##                               -2                               -2 
##           Enum.tblLUSurveySource               Enum.tblLUTristate 
##                               -2                               -2 
##                  Enum.tblLUYesNo             Metadata.tblMzManual 
##                               -2                               -2 
## Metadata.tblRosterGen1Assignment             Metadata.tblVariable 
##                               -2                               -2
```

```r
# d <- ds_file %>%
#   dplyr::select(table_name, entries) %>%
#   dplyr::filter(table_name=="Enum.tblLURosterGen1") %>%
#   tibble::deframe() %>%
#   .[[1]]

# d2 <- d[, 1:16]
# RODBC::sqlSave(channel, dat=d, tablename="Enum.tblLURosterGen1", safer=TRUE, rownames=FALSE, append=TRUE)

purrr::map2_int(
  ds_file$entries,
  ds_file$table_name,
  function( d, table_name ) {
    RODBC::sqlSave(
      channel     = channel,
      dat         = d,
      tablename   = table_name,
      safer       = TRUE,       # Don't keep the existing table.
      rownames    = FALSE,
      append      = TRUE
    )
  }
) %>%
purrr::set_names(ds_file$table_name)
```

```
##                 Metadata.tblItem          Enum.tblLUExtractSource 
##                                1                                1 
##         Enum.tblLUMarkerEvidence             Enum.tblLUMarkerType 
##                                1                                1 
##          Enum.tblLUMultipleBirth             Enum.tblLURaceCohort 
##                                1                                1 
##       Enum.tblLURelationshipPath             Enum.tblLURosterGen1 
##                                1                                1 
##           Enum.tblLUSurveySource               Enum.tblLUTristate 
##                                1                                1 
##                  Enum.tblLUYesNo             Metadata.tblMzManual 
##                                1                                1 
## Metadata.tblRosterGen1Assignment             Metadata.tblVariable 
##                                1                                1
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
##  [5] R6_2.2.1         evaluate_0.10    rlang_0.1.1      stringi_1.1.5   
##  [9] testit_0.7       RODBC_1.3-15     tools_3.4.0      stringr_1.2.0   
## [13] readr_1.1.1      glue_1.1.0       markdown_0.8     purrr_0.2.2.2   
## [17] hms_0.3          compiler_3.4.0   pkgconfig_2.0.1  bindr_0.1       
## [21] knitr_1.16       tibble_1.3.3
```

```r
Sys.time()
```

```
## [1] "2017-06-21 22:52:40 CDT"
```

