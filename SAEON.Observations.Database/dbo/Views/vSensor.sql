--> Changed 2.0.3 20160503 TimPN
--Renamed SensorProcedure to Sensor
--< Changed 2.0.3 20160503 TimPN
--> Removed 2.0.17 20161128 TimPN
--CREATE VIEW [dbo].[vSensor]
--AS
--SELECT 
--s.ID,
--s.Code,
--s.Name,
--s.[Description],
--s.Url,
--s.StationID,
--s.DataSourceID,
--s.PhenomenonID,
----> Added 2.0.7 20160609 TimPN
--  [Phenomenon].Name PhenomenonName,
----< Added 2.0.7 20160609 TimPN
--s.UserId,
--st.Name AS StationName,
--d.Name AS DataSourceName,
----> Added 20161110 TimPN
--s.DataSchemaID,
----< Added 20161110 TimPN
----> Changed 20161110 TimPN
----ds.[Description] DataSchemaName
--ds.[Name] DataSchemaName
----< Changed 20161110 TimPN
--FROM Sensor s
--INNER JOIN DataSource d
--    on s.DataSourceID = d.ID
--INNER JOIN Station st
--    on s.StationID = st.ID
--LEFT JOIN DataSchema ds
--    on s.DataSchemaID = ds.ID
----> Added 2.0.7 20160609 TimPN
--  inner join [Phenomenon]
--    on (s.PhenomenonID = [Phenomenon].ID)
----< Added 2.0.7 20160609 TimPN
--< Removed 2.0.17 20161128 TimPN
--> Added 2.0.17 20161128 TimPN
CREATE VIEW [dbo].[vSensor]
AS
SELECT 
  Sensor.ID,
  Sensor.Code,
  Sensor.Name,
  Sensor.[Description],
  Sensor.Url,
  Sensor.DataSourceID,
  Sensor.PhenomenonID,
  [Phenomenon].Name PhenomenonName,
  Sensor.UserId,
  Site.Name Site,
  Station.Name Station,
  Instrument.Name Instrument,
  d.Name AS DataSourceName,
  Sensor.DataSchemaID,
  ds.[Name] DataSchemaName
FROM 
  Sensor 
--> Changed 20170123 TimPN
--  inner join Instrument_Sensor
  left join Instrument_Sensor
--< Changed 20170123 TimPN
	on (Instrument_Sensor.SensorID = Sensor.ID)
--> Changed 20170123 TimPN
--  inner join Instrument
  left join Instrument
--< Changed 20170123 TimPN
	on (Instrument_Sensor.InstrumentID = Instrument.ID)
--> Changed 20170123 TimPN
--  inner join Station_Instrument
  left join Station_Instrument
--< Changed 20170123 TimPN
    on (Station_Instrument.InstrumentID = Instrument.ID)
--> Changed 20170123 TimPN
--  inner join Station 
  left join Station 
--< Changed 20170123 TimPN
    on (Station_Instrument.StationID = Station.ID)
--> Changed 20170123 TimPN
--  inner join Site
  left join Site
--< Changed 20170123 TimPN
    on (Station.SiteID = Site.ID)
INNER JOIN DataSource d
    on Sensor.DataSourceID = d.ID
LEFT JOIN DataSchema ds
    on Sensor.DataSchemaID = ds.ID
  inner join [Phenomenon]
    on (Sensor.PhenomenonID = [Phenomenon].ID)
--< Added 2.0.17 20161128 TimPN



