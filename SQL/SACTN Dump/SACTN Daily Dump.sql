Select 
  SiteName, StationName, InstrumentName, SensorName, PhenomenonName, OfferingName, UnitOfMeasureUnit, ValueDate, DataValue 
from
  vObservationExpansion
where
  (StationName like 'SACTN%') and (SensorName like '% Daily %')
order by
  SiteName, StationName, InstrumentName, SensorName, PhenomenonName, OfferingName, UnitOfMeasureUnit, ValueDate