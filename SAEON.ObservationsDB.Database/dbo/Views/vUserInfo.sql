
CREATE VIEW [dbo].[vUserInfo]
AS
SELECT     dbo.aspnet_Users.UserId, dbo.aspnet_Users.LastActivityDate, dbo.aspnet_Users.UserName, dbo.aspnet_Membership.CreateDate, dbo.aspnet_Membership.Email, 
                      dbo.aspnet_Membership.Comment
FROM         dbo.aspnet_Users INNER JOIN
                      dbo.aspnet_Membership ON dbo.aspnet_Users.UserId = dbo.aspnet_Membership.UserId

GO
