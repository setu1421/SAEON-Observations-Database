CREATE VIEW [dbo].[vSensorProcedure]
AS

SELECT 
s.ID,
s.Code,
s.Name,
s.[Description],
s.Url,
s.StationID,
s.DataSourceID,
s.PhenomenonID,
s.UserId,
st.Name AS StationName,
d.Name AS DataSourceName,
s.DataSchemaID,
ds.[Description] DataSchemaName
FROM SensorProcedure s
INNER JOIN DataSource d
	on s.DataSourceID = d.ID
INNER JOIN Station st
	on s.StationID = st.ID
LEFT JOIN DataSchema ds
	on s.DataSchemaID = ds.ID


