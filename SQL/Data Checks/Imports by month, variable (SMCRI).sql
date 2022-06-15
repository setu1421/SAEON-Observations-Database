with ImportDates
as
(
Select
  DATEFROMPARTS(Year(ImportBatch.ImportDate),Month(ImportBatch.ImportDate),1) Date, PhenomenonName Phenomenon, OfferingName Offering, UnitOfMeasureUnit Unit, [Count]
from
  vImportBatchSummary
  inner join ImportBatch
    on (vImportBatchSummary.ImportBatchID = ImportBatch.ID)
where
  (OrganisationCode = 'SMCRI')
)
Select
  Date Month, Phenomenon, Offering, Unit, Count(*) Imports, Sum([Count]) Observations
from 
  ImportDates
group by
  Date, Phenomenon, Offering, Unit
order by 
  Date Desc, Phenomenon, Offering, Unit
