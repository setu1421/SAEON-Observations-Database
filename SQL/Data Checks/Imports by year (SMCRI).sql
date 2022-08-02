use Observations;
with ImportDates
as
(
Select
  Year(ImportBatch.ImportDate) Year, [Count]--, *
from
  vImportBatchSummary
  inner join ImportBatch
    on (vImportBatchSummary.ImportBatchID = ImportBatch.ID)
where
  (OrganisationCode = 'SMCRI')
)
Select
  Year, Count(*) Imports, Sum([Count]) Observations
from 
  ImportDates
group by
  Year
order by 
  Year Desc
