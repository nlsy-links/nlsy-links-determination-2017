dsRaw2 <- sqldf::read.csv.sql(
  file        = "data-public/derived/links-archive-2017-97.csv",
  sql         = "SELECT * FROM file WHERE AlgorithmVersion IN (2, 3)",
  eol         = "\n"#,
  # colClasses  = col_types
)

ds_raw_1a <- dsRaw %>%
  tibble::as_tibble() %>%
  dplyr::select(AlgorithmVersion, SubjectTag_S1, SubjectTag_S2, RRoster) %>%
  dplyr::mutate(
    RRoster    = as.character(RRoster)
  )

ds_raw_2a <- dsRaw2 %>%
  tibble::as_tibble() %>%
  dplyr::select(AlgorithmVersion, SubjectTag_S1, SubjectTag_S2, RRoster) %>%
  dplyr::mutate(
    # RRoster    = as.numeric(RRoster)
  )
table(ds_raw_2a$RRoster, useNA = "always")
table(ds_raw_2a$RRoster)


0 0.25  0.5 <NA>
  1658  252 3128    0
ds_raw_2a %>%
  dplyr::anti_join(ds_raw_1a)
