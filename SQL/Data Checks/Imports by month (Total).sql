use Observations;
with ImportDates
as
(
Select
  DATEFROMPARTS(Year(ImportBatch.ImportDate),Month(ImportBatch.ImportDate),1) Date, [Count]--, *
from
  vImportBatchSummary
  inner join ImportBatch
    on (vImportBatchSummary.ImportBatchID = ImportBatch.ID)
where
  ((OrganisationCode = 'SMCRI') or (OrganisationCode = 'SAEON'))
)
Select
  Date Month, Count(*) Imports, Sum([Count]) Observations
from 
  ImportDates
group by
  Date
order by 
  Date Desc
