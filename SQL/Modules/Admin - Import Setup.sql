Declare @AdminModuleID UniqueIdentifier = (Select ID from Module where Name like 'System Administration')
--Delete RoleModule from RoleModule inner join Module on (RoleModule.ModuleID = Module.ID) where Module.Name = 'Import Setup'
--Delete Module where Name like 'Import Setup'
if not Exists(select * from Module where Name like 'Import Setup') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Import Setup','Import setup from template spreadsheet','Admin/ImportSetup',1125,@AdminModuleId,315)
end
Declare @ImportSetupModuleID UniqueIdentifier = (Select ID from Module where Name like 'Import Setup')
Declare @AdminRoleID UniqueIdentifier = (Select RoleID from aspnet_Roles where RoleName like 'Administrator')
Select @AdminModuleID, @AdminRoleID, @ImportSetupModuleID
if not Exists(select * from RoleModule where (ModuleId = @ImportSetupModuleID) and (RoleID = @AdminRoleID))
begin
  Insert into RoleModule
    (ModuleID, RoleId)
  values 
    (@ImportSetupModuleID, @AdminRoleID)
end