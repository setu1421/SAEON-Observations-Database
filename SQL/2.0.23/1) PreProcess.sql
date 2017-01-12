-- AuditLog
print 'AuditLog'
go
disable trigger TR_AuditLog_Update on AuditLog;
Update AuditLog set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update AuditLog set AddedAt = UpdatedAt where AddedAt is null;
Update 
  AuditLog
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from AuditLog) src
where
  (AuditLog.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_AuditLog_Update on AuditLog;
disable trigger TR_DataLog_Update on DataLog;
-- DataLog
print 'DataLog'
go
disable trigger TR_DataLog_Update on DataLog;
Update DataLog set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update DataLog set AddedAt = UpdatedAt where AddedAt is null;
Update 
  DataLog
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt, ValueDate) RowNum from DataLog) src
where
  (DataLog.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_DataLog_Update on DataLog;
-- DataSchema
print 'DataSchema'
go
disable trigger TR_DataSchema_Update on DataSchema;
Update DataSchema set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update DataSchema set AddedAt = UpdatedAt where AddedAt is null;
Update 
  DataSchema
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from DataSchema) src
where
  (DataSchema.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_DataSchema_Update on DataSchema;
-- DataSource
print 'DataSource'
go
disable trigger TR_DataSource_Update on DataSource;
Update DataSource set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update DataSource set AddedAt = UpdatedAt where AddedAt is null;
Update 
  DataSource
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from DataSource) src
where
  (DataSource.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_DataSource_Update on DataSource;
-- DataSourceRole
print 'DataSourceRole'
go
disable trigger TR_DataSourceRole_Update on DataSourceRole;
Update DataSourceRole set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update DataSourceRole set AddedAt = UpdatedAt where AddedAt is null;
Update 
  DataSourceRole
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from DataSourceRole) src
where
  (DataSourceRole.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_DataSourceRole_Update on DataSourceRole;
-- DataSourceTransformation
print 'DataSourceTransformation'
go
disable trigger TR_DataSourceTransformation_Update on DataSourceTransformation;
Update DataSourceTransformation set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update DataSourceTransformation set AddedAt = UpdatedAt where AddedAt is null;
Update 
  DataSourceTransformation
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from DataSourceTransformation) src
where
  (DataSourceTransformation.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_DataSourceTransformation_Update on DataSourceTransformation;
-- DataSourceType
print 'DataSourceType'
go
disable trigger TR_DataSourceType_Update on DataSourceType;
Update DataSourceType set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update DataSourceType set AddedAt = UpdatedAt where AddedAt is null;
Update 
  DataSourceType
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from DataSourceType) src
where
  (DataSourceType.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_DataSourceType_Update on DataSourceType;
-- ImportBatch
print 'ImportBatch'
go
disable trigger TR_ImportBatch_Update on ImportBatch;
Update ImportBatch set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update ImportBatch set AddedAt = UpdatedAt where AddedAt is null;
Update 
  ImportBatch
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from ImportBatch) src
where
  (ImportBatch.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_ImportBatch_Update on ImportBatch;
-- Instrument
print 'Instrument'
go
disable trigger TR_Instrument_Update on Instrument;
Update Instrument set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Instrument set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Instrument
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Instrument) src
where
  (Instrument.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Instrument_Update on Instrument;
-- Instrument_DataSource
print 'Instrument_DataSource'
go
disable trigger TR_Instrument_DataSource_Update on Instrument_DataSource;
Update Instrument_DataSource set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Instrument_DataSource set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Instrument_DataSource
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Instrument_DataSource) src
where
  (Instrument_DataSource.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Instrument_DataSource_Update on Instrument_DataSource;
-- Instrument_Sensor
print 'Instrument_Sensor'
go
disable trigger TR_Instrument_Sensor_Update on Instrument_Sensor;
Update Instrument_Sensor set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Instrument_Sensor set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Instrument_Sensor
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Instrument_Sensor) src
where
  (Instrument_Sensor.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Instrument_Sensor_Update on Instrument_Sensor;
-- Observation
print 'Observation'
go
disable trigger TR_Observation_Update on Observation;
Update Observation set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Observation set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Observation
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt, ValueDate) RowNum from Observation) src
where
  (Observation.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Observation_Update on Observation;
-- Offering
print 'Offering'
go
disable trigger TR_Offering_Update on Offering;
Update Offering set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Offering set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Offering
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Offering) src
where
  (Offering.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Offering_Update on Offering;
-- Organisation
print 'Organisation'
go
disable trigger TR_Organisation_Update on Organisation;
Update Organisation set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Organisation set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Organisation
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Organisation) src
where
  (Organisation.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Organisation_Update on Organisation;
-- Organisation_Instrument
print 'Organisation_Instrument'
go
disable trigger TR_Organisation_Instrument_Update on Organisation_Instrument;
Update Organisation_Instrument set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Organisation_Instrument set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Organisation_Instrument
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Organisation_Instrument) src
where
  (Organisation_Instrument.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Organisation_Instrument_Update on Organisation_Instrument;
-- Organisation_Site
print 'Organisation_Site'
go
disable trigger TR_Organisation_Site_Update on Organisation_Site;
Update Organisation_Site set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Organisation_Site set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Organisation_Site
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Organisation_Site) src
where
  (Organisation_Site.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Organisation_Site_Update on Organisation_Site;
-- Organisation_Station
print 'Organisation_Station'
go
disable trigger TR_Organisation_Station_Update on Organisation_Station;
Update Organisation_Station set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Organisation_Station set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Organisation_Station
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Organisation_Station) src
where
  (Organisation_Station.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Organisation_Station_Update on Organisation_Station;
-- OrganisationRole
print 'OrganisationRole'
go
disable trigger TR_OrganisationRole_Update on OrganisationRole;
Update OrganisationRole set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update OrganisationRole set AddedAt = UpdatedAt where AddedAt is null;
Update 
  OrganisationRole
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from OrganisationRole) src
where
  (OrganisationRole.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_OrganisationRole_Update on OrganisationRole;
-- Phenomenon
print 'Phenomenon'
go
disable trigger TR_Phenomenon_Update on Phenomenon;
Update Phenomenon set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Phenomenon set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Phenomenon
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Phenomenon) src
where
  (Phenomenon.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Phenomenon_Update on Phenomenon;
-- PhenomenonOffering
print 'PhenomenonOffering'
go
disable trigger TR_PhenomenonOffering_Update on PhenomenonOffering;
Update PhenomenonOffering set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update PhenomenonOffering set AddedAt = UpdatedAt where AddedAt is null;
Update 
  PhenomenonOffering
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from PhenomenonOffering) src
where
  (PhenomenonOffering.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_PhenomenonOffering_Update on PhenomenonOffering;
-- PhenomenonUOM
print 'PhenomenonUOM'
go
disable trigger TR_PhenomenonUOM_Update on PhenomenonUOM;
Update PhenomenonUOM set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update PhenomenonUOM set AddedAt = UpdatedAt where AddedAt is null;
Update 
  PhenomenonUOM
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from PhenomenonUOM) src
where
  (PhenomenonUOM.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_PhenomenonUOM_Update on PhenomenonUOM;
-- Programme
print 'Programme'
go
disable trigger TR_Programme_Update on Programme;
Update Programme set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Programme set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Programme
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Programme) src
where
  (Programme.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Programme_Update on Programme;
-- Project
print 'Project'
go
disable trigger TR_Project_Update on Project;
Update Project set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Project set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Project
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Project) src
where
  (Project.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Project_Update on Project;
-- Project_Station
print 'Project_Station'
go
disable trigger TR_Project_Station_Update on Project_Station;
Update Project_Station set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Project_Station set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Project_Station
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Project_Station) src
where
  (Project_Station.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Project_Station_Update on Project_Station;
-- SchemaColumn
print 'SchemaColumn'
go
disable trigger TR_SchemaColumn_Update on SchemaColumn;
Update SchemaColumn set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update SchemaColumn set AddedAt = UpdatedAt where AddedAt is null;
Update 
  SchemaColumn
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from SchemaColumn) src
where
  (SchemaColumn.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_SchemaColumn_Update on SchemaColumn;
-- SchemaColumnType
print 'SchemaColumnType'
go
disable trigger TR_SchemaColumnType_Update on SchemaColumnType;
Update SchemaColumnType set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update SchemaColumnType set AddedAt = UpdatedAt where AddedAt is null;
Update 
  SchemaColumnType
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from SchemaColumnType) src
where
  (SchemaColumnType.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_SchemaColumnType_Update on SchemaColumnType;
-- Sensor
print 'Sensor'
go
disable trigger TR_Sensor_Update on Sensor;
Update Sensor set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Sensor set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Sensor
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Sensor) src
where
  (Sensor.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Sensor_Update on Sensor;
-- [Site]
print 'Site'
go
disable trigger TR_Site_Update on Site;
Update Site set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Site set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Site
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Site) src
where
  (Site.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Site_Update on Site;
-- Station
print 'Station'
go
disable trigger TR_Station_Update on Station;
Update Station set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Station set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Station
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Station) src
where
  (Station.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Station_Update on Station;
-- Station_Instrument
print 'Station_Instrument'
go
disable trigger TR_Station_Instrument_Update on Station_Instrument;
Update Station_Instrument set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Station_Instrument set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Station_Instrument
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Station_Instrument) src
where
  (Station_Instrument.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Station_Instrument_Update on Station_Instrument;
-- Status
print 'Status'
go
disable trigger TR_Status_Update on Status;
Update Status set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update Status set AddedAt = UpdatedAt where AddedAt is null;
Update 
  Status
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from Status) src
where
  (Status.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_Status_Update on Status;
-- StatusReason
print 'StatusReason'
go
disable trigger TR_StatusReason_Update on StatusReason;
Update StatusReason set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update StatusReason set AddedAt = UpdatedAt where AddedAt is null;
Update 
  StatusReason
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from StatusReason) src
where
  (StatusReason.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_StatusReason_Update on StatusReason;
-- TransformationType
print 'TransformationType'
go
disable trigger TR_TransformationType_Update on TransformationType;
Update TransformationType set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update TransformationType set AddedAt = UpdatedAt where AddedAt is null;
Update 
  TransformationType
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from TransformationType) src
where
  (TransformationType.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_TransformationType_Update on TransformationType;
-- UnitOfMeasure
print 'UnitOfMeasure'
go
disable trigger TR_UnitOfMeasure_Update on UnitOfMeasure;
Update UnitOfMeasure set UpdatedAt = GetDate() where AddedAt is null and UpdatedAt is Null
Update UnitOfMeasure set AddedAt = UpdatedAt where AddedAt is null;
Update 
  UnitOfMeasure
Set
  AddedAt = DATEADD(ms,(RowNum-1)*10,AddedAt)
from
  (Select ID, ROW_NUMBER() OVER (Partition By AddedAt Order By AddedAt) RowNum from UnitOfMeasure) src
where
  (UnitOfMeasure.ID = src.ID) and (src.RowNum > 1);
enable trigger TR_UnitOfMeasure_Update on UnitOfMeasure;
print 'Done'
