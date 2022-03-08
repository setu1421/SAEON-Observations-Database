Declare @SensorID UniqueIdentifier = '27402dd7-4c16-47b9-94a6-77f08491f9cc'
--Declare @InstrumentID UniqueIdentifier = ''
--Declare @StationID UniqueIdentifier = ''
--Declare @SiteID UniqueIdentifier = ''
Declare @PhenomenonOfferingID UniqueIdentifier = 'cb0a2ee7-7984-46a4-aab7-f511880b82da'
Declare @PhenomenonUOMID UniqueIdentifier = '6fdd2f2a-6088-4362-9e8e-423c529e6441'

Select
  *
from
  vObservationExpansion
where
  (SensorID = @SensorID) and
  (PhenomenonOfferingID = @PhenomenonOfferingID) and
  (PhenomenonUOMID = @PhenomenonUOMID)

Select
  *
from
  vImportBatchSummary
where
  (SensorID = @SensorID) and
  (PhenomenonOfferingID = @PhenomenonOfferingID) and
  (PhenomenonUOMID = @PhenomenonUOMID)
