use Observations
Select
  PhenomenonName Phenomenon, OfferingName Offering, UnitOfMeasureUnit Unit, Count(*) Count
from
  vImportBatchSummary
group by
  PhenomenonName, OfferingName, UnitOfMeasureUnit
order by
  PhenomenonName, OfferingName, UnitOfMeasureUnit
