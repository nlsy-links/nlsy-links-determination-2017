# This is a hack of https://github.com/OuhscBbmc/miechv-3/blob/master/manipulation/osdh/osdh-flow.R
#   That file runs everything (those dozens of files) dynamically.
#   This one is hard-coded, and requires one manual stop (to run the C#).
#   But since there are so few files, I think it's an acceptable compromise.

rm(list=ls(all=TRUE)) #Clear the memory for any variables set from any previous runs.

# ---- load-sources ------------------------------------------------------------


# ---- load-packages -----------------------------------------------------------
library(magrittr)
requireNamespace("testit")

# ---- declare-globals ---------------------------------------------------------
path_sources <- c(
  # "dal/outcomes/outcomes-97.R",
  "dal/import-97-metadata.R",
  "dal/import-97-raw.R",
  "analysis/eda/counts/counts.Rmd"
)

file.exists(path_sources)
all_sources_exist <- path_sources %>%
  purrr::map_lgl(file.exists) %>%
  all()
if( !all_sources_exist ) stop("All source files to be run should exist.")


# ---- load-data ---------------------------------------------------------------

# ---- tweak-data --------------------------------------------------------------

# ---- run-sources -------------------------------------------------------------

message("Preparing to run\n\t", paste(path_sources, collapse="\n\t"))

(start_time <- Sys.time())


# dir.create(output="./stitched-output/dal/", recursive=T)
knitr::stitch_rmd(script="./dal/import-97-metadata.R", output="./stitched-output/dal/import-97-metadata.md")
knitr::stitch_rmd(script="./dal/import-97-raw.R", output="./stitched-output/dal/import-97-raw.md")
rmarkdown::render("analysis/eda/counts/counts.Rmd")                                                               # Watch out, this file is actually knitted twice (see below).

stop("Now run the C# program, then come back to run the rest of the R scripts.")

# knitr::stitch_rmd(script="./dal/outcomes/outcomes-97.R", output="./stitched-output/dal/outcomes/outcomes-97.md") # dir.create("./stitched-output/dal/outcomes/", recursive=T)

rmarkdown::render("analysis/eda/counts/counts.Rmd")                                                               # Watch out, this file is actually knitted twice (see above).
knitr::stitch_rmd(script="./dal/related-values-scribe-97.R", output="./stitched-output/dal/related-values-scribe-97.md")
rmarkdown::render("analysis/archive-comparison-97/archive-comparison-97.Rmd")
base::closeAllConnections() # Check back with https://stackoverflow.com/questions/50937423/closing-unused-connection-after-sqldfread-csv-sql

elapsed_duration <- (Sys.time() - start_time)
message(sprintf("Completed flow-97 at %s (in %0.2f mins.)", start_time, elapsed_duration))

# ---- verify-values -----------------------------------------------------------
