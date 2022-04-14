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
	--[DigitalObjectIdentifierID] Int null,
    [Count] INT NOT NULL, 
    [ValueCount] int NOT null,
    [NullCount] as Count - ValueCount,
    [VerifiedCount] INT NOT NULL, 
    [UnverifiedCount] INT NOT NULL, 
    [Minimum] FLOAT NULL, 
    [Maximum] FLOAT NULL, 
    [Average] FLOAT NULL, 
    [StandardDeviation] FLOAT NULL, 
    [Variance] FLOAT NULL,
    [LatitudeNorth] FLOAT NULL, 
    [LatitudeSouth] FLOAT NULL, 
    [LongitudeWest] FLOAT NULL, 
    [LongitudeEast] FLOAT NULL, 
    [ElevationMinimum] FLOAT NULL, 
    [ElevationMaximum] FLOAT NULL, 
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
    --Constraint [FK_ImportBatchSummary_DigitalObjectIdentifierID] Foreign Key ([DigitalObjectIdentifierID]) References [dbo].[DigitalObjectIdentifiers] ([ID]),
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
--CREATE INDEX [IX_ImportBatchSummary_DigitalObjectIdentifierID] ON [dbo].[ImportBatchSummary]([DigitalObjectIdentifierID])

GO
CREATE INDEX [IX_ImportBatchSummary_Count] ON [dbo].[ImportBatchSummary] ([Count])

GO
CREATE INDEX [IX_ImportBatchSummary_ValueCount] ON [dbo].[ImportBatchSummary] ([ValueCount])

GO
CREATE INDEX [IX_ImportBatchSummary_NullCount] ON [dbo].[ImportBatchSummary] ([NullCount])

GO
CREATE INDEX [IX_ImportBatchSummary_VerifiedCount] ON [dbo].[ImportBatchSummary] ([VerifiedCount])

GO
CREATE INDEX [IX_ImportBatchSummary_UnverifiedCount] ON [dbo].[ImportBatchSummary] ([UnverifiedCount])

GO
CREATE INDEX [IX_ImportBatchSummary_StartDate] ON [dbo].[ImportBatchSummary] ([StartDate])

GO
CREATE INDEX [IX_ImportBatchSummary_EndDate] ON [dbo].[ImportBatchSummary] ([EndDate])

GO
CREATE INDEX [IX_ImportBatchSummary_LatitudeSouth] ON [dbo].[ImportBatchSummary] ([LatitudeSouth])

GO
CREATE INDEX [IX_ImportBatchSummary_LatitudeNorth] ON [dbo].[ImportBatchSummary] ([LatitudeNorth])

GO
CREATE INDEX [IX_ImportBatchSummary_LongitudeWest] ON [dbo].[ImportBatchSummary] ([LongitudeWest])

GO
CREATE INDEX [IX_ImportBatchSummary_LongitudeEast] ON [dbo].[ImportBatchSummary] ([LongitudeEast])

GO
CREATE INDEX [IX_ImportBatchSummary_ElevationMinimum] ON [dbo].[ImportBatchSummary] ([ElevationMinimum])

GO
CREATE INDEX [IX_ImportBatchSummary_ElevationMaximum] ON [dbo].[ImportBatchSummary] ([ElevationMaximum])