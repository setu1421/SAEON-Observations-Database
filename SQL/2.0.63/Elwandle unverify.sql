--Select distinct
  --SensorName, SensorCode, InstrumentCode, InstrumentName, StationCode, StationName, SiteCode, SiteName
--  Sum(Count)
--from 
--  vImportBatchSummary
--where 
  --(SensorCode like 'Algb%') and
  --(SiteCode in ('ALGB','ALGBPP','ISAR','STFR','MOSB')) --and
  --((SensorName like 'Onset Hobo%') or
  --(SensorName like 'MANUAL Transducer Current Direction%') or
  --(SensorName like 'MANUAL Calculated%') or
  --(SensorName like 'MANUAL Transducer Current speed%') or
  --(SensorName like 'MANUAL Pressure Sensor%') or
  --(SensorName like 'MANUAL Thermistor%') or
  --(SensorName like 'MANUAL Aanderaa%') or
  --(SensorName = 'MCS Single Channel Temperature Data Logger'))
--order by
--  SensorName, SensorCode, InstrumentCode, InstrumentName, StationCode, StationName, SiteCode, SiteName

Declare @StatusID UniqueIdentifier = (Select ID from Status where Code = 'QA-97')
Declare @StatusReasonID UniqueIdentifier = (Select ID from StatusReason where Code = 'QAR-38')
Update
  Observation
set
  StatusID = @StatusID,
  @StatusReasonID = @StatusReasonID
from
  vImportBatchSummary
  inner join Observation
    on (Observation.ImportBatchID = vImportBatchSummary.ImportBatchID) and
	   (Observation.SensorID =vImportBatchSummary.SensorID) and
	   (Observation.PhenomenonOfferingID = vImportBatchSummary.PhenomenonOfferingID) and
	   (Observation.PhenomenonUOMID = vImportBatchSummary.PhenomenonUOMID)
where
  (SiteCode in ('ALGB','ALGBPP','ISAR','STFR','MOSB')) and
  (StatusID is null)

Select
  Count(*)
from
  vImportBatchSummary
  inner join Observation
    on (Observation.ImportBatchID = vImportBatchSummary.ImportBatchID) and
	   (Observation.SensorID =vImportBatchSummary.SensorID) and
	   (Observation.PhenomenonOfferingID = vImportBatchSummary.PhenomenonOfferingID) and
	   (Observation.PhenomenonUOMID = vImportBatchSummary.PhenomenonUOMID)
where 
  (SiteCode in ('ALGB','ALGBPP','ISAR','STFR','MOSB'))



