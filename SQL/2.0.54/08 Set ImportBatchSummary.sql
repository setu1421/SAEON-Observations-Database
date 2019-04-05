use Observations;
Update
  ImportBatchSummary
set
  PhenomenonOfferingID = NewPhenomenonOfferingID
from
  ImportBatchSummary
  inner join vNewDepthPhenomenonOfferings
    on (ImportBatchSummary.PhenomenonOfferingID = OldPhenomenonOfferingID)

