--> Added 2.0.14 20161011 TimPN
CREATE VIEW [dbo].[vDataSourceRole]
AS 
SELECT 
  DataSourceRole.*,
  DataSource.Code DataSourceCode,
  DataSource.Name DataSourceName,
  aspnet_Roles.RoleName ActualRoleName,
  aspnet_Roles.Description RoleDescription
FROM 
  DataSourceRole
  inner join DataSource
    on (DataSourceRole.DataSourceID = DataSource.ID)
  inner join aspnet_Roles
    on (DataSourceRole.RoleId = aspnet_Roles.RoleId)
--< Added 2.0.14 20161011 TimPN
