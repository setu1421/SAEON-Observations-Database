use Observations;
with ImportDates
as
(
Select
  ImportBatch.ImportDate Date, [Count]--, *
from
  vImportBatchSummary
  inner join ImportBatch
    on (vImportBatchSummary.ImportBatchID = ImportBatch.ID)
where
  ((OrganisationCode = 'SMCRI') or (OrganisationCode = 'SAEON')) and
  (ImportBatch.ImportDate >= '2022-04-12')
)
Select
  Count(*) Imports, Sum([Count]) Observations
from 
  ImportDates
