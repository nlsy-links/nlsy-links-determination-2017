---
title: Comparison of Versions of NLSY97 Kinship Links
date: "Date: 2018-06-20"
output:
  html_document:
    keep_md: yes
    toc: 4
    toc_float: true
    number_sections: true
---

This report covers the analyses used in development of the NLSY Behavior Genetics kinship links ([Joseph Rodgers](https://www.vanderbilt.edu/psychological_sciences/bio/joe-rodgers), PI).

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
1. The current report compares the versions of the NLSY97 Links
    * Version 10: allow nonsibs to still be r>0.
    * Version 11: refresh.

# Comparison of Agreement
![](figure-png/graph-roc-1.png)<!-- -->

# Table of *R* Assignments
<table class="table table-striped table-hover table-condensed table-responsive" style="width: auto !important; ">
<caption>Counts for 97 Housemates</caption>
 <thead>
  <tr>
   <th style="text-align:left;"> R </th>
   <th style="text-align:right;"> Implicit </th>
   <th style="text-align:right;"> Explicit </th>
   <th style="text-align:right;"> Roster </th>
   <th style="text-align:right;"> Eventual </th>
  </tr>
 </thead>
<tbody>
  <tr>
   <td style="text-align:left;"> -- </td>
   <td style="text-align:right;"> 2,519 </td>
   <td style="text-align:right;"> 2,519 </td>
   <td style="text-align:right;"> 108 </td>
   <td style="text-align:right;"> 107 </td>
  </tr>
  <tr>
   <td style="text-align:left;"> 0.000 </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> 174 </td>
   <td style="text-align:right;"> 177 </td>
  </tr>
  <tr>
   <td style="text-align:left;"> 0.250 </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> 126 </td>
   <td style="text-align:right;"> 124 </td>
  </tr>
  <tr>
   <td style="text-align:left;"> 0.500 </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> 2,111 </td>
   <td style="text-align:right;"> 2,083 </td>
  </tr>
  <tr>
   <td style="text-align:left;"> 1.000 </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> 28 </td>
  </tr>
</tbody>
</table>

<table class="table table-striped table-hover table-condensed table-responsive" style="width: auto !important; ">
<caption>Counts for 97 Housemates (Previous version of links)</caption>
 <thead>
  <tr>
   <th style="text-align:left;"> R </th>
   <th style="text-align:right;"> Implicit </th>
   <th style="text-align:right;"> Explicit </th>
   <th style="text-align:right;"> Roster </th>
   <th style="text-align:right;"> Eventual </th>
  </tr>
 </thead>
<tbody>
  <tr>
   <td style="text-align:left;"> -- </td>
   <td style="text-align:right;"> 2,519 </td>
   <td style="text-align:right;"> 2,519 </td>
   <td style="text-align:right;"> 108 </td>
   <td style="text-align:right;"> 107 </td>
  </tr>
  <tr>
   <td style="text-align:left;"> 0.000 </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> 174 </td>
   <td style="text-align:right;"> 177 </td>
  </tr>
  <tr>
   <td style="text-align:left;"> 0.250 </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> 126 </td>
   <td style="text-align:right;"> 124 </td>
  </tr>
  <tr>
   <td style="text-align:left;"> 0.500 </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> 2,111 </td>
   <td style="text-align:right;"> 2,083 </td>
  </tr>
  <tr>
   <td style="text-align:left;"> 1.000 </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> 28 </td>
  </tr>
</tbody>
</table>

# Breakdown of Agreements 
<table class="table table-striped table-hover table-condensed table-responsive" style="width: auto !important; ">
<caption>Joint Frequencies for 97 Housemates</caption>
 <thead>
  <tr>
   <th style="text-align:right;"> count current </th>
   <th style="text-align:right;"> RImplicit </th>
   <th style="text-align:right;"> RExplicit </th>
   <th style="text-align:right;"> RRoster </th>
   <th style="text-align:right;"> Delta </th>
  </tr>
 </thead>
<tbody>
  <tr>
   <td style="text-align:right;background-color: #a6e8a1;"> 2,111 </td>
   <td style="text-align:right;background-color: #a6e8a1;"> -- </td>
   <td style="text-align:right;background-color: #a6e8a1;"> -- </td>
   <td style="text-align:right;background-color: #a6e8a1;"> 0.500 </td>
   <td style="text-align:right;background-color: #a6e8a1;"> 0 </td>
  </tr>
  <tr>
   <td style="text-align:right;background-color: #a6e8a1;"> 174 </td>
   <td style="text-align:right;background-color: #a6e8a1;"> -- </td>
   <td style="text-align:right;background-color: #a6e8a1;"> -- </td>
   <td style="text-align:right;background-color: #a6e8a1;"> 0.000 </td>
   <td style="text-align:right;background-color: #a6e8a1;"> 0 </td>
  </tr>
  <tr>
   <td style="text-align:right;background-color: #a6e8a1;"> 126 </td>
   <td style="text-align:right;background-color: #a6e8a1;"> -- </td>
   <td style="text-align:right;background-color: #a6e8a1;"> -- </td>
   <td style="text-align:right;background-color: #a6e8a1;"> 0.250 </td>
   <td style="text-align:right;background-color: #a6e8a1;"> 0 </td>
  </tr>
  <tr>
   <td style="text-align:right;background-color: #a6e8a1;"> 108 </td>
   <td style="text-align:right;background-color: #a6e8a1;"> -- </td>
   <td style="text-align:right;background-color: #a6e8a1;"> -- </td>
   <td style="text-align:right;background-color: #a6e8a1;"> -- </td>
   <td style="text-align:right;background-color: #a6e8a1;"> 0 </td>
  </tr>
</tbody>
</table>

# By Roster 

| count| roster<br/>response<br/>older| roster<br/>response<br/>younger| RRoster<br/>mean| RPass1<br/>mean| R<br/>mean| RFull<br/>mean|
|-----:|-----------------------------:|-------------------------------:|----------------:|---------------:|----------:|--------------:|
| 1,017|                   sister_full|                    brother_full|            0.500|           0.500|      0.500|          0.500|
|   577|                  brother_full|                    brother_full|            0.500|           0.510|      0.510|          0.510|
|   517|                   sister_full|                     sister_full|            0.500|           0.516|      0.516|          0.516|
|    66|       sister_half_same_mother|        brother_half_same_mother|            0.250|           0.250|      0.250|          0.250|
|    40|                   sister_step|                    brother_step|            0.000|           0.000|         --|          0.000|
|    38|          cousin_female_unsure|              cousin_male_unsure|               --|              --|         --|             --|
|    31|      brother_half_same_mother|        brother_half_same_mother|            0.250|           0.242|         --|          0.242|
|    29|                  brother_step|                    brother_step|            0.000|           0.000|         --|          0.000|
|    29|             nonrelative_other|               nonrelative_other|            0.000|           0.000|         --|          0.000|
|    28|            cousin_male_unsure|              cousin_male_unsure|               --|              --|         --|             --|
|    24|          cousin_female_unsure|            cousin_female_unsure|               --|              --|         --|             --|
|    24|       sister_half_same_mother|         sister_half_same_mother|            0.250|           0.250|      0.250|          0.250|
|    19|                   sister_step|                     sister_step|            0.000|           0.000|         --|          0.000|
|    14|                 sister_foster|                   sister_foster|            0.000|           0.000|         --|          0.000|
|    11|                brother_foster|                  brother_foster|            0.000|           0.000|         --|          0.000|
|    10|               sister_adoptive|                brother_adoptive|            0.000|           0.000|         --|          0.000|
|     7|              brother_adoptive|                brother_adoptive|            0.000|           0.000|         --|          0.000|
|     7|                  uncle_unsure|                   nephew_unsure|               --|              --|         --|             --|
|     6|                 sister_foster|                  brother_foster|            0.000|           0.000|         --|          0.000|
|     5|                  uncle_unsure|                    niece_unsure|               --|              --|         --|             --|
|     4|      brother_half_same_father|        brother_half_same_father|            0.250|           0.188|         --|          0.188|
|     3|                   aunt_unsure|                    niece_unsure|               --|              --|         --|             --|
|     3|               sister_adoptive|                 sister_adoptive|            0.000|           0.000|         --|          0.000|
|     2|                       partner|                         partner|            0.000|           0.000|         --|          0.000|
|     2|                 sister_in_law|                   sister_in_law|            0.000|           0.000|         --|          0.000|
|     1|                   aunt_unsure|                   nephew_unsure|               --|              --|         --|             --|
|     1|           daughter_of_partner|               nonrelative_other|            0.000|           0.000|         --|          0.000|
|     1|                   do_not_know|                         refusal|               --|              --|         --|             --|
|     1|                       refusal|                         refusal|               --|              --|         --|             --|
|     1|                      roommate|                        roommate|            0.000|           0.000|         --|          0.000|
|     1|       sister_half_same_father|        brother_half_same_father|            0.250|           0.250|      0.250|          0.250|



| count| RRoster| RPass1|     R| RFull|    roster response older|  roster response younger| concern|
|-----:|-------:|------:|-----:|-----:|------------------------:|------------------------:|-------:|
| 1,017|   0.500|  0.500| 0.500| 0.500|              sister_full|             brother_full|       -|
|   566|   0.500|  0.500| 0.500| 0.500|             brother_full|             brother_full|       -|
|   500|   0.500|  0.500| 0.500| 0.500|              sister_full|              sister_full|       -|
|    66|   0.250|  0.250| 0.250| 0.250|  sister_half_same_mother| brother_half_same_mother|       -|
|    40|   0.000|  0.000|    --| 0.000|              sister_step|             brother_step|       -|
|    38|      --|     --|    --|    --|     cousin_female_unsure|       cousin_male_unsure|       -|
|    30|   0.250|  0.250| 0.250| 0.250| brother_half_same_mother| brother_half_same_mother|       -|
|    29|   0.000|  0.000|    --| 0.000|             brother_step|             brother_step|       -|
|    29|   0.000|  0.000|    --| 0.000|        nonrelative_other|        nonrelative_other|       -|
|    28|      --|     --|    --|    --|       cousin_male_unsure|       cousin_male_unsure|       -|
|    24|   0.250|  0.250| 0.250| 0.250|  sister_half_same_mother|  sister_half_same_mother|       -|
|    23|      --|     --|    --|    --|     cousin_female_unsure|     cousin_female_unsure|       -|
|    19|   0.000|  0.000|    --| 0.000|              sister_step|              sister_step|       -|
|    17|   0.500|  1.000| 1.000| 1.000|              sister_full|              sister_full|       -|
|    14|   0.000|  0.000|    --| 0.000|            sister_foster|            sister_foster|       -|
|    11|   0.000|  0.000|    --| 0.000|           brother_foster|           brother_foster|       -|
|    11|   0.500|  1.000| 1.000| 1.000|             brother_full|             brother_full|       -|
|    10|   0.000|  0.000|    --| 0.000|          sister_adoptive|         brother_adoptive|       -|
|     7|      --|     --|    --|    --|             uncle_unsure|            nephew_unsure|       -|
|     7|   0.000|  0.000|    --| 0.000|         brother_adoptive|         brother_adoptive|       -|
|     6|   0.000|  0.000|    --| 0.000|            sister_foster|           brother_foster|       -|
|     5|      --|     --|    --|    --|             uncle_unsure|             niece_unsure|       -|
|     3|      --|     --|    --|    --|              aunt_unsure|             niece_unsure|       -|
|     3|   0.000|  0.000|    --| 0.000|          sister_adoptive|          sister_adoptive|       -|
|     3|   0.250|  0.250| 0.250| 0.250| brother_half_same_father| brother_half_same_father|       -|
|     2|   0.000|  0.000|    --| 0.000|                  partner|                  partner|       -|
|     2|   0.000|  0.000|    --| 0.000|            sister_in_law|            sister_in_law|       -|
|     1|      --|     --|    --|    --|              aunt_unsure|            nephew_unsure|       -|
|     1|      --|     --|    --|    --|              do_not_know|                  refusal|       -|
|     1|      --|     --|    --|    --|                  refusal|                  refusal|       -|
|     1|      --|  0.000|    --| 0.000|     cousin_female_unsure|     cousin_female_unsure|       -|
|     1|   0.000|  0.000|    --| 0.000|      daughter_of_partner|        nonrelative_other|       -|
|     1|   0.000|  0.000|    --| 0.000|                 roommate|                 roommate|       -|
|     1|   0.250|  0.000|    --| 0.000| brother_half_same_father| brother_half_same_father|       -|
|     1|   0.250|  0.000|    --| 0.000| brother_half_same_mother| brother_half_same_mother|       -|
|     1|   0.250|  0.250| 0.250| 0.250|  sister_half_same_father| brother_half_same_father|       -|



# Session Information
For the sake of documentation and reproducibility, the current report was rendered in the following environment.  Click the line below to expand.

<details>
  <summary>Environment <span class="glyphicon glyphicon-plus-sign"></span></summary>

```
Session info --------------------------------------------------------------------------------------
```

```
 setting  value                                      
 version  R version 3.5.0 Patched (2018-05-14 r74725)
 system   x86_64, mingw32                            
 ui       RTerm                                      
 language (EN)                                       
 collate  English_United States.1252                 
 tz       America/Chicago                            
 date     2018-06-20                                 
```

```
Packages ------------------------------------------------------------------------------------------
```

```
 package     * version    date       source                          
 assertthat    0.2.0      2017-04-11 CRAN (R 3.5.0)                  
 backports     1.1.2      2017-12-13 CRAN (R 3.5.0)                  
 base        * 3.5.0      2018-05-15 local                           
 bindr         0.1.1      2018-03-13 CRAN (R 3.5.0)                  
 bindrcpp    * 0.2.2      2018-03-29 CRAN (R 3.5.0)                  
 bit           1.1-14     2018-05-29 CRAN (R 3.5.0)                  
 bit64         0.9-7      2017-05-08 CRAN (R 3.5.0)                  
 blob          1.1.1      2018-03-25 CRAN (R 3.5.0)                  
 checkmate     1.8.6      2018-06-20 Github (mllg/checkmate@bc16595) 
 colorspace    1.3-2      2016-12-14 CRAN (R 3.5.0)                  
 compiler      3.5.0      2018-05-15 local                           
 config        0.3        2018-03-27 CRAN (R 3.5.0)                  
 datasets    * 3.5.0      2018-05-15 local                           
 DBI           1.0.0      2018-05-02 CRAN (R 3.5.0)                  
 devtools      1.13.5     2018-02-18 CRAN (R 3.5.0)                  
 digest        0.6.15     2018-01-28 CRAN (R 3.5.0)                  
 dplyr         0.7.5      2018-05-19 CRAN (R 3.5.0)                  
 evaluate      0.10.1     2017-06-24 CRAN (R 3.5.0)                  
 ggplot2     * 2.2.1      2016-12-30 CRAN (R 3.5.0)                  
 glue          1.2.0      2017-10-29 CRAN (R 3.5.0)                  
 graphics    * 3.5.0      2018-05-15 local                           
 grDevices   * 3.5.0      2018-05-15 local                           
 grid          3.5.0      2018-05-15 local                           
 gtable        0.2.0      2016-02-26 CRAN (R 3.5.0)                  
 highr         0.7        2018-06-09 CRAN (R 3.5.0)                  
 hms           0.4.2.9000 2018-06-20 Github (tidyverse/hms@2e0a39a)  
 htmltools     0.3.6      2017-04-28 CRAN (R 3.5.0)                  
 httr          1.3.1      2017-08-20 CRAN (R 3.5.0)                  
 kableExtra    0.9.0      2018-05-21 CRAN (R 3.5.0)                  
 knitr       * 1.20       2018-02-20 CRAN (R 3.5.0)                  
 labeling      0.3        2014-08-23 CRAN (R 3.5.0)                  
 lazyeval      0.2.1      2017-10-29 CRAN (R 3.5.0)                  
 magrittr    * 1.5        2014-11-22 CRAN (R 3.5.0)                  
 memoise       1.1.0      2017-04-21 CRAN (R 3.5.0)                  
 methods     * 3.5.0      2018-05-15 local                           
 munsell       0.5.0      2018-06-12 CRAN (R 3.5.0)                  
 odbc          1.1.5      2018-06-20 Github (r-dbi/odbc@2255001)     
 OuhscMunge    0.1.9.9008 2018-06-20 local                           
 pillar        1.2.3      2018-05-25 CRAN (R 3.5.0)                  
 pkgconfig     2.0.1      2017-03-21 CRAN (R 3.5.0)                  
 plyr          1.8.4      2016-06-08 CRAN (R 3.5.0)                  
 purrr         0.2.5      2018-05-29 CRAN (R 3.5.0)                  
 R6            2.2.2      2017-06-17 CRAN (R 3.5.0)                  
 Rcpp          0.12.17    2018-05-18 CRAN (R 3.5.0)                  
 readr         1.2.0      2018-06-20 Github (tidyverse/readr@05890c3)
 rlang         0.2.1      2018-05-30 CRAN (R 3.5.0)                  
 rmarkdown     1.10       2018-06-11 CRAN (R 3.5.0)                  
 rprojroot     1.3-2      2018-01-03 CRAN (R 3.5.0)                  
 RSQLite       2.1.1      2018-05-06 CRAN (R 3.5.0)                  
 rstudioapi    0.7        2017-09-07 CRAN (R 3.5.0)                  
 rvest         0.3.2      2016-06-17 CRAN (R 3.5.0)                  
 scales        0.5.0      2017-08-24 CRAN (R 3.5.0)                  
 stats       * 3.5.0      2018-05-15 local                           
 stringi       1.2.3      2018-06-12 CRAN (R 3.5.0)                  
 stringr       1.3.1      2018-05-10 CRAN (R 3.5.0)                  
 testthat      2.0.0      2017-12-13 CRAN (R 3.5.0)                  
 tibble        1.4.2      2018-01-22 CRAN (R 3.5.0)                  
 tidyselect    0.2.4      2018-02-26 CRAN (R 3.5.0)                  
 tools         3.5.0      2018-05-15 local                           
 utils       * 3.5.0      2018-05-15 local                           
 viridisLite   0.3.0      2018-02-01 CRAN (R 3.5.0)                  
 withr         2.1.2      2018-03-15 CRAN (R 3.5.0)                  
 xml2          1.2.0      2018-01-24 CRAN (R 3.5.0)                  
 yaml          2.1.19     2018-05-01 CRAN (R 3.5.0)                  
```
</details>



Report rendered by Will at 2018-06-20, 18:02 -0500 in 2 seconds.
