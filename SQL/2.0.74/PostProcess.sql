Update
  Station_Instrument
set
  EndDate = DateAdd(Second,-1,DateAdd(Day,1,EndDate))
where
  (EndDate is not null) and
  (DatePart(Hour,EndDate) = 0)  and
  (DatePart(Minute,EndDate) = 0)  and
  (DatePart(Second,EndDate) = 0)

Declare @Code VarChar(100) = '%-126929'
Select * from Instrument where code like @Code
Select * from Sensor where code like @Code+'_EOV-SEATEMP-SUB'
Select * from DataSource where code like @Code
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
Delete Sensor where (Code like @Code+'_EOV-SEATEMP-SUB')
Delete DataSource where (Code like @Code)