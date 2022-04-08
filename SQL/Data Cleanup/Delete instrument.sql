use Observations;
Declare @Code VarChar(100) = '%-126929'
Select * from Instrument where code like @Code
Delete
  Station_Instrument
from
  Station_Instrument 
  inner join Instrument
    on (Station_Instrument.InstrumentID = Instrument.ID)
where
  (Code like @Code)
Delete
  Instrument_Sensor
from 
  Instrument_Sensor
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
where
  (Code like @Code)
Delete Instrument where (Code like @Code)