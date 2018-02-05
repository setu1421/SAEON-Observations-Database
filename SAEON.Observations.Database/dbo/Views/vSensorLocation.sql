--> Added 2.0.37 20180202 TimPN
CREATE VIEW [dbo].[vSensorLocation]
AS
Select
  Sensor.ID SensorID,
  Coalesce(Sensor.Latitude, Instrument_Sensor.Latitude, Instrument.Latitude, Station_Instrument.Latitude, Station.Latitude) Latitude,
  Coalesce(Sensor.Longitude, Instrument_Sensor.Longitude, Instrument.Longitude, Station_Instrument.Longitude, Station.Longitude) Longitude,
  Coalesce(Sensor.Elevation, Instrument_Sensor.Elevation, Instrument.Elevation, Station_Instrument.Elevation, Station.Elevation) Elevation
from
  Station
  inner join Station_Instrument
    on (Station_Instrument.StationID = Station.ID)
  inner join Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
  inner join Instrument_Sensor
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
  inner join Sensor 
    on (Instrument_Sensor.SensorID = Sensor.ID)
--< Added 2.0.37 20180202 TimPN
