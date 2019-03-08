use Observations;
Update
  DataSourceTransformation
set
  PhenomenonOfferingID = vNewDepthPhenomenonOfferings.NewPhenomenonOfferingID
from
  DataSourceTransformation
  inner join vNewDepthPhenomenonOfferings 
    on (DataSourceTransformation.PhenomenonOfferingID = vNewDepthPhenomenonOfferings.OldPhenomenonOfferingID)

Update
  DataSourceTransformation
set
  NewPhenomenonOfferingID = vNewDepthPhenomenonOfferings.NewPhenomenonOfferingID
from
  DataSourceTransformation
  inner join vNewDepthPhenomenonOfferings 
    on (DataSourceTransformation.NewPhenomenonOfferingID = vNewDepthPhenomenonOfferings.OldPhenomenonOfferingID)
