use Observations;
Update
  ImportBatchSummary
set
  PhenomenonOfferingID = NewPhenomenonOfferingID
from
  ImportBatchSummary
  inner join NewDepthPhenomenonOfferings
    on (ImportBatchSummary.PhenomenonOfferingID = OldPhenomenonOfferingID)

