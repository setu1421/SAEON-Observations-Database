use Observations
Select
  PhenomenonName Phenomenon, OfferingName Offering, UnitOfMeasureUnit Unit, SiteName Site, StationName Station, Count(*) Count
from
  vImportBatchSummary
group by
  PhenomenonName, OfferingName, UnitOfMeasureUnit, SiteName, StationName
order by
  PhenomenonName, OfferingName, UnitOfMeasureUnit, SiteName, StationName
