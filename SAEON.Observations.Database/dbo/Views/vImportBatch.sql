CREATE VIEW [dbo].[vImportBatch]
AS

SELECT 
b.ID,
b.Code,
b.DataSourceID,
b.ImportDate, 
b.[Status],
d.Name DataSourceName,
b.UserId,
u.UserName,
CASE b.[Status]
	WHEN 0 THEN 'Errors in Datalog'
	WHEN 1 THEN 'No Errors in Log'
	WHEN 2 THEN 'Moved to Datalog'
END StatusDescription,
b.[FileName],
b.LogFileName,
b.Issues

FROM ImportBatch b
INNER JOIN DataSource d
	on b.DataSourceID = d.ID
INNER JOIN aspnet_Users u
  on b.UserId = u.UserId




