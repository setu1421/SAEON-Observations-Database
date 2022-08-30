Use Observations
Declare @BatchCode Int = 9807
Declare @Date DateTime = '2022-06-27 06:00'
Select
  *
from
  vObservationExpansion
  inner join ImportBatch
    on (ImportBatchID  = ImportBatch.Id)
where
  (ImportBatch.Code = @BatchCode) and (ValueDate < @Date)
Delete
  Observation
from
  Observation
  inner join ImportBatch
    on (ImportBatchID  = ImportBatch.Id)
where
  (ImportBatch.Code = @BatchCode) and (ValueDate < @Date)
