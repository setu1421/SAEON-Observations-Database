use ObservationsElwandle
Update Organisation set DigitalObjectIdentifierID = null
Update Programme set DigitalObjectIdentifierID = null
Update Project set DigitalObjectIdentifierID = null
Update Site set DigitalObjectIdentifierID = null
Update Station set DigitalObjectIdentifierID = null
Delete UserDownloads
Delete DigitalObjectIdentifiers
DBCC CheckIdent('DigitalObjectIdentifiers',Reseed, 0)
