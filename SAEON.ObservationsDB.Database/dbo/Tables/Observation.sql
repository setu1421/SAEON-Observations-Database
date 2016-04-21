CREATE TABLE [dbo].[Observation] (
    [ID]                    INT              IDENTITY (1, 1) NOT NULL,
    [SensorProcedureID]     UNIQUEIDENTIFIER NOT NULL,
    [ValueDate]             DATETIME         NOT NULL,
    [RawValue]              FLOAT (53)       NULL,
    [DataValue]             FLOAT (53)       NULL,
    [Comment]               VARCHAR (250)    NULL,
--> Changed 2.0.0.3 20160421 TimPN
--    [PhenonmenonOfferingID] UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonOfferingID] UNIQUEIDENTIFIER NOT NULL,
--< Changed 2.0.0.3 20160421 TimPN
--> Changed 2.0.0.3 20160421 TimPN
--    [PhenonmenonUOMID]      UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonUOMID]      UNIQUEIDENTIFIER NOT NULL,
--< Changed 2.0.0.3 20160421 TimPN
    [ImportBatchID]         INT              NOT NULL,
    [UserId]                UNIQUEIDENTIFIER NOT NULL,
    [AddedDate]             DATETIME         CONSTRAINT [DF_Observation_AddedDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Observation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Observation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_Observation_ImportBatch] FOREIGN KEY ([ImportBatchID]) REFERENCES [dbo].[ImportBatch] ([ID]),
    CONSTRAINT [FK_Observation_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_Observation_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
    CONSTRAINT [FK_Observation_SensorProcedure] FOREIGN KEY ([SensorProcedureID]) REFERENCES [dbo].[SensorProcedure] ([ID])
);
GO
CREATE INDEX [IX_Observation] ON [dbo].[Observation]([SensorProcedureID] ASC, [ValueDate] ASC, [RawValue])
GO
--> Changed 20160329 TimPN
--CREATE INDEX [IX_Observation_BatchID] ON [dbo].[Observation]([ImportBatchID])
CREATE INDEX [IX_Observation_ImportBatchID] ON [dbo].[Observation]([ImportBatchID])
--< Changed 20160329 TimPN
--> Added 2.0.0.0 20160406 TimPN
GO
CREATE INDEX [IX_Observation_SensorProcedureID] ON [dbo].[Observation] ([SensorProcedureID])
GO
CREATE INDEX [IX_Observation_PhenomenonOfferingID] ON [dbo].[Observation] ([PhenomenonOfferingID])
GO
CREATE INDEX [IX_Observation_PhenomenonUOMID] ON [dbo].[Observation] ([PhenomenonUOMID])
GO
CREATE INDEX [IX_Observation_UserId] ON [dbo].[Observation] ([UserId])
--< Added 2.0.0.0 20160406 TimPN
