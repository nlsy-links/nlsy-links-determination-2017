library(RODBC)
library(plyr)
library(lubridate)
rm(list=ls(all=TRUE))

generation <- 2
pathInputHeight <- "./ForDistribution/Outcomes/Gen2Height/Gen2Height.csv"
pathInputWeight <- "./ForDistribution/Outcomes/Gen2Weight/Gen2Weight.csv"
pathInputMath <- "./ForDistribution/Outcomes/Gen2Math/Gen2Math.csv"
# pathInputMath <- "./OutsideData/KellyHeightWeightMath2012-03-09/ExtraOutcomes79FromKelly2012March.csv"
pathOutput <- "./ForDistribution/Outcomes/OutcomesGen2.csv"

channel <- RODBC::odbcDriverConnect("driver={SQL Server}; Server=Bee\\Bass; Database=NlsLinks; Uid=NlsyReadWrite; Pwd=nophi")
odbcGetInfo(channel)
ds <- sqlQuery(channel, paste0("SELECT SubjectTag, SubjectID, Generation FROM Process.tblSubject WHERE Generation=", generation))
odbcClose(channel)

### Merge Height
dsHeight <- read.csv(pathInputHeight, stringsAsFactors=F) 
dsHeight <- dsHeight[, c("SubjectTag", "ZGenderAge")] #, "HeightZGender"
dsHeight <- plyr::rename(dsHeight, replace=c("ZGenderAge"="HeightZGenderAge")) #"ZGender"="HeightZGender",
ds <- merge(x=ds, y=dsHeight, by="SubjectTag", all.x=TRUE)
rm(dsHeight)

### Merge Weight
dsWeight <- read.csv(pathInputHeight, stringsAsFactors=F) 
dsWeight <- dsWeight[, c("SubjectTag", "ZGenderAge")] 
dsWeight <- plyr::rename(dsWeight, replace=c("ZGenderAge"="WeightZGenderAge"))
ds <- merge(x=ds, y=dsWeight, by="SubjectTag", all.x=TRUE)
rm(dsWeight)

### Merge Math
dsMath <- read.csv(pathInputMath, stringsAsFactors=F)
# dsMath <- dsMath[, c("SubjectTag", "ZGenderAge")]
# dsMath <- plyr::rename(dsMath, replace=c("ZGenderAge"="MathZGenderAge"))
dsMath <- dsMath[, c("SubjectTag", "Score")]
dsMath <- plyr::rename(dsMath, replace=c("Score"="MathStandardized"))
# dsMath <- plyr::rename(dsMath, replace=c("Score"="MathStandard"))
# dsMath <- plyr::rename(dsMath, replace=c("Score"="MathGaussified"))
ds <- merge(x=ds, y=dsMath, by="SubjectTag", all.x=TRUE)
rm(dsMath)



HistogramWithCurve <- function( scores, title="", breaks=30) {
  hist(scores, breaks=breaks, freq=F, main=title)
  curve(dnorm(x, mean=mean(scores, na.rm=T),  sd=sd(scores, na.rm=T)), add=T)  
}
par(mar=c(2,2,2,0), mgp=c(1,0,0), tcl=0)

HistogramWithCurve(ds$HeightZGenderAge, "HeightZGenderAge")
HistogramWithCurve(ds$WeightZGenderAge, "WeightZGenderAge")
# HistogramWithCurve(ds$HeightZGender, "HeightZGender")
# HistogramWithCurve(ds$MathZGenderAge, "MathZGenderAge")
HistogramWithCurve(ds$MathStandard, "MathStandard")
# HistogramWithCurve(ds$MathGaussified, "MathGaussified")

write.csv(ds, pathOutput, row.names=F)
