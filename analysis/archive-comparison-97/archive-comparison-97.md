---
title: Comparison of Versions of NLSY97 Kinship Links
date: "Date: 2018-06-19"
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
    * Version 2: account for twins.
    * Version 3: same sib full twins are R=.5 by default, and overridden if explicitly MZ.

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
   <td style="text-align:right;"> 0 </td>
  </tr>
  <tr>
   <td style="text-align:left;"> 0.000 </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> 174 </td>
   <td style="text-align:right;"> 1,318 </td>
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
   <td style="text-align:right;"> 1,049 </td>
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
   <td style="text-align:right;"> 1,202 </td>
   <td style="text-align:right;"> 0 </td>
  </tr>
  <tr>
   <td style="text-align:left;"> 0.000 </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> -- </td>
   <td style="text-align:right;"> 174 </td>
   <td style="text-align:right;"> 1,318 </td>
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
   <td style="text-align:right;"> 1,017 </td>
   <td style="text-align:right;"> 1,049 </td>
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
   <td style="text-align:right;background-color: #a6e8a1;"> 1094 </td>
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
   <td style="text-align:right;background-color: #a6e8a1;"> -1094 </td>
  </tr>
</tbody>
</table>



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
 ui       RStudio (1.1.446)                          
 language (EN)                                       
 collate  English_United States.1252                 
 tz       America/Chicago                            
 date     2018-06-19                                 
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
 checkmate     1.8.6      2018-05-23 Github (mllg/checkmate@f161dc3) 
 chron         2.3-52     2018-01-06 CRAN (R 3.5.0)                  
 cli           1.0.0      2017-11-05 CRAN (R 3.5.0)                  
 colorspace    1.3-2      2016-12-14 CRAN (R 3.5.0)                  
 compiler      3.5.0      2018-05-15 local                           
 config        0.3        2018-03-27 CRAN (R 3.5.0)                  
 crayon        1.3.4      2017-09-16 CRAN (R 3.5.0)                  
 datasets    * 3.5.0      2018-05-15 local                           
 DBI         * 1.0.0      2018-05-02 CRAN (R 3.5.0)                  
 devtools      1.13.5     2018-02-18 CRAN (R 3.5.0)                  
 digest        0.6.15     2018-01-28 CRAN (R 3.5.0)                  
 dplyr         0.7.5      2018-05-19 CRAN (R 3.5.0)                  
 evaluate      0.10.1     2017-06-24 CRAN (R 3.5.0)                  
 ggplot2     * 2.2.1      2016-12-30 CRAN (R 3.5.0)                  
 glue          1.2.0      2017-10-29 CRAN (R 3.5.0)                  
 graphics    * 3.5.0      2018-05-15 local                           
 grDevices   * 3.5.0      2018-05-15 local                           
 grid          3.5.0      2018-05-15 local                           
 gsubfn        0.7        2018-03-16 CRAN (R 3.5.0)                  
 gtable        0.2.0      2016-02-26 CRAN (R 3.5.0)                  
 highr         0.7        2018-06-09 CRAN (R 3.5.0)                  
 hms           0.4.2.9000 2018-05-30 Github (tidyverse/hms@14e74ab)  
 htmltools     0.3.6      2017-04-28 CRAN (R 3.5.0)                  
 httr          1.3.1      2017-08-20 CRAN (R 3.5.0)                  
 kableExtra    0.9.0      2018-05-21 CRAN (R 3.5.0)                  
 knitr       * 1.20       2018-02-20 CRAN (R 3.5.0)                  
 labeling      0.3        2014-08-23 CRAN (R 3.5.0)                  
 lazyeval      0.2.1      2017-10-29 CRAN (R 3.5.0)                  
 magrittr    * 1.5        2014-11-22 CRAN (R 3.5.0)                  
 markdown      0.8        2017-04-20 CRAN (R 3.5.0)                  
 memoise       1.1.0      2017-04-21 CRAN (R 3.5.0)                  
 methods     * 3.5.0      2018-05-15 local                           
 munsell       0.4.3      2016-02-13 CRAN (R 3.5.0)                  
 odbc          1.1.6      2018-06-09 CRAN (R 3.5.0)                  
 OuhscMunge    0.1.9.9007 2018-05-23 local                           
 pillar        1.2.3      2018-05-25 CRAN (R 3.5.0)                  
 pkgconfig     2.0.1      2017-03-21 CRAN (R 3.5.0)                  
 plyr          1.8.4      2016-06-08 CRAN (R 3.5.0)                  
 proto         1.0.0      2016-10-29 CRAN (R 3.5.0)                  
 purrr         0.2.5      2018-05-29 CRAN (R 3.5.0)                  
 R6            2.2.2      2017-06-17 CRAN (R 3.5.0)                  
 Rcpp          0.12.17    2018-05-18 CRAN (R 3.5.0)                  
 readr         1.2.0      2018-05-30 Github (tidyverse/readr@d6d622b)
 rlang         0.2.1      2018-05-30 CRAN (R 3.5.0)                  
 rmarkdown     1.9        2018-03-01 CRAN (R 3.5.0)                  
 RODBC         1.3-15     2017-04-13 CRAN (R 3.5.0)                  
 rprojroot     1.3-2      2018-01-03 CRAN (R 3.5.0)                  
 rsconnect     0.8.8      2018-03-09 CRAN (R 3.5.0)                  
 RSQLite     * 2.1.1      2018-05-06 CRAN (R 3.5.0)                  
 rstudioapi    0.7        2017-09-07 CRAN (R 3.5.0)                  
 rvest         0.3.2      2016-06-17 CRAN (R 3.5.0)                  
 scales        0.5.0      2017-08-24 CRAN (R 3.5.0)                  
 sqldf         0.4-11     2017-06-28 CRAN (R 3.5.0)                  
 stats       * 3.5.0      2018-05-15 local                           
 stringi       1.2.2      2018-05-02 CRAN (R 3.5.0)                  
 stringr       1.3.1      2018-05-10 CRAN (R 3.5.0)                  
 tcltk         3.5.0      2018-05-15 local                           
 testit        0.7        2017-05-22 CRAN (R 3.5.0)                  
 tibble        1.4.2      2018-01-22 CRAN (R 3.5.0)                  
 tidyr         0.8.1      2018-05-18 CRAN (R 3.5.0)                  
 tidyselect    0.2.4      2018-02-26 CRAN (R 3.5.0)                  
 tools         3.5.0      2018-05-15 local                           
 utf8          1.1.4      2018-05-24 CRAN (R 3.5.0)                  
 utils       * 3.5.0      2018-05-15 local                           
 viridisLite   0.3.0      2018-02-01 CRAN (R 3.5.0)                  
 withr         2.1.2      2018-03-15 CRAN (R 3.5.0)                  
 xml2          1.2.0      2018-01-24 CRAN (R 3.5.0)                  
 yaml          2.1.19     2018-05-01 CRAN (R 3.5.0)                  
```
</details>



Report rendered by Will at 2018-06-19, 13:02 -0500 in 1 seconds.
