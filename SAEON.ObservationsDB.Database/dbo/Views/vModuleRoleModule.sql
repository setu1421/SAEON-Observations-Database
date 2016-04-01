CREATE VIEW dbo.vModuleRoleModule
AS
SELECT     TOP (100) PERCENT dbo.RoleModule.ID, dbo.RoleModule.RoleId, dbo.RoleModule.ModuleID, dbo.Module.Name, dbo.Module.Description, dbo.Module.Url, 
                      dbo.Module.Icon, dbo.Module.ModuleID AS BaseModuleID
FROM         dbo.RoleModule INNER JOIN
                      dbo.Module ON dbo.RoleModule.ModuleID = dbo.Module.ID
ORDER BY dbo.RoleModule.RoleId
