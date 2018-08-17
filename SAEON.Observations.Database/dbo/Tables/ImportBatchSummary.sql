CREATE TABLE [dbo].[ImportBatchSummary]
(
    [ID] UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL CONSTRAINT [DF_ImportBatchSummary_ID] DEFAULT (newid()), 
    [ImportBatchID] UNIQUEIDENTIFIER NOT NULL, 
    [SensorID] UNIQUEIDENTIFIER NOT NULL, 
    [InstrumentID] UNIQUEIDENTIFIER NOT NULL, 
    [StationID] UNIQUEIDENTIFIER NOT NULL, 
    [SiteID] UNIQUEIDENTIFIER NOT NULL, 
    [PhenomenonOfferingID] UNIQUEIDENTIFIER NOT NULL, 
    [PhenomenonUOMID] UNIQUEIDENTIFIER NOT NULL, 
    [Count] INT NOT NULL, 
    [Minimum] FLOAT NULL, 
    [Maximum] FLOAT NULL, 
    [Average] FLOAT NULL, 
    [StandardDeviation] FLOAT NULL, 
    [Variance] FLOAT NULL,
    [TopLatitude] FLOAT NULL, 
    [BottomLatitude] FLOAT NULL, 
    [LeftLongitude] FLOAT NULL, 
    [RightLongitude] FLOAT NULL, 
    [StartDate] DATETIME NULL, 
    [EndDate] DATETIME NULL, 
    CONSTRAINT [PK_ImportBatchSummary] PRIMARY KEY CLUSTERED ([ID]), 
    CONSTRAINT [FK_ImportBatchSummary_ImportBatchID] FOREIGN KEY ([ImportBatchID]) REFERENCES [ImportBatch]([ID]), 
    CONSTRAINT [FK_ImportBatchSummary_SensorID] FOREIGN KEY ([SensorID]) REFERENCES [Sensor]([ID]), 
    CONSTRAINT [FK_ImportBatchSummary_InstrumentID] FOREIGN KEY ([InstrumentID]) REFERENCES [Instrument]([ID]), 
    CONSTRAINT [FK_ImportBatchSummary_StationID] FOREIGN KEY ([StationID]) REFERENCES [Station]([ID]), 
    CONSTRAINT [FK_ImportBatchSummary_SiteID] FOREIGN KEY ([SiteID]) REFERENCES [Site]([ID]), 
    CONSTRAINT [FK_ImportBatchSummary_PhenomenonOfferingID] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [PhenomenonOffering]([ID]), 
    CONSTRAINT [FK_ImportBatchSummary_PhenomenonUOMID] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [PhenomenonUOM]([ID]),
    CONSTRAINT [UX_ImportBatchSummary] UNIQUE (ImportBatchID,SensorID,PhenomenonOfferingID,PhenomenonUOMID),
)

GO
CREATE INDEX [IX_ImportBatchSummary_ImportBatchID] ON [dbo].[ImportBatchSummary] ([ImportBatchID])

GO
CREATE INDEX [IX_ImportBatchSummary_SensorID] ON [dbo].[ImportBatchSummary] ([SensorID])

GO
CREATE INDEX [IX_ImportBatchSummary_InstrumentID] ON [dbo].[ImportBatchSummary] ([InstrumentID])

GO
CREATE INDEX [IX_ImportBatchSummary_StationID] ON [dbo].[ImportBatchSummary] ([StationID])

GO
CREATE INDEX [IX_ImportBatchSummary_SiteID] ON [dbo].[ImportBatchSummary] ([SiteID])

GO
CREATE INDEX [IX_ImportBatchSummary_PhenomenonOfferingID] ON [dbo].[ImportBatchSummary] ([PhenomenonOfferingID])

GO
CREATE INDEX [IX_ImportBatchSummary_PhenomenonUOMID] ON [dbo].[ImportBatchSummary] ([PhenomenonUOMID])

GO
CREATE INDEX [IX_ImportBatchSummary_Count] ON [dbo].[ImportBatchSummary] ([Count])

GO
CREATE INDEX [IX_ImportBatchSummary_StartDate] ON [dbo].[ImportBatchSummary] ([StartDate])

GO
CREATE INDEX [IX_ImportBatchSummary_EndDate] ON [dbo].[ImportBatchSummary] ([EndDate])
