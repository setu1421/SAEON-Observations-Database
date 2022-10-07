use Observations;
with ImportDates
as
(
Select
  OrganisationCode, DATEFROMPARTS(Year(ImportBatch.ImportDate),Month(ImportBatch.ImportDate),1) Date, [Count]--, *
from
  vImportBatchSummary
  inner join ImportBatch
    on (vImportBatchSummary.ImportBatchID = ImportBatch.ID)
where
  ((OrganisationCode = 'SMCRI') or (OrganisationCode = 'SAEON'))
)
Select
  *
from
(
Select
  'SMCRI' [Type], Date Month, Count(*) Imports, Coalesce(Sum([Count]),0) Observations
from 
  ImportDates
where
  (OrganisationCode = 'SMCRI')
group by
  Date
union
Select
  'SAEON' [Type], Date Month, Count(*) Imports, Coalesce(Sum([Count]),0) Observations
from 
  ImportDates
where
  (OrganisationCode = 'SAEON')
group by
  Date
union
Select
  'Total' [Type], Date Month, Count(*) Imports, Coalesce(Sum([Count]),0) Observations
from 
  ImportDates
where
  ((OrganisationCode = 'SMCRI') or (OrganisationCode = 'SAEON'))
group by
  Date
) a
order by 
  Month Desc, Type
