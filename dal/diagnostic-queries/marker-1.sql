SELECT TOP (1000)
	 m.[ID]
	,m.[ExtendedID]
	,rs.SubjectTag_S1
	,rs.SubjectTag_S2
	,m.[RelatedID]
	,m.[MarkerType]
	,m.[SurveyYear]
	,m.[MzEvidence]
	,m.[SameGeneration]
	,m.[ShareBiomomEvidence]
	,m.[ShareBiodadEvidence]
	,m.[ShareBioGrandparentEvidence]
	  -- delete
FROM [NlsyLinks97].[Process].[tblMarker] m
  left join [NlsyLinks97].[Process].[tblRelatedStructure] rs on m.RelatedID=rs.id
order by m.[ExtendedID]	,rs.SubjectTag_S1	,rs.SubjectTag_S2
