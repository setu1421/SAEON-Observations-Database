-- Delete old SMCRI Projects
Select * from Programme where (Description like 'SMCRI%')
Select 
  * 
from 
  Project
  inner join Programme 
    on (Project.ProgrammeID = Programme.ID)
where
  (Programme.Description  like 'SMCRI%')
Delete
  Project 
from 
  Project
  inner join Programme 
    on (Project.ProgrammeID = Programme.ID)
where
  (Programme.Description  like 'SMCRI%')
Delete Programme where (Description like 'SMCRI%')
-- Fix SACTN
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
-- Fix SAEON
Select * from vImportBatchSummary where ProjectID is null
Select distinct 
  (Select ID from Project where Code = 'SAEON') ProjectID,
  StationID 
from 
  vImportBatchSummary 
  inner join Station
    on (StationID = Station.ID)
where 
  (ProjectID is null)
Insert
  Project_Station
  (ProjectID, StationID, UserId)
Select distinct 
  (Select ID from Project where Code = 'SAEON') ProjectID,
  StationID,
  (Select UserID from aspnet_Users where UserName = 'TimPN') UserId
from 
  vImportBatchSummary 
  inner join Station
    on (StationID = Station.ID)
where 
  (ProjectID is null)
