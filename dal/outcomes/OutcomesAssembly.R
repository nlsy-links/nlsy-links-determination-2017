library(RODBC)
requireNamespace("plyr")
# library(lubridate)
rm(list=ls(all=TRUE))

channel <- RODBC::odbcDriverConnect("driver={SQL Server}; Server=Bee\\Bass; Database=NlsLinks; Uid=NlsyReadWrite; Pwd=nophi")
algorithmVersion <- max(sqlQuery(channel, "SELECT MAX(AlgorithmVersion) as AlgorithmVersion  FROM [NlsLinks].[Process].[tblRelatedValuesArchive]"))
odbcClose(channel)

pathInputGen1 <- "./ForDistribution/Outcomes/OutcomesGen1.csv"
pathInputGen2 <- "./ForDistribution/Outcomes/OutcomesGen2.csv"
pathOutput <-  sprintf("./ForDistribution/Outcomes/ExtraOutcomes79V%d.csv", algorithmVersion)

dsGen1 <- read.csv(pathInputGen1, stringsAsFactors=FALSE)
dsGen2 <- read.csv(pathInputGen2, stringsAsFactors=FALSE)

ds <- plyr::rbind.fill(dsGen1, dsGen2)

length(ds$HeightZGenderAge)

write.csv(ds, pathOutput, row.names=F)
