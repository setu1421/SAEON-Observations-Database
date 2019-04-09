Declare @AdminModuleID UniqueIdentifier = (Select ID from Module where Name like 'System Administration')
if not Exists(select * from Module where Name like 'Folders') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Folders','Create folders from template','Admin/Folders',1125,@AdminModuleId,310)
end
Declare @FolderModuleID UniqueIdentifier = (Select ID from Module where Name like 'Folders')
Declare @AdminRoleID UniqueIdentifier = (Select RoleID from aspnet_Roles where RoleName like 'Administrator')
Select @AdminModuleID, @AdminRoleID, @FolderModuleID
if not Exists(select * from RoleModule where (ModuleId = @FolderModuleID) and (RoleID = @AdminRoleID))
begin
  Insert into RoleModule
    (ModuleID, RoleId)
  values 
    (@FolderModuleID, @AdminRoleID)
end