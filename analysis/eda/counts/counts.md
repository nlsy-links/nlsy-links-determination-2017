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
|Archive     |tblArchiveDescription      |         0|            4|
|Archive     |tblRelatedValuesArchive    |         0|           24|
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
|Extract     |tblLinks2004Gen1           |         0|            9|
|Extract     |tblLinks2004Gen2           |         0|            5|
|Metadata    |tblItem                    |       108|            7|
|Metadata    |tblMzManual                |       206|            9|
|Metadata    |tblRosterGen1Assignment    |        50|           16|
|Metadata    |tblVariable                |     1,559|           11|
|Process     |tblBabyDaddy               |         0|           11|
|Process     |tblFatherOfGen2            |         0|            7|
|Process     |tblIRDemo1                 |         0|            5|
|Process     |tblMarkerGen1              |         0|           10|
|Process     |tblMarkerGen2              |         0|            8|
|Process     |tblOutcome                 |         0|            5|
|Process     |tblOutcomesOLD             |         0|            4|
|Process     |tblParentsOfGen1Current    |         0|           19|
|Process     |tblParentsOfGen1Retro      |         0|            7|
|Process     |tblRelatedStructure        |         0|            6|
|Process     |tblRelatedValues           |         0|           24|
|Process     |tblResponse                |         0|            9|
|Process     |tblRosterGen1              |         0|           13|
|Process     |tblSubject                 |         0|            5|
|Process     |tblSubjectDetails          |         0|           15|
|Process     |tblSurveyTime              |         0|            7|



# Session Information
For the sake of documentation and reproducibility, the current report was rendered in the following environment.  Click the line below to expand.

<details>
  <summary>Environment <span class="glyphicon glyphicon-plus-sign"></span></summary>

```
Session info --------------------------------------------------------------------------------------
```

```
 setting  value                                      
 version  R version 3.4.0 Patched (2017-05-16 r72684)
 system   x86_64, mingw32                            
 ui       RTerm                                      
 language (EN)                                       
 collate  English_United States.1252                 
 tz       America/Chicago                            
 date     2017-06-22                                 
```

```
Packages ------------------------------------------------------------------------------------------
```

```
 package    * version date       source        
 assertthat   0.2.0   2017-04-11 CRAN (R 3.3.3)
 backports    1.1.0   2017-05-22 CRAN (R 3.4.0)
 base       * 3.4.0   2017-05-18 local         
 bindr        0.1     2016-11-13 CRAN (R 3.3.2)
 bindrcpp   * 0.1     2016-12-11 CRAN (R 3.3.2)
 colorspace   1.3-2   2016-12-14 CRAN (R 3.3.2)
 compiler     3.4.0   2017-05-18 local         
 datasets   * 3.4.0   2017-05-18 local         
 devtools     1.13.2  2017-06-02 CRAN (R 3.4.0)
 digest       0.6.12  2017-01-27 CRAN (R 3.3.2)
 dplyr        0.7.0   2017-06-09 CRAN (R 3.4.0)
 evaluate     0.10    2016-10-11 CRAN (R 3.3.1)
 glue         1.1.0   2017-06-13 CRAN (R 3.4.0)
 graphics   * 3.4.0   2017-05-18 local         
 grDevices  * 3.4.0   2017-05-18 local         
 highr        0.6     2016-05-09 CRAN (R 3.3.0)
 htmltools    0.3.6   2017-04-28 CRAN (R 3.3.3)
 knitr      * 1.16    2017-05-18 CRAN (R 3.4.0)
 magrittr   * 1.5     2014-11-22 CRAN (R 3.2.0)
 memoise      1.1.0   2017-04-21 CRAN (R 3.3.3)
 methods    * 3.4.0   2017-05-18 local         
 munsell      0.4.3   2016-02-13 CRAN (R 3.2.3)
 plyr         1.8.4   2016-06-08 CRAN (R 3.3.0)
 R6           2.2.1   2017-05-10 CRAN (R 3.4.0)
 Rcpp         0.12.11 2017-05-22 CRAN (R 3.4.0)
 rlang        0.1.1   2017-05-18 CRAN (R 3.4.0)
 rmarkdown    1.6     2017-06-15 CRAN (R 3.4.0)
 RODBC        1.3-15  2017-04-13 CRAN (R 3.3.3)
 rprojroot    1.2     2017-01-16 CRAN (R 3.3.2)
 scales       0.4.1   2016-11-09 CRAN (R 3.3.2)
 stats      * 3.4.0   2017-05-18 local         
 stringi      1.1.5   2017-04-07 CRAN (R 3.3.3)
 stringr      1.2.0   2017-02-18 CRAN (R 3.3.2)
 testit       0.7     2017-05-22 CRAN (R 3.4.0)
 tibble       1.3.3   2017-05-28 CRAN (R 3.4.0)
 tools        3.4.0   2017-05-18 local         
 utils      * 3.4.0   2017-05-18 local         
 withr        1.0.2   2016-06-20 CRAN (R 3.3.1)
 yaml         2.1.14  2016-11-12 CRAN (R 3.3.2)
```
</details>



Report rendered by Will at 2017-06-22, 00:30 -0500 in 1 seconds.

