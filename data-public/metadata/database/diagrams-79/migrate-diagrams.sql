--Requires a "Linked server": https://docs.microsoft.com/en-us/sql/relational-databases/linked-servers/linked-servers-database-engine
--Code from https://stackoverflow.com/questions/527327/how-do-you-migrate-sql-server-database-diagrams-to-another-database

SELECT [name]
      ,[principal_id]
      ,[diagram_id]
      ,[version]
      ,[definition]
  FROM [BEE\BASS].[NlsLinks].[dbo].[sysdiagrams]




USE NlsLinks

DELETE sysDiagrams
WHERE name IN (
	SELECT name FROM [BEE\BASS].[NlsLinks].[dbo].[sysdiagrams]
	)

SET IDENTITY_INSERT sysDiagrams ON

INSERT sysDiagrams (
	name,
	principal_id,
	diagram_id,
	version,
	definition
)
SELECT
	name,
	principal_id,
	diagram_id,
	version,
	definition
FROM [BEE\BASS].[NlsLinks].[dbo].[sysdiagrams]

SET IDENTITY_INSERT sysDiagrams OFF
