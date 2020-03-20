CREATE VIEW [dbo].[vSensorThingsAPISensors]
AS
Select Distinct
  Sensor.ID, Sensor.Code, Sensor.Name, Sensor.Description, Sensor.Url, PhenomenonOfferingID
from
  vInventory
  inner join Sensor
    on (vInventory.SensorID = Sensor.ID)
