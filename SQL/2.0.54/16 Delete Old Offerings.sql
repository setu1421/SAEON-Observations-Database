use Observations
Delete
  Offering
from
  Offering
  inner join vNewDepthOfferings
    on (vNewDepthOfferings.OldOfferingCode = Offering.Code)	
