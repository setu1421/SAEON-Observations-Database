--use Observations
begin transaction
Delete ImportBatchSummary
Insert Into ImportBatchSummary
  (ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, Count, Minimum, Maximum, Average, StandardDeviation, Variance, 
   LatitudeNorth, LatitudeSouth, LongitudeWest, LongitudeEast, ElevationMinimum, ElevationMaximum, StartDate, EndDate, VerifiedCount)
Select
  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(DataValue) Count, MIN(DataValue) Minimum, MAX(DataValue) Maximum, 
  AVG(DataValue) Average, STDEV(DataValue) StandardDeviation, VAR(DataValue) Variance, 
  Max(Latitude) LatitudeNorth, Min(Latitude) LatitudeSouth, Min(Longitude) LongitudeWest, Max(Longitude) LongitudeEast,
  Min(Elevation) ElevationMinimum, Max(Elevation) ElevationMaximum, Min(ValueDate) StartDate, Max(ValueDate) EndDate,
  (
  Select 
    Count(*) 
  from 
    Observation
	left join Status
	  on (Observation.StatusID = Status.ID)
  where 
    ((Observation.ImportBatchID = vObservationExpansion.ImportBatchID) and 
	 (Observation.SensorID = vObservationExpansion.SensorID) and
	 (Observation.PhenomenonOfferingID = vObservationExpansion.PhenomenonOfferingID) and
	 (Observation.PhenomenonUOMID = vObservationExpansion.PhenomenonUOMID) and
	 ((Observation.StatusID is null) or (Status.Name = 'Verified')))
  ) VerifiedCount
from
  vObservationExpansion
group by
  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID
--OPTION(RECOMPILE)
commit transaction
