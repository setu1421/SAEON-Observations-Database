with NameDups
as
(
Select 
  Name, Count(*) Count
from 
  Sensor

group by 
  Name
having
  (Count(*) > 1)
)
Select distinct
  SensorName, SensorCode, InstrumentCode, InstrumentName, StationCode, StationName
from 
  NameDups 
  inner join vImportBatchSummary
    on (vImportBatchSummary.SensorName = NameDups.Name)
where 
  (NameDups.Name not like 'Onset Hobo%') and
  (NameDups.Name not like 'MANUAL Transducer Current Direction%') and
  (NameDups.Name not like 'MANUAL Calculated%') and
  (NameDups.Name not like 'MANUAL Transducer Current speed%') and
  (NameDups.Name not like 'MANUAL Pressure Sensor%') and
  (NameDups.Name not like 'MANUAL Thermistor%') and
  (NameDups.Name not like 'MANUAL Aanderaa%') and
  (NameDups.Name <> 'MCS Single Channel Temperature Data Logger') 
order by
  SensorName, SensorCode, InstrumentCode, InstrumentName, StationCode, StationName



