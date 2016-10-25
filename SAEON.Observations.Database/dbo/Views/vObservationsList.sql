--> Added 2.0.15 20161024 TimPN
CREATE VIEW [dbo].[vObservationsList]
AS 
SELECT 
  Observation.*,
  Sensor.Code SensorCode,
  Sensor.Name SensorName,
  Phenomenon.Code PhenomenonCode,
  Phenomenon.Name PhenomenonName,
  Offering.Code OfferingCode,
  Offering.Name OfferingName,
  UnitOfMeasure.Code UnitOfMeasureCode,
  UnitOfMeasure.Unit UnitOfMeasureUnit,
  Status.Code StatusCode,
  Status.Name StatusName,
  StatusReason.Code StatusReasonCode,
  StatusReason.Name StatusReasonName
FROM
  Observation
  inner join Sensor
    on (Observation.SensorID = Sensor.ID)
  inner join PhenomenonOffering
    on (Observation.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (Observation.PhenomenonUOMID = PhenomenonUOM.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
  left join Status
    on (Observation.StatusID = Status.ID)
  left join StatusReason
    on (Observation.StatusReasonID = StatusReason.ID)
--> Added 2.0.15 20161024 TimPN
