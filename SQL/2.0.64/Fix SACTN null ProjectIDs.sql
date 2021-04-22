Select * from vImportBatchSummary where ProjectID is null
Select distinct 
  (Select ID from Project where Code = 'SACTN') ProjectID,
  StationID 
from 
  vImportBatchSummary 
  inner join Station
    on (StationID = Station.ID)
where 
  (ProjectID is null) and (Station.Code like 'SACTN%')
Insert
  Project_Station
  (ProjectID, StationID, UserId)
Select distinct 
  (Select ID from Project where Code = 'SACTN') ProjectID,
  StationID,
  (Select UserID from aspnet_Users where UserName = 'TimPN') UserId
from 
  vImportBatchSummary 
  inner join Station
    on (StationID = Station.ID)
where 
  (ProjectID is null) and (Station.Code like 'SACTN%')
