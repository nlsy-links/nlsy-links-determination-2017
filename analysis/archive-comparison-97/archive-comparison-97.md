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

The roster is the starting place for the NLSY97 kinship links.  `R` values can later be adjusted higher (*e.g.*, for MZs) or lower (*e.g.*, the half siblings who later appear to share zero parents).

The codebook entry for this item (`SH-26 []`) is available online as a  [static web page](https://www.nlsinfo.org/sites/nlsinfo.org/files/attachments/121128/nlsy97r1scrhh.html#SH-26) or through the [interactive NLS Investigator](https://www.nlsinfo.org/investigator/pages/search.jsp#R11170.00).  

See the third table in this section to view the exact wording the participant selected.  For example, the label `cousin_female_unsure` in the first two tables corresponds to the response 82, "Female cousin (biological or social)".

The designation of "lower" and "upper" is fairly arbitrary.  It's organized so a brother-sister pair contribute to the same row as a sister-brother pair.


## Mean Rs within Roster categories



| count| RRoster<br/>mean| roster<br/>response<br/>lower| roster<br/>response<br/>upper| RPass1<br/>mean| RFull<br/>mean| R<br/>mean| concern|
|-----:|----------------:|-----------------------------:|-----------------------------:|---------------:|--------------:|----------:|-------:|
|   577|            0.500|                  brother_full|                  brother_full|           0.510|          0.510|      0.510|       -|
| 1,017|            0.500|                   sister_full|                  brother_full|           0.500|          0.500|      0.500|       -|
|   517|            0.500|                   sister_full|                   sister_full|           0.516|          0.516|      0.516|       -|
|     4|            0.250|      brother_half_same_father|      brother_half_same_father|           0.188|          0.188|         --|       -|
|    31|            0.250|      brother_half_same_mother|      brother_half_same_mother|           0.242|          0.242|         --|       -|
|     1|            0.250|       sister_half_same_father|      brother_half_same_father|           0.250|          0.250|      0.250|       -|
|    66|            0.250|       sister_half_same_mother|      brother_half_same_mother|           0.250|          0.250|      0.250|       -|
|    24|            0.250|       sister_half_same_mother|       sister_half_same_mother|           0.250|          0.250|      0.250|       -|
|     7|            0.000|              brother_adoptive|              brother_adoptive|           0.000|          0.000|         --|       -|
|    11|            0.000|                brother_foster|                brother_foster|           0.000|          0.000|         --|       -|
|    29|            0.000|                  brother_step|                  brother_step|           0.000|          0.000|         --|       -|
|     1|            0.000|           daughter_of_partner|             nonrelative_other|           0.000|          0.000|         --|       -|
|    29|            0.000|             nonrelative_other|             nonrelative_other|           0.000|          0.000|         --|       -|
|     2|            0.000|                       partner|                       partner|           0.000|          0.000|         --|       -|
|     1|            0.000|                      roommate|                      roommate|           0.000|          0.000|         --|       -|
|    10|            0.000|               sister_adoptive|              brother_adoptive|           0.000|          0.000|         --|       -|
|     3|            0.000|               sister_adoptive|               sister_adoptive|           0.000|          0.000|         --|       -|
|     6|            0.000|                 sister_foster|                brother_foster|           0.000|          0.000|         --|       -|
|    14|            0.000|                 sister_foster|                 sister_foster|           0.000|          0.000|         --|       -|
|     2|            0.000|                 sister_in_law|                 sister_in_law|           0.000|          0.000|         --|       -|
|    40|            0.000|                   sister_step|                  brother_step|           0.000|          0.000|         --|       -|
|    19|            0.000|                   sister_step|                   sister_step|           0.000|          0.000|         --|       -|
|     1|               --|                   aunt_unsure|                 nephew_unsure|              --|             --|         --|       -|
|     3|               --|                   aunt_unsure|                  niece_unsure|              --|             --|         --|       -|
|    24|               --|          cousin_female_unsure|          cousin_female_unsure|              --|             --|         --|       -|
|    38|               --|          cousin_female_unsure|            cousin_male_unsure|              --|             --|         --|       -|
|    28|               --|            cousin_male_unsure|            cousin_male_unsure|              --|             --|         --|       -|
|     1|               --|                   do_not_know|                       refusal|              --|             --|         --|       -|
|     1|               --|                       refusal|                       refusal|              --|             --|         --|       -|
|     7|               --|                  uncle_unsure|                 nephew_unsure|              --|             --|         --|       -|
|     5|               --|                  uncle_unsure|                  niece_unsure|              --|             --|         --|       -|


## Exact Rs of Roster categories



| count| RRoster| roster<br/>response<br/>lower| roster<br/>response<br/>upper| RPass1| RFull|     R| concern|
|-----:|-------:|-----------------------------:|-----------------------------:|------:|-----:|-----:|-------:|
|   566|   0.500|                  brother_full|                  brother_full|  0.500| 0.500| 0.500|       -|
|    11|   0.500|                  brother_full|                  brother_full|  1.000| 1.000| 1.000|       -|
| 1,017|   0.500|                   sister_full|                  brother_full|  0.500| 0.500| 0.500|       -|
|   500|   0.500|                   sister_full|                   sister_full|  0.500| 0.500| 0.500|       -|
|    17|   0.500|                   sister_full|                   sister_full|  1.000| 1.000| 1.000|       -|
|     3|   0.250|      brother_half_same_father|      brother_half_same_father|  0.250| 0.250| 0.250|       -|
|     1|   0.250|      brother_half_same_father|      brother_half_same_father|  0.000| 0.000|    --|       -|
|    30|   0.250|      brother_half_same_mother|      brother_half_same_mother|  0.250| 0.250| 0.250|       -|
|     1|   0.250|      brother_half_same_mother|      brother_half_same_mother|  0.000| 0.000|    --|       -|
|     1|   0.250|       sister_half_same_father|      brother_half_same_father|  0.250| 0.250| 0.250|       -|
|    66|   0.250|       sister_half_same_mother|      brother_half_same_mother|  0.250| 0.250| 0.250|       -|
|    24|   0.250|       sister_half_same_mother|       sister_half_same_mother|  0.250| 0.250| 0.250|       -|
|     7|   0.000|              brother_adoptive|              brother_adoptive|  0.000| 0.000|    --|       -|
|    11|   0.000|                brother_foster|                brother_foster|  0.000| 0.000|    --|       -|
|    29|   0.000|                  brother_step|                  brother_step|  0.000| 0.000|    --|       -|
|     1|   0.000|           daughter_of_partner|             nonrelative_other|  0.000| 0.000|    --|       -|
|    29|   0.000|             nonrelative_other|             nonrelative_other|  0.000| 0.000|    --|       -|
|     2|   0.000|                       partner|                       partner|  0.000| 0.000|    --|       -|
|     1|   0.000|                      roommate|                      roommate|  0.000| 0.000|    --|       -|
|    10|   0.000|               sister_adoptive|              brother_adoptive|  0.000| 0.000|    --|       -|
|     3|   0.000|               sister_adoptive|               sister_adoptive|  0.000| 0.000|    --|       -|
|     6|   0.000|                 sister_foster|                brother_foster|  0.000| 0.000|    --|       -|
|    14|   0.000|                 sister_foster|                 sister_foster|  0.000| 0.000|    --|       -|
|     2|   0.000|                 sister_in_law|                 sister_in_law|  0.000| 0.000|    --|       -|
|    40|   0.000|                   sister_step|                  brother_step|  0.000| 0.000|    --|       -|
|    19|   0.000|                   sister_step|                   sister_step|  0.000| 0.000|    --|       -|
|     1|      --|                   aunt_unsure|                 nephew_unsure|     --|    --|    --|       -|
|     3|      --|                   aunt_unsure|                  niece_unsure|     --|    --|    --|       -|
|    23|      --|          cousin_female_unsure|          cousin_female_unsure|     --|    --|    --|       -|
|     1|      --|          cousin_female_unsure|          cousin_female_unsure|  0.000| 0.000|    --|       -|
|    38|      --|          cousin_female_unsure|            cousin_male_unsure|     --|    --|    --|       -|
|    28|      --|            cousin_male_unsure|            cousin_male_unsure|     --|    --|    --|       -|
|     1|      --|                   do_not_know|                       refusal|     --|    --|    --|       -|
|     1|      --|                       refusal|                       refusal|     --|    --|    --|       -|
|     7|      --|                  uncle_unsure|                 nephew_unsure|     --|    --|    --|       -|
|     5|      --|                  uncle_unsure|                  niece_unsure|     --|    --|    --|       -|


## Exact Wording in the NLSY97

The 92 possible responses to the items like:
`What is [name of person in relationship loops([loop number 3])]'s relationship to [name of person on household roster 2([loop number 1])]?`.  Some options were never selected in the survey.<table class="table table-striped table-hover table-condensed table-responsive" style="width: auto !important; ">
 <thead>
  <tr>
   <th style="text-align:right;"> NLSY ID </th>
   <th style="text-align:left;"> Exact NLSY Wording </th>
   <th style="text-align:left;"> Cannonical Label </th>
  </tr>
 </thead>
<tbody>
  <tr>
   <td style="text-align:right;"> -4 </td>
   <td style="text-align:left;"> valid_skip </td>
   <td style="text-align:left;"> <code>valid_skip</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> -2 </td>
   <td style="text-align:left;"> Don't Know </td>
   <td style="text-align:left;"> <code>do_not_know</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> -1 </td>
   <td style="text-align:left;"> refusal </td>
   <td style="text-align:left;"> <code>refusal</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 0 </td>
   <td style="text-align:left;"> Identity </td>
   <td style="text-align:left;"> <code>self</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 1 </td>
   <td style="text-align:left;"> Wife </td>
   <td style="text-align:left;"> <code>wife</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 2 </td>
   <td style="text-align:left;"> Husband </td>
   <td style="text-align:left;"> <code>husband</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 3 </td>
   <td style="text-align:left;"> Mother </td>
   <td style="text-align:left;"> <code>mother</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 4 </td>
   <td style="text-align:left;"> Father </td>
   <td style="text-align:left;"> <code>father</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 7 </td>
   <td style="text-align:left;"> Step-mother </td>
   <td style="text-align:left;"> <code>mother_step</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 8 </td>
   <td style="text-align:left;"> Step-father </td>
   <td style="text-align:left;"> <code>father_step</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 5 </td>
   <td style="text-align:left;"> Adoptive mother </td>
   <td style="text-align:left;"> <code>mother_adoptive</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 6 </td>
   <td style="text-align:left;"> Adoptive father </td>
   <td style="text-align:left;"> <code>father_adoptive</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 9 </td>
   <td style="text-align:left;"> Foster mother </td>
   <td style="text-align:left;"> <code>mother_foster</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 10 </td>
   <td style="text-align:left;"> Foster father </td>
   <td style="text-align:left;"> <code>father_foster</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 11 </td>
   <td style="text-align:left;"> Mother-in-law </td>
   <td style="text-align:left;"> <code>mother_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 12 </td>
   <td style="text-align:left;"> Father-in-law </td>
   <td style="text-align:left;"> <code>father_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 13 </td>
   <td style="text-align:left;"> Sister (FULL) </td>
   <td style="text-align:left;"> <code>sister_full</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 14 </td>
   <td style="text-align:left;"> Brother (FULL) </td>
   <td style="text-align:left;"> <code>brother_full</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 15 </td>
   <td style="text-align:left;"> Sister (HALF - Same mother) </td>
   <td style="text-align:left;"> <code>sister_half_same_mother</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 16 </td>
   <td style="text-align:left;"> Sister (HALF - Same father) </td>
   <td style="text-align:left;"> <code>sister_half_same_father</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 17 </td>
   <td style="text-align:left;"> Sister (HALF - don't know) </td>
   <td style="text-align:left;"> <code>sister_half_unsure</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 18 </td>
   <td style="text-align:left;"> Brother (HALF - Same mother) </td>
   <td style="text-align:left;"> <code>brother_half_same_mother</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 19 </td>
   <td style="text-align:left;"> Brother (HALF - Same father) </td>
   <td style="text-align:left;"> <code>brother_half_same_father</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 20 </td>
   <td style="text-align:left;"> Brother (HALF - don't know) </td>
   <td style="text-align:left;"> <code>brother_half_unsure</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 21 </td>
   <td style="text-align:left;"> Sister (STEP) </td>
   <td style="text-align:left;"> <code>sister_step</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 22 </td>
   <td style="text-align:left;"> Brother (STEP) </td>
   <td style="text-align:left;"> <code>brother_step</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 23 </td>
   <td style="text-align:left;"> Sister (ADOPTIVE) </td>
   <td style="text-align:left;"> <code>sister_adoptive</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 24 </td>
   <td style="text-align:left;"> Brother (ADOPTIVE) </td>
   <td style="text-align:left;"> <code>brother_adoptive</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 25 </td>
   <td style="text-align:left;"> Sister (FOSTER) </td>
   <td style="text-align:left;"> <code>sister_foster</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 26 </td>
   <td style="text-align:left;"> Brother (FOSTER) </td>
   <td style="text-align:left;"> <code>brother_foster</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 27 </td>
   <td style="text-align:left;"> Brother-in-law </td>
   <td style="text-align:left;"> <code>brother_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 28 </td>
   <td style="text-align:left;"> Sister-in-law </td>
   <td style="text-align:left;"> <code>sister_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 29 </td>
   <td style="text-align:left;"> Maternal Grandmother </td>
   <td style="text-align:left;"> <code>grandmother_maternal</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 30 </td>
   <td style="text-align:left;"> Paternal Grandmother </td>
   <td style="text-align:left;"> <code>grandmother_paternal</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 31 </td>
   <td style="text-align:left;"> Social Grandmother </td>
   <td style="text-align:left;"> <code>grandmother_social</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 32 </td>
   <td style="text-align:left;"> Grandmother (don't know or refused) </td>
   <td style="text-align:left;"> <code>grandmother_unsure</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 33 </td>
   <td style="text-align:left;"> Maternal Grandfather </td>
   <td style="text-align:left;"> <code>grandfather_maternal</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 34 </td>
   <td style="text-align:left;"> Paternal Grandfather </td>
   <td style="text-align:left;"> <code>grandfather_paternal</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 35 </td>
   <td style="text-align:left;"> Social Grandfather </td>
   <td style="text-align:left;"> <code>grandfather_social</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 36 </td>
   <td style="text-align:left;"> Grandfather (don't know or refused) </td>
   <td style="text-align:left;"> <code>grandfather_unsure</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 37 </td>
   <td style="text-align:left;"> Maternal Great-Grandmother </td>
   <td style="text-align:left;"> <code>great_grandmother</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 38 </td>
   <td style="text-align:left;"> Paternal Great-Grandmother </td>
   <td style="text-align:left;"> <code>great_grandfather</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 39 </td>
   <td style="text-align:left;"> Social Great-Grandmother </td>
   <td style="text-align:left;"> <code>great_grandmother_social</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 40 </td>
   <td style="text-align:left;"> Great-Grandmother (don't know or refused) </td>
   <td style="text-align:left;"> <code>great_grandmother_unsure</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 41 </td>
   <td style="text-align:left;"> Maternal Great-Grandfather </td>
   <td style="text-align:left;"> <code>great_grandfather_maternal</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 42 </td>
   <td style="text-align:left;"> Paternal Great-Grandfather </td>
   <td style="text-align:left;"> <code>great_grandfather_paternal</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 43 </td>
   <td style="text-align:left;"> Social Great-Grandfather </td>
   <td style="text-align:left;"> <code>great_grandfather_social</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 44 </td>
   <td style="text-align:left;"> Great-Grandfather (don't know or refused) </td>
   <td style="text-align:left;"> <code>great_grandfather_unsure</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 45 </td>
   <td style="text-align:left;"> Great Great Grandmother </td>
   <td style="text-align:left;"> <code>great_great_grandmother</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 46 </td>
   <td style="text-align:left;"> Great Great Grandfather </td>
   <td style="text-align:left;"> <code>great_great_grandfather</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 47 </td>
   <td style="text-align:left;"> Granddaughter (Biological or social) </td>
   <td style="text-align:left;"> <code>granddaughter</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 48 </td>
   <td style="text-align:left;"> Grandson (Biological or social) </td>
   <td style="text-align:left;"> <code>grandson</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 49 </td>
   <td style="text-align:left;"> Daughter (Biological) </td>
   <td style="text-align:left;"> <code>daughter_bio</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 50 </td>
   <td style="text-align:left;"> Son (Biological) </td>
   <td style="text-align:left;"> <code>son_bio</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 51 </td>
   <td style="text-align:left;"> Step-daughter </td>
   <td style="text-align:left;"> <code>daughter_step</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 52 </td>
   <td style="text-align:left;"> Step-son </td>
   <td style="text-align:left;"> <code>son_step</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 53 </td>
   <td style="text-align:left;"> Adoptive daughter </td>
   <td style="text-align:left;"> <code>daughter_adoptive</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 54 </td>
   <td style="text-align:left;"> Adoptive son </td>
   <td style="text-align:left;"> <code>son_adoptive</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 55 </td>
   <td style="text-align:left;"> Foster daughter </td>
   <td style="text-align:left;"> <code>daughter_foster</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 56 </td>
   <td style="text-align:left;"> Foster son </td>
   <td style="text-align:left;"> <code>son_foster</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 57 </td>
   <td style="text-align:left;"> Daughter of lover/partner </td>
   <td style="text-align:left;"> <code>daughter_of_partner</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 58 </td>
   <td style="text-align:left;"> Son of lover/partner </td>
   <td style="text-align:left;"> <code>son_of_partner</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 59 </td>
   <td style="text-align:left;"> Daughter-in-law </td>
   <td style="text-align:left;"> <code>daughter_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 60 </td>
   <td style="text-align:left;"> Son-in-law </td>
   <td style="text-align:left;"> <code>son_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 61 </td>
   <td style="text-align:left;"> Grandmother-in-law </td>
   <td style="text-align:left;"> <code>grandmother_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 62 </td>
   <td style="text-align:left;"> Grandfather-in-law </td>
   <td style="text-align:left;"> <code>grandfather_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 63 </td>
   <td style="text-align:left;"> Aunt-in-law </td>
   <td style="text-align:left;"> <code>aunt_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 64 </td>
   <td style="text-align:left;"> Uncle-in-law </td>
   <td style="text-align:left;"> <code>uncle_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 65 </td>
   <td style="text-align:left;"> Cousin-in-law </td>
   <td style="text-align:left;"> <code>cousin_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 66 </td>
   <td style="text-align:left;"> Great-Grandmother-in-law </td>
   <td style="text-align:left;"> <code>great_grandmother_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 67 </td>
   <td style="text-align:left;"> Great-Grandfather-in-law </td>
   <td style="text-align:left;"> <code>great_grandfather_in_law</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 68 </td>
   <td style="text-align:left;"> Roommate </td>
   <td style="text-align:left;"> <code>roommate</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 69 </td>
   <td style="text-align:left;"> Lover/partner </td>
   <td style="text-align:left;"> <code>partner</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 70 </td>
   <td style="text-align:left;"> Aunt (biological or social) </td>
   <td style="text-align:left;"> <code>aunt_unsure</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 71 </td>
   <td style="text-align:left;"> Great Aunt </td>
   <td style="text-align:left;"> <code>great_aunt</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 72 </td>
   <td style="text-align:left;"> Uncle (biological or social) </td>
   <td style="text-align:left;"> <code>uncle_unsure</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 73 </td>
   <td style="text-align:left;"> Great Uncle </td>
   <td style="text-align:left;"> <code>great_uncle</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 74 </td>
   <td style="text-align:left;"> Niece (biological or social) </td>
   <td style="text-align:left;"> <code>niece_unsure</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 75 </td>
   <td style="text-align:left;"> Step Niece (biological or social) </td>
   <td style="text-align:left;"> <code>niece_step</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 76 </td>
   <td style="text-align:left;"> Foster Niece (biological or social) </td>
   <td style="text-align:left;"> <code>niece_foster</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 77 </td>
   <td style="text-align:left;"> Adoptive Niece (biological or social) </td>
   <td style="text-align:left;"> <code>niece_adoptive</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 78 </td>
   <td style="text-align:left;"> Nephew (biological or social) </td>
   <td style="text-align:left;"> <code>nephew_unsure</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 79 </td>
   <td style="text-align:left;"> Step Nephew (biological or social) </td>
   <td style="text-align:left;"> <code>nephew_step</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 80 </td>
   <td style="text-align:left;"> Foster Nephew (biological or social) </td>
   <td style="text-align:left;"> <code>nephew_foster</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 81 </td>
   <td style="text-align:left;"> Adoptive Nephew (biological or social) </td>
   <td style="text-align:left;"> <code>nephew_adoptive</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 82 </td>
   <td style="text-align:left;"> Female cousin (biological or social) </td>
   <td style="text-align:left;"> <code>cousin_female_unsure</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 83 </td>
   <td style="text-align:left;"> Male cousin (biological or social) </td>
   <td style="text-align:left;"> <code>cousin_male_unsure</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 84 </td>
   <td style="text-align:left;"> Other relative </td>
   <td style="text-align:left;"> <code>relative_other</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 85 </td>
   <td style="text-align:left;"> Other non-relative </td>
   <td style="text-align:left;"> <code>nonrelative_other</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 86 </td>
   <td style="text-align:left;"> Great Grandson </td>
   <td style="text-align:left;"> <code>great_grandson</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 87 </td>
   <td style="text-align:left;"> Great Granddaughter </td>
   <td style="text-align:left;"> <code>great_granddaughter</code> </td>
  </tr>
  <tr>
   <td style="text-align:right;"> 99 </td>
   <td style="text-align:left;"> RELATIONSHIP MISSING </td>
   <td style="text-align:left;"> <code>relationship_missing</code> </td>
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



Report rendered by Will at 2018-06-20, 19:31 -0500 in 2 seconds.
