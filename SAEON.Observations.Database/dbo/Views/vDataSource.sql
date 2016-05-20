CREATE View [dbo].[vDataSource]
AS
Select 
  d.*,
  t.[Name] DataSchemaName
from 
  DataSource d
  LEFT JOIN DataSchema t  -- Must be inner join once all datasources have stations
    ON d.DataSchemaID = t.ID


