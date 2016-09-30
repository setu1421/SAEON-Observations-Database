select
  Old.ID, New.ID
from
  DataLog
  left join ImportBatch Old
    on (DataLog.ImportBatchID = Old.ID)
  left join ImportBatch New
    on (DataLog.ImportBatchGuid = New.Guid)
where
  (Old.ID <> New.ID)