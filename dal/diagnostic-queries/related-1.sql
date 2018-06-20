--use [NlsyLinks97]

WITH cte AS (
	SELECT -- TOP (1000)
		v.[ID]
		,rs.[ExtendedID]
		,rs.SubjectTag_S1
		,rs.SubjectTag_S2
		,v.[MultipleBirthIfSameSex]
		,v.[IsMz]
		,v.[LastSurvey_S1]
		,v.[LastSurvey_S2]
		,v.[ImplicitShareBiomomPass1]
		,v.[ImplicitShareBiodadPass1]
		,v.[ExplicitShareBiomomPass1]
		,v.[ExplicitShareBiodadPass1]
		,v.[ShareBiomomPass1]
		,v.[ShareBiodadPass1]
		--,v.[RImplicitPass1]
		--,v.[RImplicit]
		--,v.[RImplicitSubject]
		--,v.[RImplicitMother]
		--,v.[RExplicitOlderSibVersion]
		--,v.[RExplicitYoungerSibVersion]
		--,v.[RExplicitPass1]
		--,v.[RExplicit]
		,rt.R                AS RRoster
		,v.[RPass1]
		,v.[R]
		,v.[RFull]
		--,[RPeek]
		--,rt.ResponseLower
		--,rt.ResponseUpper
		,lur1.Label           AS response_lower
		,lur2.Label           AS response_upper
	--  delete
	FROM [Process].[tblRelatedValues] v
	  left join [Process].[tblRelatedStructure] rs   ON v.ID=rs.id
	  left join [Process].[tblRoster]           rt   ON v.ID=rt.RelatedID
	  left join [Enum].[tblLURoster]            lur1 ON rt.ResponseLower = lur1.id
	  left join [Enum].[tblLURoster]            lur2 ON rt.ResponseUpper = lur2.id
	--where RFull is null
	--where rt.ResponseLower=83
	--order by rs.[ExtendedID]	,rs.SubjectTag_S1	,rs.SubjectTag_S2

)

--SELECT * FROM cte order by ExtendedID, SubjectTag_S1, SubjectTag_S2
--,

SELECT 
  COUNT(*)      AS count,
  RRoster,     
  RPass1,
  RFull,
  R,
  response_lower      AS roster_response_lower,
  response_upper      AS roster_response_upper
FROM cte 
GROUP BY RRoster, RPass1, R, RFull, response_lower, response_upper
order by count(*) DESC, RRoster, RPass1, R, RFull, response_lower, response_upper
--order by RRoster, RPass1, R, RFull, response_older, response_younger
