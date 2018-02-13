---
title: Comparison of Versions of NLSY97 Kinship Links
date: "Date: 2018-02-13"
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
    * 1: naive roster.
    * 1: naive roster.

# Comparison of Agreement
![](figure-png/graph-roc-1.png)<!-- -->

# Table of *R* Assignments
<!-- html table generated in R 3.4.3 by xtable 1.8-2 package -->
<!-- Tue Feb 13 17:41:27 2018 -->
<table border=1>
<caption align="bottom"> Counts for 97 Housemates </caption>
<tr> <th> R </th> <th> Implicit </th> <th> Explicit </th> <th> Roster </th> <th> Eventual </th>  </tr>
  <tr> <td> - </td> <td align="right"> 2519 </td> <td align="right"> 2519 </td> <td align="right"> 1202 </td> <td align="right">   0 </td> </tr>
  <tr> <td> 0.000 </td> <td align="right"> - </td> <td align="right"> - </td> <td align="right"> 174 </td> <td align="right"> 1376 </td> </tr>
  <tr> <td> 0.250 </td> <td align="right"> - </td> <td align="right"> - </td> <td align="right"> 126 </td> <td align="right"> 126 </td> </tr>
  <tr> <td> 0.500 </td> <td align="right"> - </td> <td align="right"> - </td> <td align="right"> 1017 </td> <td align="right"> 1017 </td> </tr>
   </table>
<!-- html table generated in R 3.4.3 by xtable 1.8-2 package -->
<!-- Tue Feb 13 17:41:27 2018 -->
<table border=1>
<caption align="bottom"> Counts for 97 Housemates (Previous version of links) </caption>
<tr> <th> R </th> <th> Implicit </th> <th> Explicit </th> <th> Roster </th> <th> Eventual </th>  </tr>
  <tr> <td> - </td> <td align="right"> 2519 </td> <td align="right"> 2519 </td> <td align="right"> 1202 </td> <td align="right">   0 </td> </tr>
  <tr> <td> 0.000 </td> <td align="right"> - </td> <td align="right"> - </td> <td align="right"> 174 </td> <td align="right"> 1376 </td> </tr>
  <tr> <td> 0.250 </td> <td align="right"> - </td> <td align="right"> - </td> <td align="right"> 126 </td> <td align="right"> 126 </td> </tr>
  <tr> <td> 0.500 </td> <td align="right"> - </td> <td align="right"> - </td> <td align="right"> 1017 </td> <td align="right"> 1017 </td> </tr>
   </table>

# Breakdown of Agreements 
<!-- html table generated in R 3.4.3 by xtable 1.8-2 package -->
<!-- Tue Feb 13 17:41:27 2018 -->
<table border=1>
<caption align="bottom"> Joint Frequencies for 97 Housemates </caption>
<tr> <th> Count </th> <th> RImplicit </th> <th> RExplicit </th> <th> RRoster </th> <th> Delta </th>  </tr>
  \rowcolor{nullColor}  <tr> <td align="right"> 1202 </td> <td align="right"> - </td> <td align="right"> - </td> <td align="right"> - </td> <td align="right"> 0 </td> </tr>
   \rowcolor{nullColor} <tr> <td align="right"> 1017 </td> <td align="right"> - </td> <td align="right"> - </td> <td align="right"> 0.500 </td> <td align="right"> 0 </td> </tr>
   \rowcolor{nullColor} <tr> <td align="right"> 174 </td> <td align="right"> - </td> <td align="right"> - </td> <td align="right"> 0.000 </td> <td align="right"> 0 </td> </tr>
   \rowcolor{nullColor} <tr> <td align="right"> 126 </td> <td align="right"> - </td> <td align="right"> - </td> <td align="right"> 0.250 </td> <td align="right"> 0 </td> </tr>
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
 version  R version 3.4.3 Patched (2018-01-25 r74163)
 system   x86_64, mingw32                            
 ui       RTerm                                      
 language (EN)                                       
 collate  English_United States.1252                 
 tz       America/Chicago                            
 date     2018-02-13                                 
```

```
Packages ------------------------------------------------------------------------------------------
```

```
 package    * version    date       source                            
 assertthat   0.2.0      2017-04-11 CRAN (R 3.4.2)                    
 backports    1.1.2      2017-12-13 CRAN (R 3.4.3)                    
 base       * 3.4.3      2018-01-26 local                             
 bindr        0.1        2016-11-13 CRAN (R 3.4.2)                    
 bindrcpp   * 0.2        2017-06-17 CRAN (R 3.4.2)                    
 bit          1.1-12     2014-04-09 CRAN (R 3.4.1)                    
 bit64        0.9-7      2017-05-08 CRAN (R 3.4.1)                    
 blob         1.1.0      2017-06-17 CRAN (R 3.4.2)                    
 checkmate    1.8.5      2017-10-24 CRAN (R 3.4.2)                    
 colorspace   1.3-2      2016-12-14 CRAN (R 3.4.2)                    
 compiler     3.4.3      2018-01-26 local                             
 datasets   * 3.4.3      2018-01-26 local                             
 DBI          0.7        2017-06-18 CRAN (R 3.4.2)                    
 devtools     1.13.4     2017-11-09 CRAN (R 3.4.2)                    
 digest       0.6.15     2018-01-28 CRAN (R 3.4.3)                    
 dplyr        0.7.4.9000 2018-01-26 Github (tidyverse/dplyr@3f91e1e)  
 evaluate     0.10.1     2017-06-24 CRAN (R 3.4.2)                    
 ggplot2    * 2.2.1.9000 2017-12-20 Github (tidyverse/ggplot2@bfff1d8)
 glue         1.2.0      2017-10-29 CRAN (R 3.4.2)                    
 graphics   * 3.4.3      2018-01-26 local                             
 grDevices  * 3.4.3      2018-01-26 local                             
 grid         3.4.3      2018-01-26 local                             
 gtable       0.2.0      2016-02-26 CRAN (R 3.4.2)                    
 hms          0.4.1      2018-01-24 CRAN (R 3.4.3)                    
 htmltools    0.3.6      2017-04-28 CRAN (R 3.4.2)                    
 knitr      * 1.19       2018-01-29 CRAN (R 3.4.3)                    
 labeling     0.3        2014-08-23 CRAN (R 3.4.1)                    
 lazyeval     0.2.1      2017-10-29 CRAN (R 3.4.2)                    
 magrittr   * 1.5        2014-11-22 CRAN (R 3.4.2)                    
 memoise      1.1.0      2017-04-21 CRAN (R 3.4.2)                    
 methods    * 3.4.3      2018-01-26 local                             
 munsell      0.4.3      2016-02-13 CRAN (R 3.4.2)                    
 odbc         1.1.5      2018-01-23 CRAN (R 3.4.3)                    
 pillar       1.1.0      2018-01-14 CRAN (R 3.4.3)                    
 pkgconfig    2.0.1      2017-03-21 CRAN (R 3.4.2)                    
 plyr         1.8.4      2016-06-08 CRAN (R 3.4.2)                    
 purrr        0.2.4      2017-10-18 CRAN (R 3.4.2)                    
 R6           2.2.2      2017-06-17 CRAN (R 3.4.2)                    
 Rcpp         0.12.15    2018-01-20 CRAN (R 3.4.3)                    
 rlang        0.1.6.9003 2018-01-26 Github (tidyverse/rlang@b5da865)  
 rmarkdown    1.8        2017-11-17 CRAN (R 3.4.2)                    
 rprojroot    1.3-2      2018-01-03 CRAN (R 3.4.3)                    
 scales       0.5.0.9000 2017-10-11 Github (hadley/scales@d767915)    
 stats      * 3.4.3      2018-01-26 local                             
 stringi      1.1.6      2017-11-17 CRAN (R 3.4.2)                    
 stringr      1.2.0      2017-02-18 CRAN (R 3.4.2)                    
 testit       0.7.1      2017-12-21 Github (yihui/testit@8a346dd)     
 tibble       1.4.2      2018-01-22 CRAN (R 3.4.3)                    
 tidyselect   0.2.3      2017-11-06 CRAN (R 3.4.2)                    
 tools        3.4.3      2018-01-26 local                             
 utils      * 3.4.3      2018-01-26 local                             
 withr        2.1.1.9000 2017-12-20 Github (jimhester/withr@df18523)  
 xtable     * 1.8-2      2016-02-05 CRAN (R 3.4.2)                    
 yaml         2.1.16     2017-12-12 CRAN (R 3.4.3)                    
```
</details>



Report rendered by Will at 2018-02-13, 17:41 -0600 in 4 seconds.
