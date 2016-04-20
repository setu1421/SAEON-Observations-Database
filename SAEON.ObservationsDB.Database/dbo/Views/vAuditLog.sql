CREATE VIEW [dbo].[vAuditLog]
AS
Select
  a.*, u.UserName
from
  AuditLog a
  inner join aspnet_Users u
    on (a.UserId = u.UserId)