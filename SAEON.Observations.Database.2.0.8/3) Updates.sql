--alter table DataSourceTransformation add CONSTRAINT [UX_DataSourceTransformation] UNIQUE (DataSourceID, SensorID, Rank, TransformationTypeID, PhenomenonID, PhenomenonOfferingID, PhenomenonUOMID, NewPhenomenonOfferingID, NewPhenomenonUOMID, StartDate, EndDate)
alter table ImportBatch add CONSTRAINT [UX_ImportBatch] UNIQUE (DataSourceID, ImportDate, LogFileName)
--alter table Observation add CONSTRAINT [UX_Observation] UNIQUE ([SensorID], [ImportBatchID], [ValueDate], [PhenomenonOfferingID], [PhenomenonUOMID])
alter table DataSourceRole add CONSTRAINT [UX_DataSourceRole] UNIQUE ([DataSourceId], [RoleId], [DateStart], [DateEnd])

CREATE INDEX [IX_Observation_AddedDate] ON [dbo].[Observation] ([SensorID], [AddedDate])
CREATE INDEX [IX_Observation_ValueDate] ON [dbo].[Observation] ([SensorID], [ValueDate])
create index [IX_Observation_Natural] on Observation ([SensorID], [ImportBatchID], [ValueDate], [PhenomenonOfferingID], [PhenomenonUOMID])

--drop index observation.IX_Observation_AddedDate
--drop index observation.IX_Observation_ValueDate