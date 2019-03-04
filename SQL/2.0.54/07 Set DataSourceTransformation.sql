use Observations;
Update
  DataSourceTransformation
set
  PhenomenonOfferingID = NewDepthPhenomenonOfferings.NewPhenomenonOfferingID
from
  DataSourceTransformation
  inner join NewDepthPhenomenonOfferings 
    on (DataSourceTransformation.PhenomenonOfferingID = NewDepthPhenomenonOfferings.OldPhenomenonOfferingID)

Update
  DataSourceTransformation
set
  NewPhenomenonOfferingID = NewDepthPhenomenonOfferings.NewPhenomenonOfferingID
from
  DataSourceTransformation
  inner join NewDepthPhenomenonOfferings 
    on (DataSourceTransformation.NewPhenomenonOfferingID = NewDepthPhenomenonOfferings.OldPhenomenonOfferingID)
