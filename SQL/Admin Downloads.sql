use Observations
Select 
  AddedAt, UserName, Left(Download,Len(Download)-2) Download
from
(
Select 
  AddedAt, aspnet_Users.UserName, SubString(Description,Len('ASP.admin_dataquery_aspx.ObservationsGridStore_Submit(Log=''')+1, 5000) Download
from 
  AuditLog
  inner join aspnet_Users
    on (AuditLog.UserId = aspnet_Users.UserId)
where
  Description like '%admin_dataquery_aspx%'
) s
order by
  AddedAt Desc