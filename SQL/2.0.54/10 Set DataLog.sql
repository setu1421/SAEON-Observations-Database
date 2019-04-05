use Observations;
Update
  DataLog
set
  PhenomenonOfferingID = NewPhenomenonOfferingID
from
  DataLog
  inner join vNewDepthPhenomenonOfferings
    on (Datalog.PhenomenonOfferingID = OldPhenomenonOfferingID)

