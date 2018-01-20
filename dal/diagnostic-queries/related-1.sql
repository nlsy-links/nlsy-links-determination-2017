SELECT TOP (1000)
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
--  delete
FROM [NlsyLinks97].[Process].[tblRelatedValues] v
  left join [NlsyLinks97].[Process].[tblRelatedStructure] rs on v.ID=rs.id
order by rs.[ExtendedID]	,rs.SubjectTag_S1	,rs.SubjectTag_S2
