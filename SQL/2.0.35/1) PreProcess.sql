Delete
  DataLog
from
(
Select
  ROW_NUMBER() OVER (Partition By L.ImportBatchID, L.SensorID, L.ValueDate, L.PhenomenonOfferingID, L.PhenomenonUOMID Order By L.AddedAt) RowNum,
  L.ID
from
  DataLog L
  join DataLog R
    on (L.ID <> R.ID) and
       (L.ImportBatchID = R.ImportBatchID) and      
       (L.SensorID = R.SensorID) and
       (L.ValueDate = R.ValueDate) and
       (L.RawValue = R.RawValue) and
       --((L.RawValue is null) and (R.RawValue is null)) and
       (L.PhenomenonOfferingID = R.PhenomenonOfferingID) and
       (L.PhenomenonUOMID = R.PhenomenonUOMID)
) dups
inner join DataLog
  on (dups.ID = DataLog.ID)
where
  dups.RowNum > 1
