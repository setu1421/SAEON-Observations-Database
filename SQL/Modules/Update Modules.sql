--use Observations
Declare @MasterModuleId UniqueIdentifier = (Select ID from Module where Name like 'Master Data Management')
-- New
if not Exists(select * from Module where Name like 'Hidden') 
begin
	Insert into Module
	  (Name, Description, URL, Icon, iOrder)
	values
	  ('Hidden','Hidden','_',7,401)
end
if Exists(select * from Module where Name like 'Programmes') 
  Update Module set ModuleID = @MasterModuleId where Name like 'Programmes'
else
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Programmes','Programmes','/Admin/Programmes',1125,@MasterModuleId,300)
end
if Exists(select * from Module where Name like 'Projects') 
  Update Module set ModuleID = @MasterModuleId where Name like 'Projects'
else
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Projects','Projects','/Admin/Projects',1221,@MasterModuleId,300)
end
if Exists(select * from Module where Name like 'Sites') 
  Update Module set ModuleID = @MasterModuleId where Name like 'Sites'
else
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Sites','Sites','/Admin/Sites',(Select Icon from Module where Name = 'Projects/Sites'),@MasterModuleId,300)
end
if Exists(select * from Module where Name like 'Instruments') 
  Update Module set ModuleID = @MasterModuleId where Name like 'Instruments'
else
begin
	Insert into Module
	  (Name, Description, URL, Icon, ModuleId, iOrder)
	values
	  ('Instruments','Instruments','/Admin/Instruments',1680,@MasterModuleId,300)
end
-- Changes
Update Module set Url = '/Admin/Organisations', ModuleID = @MasterModuleId where Name like 'Organisations'
Update Module set Url = '/Admin/Stations' where Name like 'Stations'
Update Module set Url = '/Admin/Sensors' where Name like 'Sensors'
Update Module set Url = '/Admin/DataSources' where Name like 'Data Sources'
Update Module set Url = '/Admin/DataSchemas' where Name like 'Data Schemas'
Update Module set Url = '/Admin/ImportBatches' where Name like 'Import Batches'
Update Module set Url = '/Admin/Phenomena', Name = 'Phenomena' where Name like 'Phenomenon'
Update Module set Url = '/Admin/Offerings', Name = 'Offerings' where Name like 'Offering%'
Update Module set Url = '/Admin/UnitsOfMeasure', Name = 'Units Of Measure' where Name like 'Unit of measure'
Update Module set Url = '/Admin/Users' where Name like 'Users'
Update Module set Url = '/Admin/Inventory' where Name like 'Inventory'
-- Deletes
Delete RoleModule from RoleModule inner join Module on (RoleModule.ModuleID = Module.ID) where Module.Name like 'Projects/Sites'
Delete Module where Name like 'Projects/Sites'
Delete RoleModule from RoleModule inner join Module on (RoleModule.ModuleID = Module.ID) where Module.Name like 'Organisations '
Delete Module where Name like 'Organisations '
Delete RoleModule from RoleModule inner join Module on (RoleModule.ModuleID = Module.ID) where Module.Name like 'Sites '
Delete Module where Name like 'Sites '
Delete RoleModule from RoleModule inner join Module on (RoleModule.ModuleID = Module.ID) where Module.Name like 'Stations '
Delete Module where Name like 'Stations '
Delete RoleModule from RoleModule inner join Module on (RoleModule.ModuleID = Module.ID) where Module.Name like 'Data Sources '
Delete Module where Name like 'Data Sources '
Delete RoleModule from RoleModule inner join Module on (RoleModule.ModuleID = Module.ID) where Module.Name like 'Sensors '
Delete Module where Name like 'Sensors '
Delete RoleModule from RoleModule inner join Module on (RoleModule.ModuleID = Module.ID) where Module.Name like 'Version 2'
Delete Module where Name like 'Version 2'
Delete RoleModule from RoleModule inner join Module on (RoleModule.ModuleID = Module.ID) where Module.Name like 'Observations'
Delete Module where Name like 'Observations'
-- Changed
Update Module set Url = Replace(Url,'/PLATFORM_TEST/SWDB/Admin/','/Admin/') where Url like '/PLATFORM_TEST/SWDB/Admin/%'
Update Module set Url = Replace(Url,'/Observations/Admin/','/Admin/') where Url like '/Observations/Admin/%'
Update Module set Url = Replace(Url,'/ObservationsAdmin/Admin/','/Admin/') where Url like '/ObservationsAdmin/Admin/%'
Update Module set Url = Replace(Url,'/ObservationsDBv1Live/Admin/','/Admin/') where Url like '/ObservationsDBv1Live/Admin/%'
Update Module set Url = Replace(Url,'/ObservationsDBv1Staging/Admin/','/Admin/') where Url like '/ObservationsDBv1Staging/Admin/%'
Update Module set Url = Replace(Url,'/Admin/','Admin/') where Url like '/Admin/%'
Update Module set Url = Replace(Replace(Url,'.aspx',''),'//','/')
-- Hidden
--Declare @HiddenModuleId UniqueIdentifier = (Select ID from Module where Name like 'Hidden')
--Update Module set ModuleID = @HiddenModuleId where Name like 'Data Sources'
--Update Module set ModuleID = @HiddenModuleId where Name like 'Data Schemas'
--Update Module set ModuleID = @HiddenModuleId where Name like 'Import batches'
-- Unhidden
Update Module set ModuleID = @MasterModuleId where Name like 'Import batches'
-- Update order
Update Module set iOrder = 10 where Name like 'Data Views'
Update Module set iOrder = 20 where Name like 'Master Data Management'
Update Module set iOrder = 30 where Name like 'System Administration'
Update Module set iOrder = 100 where Name like 'Observations'
Update Module set iOrder = 105 where Name like 'Data Query Display'
Update Module set iOrder = 110 where Name like 'Inventory'
Update Module set iOrder = 200 where Name like 'Organisations'
Update Module set iOrder = 205, Icon = 1125 where Name like 'Programmes'
Update Module set iOrder = 210, Icon = 1221 where Name like 'Projects'
Update Module set iOrder = 215, Icon = 1056 where Name like 'Sites'
Update Module set iOrder = 220 where Name like 'Stations'
Update Module set iOrder = 225 where Name like 'Instruments'
Update Module set iOrder = 230 where Name like 'Sensors'
Update Module set iOrder = 235 where Name like 'Data Sources'
Update Module set iOrder = 240 where Name like 'Data Schemas'
Update Module set iOrder = 245 where Name like 'Phenomena'
Update Module set iOrder = 250 where Name like 'Offerings'
Update Module set iOrder = 255 where Name like 'Units of measure'
Update Module set iOrder = 260 where Name like 'Import batches'
Update Module set iOrder = 300 where Name like 'Roles'
Update Module set iOrder = 305 where Name like 'Users'
Update Module set iOrder = 400 where Name like 'Hidden' 
