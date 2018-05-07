Insert Into ImportBatchSummary
  (ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID, Count, Minimum, Maximum, Average, StandardDeviation, Variance)
Select
  ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(ImportBatchID) Count, MIN(DataValue) Minimum, MAX(DataValue) Maximum, AVG(DataValue) Average, STDEV(DataValue) StandardDeviation, VAR(DataValue) Variance
from
  Observation
group by
  ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID
drop view vDataSourceRoleOld