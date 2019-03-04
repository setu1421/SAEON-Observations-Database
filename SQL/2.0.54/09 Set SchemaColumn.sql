use Observations;
Update
  SchemaColumn
set
  PhenomenonOfferingID = NewPhenomenonOfferingID
from
  SchemaColumn
  inner join NewDepthPhenomenonOfferings
    on (SchemaColumn.PhenomenonOfferingID = OldPhenomenonOfferingID)

