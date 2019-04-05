use Observations;
Update
  Observation
set
  PhenomenonOfferingID = NewPhenomenonOfferingID
from
  Observation
  inner join vNewDepthPhenomenonOfferings
    on (Observation.PhenomenonOfferingID = OldPhenomenonOfferingID)
