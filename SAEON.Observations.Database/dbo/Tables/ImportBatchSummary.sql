--> Added 2.0.38 20180418 TimPN
CREATE TABLE [dbo].[ImportBatchSummary]
(
    [Id] UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL CONSTRAINT [DF_ImportBatchSummary_ID] DEFAULT newid(), 
    [ImportBatchID] UNIQUEIDENTIFIER NOT NULL, 
    [SensorID] UNIQUEIDENTIFIER NOT NULL, 
    [PhenomenonOfferingID] UNIQUEIDENTIFIER NOT NULL, 
    [PhenomenonUOMID] UNIQUEIDENTIFIER NOT NULL, 
    [Count] INT NOT NULL, 
    [Minimum] FLOAT NULL, 
    [Maximum] FLOAT NULL, 
    [Average] FLOAT NULL, 
    [StandardDeviation] FLOAT NULL, 
    [Variance] FLOAT NULL,
    CONSTRAINT [PK_ImportBatchSummary] PRIMARY KEY CLUSTERED ([ID]), 
    CONSTRAINT [FK_ImportBatchSummary_ImportBatch] FOREIGN KEY ([ImportBatchID]) REFERENCES [ImportBatch]([ID]), 
    CONSTRAINT [FK_ImportBatchSummary_SensorID] FOREIGN KEY ([SensorID]) REFERENCES [Sensor]([ID]), 
    CONSTRAINT [FK_ImportBatchSummary_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [PhenomenonOffering]([ID]), 
    CONSTRAINT [FK_ImportBatchSummary_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [PhenomenonUOM]([ID]),
    CONSTRAINT [UX_ImportBatchSummary] UNIQUE (ImportBatchID,SensorID,PhenomenonOfferingID,PhenomenonUOMID),
)

GO
CREATE INDEX [IX_ImportBatchSummary_ImportBatchID] ON [dbo].[ImportBatchSummary] ([ImportBatchID])

GO
CREATE INDEX [IX_ImportBatchSummary_SensorID] ON [dbo].[ImportBatchSummary] ([SensorID])

GO
CREATE INDEX [IX_ImportBatchSummary_PhenomenonOfferingID] ON [dbo].[ImportBatchSummary] ([PhenomenonOfferingID])

GO
CREATE INDEX [IX_ImportBatchSummary_PhenomenonUOMID] ON [dbo].[ImportBatchSummary] ([PhenomenonUOMID])
--< Added 2.0.38 20180418 TimPN

