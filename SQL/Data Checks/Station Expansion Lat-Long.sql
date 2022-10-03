use Observations
declare @Latitude float = -33.1240
Select
  Station.Code, Station.Latitude Station, Station_Instrument.Latitude Station_Instrument, Instrument.Latitude Instrument,
  Instrument_Sensor.Latitude Instrument_Sensor, Sensor.Latitude Sensor
from
  Sensor
  inner join Instrument_Sensor
    on (Sensor.ID = Instrument_Sensor.SensorID) 
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID) 
  inner join Station_Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID) 
  inner join Station
    on (Station_Instrument.StationID = Station.ID) 
where
  (Sensor.Latitude = @Latitude) or
  (Instrument_Sensor.Latitude = @Latitude) or
  (Instrument.Latitude = @Latitude) or
  (Station_Instrument.Latitude = @Latitude) or
  (Station.Latitude = @Latitude)
