CREATE VIEW [dbo].[vSensorThingsAPIDatastreams]
AS
Select Distinct
  SensorID ID, SensorCode Code, SensorName Name, SensorDescription Description,
  InstrumentID, InstrumentCode, InstrumentName,
  PhenomenonID, PhenomenonCode, PhenomenonName, PhenomenonDescription, PhenomenonUrl,
  PhenomenonOfferingID, OfferingCode, OfferingName, OfferingDescription,
  PhenomenonUOMID PhenomenonUnitID, UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasureSymbol,
  StartDate, EndDate, LatitudeNorth, LatitudeSouth, LongitudeWest, LongitudeEast
from
  vInventorySensors