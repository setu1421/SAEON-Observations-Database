select --top 100000
  Old.ID, New.ID
from
  Observation
  left join ImportBatch Old
    on (Observation.ImportBatchID = Old.ID)
  left join ImportBatch New
    on (Observation.ImportBatchGuid = New.Guid)
where
  (Old.ID <> New.ID)