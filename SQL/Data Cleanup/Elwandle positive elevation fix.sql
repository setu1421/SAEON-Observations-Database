Update
  Station
set
  Elevation = -Elevation 
from 
  Station
where 
  (Code like 'ELW_%') and (Elevation > 0)
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
Update
  Instrument
set
  Elevation = -Elevation 
from 
  Instrument
where 
  (Code like 'ELW_%') and (Elevation > 0)
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