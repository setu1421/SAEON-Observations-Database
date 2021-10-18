--Select
--  Count(*)
--from
--  vImportBatchSummary
--  inner join Observation
--    on (Observation.ImportBatchID = vImportBatchSummary.ImportBatchID) and
--	   (Observation.SensorID = vImportBatchSummary.SensorID) and
--	   (Observation.PhenomenonOfferingID = vImportBatchSummary.PhenomenonOfferingID) and
--	   (Observation.PhenomenonUOMID = vImportBatchSummary.PhenomenonUOMID)
--where 
--  (SiteCode in ('ALGB','ALGBPP','ISAR','STFR','MOSB')) and
--  (StatusID is null)

Declare @StatusID UniqueIdentifier = (Select ID from Status where Code = 'QA-97')
Declare @StatusReasonID UniqueIdentifier = (Select ID from StatusReason where Code = 'QAR-38')
Update
  Observation
set
  StatusID = @StatusID,
  StatusReasonID = @StatusReasonID
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






