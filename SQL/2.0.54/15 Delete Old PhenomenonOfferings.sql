use Observations
Delete
  PhenomenonOffering
from
  PhenomenonOffering
  inner join vNewDepthOfferings
    on (PhenomenonOffering.PhenomenonID = vNewDepthOfferings.PhenomenonID) and
	   (PhenomenonOffering.OfferingID = (Select ID from Offering where Offering.Code = vNewDepthOfferings.OldOfferingCode))