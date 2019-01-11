Update
  DataSourceTransformation
set
  NewPhenomenonID = coalesce(PhenomenonOffering.PhenomenonID, PhenomenonUOM.PhenomenonID)
from
  DataSourceTransformation
  left join PhenomenonOffering
    on (NewPhenomenonOfferingID = PhenomenonOffering.ID)
  left join PhenomenonUOM
    on (NewPhenomenonUOMID = PhenomenonUOM.ID)
where
  (NewPhenomenonOfferingID is not null) or
  (NewPhenomenonUOMID is not null)