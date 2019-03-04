use Observations
Delete
  Offering
from
  Offering
  inner join NewDepthOfferings
    on (NewDepthOfferings.OldOfferingCode = Offering.Code)	
