use Observations;
Declare @Code VarChar(100) = '%-991889'
Select * from Instrument where code like @Code
Delete
  Instrument_Sensor
from 
  Instrument_Sensor
  inner join Instrument
    on (Instrument_Sensor.InstrumentID = Instrument.ID)
where
  (Code like @Code)
Delete Instrument where (Code like @Code)