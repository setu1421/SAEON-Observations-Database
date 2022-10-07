use Observations;
declare @Date DateTime = '2022-09-01';
with ImportDates
as
(
Select
  OrganisationCode, ImportBatch.ImportDate Date, [Count]--, *
from
  vImportBatchSummary
  inner join ImportBatch
    on (vImportBatchSummary.ImportBatchID = ImportBatch.ID)
where
  (ImportBatch.ImportDate >= @Date)
)
Select
  'SMCRI' [Type],Count(*) Imports, Coalesce(Sum([Count]),0) Observations
from 
  ImportDates
where
  (OrganisationCode = 'SMCRI') 
union
Select
  'SAEON' [Type],Count(*) Imports, Coalesce(Sum([Count]),0) Observations
from 
  ImportDates
where
  (OrganisationCode = 'SAEON') 
union
Select
  'Total' [Type],Count(*) Imports, Coalesce(Sum([Count]),0) Observations
from 
  ImportDates
where
  ((OrganisationCode = 'SMCRI') or (OrganisationCode = 'SAEON'))
