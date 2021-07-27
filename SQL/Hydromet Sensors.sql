use Observations
Select distinct
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonCode, PhenomenonName, OfferingCode, OfferingName, UnitOfMeasureCode, UnitOfMeasureUnit, Count(*) Count
from
  vObservationExpansion
where
  StatusName = 'Unverified - Staging'
group by
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonCode, PhenomenonName, OfferingCode, OfferingName, UnitOfMeasureCode, UnitOfMeasureUnit
order by
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonCode, PhenomenonName, OfferingCode, OfferingName, UnitOfMeasureCode, UnitOfMeasureUnit
--OPTION(RECOMPILE)
Select distinct
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonCode, PhenomenonName, 
  OfferingCode, OfferingName, UnitOfMeasureCode, UnitOfMeasureUnit, ImportBatchCode, ImportBatch.FileName, Count(*) Count
from
  vObservationExpansion
  inner join ImportBatch
    on (ImportBatchID = ImportBatch.ID)
where
  StatusName = 'Unverified - Staging'
group by
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonCode, PhenomenonName, 
  OfferingCode, OfferingName, UnitOfMeasureCode, UnitOfMeasureUnit, ImportBatchCode, ImportBatch.FileName
order by
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonCode, PhenomenonName, 
  OfferingCode, OfferingName, UnitOfMeasureCode, UnitOfMeasureUnit, ImportBatchCode
