--use ObservationsElwandle
--Update Organisation set DigitalObjectIdentifierID = null where DigitalObjectIdentifierID is not null
--Update Programme set DigitalObjectIdentifierID = null where DigitalObjectIdentifierID is not null
--Update Project set DigitalObjectIdentifierID = null where DigitalObjectIdentifierID is not null
--Update Site set DigitalObjectIdentifierID = null where DigitalObjectIdentifierID is not null
--Update Station set DigitalObjectIdentifierID = null where DigitalObjectIdentifierID is not null
--update ImportBatchSummary set DigitalObjectIdentifierID = null where DigitalObjectIdentifierID is not null
Delete UserDownloads
Delete DigitalObjectIdentifiers
DBCC CheckIdent('DigitalObjectIdentifiers',Reseed, 0)
