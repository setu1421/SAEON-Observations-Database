Select 
  SiteName Site, StationName Station, InstrumentName Instrument, PhenomenonName Phenomenon, OfferingName Offering, UnitOfMeasureUnit Unit, Latitude, Longitude
from
  vObservationExpansion
--where
--  (PhenomenonName not in ('Current Direction','Current speed','Depth'))
group by
  SiteName, StationName, InstrumentName, PhenomenonName, OfferingName, UnitOfMeasureUnit, Latitude, Longitude
order by
  SiteName, StationName, InstrumentName, PhenomenonName, OfferingName, UnitOfMeasureUnit, Latitude, Longitude

