﻿--CREATE VIEW [dbo].[vUserDownloads]
--AS 
--SELECT 
--  UserDownloads.*, DOI, DOIUrl
--FROM 
--  UserDownloads
--  inner join DigitalObjectIdentifiers
--    on (UserDownloads.DigitalObjectIdentifierID = DigitalObjectIdentifiers.ID)
