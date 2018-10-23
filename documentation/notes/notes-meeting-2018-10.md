# Nlsy79 Updates with 2012 & 2014 Survey Waves

### Counts by path (including missing *R*)
| Relationship Path  | Through 2010 Wave | Through 2014 Wave |
| :----------------- | ----------------: | ----------------: |
| Gen1Housemates     |             5,302 |             5,302 |
| Gen2Siblings       |            11,088 |            11,114 |
| Gen2Cousins        |             4,995 |             5,000 |
| ParentChild        |            11,504 |            11,521 |
| AuntNiece          |             9,884 |             9,899 |

### ACE of NLSY Height

*R* is updated; height is constant.  See example 6 of vignette
> Pairs are only excluded (a) if they belong to one of the small R groups that are difficult to estimate, or (b) if the value for adult height is missing.

| Component                  | Through 2010 Wave | Through 2014 Wave |
| :------------------------- | ----------------: | ----------------: |
| <em>a</em><sup>2</sup>     |          .7364675 |          .7366543 |
| <em>c</em><sup>2</sup>     |          .0658652 |          .0658000 |
| <em>e</em><sup>2</sup>     |          .1976673 |          .1975456 |
| <em>N</em><sub>pairs</sub> |            24,697 |            24,697 |

Remarks:
1. Nice & consistent.  Boring is good here, I guess.


### Counts by path & *R*

|    | Relationship     | <em>R</em> | N<sub>2010</sub> | N<sub>2014</sub> | delta    |
| -- | :-------------   | :------  | ------: | --------: | ---:  |
|  1 | Gen1Housemates   |  0.25    |     285 |       285 |     |
|  2 | Gen1Housemates   |  0.5     |   3,967 |     3,967 |     |
|  3 | Gen1Housemates   |  1       |      11 |        11 |     |
|  4 | Gen1Housemates   | --       |   1,039 |     1,039 |     |
|  5 | Gen2Siblings     |  0.25    |   3,442 |     3,448 | + 6 |
|  6 | Gen2Siblings     |  0.375   |     610 |       622 | +12 |
|  7 | Gen2Siblings     |  0.5     |   6,997 |     7,004 | + 7 |
|  8 | Gen2Siblings     |  0.75    |      12 |        13 | + 1 |
|  9 | Gen2Siblings     |  1       |      27 |        27 |     |
| 10 | Gen2Cousins      |  0.0625  |     309 |       309 |     |
| 11 | Gen2Cousins      |  0.125   |   3,878 |     3,878 |     |
| 12 | Gen2Cousins      |  0.25    |      18 |        18 |     |
| 13 | Gen2Cousins      | --       |     790 |       795 | + 5 |
| 14 | ParentChild      |  0.5     |  11,504 |    11,521 | +17 |
| 15 | AuntNiece        |  0.125   |     587 |       588 | + 1 |
| 16 | AuntNiece        |  0.25    |   7,438 |     7,450 | + 2 |
| 17 | AuntNiece        |  0.5     |      14 |        14 |     |
| 18 | AuntNiece        | --       |   1,845 |     1,847 | + 2 |

Remarks:
1. More parent-kids than expected?


# Nlsy97


|    | Relationship     | <em>R</em><sub>Full</sub> | N<sub>2014</sub> |
| -- | :-------------   | :------  | ------: |
|  1 | Housemates       |   0      |     177 |
|  2 | Housemates       |   0.25   |     124 |
|  3 | Housemates       |   0.5    |   2,083 |
|  4 | Housemates       |   1      |      28 |
|  5 | Housemates       |   --     |     107 |

Remarks:
* No ambiguous among the explicits
* Few half sibs

# Link Determination: Easier to Update and Maintain

### Modifying Variables
1. `tblVariable` is ordered by `Item`, `ExtractSource`, `SurveyYear`, `LoopIndex1`, `LoopIndex2` instead of ordered conceptually.  It's a little harder to see some conceptual groupings (eg, father's health in 2010), but a lot easier to determine where things go, and which extracts to use.
1. Script for extract mitosis.  Useful to keep extract size manageable, and under database's limit of 1,024 columns.
1. When an extract changes, the database table structure can be updated automatically (ie, adding columns, setting primary key, & disallowing nulls).
1. Variable duplicates & discrepancies (between extracts & tables) are caught sooner in the process, with better error messages.

### More Portable

1. Better CSV arrangement
1. SQLite database is used for some intermediate storage.  SQL Server isn't required to track algorithm's longitudinal changes.

# Future

### Package: Accommodate Bigger Datasets

1. The subject * survey_year already requires manual intervention & approval from CRAN.
1. The desired pair * survey_year will blow it up.  Additional waves & Nlsy97 won't help.
1. Continue to embed the critical datasets within the package.  Deposit the bigger datasets in a Dataverse; it will download & cache the dataset the first time it's used on a machine.  Few people will use these, so the package won't be a bigger burden than necessary.  The dataverse exposes some simple tracking stats.

### Articles
1. NlsyLinks description in JSS
1. ROC used in determination of links

### Other
1. Compare w/ other people's more casual 97 links?  Pros & cons.  Don't want to appear competitive or critical.
