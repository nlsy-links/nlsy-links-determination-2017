Use NlsLinks

-- Temporarily turn off the autonumber on the primary keys

INSERT INTO NlsLinks.Archive.tblRelatedValuesArchive
SELECT * FROM [BEE\BASS].NlsLinks.Archive.tblRelatedValuesArchive

INSERT INTO NlsLinks.Archive.tblArchiveDescription
SELECT * FROM [BEE\BASS].NlsLinks.Archive.tblArchiveDescription
