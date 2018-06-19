use [NlsyLinks97]

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
	,v.[RPass1]
	,v.[R]
	,v.[RFull]
	--,[RPeek]
	--,rt.ResponseLower
	--,rt.ResponseUpper
	,lur1.Label           AS response_older
	,lur2.Label           AS response_younger
--  delete
FROM [Process].[tblRelatedValues] v
  left join [Process].[tblRelatedStructure] rs   ON v.ID=rs.id
  left join [Process].[tblRoster]           rt   ON v.ID=rt.RelatedID
  left join [Enum].[tblLURoster]            lur1 ON rt.ResponseLower = lur1.id
  left join [Enum].[tblLURoster]            lur2 ON rt.ResponseUpper = lur2.id
order by rs.[ExtendedID]	,rs.SubjectTag_S1	,rs.SubjectTag_S2
