CREATE VIEW [dbo].[vSensorThingsAPIDatastreams]
AS
Select Distinct
  Sensor.ID, Sensor.Code, Sensor.Name, Sensor.Description,
  InstrumentID, InstrumentCode, InstrumentName,
  PhenomenonCode, PhenomenonName, Phenomenon.Description PhenomenonDescription, Phenomenon.Url PhenomenonUrl,
  OfferingCode, OfferingName, Offering.Description OfferingDescription,
  UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasure.UnitSymbol UnitOfMeasureSymbol,
  StartDate, EndDate, LatitudeNorth, LatitudeSouth, LongitudeWest, LongitudeEast
from
  vInventory
  inner join Sensor
    on (vInventory.SensorID = Sensor.ID)
  inner join Phenomenon
    on (vInventory.PhenomenonCode = Phenomenon.Code)
  inner join Offering
    on (vInventory.OfferingCode = Offering.Code)
  inner join UnitOfMeasure
    on (vInventory.UnitOfMeasureCode = UnitOfMeasure.Code)
