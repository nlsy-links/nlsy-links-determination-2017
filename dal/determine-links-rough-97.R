#This next line is run when the whole file is executed, but not when knitr calls individual chunks.
rm(list=ls(all=TRUE)) #Clear the memory for any variables set from any previous runs.

# http://nlsinfo.org/content/cohorts/nlsy97/using-and-understanding-the-data/types-variables-raw-symbols-rosters-created
# http://nlsinfo.org/content/cohorts/nlsy97/topical-guide/household/household-composition
# http://nlsinfo.org/content/cohorts/nlsy97/topical-guide/household/household-composition/page/0/1

# ---- load-sources ------------------------------------------------------------
# source("./manipulation/osdh/ellis/common-ellis.R")


# ---- load-packages -----------------------------------------------------------
library(magrittr                , quietly=TRUE)
library(sparklyr                , quietly=TRUE)   # spark_install ("2.2.0")
requireNamespace("readr"                      )
requireNamespace("dplyr"                      )
requireNamespace("testit"                     )
requireNamespace("OuhscMunge"                 )   # devtools::install_github("OuhscBbmc/OuhscMunge")

# ---- declare-globals ---------------------------------------------------------
path_97_explicit        <- "data-unshared/raw/nlsy97/97-explicit.csv"
# path_97_links         <- "data-unshared/raw/nlsy97/97-links.csv"

# path_lu_cancellation_categorization    <- "data-public/raw/eto/visit-cancellation-category.csv"

col_types_default <- readr::cols(
  .default    = readr::col_integer()
)

# ---- load-data ---------------------------------------------------------------
# Retrieve location of csv to transfer.
sc <- spark_connect (master = "local")
# ds_import_explicit             <- readr::read_csv(path_97_explicit, col_types=col_types_default)

d <- spark_read_csv(sc, name = "dsp", path_97_explicit)

tidy_iris <- tbl(sc,"dsp") %>%
  # select(R0000100, R0536300) %>%
  mutate(
    R      = R0536300 + 1000L
  ) %>%
  dplyr::union_all(., .) %>%
  dplyr::union_all(., .) %>%
  dplyr::union_all(., .)

returned <- tidy_iris %>%
  collect()

spark_disconnect(sc)

# readr::spec_csv(path_lu_cancellation_categorization)
ds_lu_cancellation_categorization  <- readr::read_csv(path_lu_cancellation_categorization, col_types=col_types_cancellation_category)

rm(uri_referral_03, uri_referral_04, path_lu_cancellation_categorization)
rm(col_types_03, col_types_04, col_types_cancellation_category)

# ---- tweak-data --------------------------------------------------------------
# OuhscMunge::column_rename_headstart(ds_03)


# ----- groom-plumbing ----------------------------------------------------------
ds_03 <- ds_03 %>%
  dplyr::select_(
    "response_id_03"                   = "`Response ID_208`"
    , "case_number"                    = "`CaseNumber_9446`"
    , "date_taken"                     = "`Date Taken_208`"
    , "program_code"                   = "`Completing Program Unique Identifier_208`"

    , "staff_name_attributed"          = "`Attributed Staff Name_208`"
    , "staff_name_updated"             = "`Last Updated By_208`"
  ) %>%
  tidyr::drop_na(response_id_03, case_number, program_code, date_taken) %>%
  dplyr::mutate(
    date_taken                         = dplyr::if_else(dplyr::between(date_taken, range_visit_date[1], range_visit_date[2]), date_taken, as.Date(NA_character_)),

    worker_name                       = attribute_worker_name(staff_name_attributed, staff_name_updated)
  ) %>%
  tidyr::drop_na(date_taken) %>%   # Evict records w/ a bad/missing visit date.
  dplyr::select(
    -staff_name_attributed, -staff_name_updated
  ) %>%
  dplyr::arrange(response_id_03)

checkmate::assert_integer(ds_03$response_id_03, lower=1, any.missing=F, unique=T)

# ---- join-program ------------------------------------------------------------
ds_03 <- ds_03 %>%
  dplyr::left_join(
    ds_program %>%
      dplyr::select(program_code, model_id, program_name)
    , by="program_code"
  ) %>%
  dplyr::filter(model_id %in% desired_model_ids)


sum(is.na(ds_03$program_code))
table(ds_03[is.na(ds_03$program_code), "program_name"])
if( any(is.na(ds_03$program_name)) ) {
  ds_03 <- ds_03 %>%
    tidyr::drop_na(program_name)
  warning("Records were dropped that weren't correctly matched to an ETO Program Code.")
}
ds_unmatched_program <- ds_03[is.na(ds_03$program_name), ]

checkmate::assert_integer(ds_03$program_code, lower=1, any.missing=F, unique=F)


# ----- groom-visit-info ----------------------------------------------------------
ds_04 <- ds_04 %>%
  dplyr::rename_(
    "response_id_04"                        = "`Response ID_208`"
    , "schedule_type"                       = "`Schedule Type_9413`"
    , "encounter_type"                      = "`Type of encounter:_9396`"
    , "visit_duration_in_minutes"           = "`Duration of visit_9243`"
    , "visit_distance"                      = "`Total Miles_9404`"
    , "time_frame"                          = "`Time Frame_9395`"
    , "people_present"                      = "`People present:_9247`"
    , "visit_location"                      = "`Location of visit:_9245`"
    , "content_covered_percent"             = "`Percent of planned content covered_9360`"
    , "client_involvement"                  = "`Involvement (Client)_9342`"
    , "client_material_conflict"            = "`Conflict w/ Material (Client)_9352`"
    , "client_material_understanding"       = "`Understanding of Material (Client)_9353`"
    , "injury_education"                    = "`Was parental education provided regarding prevention of child injuries?_9361`"
    , "brain_builder_video_shown"           = "`Was brain builder video shown at the visit?_20126`"
    , "completed_how_is_it_going"           = "`Was 'How Is It Going Between Us' completed?_20094`"
    , "pcg_openess"                         = "`PCG Openess:_20116`"
    # , "cancellation_reason_long"          = "`Cancellation Reason:_19171`"
    , "cancellation_reason_short"           = "`Cancellation Reason:_20082`"
    , "pcg_engagement"                      = "`PCG Engagement:_19789`"
  )  %>%
  tidyr::drop_na(response_id_04) %>%
  dplyr::mutate(
    schedule_type                     = dplyr::coalesce(schedule_type, "Unknown"),
    encounter_type                    = tolower(stringr::str_trim(encounter_type)),
    encounter_type                    = base::iconv(x=encounter_type, from="latin1", to="ASCII//TRANSLIT", sub="?"),
    time_frame                        = dplyr::coalesce(time_frame, "Unknown"),

    visit_distance                    = OuhscMunge::deterge_to_integer(visit_distance),
    visit_distance                    = dplyr::if_else(dplyr::between(visit_distance, 0, 150), visit_distance, NA_integer_),
    # visit_duration_in_minutes         = OuhscMunge::deterge_to_integer(visit_duration_in_minutes),
    visit_duration_in_minutes         = dplyr::if_else(dplyr::between(visit_duration_in_minutes, 0, 255), visit_duration_in_minutes, NA_integer_),
    people_present_count              = nchar(gsub("[^|]", "", people_present)),
    visit_location_home               = (visit_location=="Client's Home"),

    # content_covered_percent           = OuhscMunge::deterge_to_integer(content_covered_percent),
    content_covered_percent           = dplyr::if_else(dplyr::between(content_covered_percent, 0, 100), content_covered_percent, NA_integer_),
    client_involvement                = as.integer(sub("^(\\d).+$", "\\1", client_involvement)),
    client_material_conflict          = as.integer(sub("^(\\d).+$", "\\1", client_material_conflict)),
    client_material_understanding     = as.integer(sub("^(\\d).+$", "\\1", client_material_understanding)),

    injury_education                  = dplyr::recode(injury_education          , `Yes`=TRUE, `No`=FALSE, .missing=NA),
    brain_builder_video_shown         = dplyr::recode(brain_builder_video_shown , `Yes`=TRUE, `No`=FALSE, .missing=NA),
    completed_how_is_it_going         = dplyr::recode(completed_how_is_it_going , `Yes`=TRUE, `No`=FALSE, .missing=NA),
    # cancellation_reason_long        = substr(cancellation_reason_long, 1, 255),
    pcg_engagement                    = as.integer(dplyr::recode(pcg_engagement , `1 - low`="1", `5 - high`="5", .missing=NA_character_)),
    pcg_openess                       = as.integer(dplyr::recode(pcg_openess    , `1 - low`="1", `5 - high`="5", .missing=NA_character_))
  ) %>%
  dplyr::mutate(
    encounter_type = dplyr::recode(
      encounter_type,
      "alternative encounter"                   = "alternative encounter"
      , "attempted check-in"                    = "attempted check-in"
      , "attempted home visit"                  = "attempted home visit"
      , "attempted supervisory home visit"      = "attempted supervisory home visit"
      , "attempted supervisory homeA visit"     = "attempted supervisory home visit"
      , "completed check-in"                    = "completed check-in"
      , "completed home visit"                  = "completed home visit"
      , "completedA home visit"                 = "completed home visit"
      , "completed supervisory home visit"      = "completed supervisory home visit"
      , "home visit canceled by client/family"  = "home visit canceled by client/family"
      , "home visit canceled by home visitor"   = "home visit canceled by home visitor"
      , "home visit canceled by interpreter"    = "home visit canceled by interpreter"
      , .missing                                = "unknown"
      , .default                                = "unclassified"
    )
  ) %>%
  dplyr::mutate(
    completed = dplyr::recode(
      encounter_type
      , "alternative encounter"                 = FALSE
      , "attempted check-in"                    = FALSE
      , "attempted home visit"                  = FALSE
      , "attempted supervisory home visit"      = FALSE
      , "completed check-in"                    = FALSE
      , "completed home visit"                  = TRUE
      , "completed supervisory home visit"      = TRUE
      , "home visit canceled by client/family"  = FALSE
      , "home visit canceled by home visitor"   = FALSE
      , "home visit canceled by interpreter"    = FALSE
      , "unknown"                               = FALSE
    )
  ) %>%
  dplyr::left_join(ds_lu_cancellation_categorization, by="cancellation_reason_short") %>%
  dplyr::arrange(response_id_04)

# table(ds_04$encounter_type, useNA="always")
# table(ds_04$completed, useNA="always")
# # table(ds_04$cancellation_reason_long, useNA="always")
# table(ds_04$cancellation_reason_short, useNA="always")

checkmate::assert_integer(ds_04$response_id_04, lower=1, any.missing=F, unique=T)


# ---- join-strips -------------------------------------------------------------
# library(data.table)
# ds_03 <- data.table::as.data.table(ds_03)
# ds_04 <- data.table::as.data.table(ds_04)
# data.table::setkey(ds_03, response_id)
# data.table::setkey(ds_04, response_id)
# ds <- ds_03[ds_04, on="response_id"]
system.time({
  ds <- "
  SELECT ds_03.*, ds_04.*
  FROM ds_03
  LEFT JOIN ds_04 ON ds_03.response_id_03=ds_04.response_id_04
  ORDER BY ds_03.case_number, ds_03.program_code, ds_03.date_taken
  " %>%
    sqldf::sqldf() %>%
    tibble::rowid_to_column("encounter_id")
})

# format(object.size(ds), units="Mb")


# ---- old-munging -------------------------------------------------------------
# (people_present_count <- unname(sapply(ds$people_present, function(x) length(unlist(strsplit(x, "|", fixed=T))))))

# dsEtoVisitConflictingcase_numbers <- ds %>%
#   dplyr::group_by(parent_phocis_id) %>%
#   dplyr::summarize(
#     CountOfUniquecase_numbers = dplyr::n_distinct(case_number),
#     case_numbers              = paste(unique(case_number), collapse="; ")
#   ) %>%
#   dplyr::filter(CountOfUniquecase_numbers>1)
#
# readr::write_csv(dsEtoVisitConflictingcase_numbers, "dsEtoVisitConflictingcase_numbers.csv")

# ---- verify-values -----------------------------------------------------------
checkmate::assert_integer(  ds$encounter_id   , lower=1, any.missing=F, unique=T)
checkmate::assert_integer(  ds$case_number    , lower=10000, any.missing=F)
checkmate::assert_integer(  ds$program_code   , lower=700  , upper=900, any.missing=F)
checkmate::assert_date(     ds$date_taken     , lower=range_visit_date[1], upper=range_visit_date[2], any.missing=F)
checkmate::assert_integer(  ds$model_id       , lower=1  , upper=5, any.missing=F)
checkmate::assert_character(ds$worker_name    , min.chars=2, any.missing=F) # Max is 40 in DB

checkmate::assert_logical(  ds$completed                      , any.missing=F)
checkmate::assert_character(ds$schedule_type    , min.chars= 7, any.missing=F)
checkmate::assert_character(ds$encounter_type   , min.chars= 7, any.missing=F)
checkmate::assert_character(ds$time_frame       , min.chars= 5, any.missing=T)

checkmate::assert_integer(  ds$visit_duration_in_minutes         , lower=0  , upper=255, any.missing=T)
checkmate::assert_integer(  ds$visit_distance                    , lower=0  , upper=255, any.missing=T)
checkmate::assert_logical(  ds$visit_location_home                                     , any.missing=T)
checkmate::assert_integer(  ds$people_present_count              , lower=0  , upper= 10, any.missing=T)
checkmate::assert_integer(  ds$content_covered_percent           , lower=0  , upper=100, any.missing=T)
checkmate::assert_integer(  ds$client_involvement                , lower=0  , upper=  5, any.missing=T)
checkmate::assert_integer(  ds$client_material_conflict          , lower=0  , upper=  5, any.missing=T)
checkmate::assert_integer(  ds$client_material_understanding     , lower=0  , upper=  5, any.missing=T)

checkmate::assert_logical(  ds$is_cancellation_valid                   , any.missing=T)
checkmate::assert_character(ds$cancellation_reason_tidy    , min.chars= 7, any.missing=F)

checkmate::assert_integer(programs_to_drop, lower=1, any.missing = F)


# sum(duplicated(paste(ds$case_number, ds$date_taken)))
# colnames(ds_slim)

# ---- specify-columns-to-upload -----------------------------------------------
# dput(colnames(ds))
columns_to_write <- c(
  "encounter_id", "case_number", "program_code", "date_taken", "model_id", "worker_name",
  "completed",
  "schedule_type", "encounter_type", "time_frame",
  "visit_duration_in_minutes", "visit_distance",
  "visit_location_home", #"visit_location",
  "people_present_count",  #"people_present",
  "cancellation_reason_short", # "cancellation_reason_long",
  "is_cancellation_valid",
  "client_involvement", "client_material_conflict", "client_material_understanding",
  "content_covered_percent",
  "pcg_engagement", "pcg_openess",
  "injury_education", "completed_how_is_it_going", "brain_builder_video_shown"
  #"program_name"
)
setdiff(colnames(ds), columns_to_write)

ds_slim <- ds %>%
  dplyr::select_(.dots=columns_to_write) %>%
  # dplyr::slice(1:1000) %>%
  dplyr::mutate(
    completed                     = as.integer(completed),
    visit_location_home           = as.integer(visit_location_home),
    is_cancellation_valid         = as.integer(is_cancellation_valid),
    injury_education              = as.integer(injury_education),
    completed_how_is_it_going     = as.integer(completed_how_is_it_going),
    brain_builder_video_shown     = as.integer(brain_builder_video_shown)
  )

# purrr::map_int(ds_slim, ~sum(is.na(.)))
# purrr::map_int(ds_slim, ~max(nchar(.), na.rm=T))

# ---- upload-to-db ------------------------------------------------------------
OuhscMunge::upload_sqls_rodbc(
  d             = ds_slim,
  table_name    = "osdh.tbl_eto_touchpoint_encounter",
  dsn_name      = "MiechvEvaluation",
  clear_table   = TRUE,
  create_table  = FALSE
) # 9.042 minutes 2017-09-02

spark_disconnect(sc)
