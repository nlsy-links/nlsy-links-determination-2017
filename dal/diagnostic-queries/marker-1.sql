SELECT TOP (1000)
	--m.[ID]
	m.[ExtendedID]
	,rs.SubjectTag_S1 as tag_s1
	,rs.SubjectTag_S2 as tag_s2
	--,m.[RelatedID]
	--,r.RosterAssignmentID
	,lu1.Label as roster_lower
	,lu2.Label as roster_upper
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
  left join NlsyLinks97.Process.tblRoster r on m.RelatedID=r.RelatedID
  left join NlsyLinks97.Enum.tblLURoster lu1 on r.ResponseLower=lu1.ID 
  left join NlsyLinks97.Enum.tblLURoster lu2 on r.ResponseUpper=lu2.ID 
order by m.[ExtendedID]	,rs.SubjectTag_S1	,rs.SubjectTag_S2
