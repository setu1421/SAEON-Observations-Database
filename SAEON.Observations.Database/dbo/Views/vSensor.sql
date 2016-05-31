--> Changed 2.0.3 20160503 TimPN
--Renamed SensorProcedure to Sensor
--< Changed 2.0.3 20160503 TimPN
CREATE VIEW [dbo].[vSensor]
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
ds.[Description] DataSchemaName,
--> Added 2.0.4 20160508 TimPN
i.Name as InstrumentName
--< Added 2.0.4 20160508 TimPN
FROM Sensor s
INNER JOIN DataSource d
	on s.DataSourceID = d.ID
INNER JOIN Station st
	on s.StationID = st.ID
LEFT JOIN DataSchema ds
	on s.DataSchemaID = ds.ID
--> Added 2.0.4 20160508 TimPN
LEFT Join Instrument i
  on (s.InstrumentID = i.ID)
--< Added 2.0.4 20160508 TimPN


