Select 
  Code, DurationInSecs Secs, DurationInSecs / 60.0 Mins, (Select sum(Count) from ImportBatchSummary where ImportBatchID = ImportBatch.ID) Count
from 
  ImportBatch 
where 
  DurationInSecs is not null
order by
  DurationInSecs Desc