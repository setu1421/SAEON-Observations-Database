CREATE VIEW [dbo].[vSensorThingsAPIObservations]
AS
Select
  ID, SensorID, PhenomenonOfferingID, PhenomenonUOMID PhenomenonUnitID, ValueDate, DataValue
from
  Observation
