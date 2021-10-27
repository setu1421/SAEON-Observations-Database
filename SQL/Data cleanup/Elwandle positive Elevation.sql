Update
  Station_Instrument
set
  Elevation = -Station_Instrument.Elevation 
from 
  Station_Instrument 
  inner join Station
    on (Station_Instrument.StationID = Station.ID)
where 
  (Left(Station.Code,4) = 'ELW_') and
  (Station_Instrument.Elevation > 0)
update
  Observation
set
  Elevation = -Observation.Elevation
from
  Observation 
  inner join vObservationExpansion
    on (Observation.ID = vObservationExpansion.ID)
where
  (Left(StationCode,4) = 'ELW_') and (Observation.Elevation > 0)