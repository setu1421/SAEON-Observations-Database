-- 2.0.8
Declare @UrlPrefix varchar(100)
--set @UrlPrefix = '/ObservationsDBv1Live'
--set @UrlPrefix = '/ObservationsDBv1Staging'
--set @UrlPrefix = '/Observations'
--set @UrlPrefix = '/ObservationsTest' 
set @UrlPrefix = ''
-- New
if not Exists(select * from Module where Name like 'Version 2') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, iOrder)
	values
	  ('Version 2','Master Data Management v2','_',484,103)
end
if Exists(select * from Module where Name like 'Organisations ') 
  Update module set Url = @UrlPrefix+'/Admin/Organisations' where Name like 'Organisations '
else
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Organisations ','Organisations',@UrlPrefix+'/Admin/Organisations',(Select Icon from Module where Name like 'Organisations'),(Select ID from Module where Name like 'Version 2'),300)
end
if not Exists(select * from Module where Name like 'Programmes') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Programmes','Programmes',@UrlPrefix+'/Admin/Programmes',1125,(Select ID from Module where Name like 'Version 2'),300)
end
if not Exists(select * from Module where Name like 'Projects') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Projects','Projects',@UrlPrefix+'/Admin/Projects',1221,(Select ID from Module where Name like 'Version 2'),300)
end
if not Exists(select * from Module where Name like 'Sites') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Sites','Sites',@UrlPrefix+'/Admin/Sites',(Select Icon from Module where Name = 'Projects/Sites'),(Select ID from Module where Name = 'Version 2'),300)
end
if not Exists(select * from Module where Name like 'Stations ') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Stations ','Stations',@UrlPrefix+'/Admin/Stations',(Select Icon from Module where Name = 'Stations'),(Select ID from Module where Name = 'Version 2'),300)
end
if not Exists(select * from Module where Name like 'Instruments') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Instruments','Instruments',@UrlPrefix+'/Admin/Instruments',1680,(Select ID from Module where Name like 'Version 2'),300)
end
if not Exists(select * from Module where Name like 'Data Sources ') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Data Sources ','Data Sources',@UrlPrefix+'/Admin/DataSources',(Select Icon from Module where Name like 'Data Sources'),(Select ID from Module where Name like 'Version 2'),300)
end
if not Exists(select * from Module where Name like 'Sensors ') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Sensors ','Sensors',@UrlPrefix+'/Admin/Sensors',(Select Icon from Module where Name like 'Sensors'),(Select ID from Module where Name like 'Version 2'),300)
end
-- Changed
Update Module set Url = Replace(Url,'/PLATFORM_TEST/SWDB/Admin/',@UrlPrefix+'/Admin/') where Url like '/PLATFORM_TEST/SWDB/Admin/%'
Update Module set Url = Replace(Url,'/Admin/',@UrlPrefix+'/Admin/') where Url like '/Admin/%'
Update Module set Url = Replace(Url,'Admin/',@UrlPrefix+'/Admin/') where Url like 'Admin/%'
Update Module set Url = Replace(Replace(Url,'.aspx',''),'//','/')
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
Update Module set iOrder = 300 where Name like 'Organisations '
Update Module set iOrder = 305, Icon = 1125 where Name like 'Programmes'
Update Module set iOrder = 310, Icon = 1221 where Name like 'Projects'
Update Module set iOrder = 315, Icon = 1056 where Name like 'Sites'
Update Module set iOrder = 320 where Name like 'Stations '
Update Module set iOrder = 325 where Name like 'Instruments'
Update Module set iOrder = 330 where Name like 'Sensors '
Update Module set iOrder = 335 where Name like 'Data Sources '
Update Module set iOrder = 400 where Name like 'Roles'
Update Module set iOrder = 405 where Name like 'Users'
-- New roles
Declare @RoleName varchar(100)
Declare @ModuleName varchar(100)
Set @RoleName = 'Administrator'
Set @ModuleName = 'Version 2'
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
Set @RoleName = 'Administrator'
Set @ModuleName = 'Organisations '
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
Set @RoleName = 'Administrator'
Set @ModuleName = 'Programmes'
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
Set @RoleName = 'Administrator'
Set @ModuleName = 'Projects'
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
Set @RoleName = 'Administrator'
Set @ModuleName = 'Sites'
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
Set @RoleName = 'Administrator'
Set @ModuleName = 'Instruments'
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
Set @RoleName = 'Administrator'
Set @ModuleName = 'Sensors '
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
Set @RoleName = 'Administrator'
Set @ModuleName = 'Data Sources '
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
