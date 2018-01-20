SELECT 
	--[ID]
	 r.[ExtendedID]
	,r.[SubjectTag]
	,s1.hh_internal_id
	,r.[SurveyYear]
	,r.[Item]
	,i.Label
	,r.[LoopIndex1]
	,r.[LoopIndex2]
	,r.[Value]
	,d1.Mob as mob1
	,s1.Gender as g1
	--delete
FROM [NlsyLinks97].[Process].[tblResponse] r
    left join NlsyLinks97.Metadata.tblItem i on r.Item = i.Id
	left join [NlsyLinks97].Process.tblSubject s1 on r.SubjectTag=s1.SubjectTag
	left join [NlsyLinks97].Process.tblSubjectDetails d1 on r.SubjectTag=d1.SubjectTag
where 
  -- ExtendedID = 9
  ----ExtendedID=6523
  --AND 
  --item IN (3, 105, 102) --104, 105, 106
  --item IN (3, 104, 106) --104, 105, 106
  r.item IN (121, 122, 123)

order by r.ExtendedID, r.SubjectTag, r.SurveyYear, r.item, r.LoopIndex1, r.LoopIndex2
