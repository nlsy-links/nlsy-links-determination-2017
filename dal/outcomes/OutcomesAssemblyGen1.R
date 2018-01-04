library(RODBC)
library(plyr)
library(lubridate)
rm(list=ls(all=TRUE))

generation <- 1
pathInputHeight <- "./ForDistribution/Outcomes/Gen1Height/Gen1Height.csv"
pathInputWeight <- "./ForDistribution/Outcomes/Gen1Weight/Gen1Weight.csv"
pathInputIQ <- "./ForDistribution/Outcomes/Gen1IQ/Gen1IQ.csv"
pathInputAfi <- "./OutsideData/AfiAmen2012-09-20/AfiAfm.csv"

pathOutput <- "./ForDistribution/Outcomes/OutcomesGen1.csv"

channel <- RODBC::odbcDriverConnect("driver={SQL Server}; Server=Bee\\Bass; Database=NlsLinks; Uid=NlsyReadWrite; Pwd=nophi")
odbcGetInfo(channel)
ds <- sqlQuery(channel, paste0("SELECT SubjectTag, SubjectID, Generation FROM Process.tblSubject WHERE Generation=", generation))
odbcClose(channel)


### Merge Height
dsHeight <- read.csv(pathInputHeight, stringsAsFactors=F)
dsHeight <- dsHeight[, c("SubjectTag", "ZGenderAge")] #"ZGender", 
dsHeight <- plyr::rename(dsHeight, replace=c("ZGenderAge"="HeightZGenderAge")) #"ZGender"="HeightZGender",
ds <- merge(x=ds, y=dsHeight, by="SubjectTag", all.x=TRUE)
rm(dsHeight)

### Merge Weight
dsWeight <- read.csv(pathInputWeight, stringsAsFactors=F)
dsWeight <- dsWeight[, c("SubjectTag", "ZGenderAge")] #"ZGender", 
dsWeight <- plyr::rename(dsWeight, replace=c("ZGenderAge"="WeightZGenderAge")) #"ZGender"="WeightZGender", 
ds <- merge(x=ds, y=dsWeight, by="SubjectTag", all.x=TRUE)
rm(dsWeight)

### Merge ASFT IQ
dsIQ <- read.csv(pathInputIQ, stringsAsFactors=F)
dsIQ <- dsIQ[, c("SubjectTag", "ZGenderAge")]
#dsIQ <- plyr::rename(dsIQ, replace=c("ZGenderAge"="IQZGenderAge")) 
dsIQ <- plyr::rename(dsIQ, replace=c("ZGenderAge"="AfqtRescaled2006Gaussified")) 
ds <- merge(x=ds, y=dsIQ, by="SubjectTag", all.x=TRUE)
rm(dsIQ)

### Merge Amen and Afi
dsAfi <- read.csv(pathInputAfi, stringsAsFactors=F)
dsAfi$SubjectTag <- dsAfi$ID*100
dsAfi <- dsAfi[, c("SubjectTag", "Afi", "Afm")]
dsAfi$Afi <- as.integer(ifelse(dsAfi$Afi=='.', NA_integer_, dsAfi$Afi))
dsAfi$Afm <- as.integer(ifelse(dsAfi$Afm=='.', NA_integer_, dsAfi$Afm))
# dsAfi <- plyr::rename(dsAfi, replace=c("ZGenderAge"="IQZGenderAge")) 
ds <- merge(x=ds, y=dsAfi, by="SubjectTag", all.x=TRUE)
rm(dsAfi)


#########################################
### Graph the outcomes
######################################### 
HistogramWithCurve <- function( scores, title="", breaks=30) {
  hist(scores, breaks=breaks, freq=F, main=title)
  curve(dnorm(x, mean=mean(scores, na.rm=T),  sd=sd(scores, na.rm=T)), add=T)  
}
par(mar=c(2,2,2,0), mgp=c(1,0,0), tcl=0)

HistogramWithCurve(ds$HeightZGenderAge, "HeightZGenderAge")
HistogramWithCurve(ds$WeightZGenderAge, "WeightZGenderAge")
# HistogramWithCurve(ds$IQZGenderAge, "IQZGenderAge")
HistogramWithCurve(ds$AfqtRescaled2006Gaussified, "AfqtRescaled2006Gaussified")
HistogramWithCurve(ds$Afi, "Afi")
HistogramWithCurve(ds$Afm, "Afm")

write.csv(ds, pathOutput, row.names=F)
