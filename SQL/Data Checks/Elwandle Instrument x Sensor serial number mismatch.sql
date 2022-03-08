use Observations
-- 6 Digit
Select
  Instrument.Code, Sensor.Code
from 
  Instrument
  inner join Instrument_Sensor
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
  inner join Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
where
  (Instrument.Code like 'ELW%') and
  (Sensor.Code not like '%'+Right(Instrument.Code,6)+'%')
-- 7 Digit
Select
  Instrument.Code, Sensor.Code
from 
  Instrument
  inner join Instrument_Sensor
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
  inner join Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
where
  (Instrument.Code like 'ELW%') and
  (Sensor.Code not like '%'+Right(Instrument.Code,7)+'%')-- 8 Digit
Select
  Instrument.Code, Sensor.Code
from 
  Instrument
  inner join Instrument_Sensor
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
  inner join Sensor
    on (Instrument_Sensor.SensorID = Sensor.ID)
where
  (Instrument.Code like 'ELW%') and
  (Sensor.Code not like '%'+Right(Instrument.Code,8)+'%')