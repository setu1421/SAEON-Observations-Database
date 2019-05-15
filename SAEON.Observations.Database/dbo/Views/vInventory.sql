CREATE VIEW [dbo].[vInventory]
AS
Select
  Row_Number() over (order by SiteName, StationName, InstrumentName, SensorName, PhenomenonName, OfferingName, UnitOfMeasureUnit) ID, s.*
from
(
Select
  SiteID, SiteCode, SiteName, 
  StationID, StationCode, StationName, 
  InstrumentID, InstrumentCode, InstrumentName, 
  SensorID, SensorCode, SensorName, 
  PhenomenonCode, PhenomenonName, 
  PhenomenonOfferingID, OfferingCode, OfferingName, 
  PhenomenonUOMID, UnitOfMeasureCode, UnitOfMeasureUnit, 
  Sum(Count) Count, Min(StartDate) StartDate, Max(EndDate) EndDate,
  Max(LatitudeNorth) LatitudeNorth, Min(LatitudeSouth) LatitudeSouth,
  Min(LongitudeWest) LongitudeWest, Max(LongitudeEast) LongitudeEast
from
  vImportBatchSummary
group by
  SiteID, SiteCode, SiteName, StationID, StationCode, StationName, InstrumentID, InstrumentCode, InstrumentName, 
  SensorID, SensorCode, SensorName, PhenomenonCode, PhenomenonName, 
  PhenomenonOfferingID, OfferingCode, OfferingName, 
  PhenomenonUOMID, UnitOfMeasureCode, UnitOfMeasureUnit
) s