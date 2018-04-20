Insert Into ImportBatchSummary
  (ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID, Count, Minimum, Maximum, Average, StandardDeviation, Variance)
Select
  ImportBatch.ID, SensorID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(DataValue) Count, MIN(DataValue) Minimum, MAX(DataValue) Maximum, AVG(DataValue) Average, STDEV(DataValue) StandardDeviation, VAR(DataValue) Variance
from
  ImportBatch
  inner join Observation
    on (Observation.ImportBatchID = ImportBatch.ID)
group by
  ImportBatch.ID, SensorID, PhenomenonOfferingID, PhenomenonUOMID
