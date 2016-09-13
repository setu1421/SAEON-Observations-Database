use ObservationDB_IMP
print 'Adding DataLog.merged'
go
alter table DataLog add Merged bit
go
print 'Adding IX_DataLog_merged'
go
CREATE INDEX [IX_DataLog_merged] ON [dbo].[DataLog]([Merged])
print 'Adding Observation.Merged'
go
alter table Observation add Merged bit
print 'Adding IX_Observation_Merged'
go
CREATE INDEX [IX_Observation_Merged] ON [dbo].[Observation]([Merged])
go
