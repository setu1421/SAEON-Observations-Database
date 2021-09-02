--use Observations
begin transaction
Delete ImportBatchSummary
Insert Into ImportBatchSummary
  (ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, Count, ValueCount, Minimum, Maximum, Average, StandardDeviation, Variance, 
   LatitudeNorth, LatitudeSouth, LongitudeWest, LongitudeEast, ElevationMinimum, ElevationMaximum, StartDate, EndDate, VerifiedCount, UnverifiedCount)
Select
  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(ID) Count, COUNT(DataValue) ValueCount, 
  MIN(DataValue) Minimum, MAX(DataValue) Maximum, 
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
  ) VerifiedCount,
  (
  Select 
    Count(*) 
  from 
    Observation
	inner join Status
	  on (Observation.StatusID = Status.ID)
  where 
    ((Observation.ImportBatchID = vObservationExpansion.ImportBatchID) and 
	 (Observation.SensorID = vObservationExpansion.SensorID) and
	 (Observation.PhenomenonOfferingID = vObservationExpansion.PhenomenonOfferingID) and
	 (Observation.PhenomenonUOMID = vObservationExpansion.PhenomenonUOMID) and
	 (Status.Name <> 'Verified'))
  ) UnverifiedCount
from
  vObservationExpansion
group by
  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID
--OPTION(RECOMPILE)
commit transaction
