use Observations
Delete
  PhenomenonOffering
from
  PhenomenonOffering
  inner join NewDepthOfferings
    on (PhenomenonOffering.PhenomenonID = NewDepthOfferings.PhenomenonID) and
	   (PhenomenonOffering.OfferingID = (Select ID from Offering where Offering.Code = NewDepthOfferings.OldOfferingCode))