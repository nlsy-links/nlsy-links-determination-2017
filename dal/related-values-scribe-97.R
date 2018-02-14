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

# ---- load-sources ------------------------------------------------------------
source("./utility/connectivity.R")

# ---- load-packages -----------------------------------------------------------
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

# ---- declare-globals ---------------------------------------------------------
# Constant values that won't change.

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
sql_description <- "
  SELECT MAX(AlgorithmVersion) as AlgorithmVersion
  FROM Archive.tblRelatedValuesArchive
"

# ---- load-data ---------------------------------------------------------------
channel            <- open_dsn_channel_odbc(study = "97")
# DBI::dbGetInfo(channel)
ds               <- DBI::dbGetQuery(channel, sql)
ds_description   <- DBI::dbGetQuery(channel, sql_description)
DBI::dbDisconnect(channel, sql, sql_description)

# ---- tweak-data --------------------------------------------------------------
# OuhscMunge::column_rename_headstart(ds_county) #Spit out columns to help write call ato `dplyr::rename()`.
path_out <- sprintf("data-public/derived/links-2017-v%i.csv", ds_description$AlgorithmVersion)


ds <- ds %>%
  tibble::as_tibble() %>%
  dplyr::mutate(
    RExplicit                   = NA_real_,
    RExplicitPass1              = NA_real_,
    RExplicitOlderSibVersion    = NA_real_,
    RExplicitYoungerSibVersion  = NA_real_

  )

# ---- verify-values -----------------------------------------------------------
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

# ---- specify-columns-to-upload -----------------------------------------------
# dput(colnames(ds)) # Print colnames for line below.
columns_to_write <- c(
  "ExtendedID", "SubjectTag_S1", "SubjectTag_S2", "SubjectID_S1",
  "SubjectID_S2", "RelationshipPath", "EverSharedHouse", "R", "RFull",
  "MultipleBirthIfSameSex", "IsMz", "LastSurvey_S1", "LastSurvey_S2",
  "RImplicitPass1", "RImplicit", "RExplicit", "RExplicitPass1",
  "RPass1", "RExplicitOlderSibVersion", "RExplicitYoungerSibVersion",
  "RImplicitSubject", "RImplicitMother"
)
ds_slim <- ds %>%
  # dplyr::slice(1:100) %>%
  dplyr::select_(.dots=columns_to_write)
ds_slim

rm(columns_to_write)

# ---- save-to-disk ------------------------------------------------------------
# If there's no PHI, a rectangular CSV is usually adequate, and it's portable to other machines and software.
readr::write_csv(ds, path_out)
