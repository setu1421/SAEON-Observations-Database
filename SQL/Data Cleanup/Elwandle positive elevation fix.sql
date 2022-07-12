use Observations
-- Station
Update
  Station
set
  Elevation = -Elevation 
from 
  Station
where 
  (Code like 'ELW_%') and (Elevation > 0)
-- Station_Instrument
Update
  Station_Instrument
set
  Elevation = -Station_Instrument.Elevation 
from 
  Station_Instrument 
  inner join Station
    on (Station_Instrument.StationID = Station.ID)
where 
  (Code like 'ELW_%') and (Station_Instrument.Elevation > 0)
-- Instrument
Update
  Instrument
set
  Elevation = -Elevation 
from 
  Instrument
where 
  (Code like 'ELW_%') and (Elevation > 0)
-- Instrument_Sensor
Update
  Instrument_Sensor
set
  Elevation = -Instrument_Sensor.Elevation
from
  Instrument_Sensor
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
  inner join Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
where 
  (Instrument.Code like 'ELW_%') and (Instrument_Sensor.Elevation > 0)
-- Sensor
Update 
  Sensor
set
  Elevation = -Elevation
where
  (Code like 'ELW_%') and (Elevation > 0)
-- Observations
update
  Observation
set
  Elevation = -Observation.Elevation
from
  Observation 
  inner join vObservationExpansion
    on (Observation.ID = vObservationExpansion.ID)
where
  (StationCode like 'ELW_%') and (Observation.Elevation > 0)