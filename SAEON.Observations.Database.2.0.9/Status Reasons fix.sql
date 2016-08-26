Update 
  Status 
set 
  Description = 'Staging data merged as unverfied, not in data log',
  UserId = (Select UserId from aspnet_Users where LoweredUserName='tim')
where 
  Code = 'QA-98'
Update StatusReason set Code = Replace(Code,'QASR','QAR')
Update StatusReason Set Code = 'QAR-39' where Code = 'QAR-38'
Insert into StatusReason (Code, Name, Description, UserId)
Values ('QAR-38','Historical data not yet verfied','Historical data not yet verfied',(Select UserId from aspnet_Users where loweredusername = 'tim'))
