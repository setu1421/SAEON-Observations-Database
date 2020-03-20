Declare @AdminModuleID UniqueIdentifier = (Select ID from Module where Name like 'System Administration')
--Delete RoleModule from RoleModule inner join Module on (RoleModule.ModuleID = Module.ID) where Module.Name = 'Folders'
--Delete Module where Name like 'Folders'
--Delete RoleModule from RoleModule inner join Module on (RoleModule.ModuleID = Module.ID) where Module.Name = 'Create Folders'
--Delete Module where Name like 'Create Folders'
if not Exists(select * from Module where Name like 'Create Folders') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Create Folders','Create folders from template spreadsheet','Admin/CreateFolders',1125,@AdminModuleId,310)
end
Declare @FolderModuleID UniqueIdentifier = (Select ID from Module where Name like 'Create Folders')
Declare @AdminRoleID UniqueIdentifier = (Select RoleID from aspnet_Roles where RoleName like 'Administrator')
Select @AdminModuleID, @AdminRoleID, @FolderModuleID
if not Exists(select * from RoleModule where (ModuleId = @FolderModuleID) and (RoleID = @AdminRoleID))
begin
  Insert into RoleModule
    (ModuleID, RoleId)
  values 
    (@FolderModuleID, @AdminRoleID)
end