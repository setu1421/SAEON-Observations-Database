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
  left join Instrument_Sensor
	on (Instrument_Sensor.SensorID = Sensor.ID)
  left join Instrument
	on (Instrument_Sensor.InstrumentID = Instrument.ID)
  left join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
  left join Station 
    on (Station_Instrument.StationID = Station.ID)
  left join Site
    on (Station.SiteID = Site.ID)
INNER JOIN DataSource d
    on Sensor.DataSourceID = d.ID
LEFT JOIN DataSchema ds
    on Sensor.DataSchemaID = ds.ID
  inner join [Phenomenon]
    on (Sensor.PhenomenonID = [Phenomenon].ID)




