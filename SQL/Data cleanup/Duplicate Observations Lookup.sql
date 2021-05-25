--use Observations;
Select
  Sensor.Code SensorCode, Sensor.Name SensorName, ValueDate Date, Phenomenon.Name Phenomenon, Offering.Name Offering, UnitOfMeasure.Unit Unit, o.Elevation, DataValue Value, ImportBatch.Code
from
  Observation o
  inner join ImportBatch
    on (o.ImportBatchID = ImportBatch.ID)
  inner join PhenomenonOffering
    on (o.PhenomenonOfferingID = PhenomenonOffering.ID)
  inner join Phenomenon
    on (PhenomenonOffering.PhenomenonID = Phenomenon.ID)
  inner join Offering
    on (PhenomenonOffering.OfferingID = Offering.ID)
  inner join PhenomenonUOM
    on (o.PhenomenonUOMID = PhenomenonUOM.ID)
  inner join UnitOfMeasure
    on (PhenomenonUOM.UnitOfMeasureID = UnitOfMeasure.ID)
  inner join Sensor
    on (o.SensorID = Sensor.ID)
where
  (ImportBatch.Code in (3979,3995)) and
  (ValueDate = '2017-04-10 12:00:00')
order by
  Sensor.Code, Sensor.Name, o.ValueDate, Phenomenon.Name, Offering.Name, UnitOfMeasure.Unit, o.Elevation, ImportBatch.Code;

--Select
--  SensorCode, SensorName, ValueDate Date, PhenomenonName Phenomenon, OfferingName Offering, UnitOfMeasureUnit Unit, Elevation, DataValue Value, ImportBatchCode
--from
--  vObservationExpansion
--where
--  (ImportBatchCode in (3979,3995)) and
--  (ValueDate = '2017-04-10 12:00:00')
--order by
--  SensorCode, SensorName, ValueDate, PhenomenonName, OfferingName, UnitOfMeasureUnit, Elevation, ImportBatchCode

