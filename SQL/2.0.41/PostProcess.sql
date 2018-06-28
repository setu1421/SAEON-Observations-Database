Delete ImportBatchSummary
Insert Into ImportBatchSummary
  (ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID, Count, Minimum, Maximum, Average, StandardDeviation, Variance, 
  TopLatitude, BottomLatitude, LeftLongitude, RightLongitude)
Select
  ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(DataValue) Count, MIN(DataValue) Minimum, MAX(DataValue) Maximum, 
  AVG(DataValue) Average, STDEV(DataValue) StandardDeviation, VAR(DataValue) Variance, 
  Max(Latitude) TopLatitude, Min(Latitude) BottomLatitude, Min(Longitude) LeftLongitude, Max(Longitude) RightLongitude
from
  vObservationExpansion
group by
  ImportBatchID, SensorID, PhenomenonOfferingID, PhenomenonUOMID
