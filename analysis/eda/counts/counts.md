# Row Counts of Tables
Date: `r Sys.Date()`  

This report documents the basic properties of the (input & output) tables kinship determination (Joe Rodgers, PI).  Report created by Will Beasley.

<!--  Set the working directory to the repository's base directory; this assumes the report is nested inside of two directories.-->


<!-- Set the report-wide options, and point to the external code file. -->


<!-- Load 'sourced' R files.  Suppress the output when loading sources. --> 


<!-- Load packages, or at least verify they're available on the local machine.  Suppress the output when loading packages. --> 


<!-- Load any global functions and variables declared in the R file.  Suppress the output. --> 


<!-- Declare any global functions specific to a Rmd output.  Suppress the output. --> 


<!-- Load the datasets.   -->


<!-- Tweak the datasets.   -->


# Summary {.tabset .tabset-fade .tabset-pills}

## Notes 
1. The current report covers 51 tables.

## Unanswered Questions

## Answered Questions

# Results

## Counts

|schema name |table name                 | row count| column count|
|:-----------|:--------------------------|---------:|------------:|
|Archive     |tblArchiveDescription      |        55|            4|
|Archive     |tblRelatedValuesArchive    |   676,582|           24|
|dbo         |sysdiagrams                |         4|            5|
|dbo         |tblIRDemo2                 |         0|            3|
|Enum        |tblLUBioparent-not-used    |         0|            2|
|Enum        |tblLUExtractSource         |        11|            4|
|Enum        |tblLUGender                |         3|            4|
|Enum        |tblLUMarkerEvidence        |         8|            4|
|Enum        |tblLUMarkerType            |        28|            5|
|Enum        |tblLUMultipleBirth         |         5|            4|
|Enum        |tblLURaceCohort            |         3|            4|
|Enum        |tblLURelationshipPath      |         5|            4|
|Enum        |tblLURosterGen1            |        67|            4|
|Enum        |tblLUSurveySource          |         4|            4|
|Enum        |tblLUTristate              |         3|            4|
|Enum        |tblLUYesNo                 |         6|            4|
|Extract     |tblGen1Explicit            |    12,686|           95|
|Extract     |tblGen1GeocodeSanitized    |     5,302|           29|
|Extract     |tblGen1Implicit            |    12,686|          101|
|Extract     |tblGen1Links               |    12,686|           95|
|Extract     |tblGen1MzDzDistinction2010 |         0|            7|
|Extract     |tblGen1Outcomes            |    12,686|           21|
|Extract     |tblGen2FatherFromGen1      |    12,686|          952|
|Extract     |tblGen2ImplicitFather      |    11,504|          111|
|Extract     |tblGen2Links               |    11,512|          164|
|Extract     |tblGen2LinksFromGen1       |    12,686|          106|
|Extract     |tblGen2OutcomesHeight      |    11,504|           46|
|Extract     |tblGen2OutcomesMath        |    11,504|           44|
|Extract     |tblGen2OutcomesWeight      |    11,504|           13|
|Extract     |tblLinks2004Gen1           |     3,890|            9|
|Extract     |tblLinks2004Gen2           |    12,855|            5|
|Metadata    |tblItem                    |       108|            7|
|Metadata    |tblMzManual                |       207|            9|
|Metadata    |tblRosterGen1Assignment    |        50|           16|
|Metadata    |tblVariable                |     1,559|           11|
|Process     |tblBabyDaddy               |   178,803|           11|
|Process     |tblFatherOfGen2            |    52,038|            7|
|Process     |tblIRDemo1                 |         0|            5|
|Process     |tblMarkerGen1              |   217,310|           10|
|Process     |tblMarkerGen2              |   206,263|            8|
|Process     |tblOutcome                 |   108,569|            5|
|Process     |tblOutcomesOLD             |         0|            4|
|Process     |tblParentsOfGen1Current    |    12,686|           19|
|Process     |tblParentsOfGen1Retro      |   482,068|            7|
|Process     |tblRelatedStructure        |    85,590|            6|
|Process     |tblRelatedValues           |    42,795|           24|
|Process     |tblResponse                | 2,543,774|            9|
|Process     |tblRosterGen1              |    10,604|           13|
|Process     |tblSubject                 |    24,198|            5|
|Process     |tblSubjectDetails          |    24,198|           15|
|Process     |tblSurveyTime              |   580,752|            7|



# Session Information
For the sake of documentation and reproducibility, the current report was rendered in the following environment.  Click the line below to expand.

<details>
  <summary>Environment <span class="glyphicon glyphicon-plus-sign"></span></summary>

```
Session info --------------------------------------------------------------------------------------
```

```
 setting  value                                      
 version  R version 3.4.1 Patched (2017-08-29 r73159)
 system   x86_64, mingw32                            
 ui       RTerm                                      
 language (EN)                                       
 collate  English_United States.1252                 
 tz       America/Chicago                            
 date     2017-09-06                                 
```

```
Packages ------------------------------------------------------------------------------------------
```

```
 package    * version    date       source                          
 assertthat   0.2.0      2017-04-11 CRAN (R 3.4.1)                  
 backports    1.1.0      2017-05-22 CRAN (R 3.4.0)                  
 base       * 3.4.1      2017-08-31 local                           
 bindr        0.1        2016-11-13 CRAN (R 3.4.1)                  
 bindrcpp   * 0.2        2017-06-17 CRAN (R 3.4.1)                  
 colorspace   1.3-2      2016-12-14 CRAN (R 3.4.1)                  
 compiler     3.4.1      2017-08-31 local                           
 datasets   * 3.4.1      2017-08-31 local                           
 devtools     1.13.3     2017-08-02 CRAN (R 3.4.1)                  
 digest       0.6.12     2017-01-27 CRAN (R 3.4.1)                  
 dplyr        0.7.2      2017-07-20 CRAN (R 3.4.1)                  
 evaluate     0.10.1     2017-06-24 CRAN (R 3.4.1)                  
 glue         1.1.1      2017-06-21 CRAN (R 3.4.1)                  
 graphics   * 3.4.1      2017-08-31 local                           
 grDevices  * 3.4.1      2017-08-31 local                           
 highr        0.6        2016-05-09 CRAN (R 3.4.1)                  
 htmltools    0.3.6      2017-04-28 CRAN (R 3.4.1)                  
 knitr      * 1.17       2017-08-10 CRAN (R 3.4.1)                  
 magrittr   * 1.5        2014-11-22 CRAN (R 3.4.1)                  
 memoise      1.1.0      2017-04-21 CRAN (R 3.4.1)                  
 methods    * 3.4.1      2017-08-31 local                           
 munsell      0.4.3      2016-02-13 CRAN (R 3.4.1)                  
 pkgconfig    2.0.1      2017-03-21 CRAN (R 3.4.1)                  
 plyr         1.8.4      2016-06-08 CRAN (R 3.4.1)                  
 R6           2.2.2      2017-06-17 CRAN (R 3.4.1)                  
 Rcpp         0.12.12    2017-07-15 CRAN (R 3.4.1)                  
 rlang        0.1.2.9000 2017-08-21 Github (tidyverse/rlang@f20124b)
 rmarkdown    1.6        2017-06-15 CRAN (R 3.4.1)                  
 RODBC        1.3-15     2017-04-13 CRAN (R 3.4.0)                  
 rprojroot    1.2        2017-01-16 CRAN (R 3.4.1)                  
 scales       0.5.0      2017-08-24 CRAN (R 3.4.1)                  
 stats      * 3.4.1      2017-08-31 local                           
 stringi      1.1.5      2017-04-07 CRAN (R 3.4.0)                  
 stringr      1.2.0      2017-02-18 CRAN (R 3.4.1)                  
 testit       0.7        2017-07-12 Github (yihui/testit@701fa1f)   
 tibble       1.3.4      2017-08-22 CRAN (R 3.4.1)                  
 tools        3.4.1      2017-08-31 local                           
 utils      * 3.4.1      2017-08-31 local                           
 withr        2.0.0      2017-07-28 CRAN (R 3.4.1)                  
 yaml         2.1.14     2016-11-12 CRAN (R 3.4.1)                  
```
</details>



Report rendered by Will at 2017-09-06, 21:55 -0500 in 1 seconds.

