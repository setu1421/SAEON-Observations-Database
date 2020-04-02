Select 
  ImportBatch.Code, ImportDate, DurationInSecs Secs, DurationInSecs / 60.0 Mins, 
  (Select sum(Count) from ImportBatchSummary where ImportBatchID = ImportBatch.ID) Count,
  DataSource.Name, FileName
from 
  ImportBatch 
  inner join  DataSource
    on (ImportBatch.DataSourceID = DataSource.ID)
where 
  DurationInSecs is not null
--order by
--  DurationInSecs Desc
order by
  ImportDate Desc
