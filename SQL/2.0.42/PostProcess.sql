Delete ImportBatchSummary
Insert Into ImportBatchSummary
  (ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, Count, Minimum, Maximum, Average, StandardDeviation, Variance, 
   TopLatitude, BottomLatitude, LeftLongitude, RightLongitude, StartDate, EndDate)
Select
  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(DataValue) Count, MIN(DataValue) Minimum, MAX(DataValue) Maximum, 
  AVG(DataValue) Average, STDEV(DataValue) StandardDeviation, VAR(DataValue) Variance, 
  Max(Latitude) TopLatitude, Min(Latitude) BottomLatitude, Min(Longitude) LeftLongitude, Max(Longitude) RightLongitude,
  Min(ValueDate) StartDate, Max(ValueDate) EndDate
from
  vObservationExpansion
group by
  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID
