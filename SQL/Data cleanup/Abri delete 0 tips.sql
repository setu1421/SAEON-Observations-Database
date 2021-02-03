use Observations
Select
  Count(*)
  --*
from
  vObservationExpansion
where
  (DataValue = 0) and
  (PhenomenonCode = 'Rain') and
  (OfferingCode = 'Act')

Delete
  Observation
from
  Observation
  inner join vObservationExpansion
    on (Observation.ID = vObservationExpansion.ID)
where
  (Observation.DataValue = 0) and
  (PhenomenonCode = 'Rain') and
  (OfferingCode = 'Act')

