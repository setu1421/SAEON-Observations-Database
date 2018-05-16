Delete ImportBatchSummary
Insert Into ImportBatchSummary
  (ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID, Count, Minimum, Maximum, Average, StandardDeviation, Variance)
Select
  ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(DataValue) Count, MIN(DataValue) Minimum, MAX(DataValue) Maximum, AVG(DataValue) Average, STDEV(DataValue) StandardDeviation, VAR(DataValue) Variance
from
  Observation
group by
  ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID
