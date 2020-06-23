CREATE VIEW [dbo].[vSensorThingsAPISensors]
AS
Select Distinct
  SensorID ID, SensorCode Code, SensorName Name, SensorDescription Description, SensorUrl Url, PhenomenonOfferingID
from
  vInventorySensors
