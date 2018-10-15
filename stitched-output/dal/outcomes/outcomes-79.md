



This report was automatically generated with the R package **knitr**
(version 1.20).


```r
# knitr::stitch_rmd(script="./dal/outcomes/outcomes-79.R", output="./stitched-output/dal/outcomes/outcomes-79.md") # dir.create("./stitched-output/dal/outcomes/", recursive=T)
rm(list=ls(all=TRUE))  #Clear the variables from previous runs.
```

```r
source("./utility/connectivity.R")
```

```r
# Attach these package(s) so their functions don't need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
library(magrittr            , quietly=TRUE)

# Verify these packages are available on the machine, but their functions need to be qualified: http://r-pkgs.had.co.nz/namespace.html#search-path
requireNamespace("readr"        )
requireNamespace("tidyr"        )
requireNamespace("dplyr"        ) # Avoid attaching dplyr, b/c its function names conflict with a lot of packages (esp base, stats, and plyr).
requireNamespace("testit"       ) # For asserting conditions meet expected patterns.
requireNamespace("checkmate"    ) # For asserting conditions meet expected patterns. # devtools::install_github("mllg/checkmate")
requireNamespace("DBI"          )
requireNamespace("odbc"         )
requireNamespace("config"       )
requireNamespace("OuhscMunge"   ) # devtools::install_github(repo="OuhscBbmc/OuhscMunge")
```

```r
# Constant values that won't change.
study       <- "79"
item_labels <- "
    'Gen1HeightInches', 'Gen1WeightPounds', 'Gen1AfqtScaled3Decimals',
    'Gen2HeightInchesTotal', 'Gen2HeightFeetOnly', 'Gen2HeightInchesRemainder', 'Gen2HeightInchesTotalMotherSupplement',
    'Gen2WeightPoundsYA', 'Gen2PiatMathPercentile', 'Gen2PiatMathStandard',
    'Gen2CFatherAlive'
  "


sql_response <-   glue::glue("
    SELECT --TOP (1000)
    	r.SubjectTag       AS subject_tag
    	--,r.ExtendedID      AS extended_id
    	--,r.Generation      AS generation
    	,r.SurveyYear      AS survey_year
    	--,r.Item            AS item_id
    	,i.Label           AS item_label
    	,r.Value           AS value
    	--,r.LoopIndex       AS loop_index
    FROM Process.tblResponse r
    	LEFT JOIN Metadata.tblItem i ON r.Item=i.ID
    WHERE i.Label IN (
      {item_labels} -- This is replaced by `glue::glue()`
    )
  ")

sql_survey_time <- "
  SELECT -- TOP (100000)
    t.SubjectTag          AS subject_tag
    ,t.SurveyYear         AS survey_year
    ,s.Generation         AS generation
    ,ROUND(COALESCE(t.AgeCalculateYears, t.AgeSelfReportYears),1)     AS age
  FROM Process.tblSurveyTime t
    LEFT JOIN Process.tblSubject s      ON t.SubjectTag = s.SubjectTag
  ORDER BY t.SubjectTag, t.SurveyYear
"
sql_subject_generation <- "
  SELECT
    SubjectTag    AS subject_tag,
    Generation    AS generation
  FROM Process.tblSubject
"

sql_algorithm_version_max <- "SELECT MAX(AlgorithmVersion) as version FROM Archive.tblRelatedValuesArchive"
path_out_subject_csv_raw                <- config::get("outcomes-79-subject-csv")
path_out_subject_rds                    <- config::get("outcomes-79-subject-rds")
path_out_subject_survey_csv_raw         <- config::get("outcomes-79-subject-survey-csv")
path_out_subject_survey_rds             <- config::get("outcomes-79-subject-survey-rds")
```

```r
channel                   <- open_dsn_channel_odbc(study)
system.time({
ds_response               <- DBI::dbGetQuery(channel, sql_response    )
})
```

```
##    user  system elapsed 
##    0.41    0.00  238.03
```

```r
ds_survey_time            <- DBI::dbGetQuery(channel, sql_survey_time)
ds_subject_generation     <- DBI::dbGetQuery(channel, sql_subject_generation)
ds_algorithm_version      <- DBI::dbGetQuery(channel, sql_algorithm_version_max)
DBI::dbDisconnect(channel); rm(channel, sql_response, sql_survey_time, sql_subject_generation, sql_algorithm_version_max)
```

```r
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call ato `dplyr::rename()`.
dim(ds_survey_time)
```

```
## [1] 629382      4
```

```r
dim(ds_response)
```

```
## [1] 282120      4
```

```r
dim(ds_subject_generation)
```

```
## [1] 24207     2
```

```r
ds_algorithm_version
```

```
##   version
## 1      89
```

```r
path_out_subject_csv        <- sprintf(path_out_subject_csv_raw         , ds_algorithm_version$version)
path_out_subject_survey_csv <- sprintf(path_out_subject_survey_csv_raw  , ds_algorithm_version$version)

ds_survey_time <- ds_survey_time %>%
  tibble::as_tibble()

ds_subject_generation <- ds_subject_generation %>%
  tibble::as_tibble()

ds_response <- ds_response %>%
  tibble::as_tibble() %>%
  dplyr::mutate(
    item_label          = OuhscMunge::snake_case(item_label),
    value               = dplyr::if_else(item_label=="gen1_afqt_scaled3_decimals", value / 1000, as.numeric(value)), # If the conversion is here, then the other variables must be double-precision floating points.
    item_label          = dplyr::recode(item_label, "gen1_afqt_scaled3_decimals"="afqt_scaled_gen1") # If the conversion is here, then the other variables must be double-precision floating points.
  )

# table(ds_outcome$item_label)
```

```r
ds_subject <- ds_response %>%
  dplyr::group_by(subject_tag, item_label) %>%
  dplyr::summarize(
    value  = OuhscMunge::first_nonmissing(value)
  ) %>%
  dplyr::ungroup() %>%
  tidyr::spread(key=item_label, value=value) %>%
  dplyr::left_join(ds_subject_generation, by="subject_tag") %>%
  dplyr::select(
    subject_tag,
    generation,
    height_inches   = gen1_height_inches,
    weight_pounds   = gen1_weight_pounds,
    afqt_scaled_gen1,
    father_alive    = gen2_c_father_alive
  )
```

```r
ds_subject_survey <- ds_response %>%
  tidyr::spread(key=item_label, value=value) %>%
  dplyr::left_join(ds_survey_time, by=c("subject_tag", "survey_year")) %>%
  dplyr::mutate(
    # afqt_scaled_gen1   = gen1_afqt_scaled3_decimals / 1000
  ) %>%
  dplyr::select(
    subject_tag,
    survey_year,
    generation,
    age,
    height_inches   = gen1_height_inches,
    weight_pounds   = gen1_weight_pounds,
    afqt_scaled_gen1,
    father_alive    = gen2_c_father_alive
  )

ds_subject_survey
```

```
## # A tibble: 121,293 x 8
##    subject_tag survey_year generation   age height_inches weight_pounds afqt_scaled_gen1 father_alive
##          <int>       <int>      <int> <dbl>         <dbl>         <dbl>            <dbl>        <dbl>
##  1         200        1981          1  22.1            NA           120             6.84           NA
##  2         200        1982          1  23.1            62           125            NA              NA
##  3         200        1985          1  26.1            62           118            NA              NA
##  4         201        1998          2   5.2            NA            NA            NA              NA
##  5         201        2000          2   7.2            NA            NA            NA              NA
##  6         201        2002          2   9.3            NA            NA            NA              NA
##  7         201        2004          2  11.3            NA            NA            NA              NA
##  8         202        2000          2   5.6            NA            NA            NA              NA
##  9         202        2002          2   7.6            NA            NA            NA              NA
## 10         202        2004          2   9.6            NA            NA            NA              NA
## # ... with 121,283 more rows
```

```r
# Sniff out problems
summary(ds_subject)
```

```
##   subject_tag        generation    height_inches   weight_pounds   afqt_scaled_gen1  father_alive   
##  Min.   :    200   Min.   :1.000   Min.   :48.00   Min.   : 53.0   Min.   :  0.00   Min.   :-3.000  
##  1st Qu.: 304800   1st Qu.:1.000   1st Qu.:64.00   1st Qu.:125.0   1st Qu.: 16.77   1st Qu.: 1.000  
##  Median : 603302   Median :1.000   Median :67.00   Median :140.0   Median : 38.62   Median : 1.000  
##  Mean   : 606985   Mean   :1.448   Mean   :67.08   Mean   :145.6   Mean   : 42.40   Mean   : 0.854  
##  3rd Qu.: 896501   3rd Qu.:2.000   3rd Qu.:70.00   3rd Qu.:165.0   3rd Qu.: 66.29   3rd Qu.: 1.000  
##  Max.   :1268600   Max.   :2.000   Max.   :83.00   Max.   :375.0   Max.   :100.00   Max.   : 1.000  
##                                    NA's   :10361   NA's   :10234   NA's   :10819    NA's   :15600
```

```r
checkmate::assert_integer(ds_subject$subject_tag          , lower=         100L   , upper=1268600L, any.missing=F, unique=F)
checkmate::assert_integer(ds_subject$generation           , lower=           1L   , upper=      2L, any.missing=F)
checkmate::assert_numeric(ds_subject$height_inches        , lower=          48L   , upper=     83L, any.missing=T)
checkmate::assert_numeric(ds_subject$weight_pounds        , lower=          47L   , upper=    400L, any.missing=T)
checkmate::assert_numeric(ds_subject$afqt_scaled_gen1     , lower=           0    , upper=    100 , any.missing=T)
checkmate::assert_numeric(ds_subject$father_alive         , lower=          -3L   , upper=      1L, any.missing=T)
```

```r
# Sniff out problems
summary(ds_subject_survey)
```

```
##   subject_tag       survey_year     generation         age        height_inches   weight_pounds   afqt_scaled_gen1
##  Min.   :    200   Min.   :1981   Min.   :1.000   Min.   : 0.0    Min.   :48.00   Min.   : 47.0   Min.   :  0.00  
##  1st Qu.: 286900   1st Qu.:1985   1st Qu.:1.000   1st Qu.:11.6    1st Qu.:64.00   1st Qu.:125.0   1st Qu.: 16.77  
##  Median : 579900   Median :1994   Median :2.000   Median :18.3    Median :67.00   Median :145.0   Median : 38.62  
##  Mean   : 577652   Mean   :1994   Mean   :1.708   Mean   :17.2    Mean   :67.08   Mean   :148.5   Mean   : 42.40  
##  3rd Qu.: 841600   3rd Qu.:2004   3rd Qu.:2.000   3rd Qu.:22.5    3rd Qu.:70.00   3rd Qu.:165.0   3rd Qu.: 66.29  
##  Max.   :1268600   Max.   :2014   Max.   :2.000   Max.   :41.5    Max.   :83.00   Max.   :375.0   Max.   :100.00  
##                                                   NA's   :10596   NA's   :98338   NA's   :86240   NA's   :109379  
##   father_alive  
##  Min.   :-3.00  
##  1st Qu.: 1.00  
##  Median : 1.00  
##  Mean   : 0.82  
##  3rd Qu.: 1.00  
##  Max.   : 1.00  
##  NA's   :80175
```

```r
checkmate::assert_integer(ds_subject_survey$subject_tag          , lower=         100L   , upper=1268600L, any.missing=F, unique=F)
checkmate::assert_integer(ds_subject_survey$survey_year          , lower=        1979L   , upper=   2016L, any.missing=F, unique=F)
checkmate::assert_integer(ds_subject_survey$generation           , lower=           1L   , upper=      2L, any.missing=F)
checkmate::assert_numeric(ds_subject_survey$age                  , lower=           0    , upper=     40 , any.missing=T)
```

```
## Error in knitr::stitch_rmd(script = "./dal/outcomes/outcomes-79.R", output = "./stitched-output/dal/outcomes/outcomes-79.md"): Assertion on 'ds_subject_survey$age' failed: All elements must be <= 40.
```

```r
checkmate::assert_numeric(ds_subject_survey$height_inches        , lower=          48L   , upper=     83L, any.missing=T)
checkmate::assert_numeric(ds_subject_survey$weight_pounds        , lower=          47L   , upper=    400L, any.missing=T)
checkmate::assert_numeric(ds_subject_survey$afqt_scaled_gen1     , lower=           0    , upper=    100 , any.missing=T)
checkmate::assert_numeric(ds_subject_survey$father_alive         , lower=          -3L   , upper=      1L, any.missing=T)
```

```r
# dput(colnames(ds_subject_survey)) # Print colnames for line below.
columns_to_write <- c(
  "subject_tag", "generation",
  "height_inches", "weight_pounds", "afqt_scaled_gen1",
  "father_alive"
)
ds_slim_subject <- ds_subject %>%
  # dplyr::slice(1:100) %>%
  dplyr::select(!!columns_to_write)
ds_slim_subject
```

```
## # A tibble: 22,733 x 6
##    subject_tag generation height_inches weight_pounds afqt_scaled_gen1 father_alive
##          <int>      <int>         <dbl>         <dbl>            <dbl>        <dbl>
##  1         200          1            62           120             6.84           NA
##  2         201          2            NA            NA            NA              NA
##  3         202          2            NA            NA            NA              NA
##  4         300          1            70           160            49.4            NA
##  5         301          2            NA            NA            NA              NA
##  6         302          2            NA            NA            NA              NA
##  7         303          2            NA            NA            NA              NA
##  8         400          1            67           110            55.8            NA
##  9         401          2            NA            NA            NA               1
## 10         500          1            63           130            96.8            NA
## # ... with 22,723 more rows
```

```r
rm(columns_to_write)
```

```r
# dput(colnames(ds_subject_survey)) # Print colnames for line below.
columns_to_write <- c(
  "subject_tag", "survey_year", "generation", "age",
  "height_inches", "weight_pounds", "afqt_scaled_gen1",
  "father_alive"
)
ds_slim_subject_survey <- ds_subject_survey %>%
  # dplyr::slice(1:100) %>%
  dplyr::select(!!columns_to_write)
ds_slim_subject_survey
```

```
## # A tibble: 121,293 x 8
##    subject_tag survey_year generation   age height_inches weight_pounds afqt_scaled_gen1 father_alive
##          <int>       <int>      <int> <dbl>         <dbl>         <dbl>            <dbl>        <dbl>
##  1         200        1981          1  22.1            NA           120             6.84           NA
##  2         200        1982          1  23.1            62           125            NA              NA
##  3         200        1985          1  26.1            62           118            NA              NA
##  4         201        1998          2   5.2            NA            NA            NA              NA
##  5         201        2000          2   7.2            NA            NA            NA              NA
##  6         201        2002          2   9.3            NA            NA            NA              NA
##  7         201        2004          2  11.3            NA            NA            NA              NA
##  8         202        2000          2   5.6            NA            NA            NA              NA
##  9         202        2002          2   7.6            NA            NA            NA              NA
## 10         202        2004          2   9.6            NA            NA            NA              NA
## # ... with 121,283 more rows
```

```r
rm(columns_to_write)
```

```r
# The content of these two datasets are identical.  The first one is a plain-text csv.  The second one is a native R object.
readr::write_csv(ds_slim_subject, path_out_subject_csv)
readr::write_rds(ds_slim_subject, path_out_subject_rds, compress="xz")

# The content of these two datasets are identical.  The first one is a plain-text csv.  The second one is a native R object.
readr::write_csv(ds_slim_subject_survey, path_out_subject_survey_csv)
readr::write_rds(ds_slim_subject_survey, path_out_subject_survey_rds, compress="xz")
```

The R session information (including the OS info, R version and all
packages used):


```r
sessionInfo()
```

```
## R version 3.5.1 Patched (2018-09-10 r75281)
## Platform: x86_64-w64-mingw32/x64 (64-bit)
## Running under: Windows >= 8 x64 (build 9200)
## 
## Matrix products: default
## 
## locale:
## [1] LC_COLLATE=English_United States.1252  LC_CTYPE=English_United States.1252    LC_MONETARY=English_United States.1252
## [4] LC_NUMERIC=C                           LC_TIME=English_United States.1252    
## 
## attached base packages:
## [1] stats     graphics  grDevices utils     datasets  methods   base     
## 
## other attached packages:
## [1] knitr_1.20     bindrcpp_0.2.2 magrittr_1.5  
## 
## loaded via a namespace (and not attached):
##  [1] Rcpp_0.12.19          highr_0.7             pillar_1.3.0          compiler_3.5.1        bindr_0.1.1          
##  [6] tools_3.5.1           odbc_1.1.6            packrat_0.4.9-3       digest_0.6.18         bit_1.1-14           
## [11] memoise_1.1.0         evaluate_0.12         tibble_1.4.2          checkmate_1.8.5       pkgconfig_2.0.2      
## [16] rlang_0.2.2           rstudioapi_0.8        DBI_1.0.0             cli_1.0.1             yaml_2.2.0           
## [21] withr_2.1.2           dplyr_0.7.6           stringr_1.3.1         devtools_1.13.6       hms_0.4.2.9001       
## [26] rprojroot_1.3-2       bit64_0.9-7           tidyselect_0.2.5      glue_1.3.0            OuhscMunge_0.1.9.9009
## [31] R6_2.3.0              fansi_0.4.0           rmarkdown_1.10        tidyr_0.8.1           readr_1.2.0          
## [36] purrr_0.2.5           blob_1.1.1            scales_1.0.0          backports_1.1.2       RODBC_1.3-15         
## [41] htmltools_0.3.6       assertthat_0.2.0      testit_0.8.1          colorspace_1.3-2      config_0.3           
## [46] utf8_1.1.4            stringi_1.2.4         munsell_0.5.0         markdown_0.8          crayon_1.3.4
```

```r
Sys.time()
```

```
## [1] "2018-10-14 18:26:08 CDT"
```

