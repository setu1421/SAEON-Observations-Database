-- Stations
Select Code StationCode, Name StationName, Elevation from Station where (Code like 'ELW_%') and (Elevation > 0)
-- Station_Instrument
Select
  Station.Code StationCode, Station.Name StationName, Instrument.Code InstrumentCode, Instrument.Name InstrumentName, Station_Instrument.Elevation
from 
  Station_Instrument 
  inner join Station
    on (Station_Instrument.StationID = Station.ID)
  inner join Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
where 
  (Station.Code like 'ELW_%') and (Station_Instrument.Elevation > 0)
-- Instrument_Sensor
Select
  Instrument.Code InstrumentCode, Instrument.Name InstrumentName, Sensor.Code SensorCode, Sensor.Name SensorName, Instrument_Sensor.Elevation
from
  Instrument_Sensor
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
  inner join Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
where 
  (Instrument.Code like 'ELW_%') and (Instrument_Sensor.Elevation > 0)
-- Sensor
Select Code InstrumentCode, Name InstrumentName, Elevation from Instrument where (Code like 'ELW_%') and (Elevation > 0)
-- Observations
Select
  StationCode, StationName, Observation.Elevation
from
  Observation 
  inner join vObservationExpansion
    on (Observation.ID = vObservationExpansion.ID)
where
  (StationCode like 'ELW_%') and (Observation.Elevation > 0)