SELECT
	r.[ResponseLower]
	,r.[ResponseUpper]
	,COUNT(r.RelatedID) AS Freq
	,lu1.Label AS ResponseLowerLabel
	,lu2.Label AS ResponseUpperLabel
FROM [NlsyLinks97].[Process].[tblRoster] r
	left join enum.tblLURoster lu1 on r.ResponseLower=lu1.ID
	left join enum.tblLURoster lu2 on r.ResponseUpper=lu2.ID

GROUP BY [ResponseLower], [ResponseUpper], lu1.Label, lu2.Label
ORDER BY [ResponseLower], [ResponseUpper], lu1.Label, lu2.Label
