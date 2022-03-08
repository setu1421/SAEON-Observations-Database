use Observations;
Select
  SiteCode, SiteName, StationCode, StationName, StationDescription, LatitudeNorth Latitude, LongitudeWest Longitude, 
  PhenomenonName Phenomenon, OfferingName Offering, UnitOfMeasureUnit Unit
from 
  vInventoryDatasets
where
  (SiteCode not in ('ALGB','ALGBPP','ISAR','STFR','MOSB')) and
  (StationCode not like 'SACTN%')
order by
  SiteName, StationName, PhenomenonName, OfferingName, UnitOfMeasureUnit