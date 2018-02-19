USE [NlsyLinks97]
SELECT 
	--[ID]
	 r.[ExtendedID]
	,r.[SubjectTag] as SubjectTag_S1
	,rs.SubjectTag_S2
	,s1.hh_internal_id      AS hh_internal_id_s1
	,rs.hh_internal_id_s2
	,r.[SurveyYear]
	,r.[Item]
	,i.Label
	,r.[LoopIndex1]
	,r.[LoopIndex2]
	,r.[Value]
	,d1.Mob as mob1
	,s1.Gender as g1
	--delete
FROM Process.tblResponse r
    left join Metadata.tblItem i on r.Item = i.Id
	left join Process.tblSubject s1 on r.SubjectTag=s1.SubjectTag
	left join Process.tblSubjectDetails d1 on r.SubjectTag=d1.SubjectTag
	left join [Process].[tblRelatedStructure] rs ON (r.SubjectTag=rs.SubjectTag_S1) AND (r.LoopIndex1=rs.hh_internal_id_s1) AND (r.LoopIndex2=rs.hh_internal_id_s2)
where 
  -- ExtendedID = 9
  ----ExtendedID=6523
  --AND 
  --item IN (3, 105, 102) --104, 105, 106
  item IN (3, 104) --104, 105, 106
  --r.item IN (123, 124)
  --r.item=122

  --AND (hh_internal_id=LoopIndex1 or hh_internal_id=LoopIndex2)
  AND
  ----r.value IN (15, 16, 17, 18, 19, 20)  -- Get just the half-siblings
  r.value IN (82, 83) 

  --AND
  --rs.SubjectTag_S2 IS NOT NULL
  --(rs.SubjectTag_S1=6229 or rs.SubjectTag_S2=6229 or rs.SubjectTag_S1=6230 or rs.SubjectTag_S2=6230)
order by r.ExtendedID, r.SubjectTag, r.SurveyYear, r.item, r.LoopIndex1, r.LoopIndex2
