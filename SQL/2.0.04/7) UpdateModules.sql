-- 2.0.0.2
Declare @UrlPrefix varchar(100)
--set @UrlPrefix = '/ObservationsDBv2'
--set @UrlPrefix = '/Observations'
set @UrlPrefix = ''
-- New
if not Exists(select * from Module where Name like 'Stations ') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Stations ','Stations',@UrlPrefix+'/Admin/Stations',(Select Icon from Module where Name = 'Stations'),(Select ID from Module where Name = 'Version 2'),300)
end
-- Update order
Update Module set iOrder = 10 where Name like 'Data Views'
Update Module set iOrder = 20 where Name like 'Master Data Management'
Update Module set iOrder = 30 where Name like 'Version 2'
Update Module set iOrder = 40 where Name like 'System Administration'
Update Module set iOrder = 100 where Name like 'Observations'
Update Module set iOrder = 105 where Name like 'Data Query Display'
Update Module set iOrder = 110 where Name like 'Inventory'
Update Module set iOrder = 200 where Name like 'Organisations'
Update Module set iOrder = 205 where Name like 'Projects/Sites'
Update Module set iOrder = 210 where Name like 'Stations'
Update Module set iOrder = 215 where Name like 'Data Sources'
Update Module set iOrder = 220 where Name like 'Data Schemas'
Update Module set iOrder = 225 where Name like 'Sensors'
Update Module set iOrder = 230 where Name like 'Phenomenon'
Update Module set iOrder = 235 where Name like 'Unit of measure'
Update Module set iOrder = 240 where Name like 'Offerings'
Update Module set iOrder = 255 where Name like 'Import batches'
Update Module set iOrder = 300 where Name like 'Sites'
Update Module set iOrder = 305 where Name like 'Stations '
Update Module set iOrder = 400 where Name like 'Roles'
Update Module set iOrder = 405 where Name like 'Users'
-- New roles
Declare @RoleName varchar(100)
Declare @ModuleName varchar(100)
Set @RoleName = 'Administrator'
Set @ModuleName = 'Stations '
if not Exists(
	Select 
	  * 
	from
	  RoleModule rm
	  inner join aspnet_Roles r
		on rm.RoleId = r.RoleId
	  inner join Module m
	    on (rm.ModuleID = m.ID)
	where 
	  (r.RoleName = @RoleName) and
	  (m.Name like @ModuleName)
	)
begin
  Insert into RoleModule (RoleId, ModuleID) values ((Select RoleId from aspnet_Roles where RoleName = @RoleName), (Select Id from Module where Name like @ModuleName))
end
Set @RoleName = 'Data_provider_admin'
if not Exists(
	Select 
	  * 
	from
	  RoleModule rm
	  inner join aspnet_Roles r
		on rm.RoleId = r.RoleId
	  inner join Module m
	    on (rm.ModuleID = m.ID)
	where 
	  (r.RoleName = @RoleName) and
	  (m.Name like @ModuleName)
	)
begin
  Insert into RoleModule (RoleId, ModuleID) values ((Select RoleId from aspnet_Roles where RoleName = @RoleName), (Select Id from Module where Name like @ModuleName))
end
