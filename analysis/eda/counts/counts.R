rm(list=ls(all=TRUE)) #Clear the memory of variables from previous run. This is not called by knitr, because it's above the first chunk.

# ---- load-sources ------------------------------------------------------------
#Load any source files that contain/define functions, but that don't load any other types of variables
#   into memory.  Avoid side effects and don't pollute the global environment.
source("./utility/connectivity.R")

# ---- load-packages -----------------------------------------------------------
library(magrittr) #Pipes
# library(ggplot2) #For graphing
requireNamespace("RODBC")
requireNamespace("dplyr")
requireNamespace("scales") #For formating values in graphs
requireNamespace("knitr") #For the kable function for tables

# ---- declare-globals ---------------------------------------------------------
options(show.signif.stars=F) #Turn off the annotations on p-values


# ---- load-data ---------------------------------------------------------------
ds <- database_inventory()

# ---- tweak-data --------------------------------------------------------------
ds_pretty <- ds %>%
  dplyr::mutate(
    row_count       = scales::comma(row_count),
    column_count    = scales::comma(column_count)
  )

# ---- table ---------------------------------------------------------------
ds_pretty %>%
  knitr::kable(
    col.names   = gsub("_", " ", colnames(.)),
    align       = "llrr",
    digits      = 2,
    format      = "markdown"
  )

