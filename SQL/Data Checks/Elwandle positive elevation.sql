Select Code, Name, Elevation from Station where (Code like 'ELW_%') and (Elevation > 0)
Select
  Code, Name, Station_Instrument.Elevation
from 
  Station_Instrument 
  inner join Station
    on (Station_Instrument.StationID = Station.ID)
where 
  (Code like 'ELW_%') and (Station_Instrument.Elevation > 0)
Select Code, Name, Elevation from Instrument where (Code like 'ELW_%') and (Elevation > 0)
Select
  StationCode, StationName, Observation.Elevation
from
  Observation 
  inner join vObservationExpansion
    on (Observation.ID = vObservationExpansion.ID)
where
  (StationCode like 'ELW_%') and (Observation.Elevation > 0)