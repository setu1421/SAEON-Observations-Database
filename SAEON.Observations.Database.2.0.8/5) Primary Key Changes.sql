-- 1) Add ID Guid columns
print 'Add DataLog.Guid'
go
alter table DataLog add Guid UniqueIdentifier constraint DF_DataLog_ID default newid() not null
go
print 'Add DataLog.UX_DataLog_Guid'
go
alter table DataLog add constraint UX_DataLog_Guid unique (Guid)
go
print 'Add Observation.Guid'
go
alter table Observation add Guid UniqueIdentifier constraint DF_Observation_ID default newid() not null
go
print 'Add Observation.UX_Observation_Guid'
go
alter table Observation add constraint UX_Observation_Guid unique (Guid)
go
print 'Add ImportBatch.DF_ImportBatch_Guid'
go
alter table ImportBatch add constraint DF_ImportBatch_Guid default newid() for Guid
go
print 'Add ImportBatch.UX_ImportBatch_Guid'
go
alter table ImportBatch add constraint UX_ImportBatch_Guid unique (Guid)
go
print '1 Done'
go
-- 2) Add ID Guid foreign keys
print 'Add DataLog.ImportBatchGuid'
go
alter table DataLog add ImportBatchGuid UniqueIdentifier null
go
print 'Add DataLog.FK_DataLog_ImportBatchGuid'
go
alter table DataLog add constraint FK_DataLog_ImportBatchGuid foreign key (ImportBatchGuid) references ImportBatch (Guid)
go
print 'Add Observation.ImportBatchGuid'
go
alter table Observation add ImportBatchGuid UniqueIdentifier null
go
print 'Add Observation.FK_Observation_ImportBatchGuid'
go
alter table Observation add constraint FK_Observation_ImportBatchGuid foreign key (ImportBatchGuid) references ImportBatch (Guid)
go
print '2 Done'
go
-- 3) Copy data to new columns
print 'Update DataLog.ImportBatchGuid'
go
Update DataLog Set ImportBatchGuid = (Select Guid from ImportBatch where ImportBatch.ID = ImportBatchID)
go
print 'Set DataLog.ImportBatchGuid not null'
go
alter table DataLog alter column ImportBatchGuid UniqueIdentifier not null
go
print 'Update Observation.ImportBatchGuid'
go
Update Observation Set ImportBatchGuid = (Select Guid from ImportBatch where ImportBatch.ID = ImportBatchID)
go
print 'Set Observation.ImportBatchGuid not null'
go
alter table Observation alter column ImportBatchGuid UniqueIdentifier not null
go
print '3 Done'
go
-- 4)
-- Check DataLog, Observations data copied
print '4 Done'
go
-- 5) Primary key changes
-- DataLog
print 'Drop DataLog.PK_DataLog'
go
alter table DataLog drop constraint PK_DataLog
go
print 'Drop DataLog.ID'
go
alter table DataLog drop column ID
go
print 'Rename DataLog.Guid to ID'
go
EXEC sp_rename @objname = 'DataLog.Guid', @newname = 'ID', @objtype = 'COLUMN'
go
print 'Add DataLog.PK_DataLog'
go
alter table DataLog add constraint PK_DataLog primary key nonclustered (ID)
go
-- Observations
print 'Drop Observation.PK_Observation'
go
alter table Observation drop constraint PK_Observation
go
print 'Drop Observation.ID'
go
alter table Observation drop column ID
go
print 'Rename Observation.Guid to ID'
go
EXEC sp_rename @objname = 'Observation.Guid', @newname = 'ID', @objtype = 'COLUMN'
go
print 'Add Observation.PK_Observation'
go
alter table Observation add constraint PK_Observation primary key nonclustered (ID)
go
-- ImportBatch
print 'Drop DataLog.FK_DataLog_ImportBatch'
go
alter table DataLog drop constraint FK_DataLog_ImportBatch
go
print 'Drop DataLog.FK_DataLog_ImportBatchGuid'
go
alter table DataLog drop constraint FK_DataLog_ImportBatchGuid
go
print 'Drop DataLog.UX_DataLog_Guid'
go
alter table DataLog drop constraint UX_DataLog_Guid
go
print 'Drop Observation.FK_Observation_ImportBatch'
go
alter table Observation drop constraint FK_Observation_ImportBatch
go
print 'Drop Observation.FK_Observation_Observation'
go
alter table Observation drop constraint FK_Observation_Observation
go
print 'Drop Observation.FK_Observation_ImportBatchGuid'
go
alter table Observation drop constraint FK_Observation_ImportBatchGuid
go
print 'Drop Observation.UX_Observation_Guid'
go
alter table Observation drop constraint UX_Observation_Guid
go
print 'Drop ImportBatch.FK_ImportBatch_ImportBatch'
go
alter table ImportBatch drop constraint FK_ImportBatch_ImportBatch
go
print 'Drop ImportBatch.PK_ImportBatch'
go
alter table ImportBatch drop constraint PK_ImportBatch
go
print 'Drop ImportBatch.UX_ImportBatch_Guid'
go
alter table ImportBatch drop constraint UX_ImportBatch_Guid
go
print 'Drop ImportBatch.ID'
go
alter table ImportBatch drop column ID
go
print 'Rename ImportBatch.Guid to ID'
go
EXEC sp_rename @objname = 'ImportBatch.Guid', @newname = 'ID', @objtype = 'COLUMN'
go
print 'Add ImportBatch.PK_ImportBatch'
go
alter table ImportBatch add constraint PK_ImportBatch primary key nonclustered (ID)
go
print 'Drop DataLog.IX_DataLog'
go
drop index DataLog.IX_DataLog
go
print 'Drop DataLog.ImportBatchID'
go
alter table DataLog drop column ImportBatchID
go
print 'Rename DataLog.ImportBatchGuid to ImportBatchID'
go
EXEC sp_rename @objname = 'DataLog.ImportBatchGuid', @newname = 'ImportBatchID', @objtype = 'COLUMN'
go
print 'Add DataLog.FK_DataLog_ImportBatchID'
go
alter table DataLog add constraint FK_DataLog_ImportBatchID foreign key (ImportBatchID) references ImportBatch (ID)
go
print 'Drop Observation.IX_Observation'
go
drop index Observation.IX_Observation
go
print 'Drop Observation.IX_Observation_BatchID'
go
drop index Observation.IX_Observation_BatchID
go
print 'Drop Observation.IX_Observation_ImportBatchID'
go
drop index Observation.IX_Observation_ImportBatchID
go
print 'Drop Observation.IX_Observation_Natural'
go
drop index Observation.IX_Observation_Natural
go
print 'Drop Observation.ImportBatchID'
go
alter table Observation drop column ImportBatchID
go
print 'Rename Observation.ImportBatchGuid to ImportBatchID'
go
EXEC sp_rename @objname = 'Observation.ImportBatchGuid', @newname = 'ImportBatchID', @objtype = 'COLUMN'
go
print 'Add Observation.FK_Observation_ImportBatchID'
go
alter table Observation add constraint FK_Observation_ImportBatchID foreign key (ImportBatchID) references ImportBatch (ID)
go
print '5 Done'
go
