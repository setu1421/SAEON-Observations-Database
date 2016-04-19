CREATE View [dbo].[vDataSource]
AS
Select 
  d.*,
  t.[Name] DataSchemaName,
  s.Code + ' - ' + s.Name StationName
from 
  DataSource d
  inner join Station s
    on (d.StationID = s.ID)
  LEFT JOIN DataSchema t  -- Must be inner join once all datasources have stations
    ON d.DataSchemaID = t.ID


