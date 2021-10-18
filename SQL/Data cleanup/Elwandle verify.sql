Declare @StatusID UniqueIdentifier = (Select ID from Status where Code = 'QA-97')
--Select
--  Count(*)
--from
--  vImportBatchSummary
--  inner join Observation
--    on (Observation.ImportBatchID = vImportBatchSummary.ImportBatchID) and
--	   (Observation.SensorID =vImportBatchSummary.SensorID) and
--	   (Observation.PhenomenonOfferingID = vImportBatchSummary.PhenomenonOfferingID) and
--	   (Observation.PhenomenonUOMID = vImportBatchSummary.PhenomenonUOMID)
--where
--  (SiteCode in ('ALGB','ALGBPP','ISAR','STFR','MOSB')) and
--  (StatusID = @StatusID)

Update
  Observation
set
  StatusID = null,
  StatusReasonID = null
from
  vImportBatchSummary
  inner join Observation
    on (Observation.ImportBatchID = vImportBatchSummary.ImportBatchID) and
	   (Observation.SensorID =vImportBatchSummary.SensorID) and
	   (Observation.PhenomenonOfferingID = vImportBatchSummary.PhenomenonOfferingID) and
	   (Observation.PhenomenonUOMID = vImportBatchSummary.PhenomenonUOMID)
where
  (SiteCode in ('ALGB','ALGBPP','ISAR','STFR','MOSB')) and
  (StatusID = @StatusID)

