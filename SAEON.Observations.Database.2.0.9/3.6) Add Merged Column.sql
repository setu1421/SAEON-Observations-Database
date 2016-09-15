use ObservationDB_IMP
print 'Adding DataLog.merged'
go
alter table DataLog add Merged bit constraint DF_DataLog_Merged default 0 with values
go
--print 'Setting DataLog.merged'
--go
--Update DataLog set Merged = 0 where Merged is null
--go
print 'Adding IX_DataLog_merged'
go
CREATE INDEX [IX_DataLog_merged] ON [dbo].[DataLog]([Merged])
go
print 'Adding Observation.Merged'
go
alter table Observation add Merged bit constraint DF_DataLog_Merged default 0 with values
go
--print 'Setting Observation.Merged'
--go
--Update Observation set Merged = 0 where Merged is null
--go
print 'Adding IX_Observation_Merged'
go
CREATE INDEX [IX_Observation_Merged] ON [dbo].[Observation]([Merged])
go
