Select distinct
  ProgrammeName Programme, ProjectName Project, SiteName Site, StationName Station, InstrumentName Instrument, PhenomenonName Phenomenon, OfferingName Offering, UnitOfMeasureUnit Unit, StartDate Start, EndDate [End], LatitudeSouth Latitude, LongitudeWest Longitude
from 
  vInventorySensors
order by
  ProgrammeName, ProjectName, SiteName, StationName, InstrumentName, PhenomenonName, OfferingName, UnitOfMeasureUnit