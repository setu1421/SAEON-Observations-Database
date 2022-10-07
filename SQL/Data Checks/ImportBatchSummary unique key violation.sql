use Observations;
with ImportBatches
as
(
Select
  ImportBatchCode, SensorCode, InstrumentCode, StationCode, SiteCode, PhenomenonName, OfferingName, UnitOfMeasureUnit
from
  vObservationExpansion
group by
  ImportBatchCode, SensorCode, InstrumentCode, StationCode, SiteCode, PhenomenonName, OfferingName, UnitOfMeasureUnit
)
Select 
  ImportBatchCode, SensorCode, PhenomenonName, OfferingName, UnitOfMeasureUnit, Count(*) Count
from
  ImportBatches
group by
  ImportBatchCode, SensorCode, PhenomenonName, OfferingName, UnitOfMeasureUnit
having
  Count(*) > 1
order by
  Count Desc, ImportBatchCode, SensorCode, PhenomenonName, OfferingName, UnitOfMeasureUnit
