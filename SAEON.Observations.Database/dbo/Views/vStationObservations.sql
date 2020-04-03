CREATE VIEW [dbo].[vStationObservations]
AS
Select
  ID,
  StationID,
  InstrumentID, InstrumentCode, InstrumentName, InstrumentDescription,
  SensorID, SensorCode, SensorName, SensorDescription,
  ValueDate,
  DataValue,
  TextValue,
  Latitude,
  Longitude,
  Elevation,
  PhenomenonID, PhenomenonCode, PhenomenonName, PhenomenonDescription,
  PhenomenonOfferingID,
  OfferingID, OfferingCode, OfferingName, OfferingDescription,
  PhenomenonUOMID,
  UnitOfMeasureID, UnitOfMeasureCode, UnitOfMeasureUnit, UnitOfMeasureSymbol,
  CorrelationID,
  Comment,
  StatusCode, StatusName, StatusDescription,
  StatusReasonCode, StatusReasonName, StatusReasonDescription
from
  vObservationExpansion