Delete ImportBatchSummary
Insert Into ImportBatchSummary
  (ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, Count, Minimum, Maximum, Average, StandardDeviation, Variance, 
   LatitudeNorth, LatitudeSouth, LongitudeWest, LongitudeEast, ElevationMinimum, ElevationMaximum, StartDate, EndDate)
Select
  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID, COUNT(DataValue) Count, MIN(DataValue) Minimum, MAX(DataValue) Maximum, 
  AVG(DataValue) Average, STDEV(DataValue) StandardDeviation, VAR(DataValue) Variance, 
  Max(Latitude) LatitudeNorth, Min(Latitude) LatitudeSouth, Min(Longitude) LongitudeWest, Max(Longitude) LongitudeEast,
  Min(Elevation) ElevationMinimum, Max(Elevation) ElevationMaximum, Min(ValueDate) StartDate, Max(ValueDate) EndDate
from
  vObservationExpansion
group by
  ImportBatchID, SensorID, InstrumentID, StationID, SiteID, PhenomenonOfferingID, PhenomenonUOMID
SET IDENTITY_INSERT DigitalObjectIdentifiers ON
Insert into DigitalObjectIdentifiers
  (ID, Name,AddedBy,UpdatedBy)
values
  (1, 'Observations Database',(Select UserID from aspnet_Users where UserName = 'TimPN'),(Select UserID from aspnet_Users where UserName = 'TimPN'))
SET IDENTITY_INSERT DigitalObjectIdentifiers OFF


