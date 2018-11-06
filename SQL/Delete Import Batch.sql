Declare @Code Int = -1
Delete
  DataLog
from
  DataLog
  inner join ImportBatch
    on (DataLog.ImportBatchID = ImportBatch.ID)
where
  (ImportBatch.Code = @Code)
Delete
  Observation
from
  Observation
  inner join ImportBatch
    on (Observation.ImportBatchID = ImportBatch.ID)
where
  (ImportBatch.Code = @Code)
    