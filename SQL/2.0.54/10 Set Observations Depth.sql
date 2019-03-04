use Observations;
Update
  Observation
set
  Elevation = Depth,
  PhenomenonOfferingID = NewPhenomenonOfferingID
from
  Observation
  inner join NewDepthPhenomenonOfferings
    on (Observation.PhenomenonOfferingID = OldPhenomenonOfferingID)
where
  (Elevation is null)
