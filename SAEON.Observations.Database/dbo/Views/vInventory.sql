CREATE VIEW [dbo].[vInventory]
AS
Select
  NEWID() ID, s.*
from
(
Select
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonCode, PhenomenonName, 
  OfferingCode, OfferingName, UnitOfMeasureCode, UnitOfMeasureUnit, Sum(Count) Count, Min(StartDate) StartDate, Max(EndDate) EndDate
from
  vImportBatchSummary
group by
  SiteCode, SiteName, StationCode, StationName, InstrumentCode, InstrumentName, SensorCode, SensorName, PhenomenonCode, PhenomenonName, 
  OfferingCode, OfferingName, UnitOfMeasureCode, UnitOfMeasureUnit
) s