use Observations;
Update
  SchemaColumn
set
  PhenomenonOfferingID = NewPhenomenonOfferingID
from
  SchemaColumn
  inner join vNewDepthPhenomenonOfferings
    on (SchemaColumn.PhenomenonOfferingID = OldPhenomenonOfferingID)

