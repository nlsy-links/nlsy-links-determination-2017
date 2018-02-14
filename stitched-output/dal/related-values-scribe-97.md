



This report was automatically generated with the R package **knitr**
(version 1.19).


```r
# rm(list=ls(all=TRUE))
# library(RODBC)
# library(plyr)
#
# channel <- RODBC::odbcDriverConnect("driver={SQL Server}; Server=Bee\\Bass; Database=NlsLinks; Uid=NlsyReadWrite; Pwd=nophi")
# algorithmVersion <- max(sqlQuery(channel, "SELECT MAX(AlgorithmVersion) as AlgorithmVersion  FROM [NlsLinks].[Process].[tblRelatedValuesArchive]"))
# odbcClose(channel)
#
# isGen1_S1 <- grepl("^\\d{1,7}00$", ds$SubjectTag_S1, perl=TRUE);
# isGen1_S2 <- grepl("^\\d{1,7}00$", ds$SubjectTag_S2, perl=TRUE);
#
# ds$Generation_S1 <- ifelse(isGen1_S1, 1L, 2L)
# ds$Generation_S2 <- ifelse(isGen1_S2, 1L, 2L)
#
# ds$SubjectID_S1 <- ifelse(isGen1_S1, ds$SubjectTag_S1 / 100, ds$SubjectTag_S1)
# ds$SubjectID_S2 <- ifelse(isGen1_S2, ds$SubjectTag_S2 / 100, ds$SubjectTag_S2)
#
# if( any((ds$SubjectID_S1 %% 1) != 0) ) stop("A Gen2 subject was accidentally classified as Gen1.")
# if( any((ds$SubjectID_S2 %% 1) != 0) ) stop("A Gen2 subject was accidentally classified as Gen1.")
#
# ds$SubjectID_S1 <- as.integer(ds$SubjectID_S1)
# ds$SubjectID_S2 <- as.integer(ds$SubjectID_S2)
#
#
# fileName <- sprintf("./ForDistribution/Links/Links2011V%d.csv", algorithmVersion)
#
# plyr::count(ds, vars=c("RelationshipPath", "R"))
#
# write.csv(ds, file=fileName, row.names=FALSE)
# summary(ds)

# table(ds$RelationshipPath, is.na(ds$RFull))
#############################################################################################################################
#############################################################################################################################
#############################################################################################################################


# knitr::stitch_rmd(script="./manipulation/te-ellis.R", output="./stitched-output/manipulation/te-ellis.md") # dir.create("./stitched-output/manipulation/", recursive=T)
rm(list=ls(all=TRUE))  #Clear the variables from previous runs.
```

```r
source("./utility/connectivity.R")
```

```r
# Attach these package(s) so their functions don't need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
library(magrittr            , quietly=TRUE)
library(DBI                 , quietly=TRUE)

# Verify these packages are available on the machine, but their functions need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
requireNamespace("readr"        )
requireNamespace("tidyr"        )
requireNamespace("dplyr"        ) # Avoid attaching dplyr, b/c its function names conflict with a lot of packages (esp base, stats, and plyr).
requireNamespace("testit"       ) # For asserting conditions meet expected patterns/conditions.
requireNamespace("checkmate"    ) # For asserting conditions meet expected patterns/conditions. # remotes::install_github("mllg/checkmate")
# requireNamespace("RODBC"      ) # For communicating with SQL Server over a locally-configured DSN.  Uncomment if you use 'upload-to-db' chunk.
requireNamespace("OuhscMunge"   ) # remotes::install_github(repo="OuhscBbmc/OuhscMunge")
```

```r
# Constant values that won't change.
config              <- config::get()

sql <- "
	SELECT
    rs.ExtendedID,
    rs.SubjectTag_S1,
    rs.SubjectTag_S2,
    s1.SubjectID             AS SubjectID_S1,
    s2.SubjectID             AS SubjectID_S2,
    rs.RelationshipPath,
    rs.EverSharedHouse,
    rv.R,
    rv.RFull,
    rv.MultipleBirthIfSameSex,
    rv.IsMz,
    rv.LastSurvey_S1,
    rv.LastSurvey_S2,
    rv.RImplicitPass1,
    rv.RImplicit,
    -- rv.RImplicit2004,
    -- rv.RImplicit - rv.RImplicit2004 AS RImplicitDifference,
    rv.RExplicit,
    rv.RExplicitPass1,
    rv.RPass1,
    rv.RExplicitOlderSibVersion,
    rv.RExplicitYoungerSibVersion,
    rv.RImplicitSubject,
    rv.RImplicitMother
  FROM Process.tblRelatedStructure rs
    LEFT JOIN Process.tblRelatedValues rv ON rs.ID = rv.ID
    LEFT JOIN Process.tblSubject s1 ON rs.SubjectTag_S1 = s1.SubjectID
    LEFT JOIN Process.tblSubject s2 ON rs.SubjectTag_S2 = s2.SubjectID
  WHERE rs.SubjectTag_S1 < rs.SubjectTag_S2
  ORDER BY ExtendedID, SubjectTag_S1, SubjectTag_S2
"
sql_archive <- "
  SELECT
    --a.ID
    a.AlgorithmVersion
    ,rs.ExtendedID
    ,a.SubjectTag_S1
    ,a.SubjectTag_S2
    ,s1.SubjectID             AS SubjectID_S1
    ,s2.SubjectID             AS SubjectID_S2
    ,a.MultipleBirthIfSameSex
    ,a.IsMz
    ,a.SameGeneration
    ,a.RosterAssignmentID
    ,a.RRoster
    ,a.LastSurvey_S1
    ,a.LastSurvey_S2
    ,a.RImplicitPass1
    ,a.RImplicit
    ,a.RImplicitSubject
    ,a.RImplicitMother
    ,a.RExplicitOldestSibVersion         AS RExplicitOlderSibVersion
    ,a.RExplicitYoungestSibVersion       AS RExplicitYoungerSibVersion
    ,a.RExplicitPass1
    ,a.RExplicit
    ,a.RPass1
    ,a.R
    ,a.RFull
    ,a.RPeek
  FROM [NlsyLinks97].[Archive].[tblRelatedValuesArchive]  a
    LEFT JOIN Process.tblRelatedStructure rs          ON (a.SubjectTag_S1=rs.SubjectTag_S1 AND a.SubjectTag_S2=rs.SubjectTag_S2)
    LEFT JOIN Process.tblSubject s1                   ON a.SubjectTag_S1 = s1.SubjectID
    LEFT JOIN Process.tblSubject s2                   ON a.SubjectTag_S2 = s2.SubjectID
  ORDER BY a.AlgorithmVersion, rs.ExtendedID, a.SubjectTag_S1, a.SubjectTag_S2
"
sql_description <- "
  SELECT MAX(AlgorithmVersion) as AlgorithmVersion
  FROM Archive.tblRelatedValuesArchive
"
```

```r
channel            <- open_dsn_channel_odbc(study = "97")
# DBI::dbGetInfo(channel)
ds                <- DBI::dbGetQuery(channel, sql)
ds_archive        <- DBI::dbGetQuery(channel, sql_archive)
ds_description    <- DBI::dbGetQuery(channel, sql_description)
DBI::dbDisconnect(channel, sql, sql_archive, sql_description)
```

```r
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call ato `dplyr::rename()`.
path_out_current    <- sprintf(config$links_97_current, ds_description$AlgorithmVersion)

ds <- ds %>%
  tibble::as_tibble() %>%
  dplyr::mutate(
    RExplicit                   = NA_real_,
    RExplicitPass1              = NA_real_,
    RExplicitOlderSibVersion    = NA_real_,
    RExplicitYoungerSibVersion  = NA_real_

    # RExplicitOlderSibVersion    = as.numeric(RExplicitOlderSibVersion   ),
    # RExplicitYoungerSibVersion  = as.numeric(RExplicitYoungerSibVersion )
  )

ds_archive <- ds_archive %>%
  tibble::as_tibble() %>%
  dplyr::mutate(
    RExplicit                   = NA_real_,
    RExplicitPass1              = NA_real_,
    RExplicitOlderSibVersion    = NA_real_,
    RExplicitYoungerSibVersion  = NA_real_

    # RExplicitOlderSibVersion    = as.numeric(RExplicitOlderSibVersion   ),
    # RExplicitYoungerSibVersion  = as.numeric(RExplicitYoungerSibVersion )
  )
#
# ds <- ds_archive %>%
#   dplyr::filter(.data$AlgorithmVersion == max(.data$AlgorithmVersion))
```

```r
# Sniff out problems
# OuhscMunge::verify_value_headstart(ds)
checkmate::assert_integer( ds$ExtendedID                 , any.missing=F , lower=8, upper=7477    )
checkmate::assert_integer( ds$SubjectTag_S1              , any.missing=F , lower=6, upper=9021    )
checkmate::assert_integer( ds$SubjectTag_S2              , any.missing=F , lower=7, upper=9022    )
checkmate::assert_integer( ds$SubjectID_S1               , any.missing=F , lower=6, upper=9021    )
checkmate::assert_integer( ds$SubjectID_S2               , any.missing=F , lower=7, upper=9022    )
checkmate::assert_integer( ds$RelationshipPath           , any.missing=F , lower=1, upper=1       )
checkmate::assert_logical( ds$EverSharedHouse            , any.missing=F                          )
checkmate::assert_numeric( ds$R                          , any.missing=T , lower=0, upper=1       )
checkmate::assert_numeric( ds$RFull                      , any.missing=T , lower=0, upper=1       )
checkmate::assert_integer( ds$MultipleBirthIfSameSex     , any.missing=T , lower=0, upper=255     )
checkmate::assert_integer( ds$IsMz                       , any.missing=T , lower=0, upper=255     )
checkmate::assert_integer( ds$LastSurvey_S1              , any.missing=T , lower=1997, upper=2015 )
checkmate::assert_integer( ds$LastSurvey_S2              , any.missing=T , lower=1997, upper=2015 )
checkmate::assert_numeric( ds$RImplicitPass1             , any.missing=T , lower=0, upper=1       )
checkmate::assert_numeric( ds$RImplicit                  , any.missing=T , lower=0, upper=1       )
checkmate::assert_numeric( ds$RExplicit                  , any.missing=T , lower=0, upper=1       )
checkmate::assert_numeric( ds$RExplicitPass1             , any.missing=T , lower=0, upper=1       )
checkmate::assert_numeric( ds$RPass1                     , any.missing=T , lower=0, upper=1       )
checkmate::assert_numeric( ds$RExplicitOlderSibVersion   , any.missing=T , lower=0, upper=1       )
checkmate::assert_numeric( ds$RExplicitYoungerSibVersion , any.missing=T , lower=0, upper=1       )
checkmate::assert_numeric( ds$RImplicitSubject           , any.missing=T , lower=0, upper=1       )
checkmate::assert_numeric( ds$RImplicitMother            , any.missing=T , lower=0, upper=1       )


subject_combo   <- paste0(ds$SubjectTag_S1, "vs", ds$SubjectTag_S2)
checkmate::assert_character(subject_combo, min.chars=3            , any.missing=F, unique=T)
checkmate::assert_character(subject_combo, pattern  ="^\\d{1,4}vs\\d{1,4}$"            , any.missing=F, unique=T)
```

```r
# dput(colnames(ds)) # Print colnames for line below.
columns_to_write <- c(
  "ExtendedID", "SubjectTag_S1", "SubjectTag_S2", "SubjectID_S1",
  "SubjectID_S2", "RelationshipPath", "EverSharedHouse",
  "R", "RFull",
  "MultipleBirthIfSameSex", "IsMz", "LastSurvey_S1", "LastSurvey_S2",
  "RImplicitPass1", "RImplicit", "RExplicit", "RExplicitPass1",
  "RPass1", "RExplicitOlderSibVersion", "RExplicitYoungerSibVersion",
  "RImplicitSubject", "RImplicitMother"
)
ds_slim <- ds %>%
  # dplyr::slice(1:100) %>%
  dplyr::select_(.dots=columns_to_write)
ds_slim
```

```
## # A tibble: 2,519 x 22
##    ExtendedID SubjectTag_S1 SubjectTag_S2 SubjectID_S1 SubjectID_S2
##         <int>         <int>         <int>        <int>        <int>
##  1          8             6             7            6            7
##  2          9             8             9            8            9
##  3          9             8            10            8           10
##  4          9             9            10            9           10
##  5         17            18            19           18           19
##  6         37            37            38           37           38
##  7         44            45            46           45           46
##  8         45            47            48           47           48
##  9         48            51            52           51           52
## 10         59            62            63           62           63
## # ... with 2,509 more rows, and 17 more variables: RelationshipPath <int>,
## #   EverSharedHouse <lgl>, R <dbl>, RFull <dbl>,
## #   MultipleBirthIfSameSex <int>, IsMz <int>, LastSurvey_S1 <int>,
## #   LastSurvey_S2 <int>, RImplicitPass1 <dbl>, RImplicit <dbl>,
## #   RExplicit <dbl>, RExplicitPass1 <dbl>, RPass1 <dbl>,
## #   RExplicitOlderSibVersion <dbl>, RExplicitYoungerSibVersion <dbl>,
## #   RImplicitSubject <dbl>, RImplicitMother <dbl>
```

```r
rm(columns_to_write)
```

```r
# If there's no PHI, a rectangular CSV is usually adequate, and it's portable to other machines and software.
readr::write_csv(ds_slim, path_out_current)

readr::write_csv(ds_archive, config$links_97_archive)
```

The R session information (including the OS info, R version and all
packages used):


```r
sessionInfo()
```

```
## R version 3.4.3 Patched (2018-01-25 r74163)
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
## [1] ggplot2_2.2.1.9000 xtable_1.8-2       knitr_1.19        
## [4] bindrcpp_0.2       DBI_0.7            magrittr_1.5      
## 
## loaded via a namespace (and not attached):
##  [1] Rcpp_0.12.15          highr_0.6             plyr_1.8.4           
##  [4] pillar_1.1.0          compiler_3.4.3        bindr_0.1            
##  [7] tools_3.4.3           odbc_1.1.5            digest_0.6.15        
## [10] bit_1.1-12            gtable_0.2.0          memoise_1.1.0        
## [13] evaluate_0.10.1       tibble_1.4.2          checkmate_1.8.5      
## [16] pkgconfig_2.0.1       rlang_0.1.6.9003      cli_1.0.0            
## [19] rstudioapi_0.7        yaml_2.1.16           withr_2.1.1.9000     
## [22] dplyr_0.7.4.9000      stringr_1.2.0         devtools_1.13.4      
## [25] hms_0.4.1             grid_3.4.3            bit64_0.9-7          
## [28] rprojroot_1.3-2       tidyselect_0.2.3      OuhscMunge_0.1.8.9006
## [31] glue_1.2.0            R6_2.2.2              rmarkdown_1.8        
## [34] tidyr_0.8.0           readr_1.1.1           purrr_0.2.4          
## [37] blob_1.1.0            RODBC_1.3-15          scales_0.5.0.9000    
## [40] backports_1.1.2       htmltools_0.3.6       rsconnect_0.8.5      
## [43] assertthat_0.2.0      testit_0.7.1          colorspace_1.3-2     
## [46] labeling_0.3          config_0.2            utf8_1.1.3           
## [49] stringi_1.1.6         lazyeval_0.2.1        munsell_0.4.3        
## [52] markdown_0.8          crayon_1.3.4
```

```r
Sys.time()
```

```
## [1] "2018-02-14 13:08:55 CST"
```

