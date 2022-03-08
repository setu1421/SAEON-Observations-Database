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
  (StationCode like 'ELW%')
)
Select
  Date Month, Count(*) Imports, Sum([Count]) Observations
from 
  ImportDates
group by
  Date
order by 
  Date Desc
