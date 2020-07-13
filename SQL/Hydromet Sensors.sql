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
