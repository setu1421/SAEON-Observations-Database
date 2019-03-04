use Observations;
Update
  Observation
set
  PhenomenonOfferingID = NewPhenomenonOfferingID
from
  Observation
  inner join NewDepthPhenomenonOfferings
    on (Observation.PhenomenonOfferingID = OldPhenomenonOfferingID)
