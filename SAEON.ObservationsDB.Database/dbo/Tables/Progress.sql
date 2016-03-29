CREATE TABLE [dbo].[Progress] (
--> Changed 20160329 TimPN
--    [ImportBatchID]         INT              NULL,
--    [StartDate]             DATETIME         NULL,
--    [EndDate]               DATETIME         NULL,
--    [DateUploaded]          DATETIME         NULL,
--    [Observations]          BIGINT           NULL,
--    [UserId]                UNIQUEIDENTIFIER NULL,
--    [SensorProcedureID]     UNIQUEIDENTIFIER NULL,
--    [PhenonmenonOfferingID] UNIQUEIDENTIFIER NULL
    [ImportBatchID]         INT              NOT NULL,
    [StartDate]             DATETIME         NULL,
    [EndDate]               DATETIME         NULL,
    [DateUploaded]          DATETIME         NULL,
    [Observations]          BIGINT           NULL,
    [UserId]                UNIQUEIDENTIFIER NOT NULL,
    [SensorProcedureID]     UNIQUEIDENTIFIER NOT NULL,
    [PhenonmenonOfferingID] UNIQUEIDENTIFIER NOT NULL,
--< Changed 20160329 TimPN
--> Added 20160329 TimPN
    CONSTRAINT [FK_Progress_ImportBatch] FOREIGN KEY ([ImportBatchID]) REFERENCES [ImportBatch]([ID]),
    CONSTRAINT [FK_Progress_SensorProcedure] FOREIGN KEY ([SensorProcedureID]) REFERENCES [SensorProcedure]([ID]),
    CONSTRAINT [FK_Progress_PhenomenonOffering] FOREIGN KEY ([PhenonmenonOfferingID]) REFERENCES [PhenomenonOffering]([ID]),
    CONSTRAINT [FK_Progress_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
--< Added 20160329 TimPN
);
--> Added 20160329 TimPN
GO
CREATE INDEX [IX_Progress_ImportBatchID] ON [dbo].[Progress] ([ImportBatchID])
GO
CREATE INDEX [IX_Progress_SensorProcedureID] ON [dbo].[Progress] ([SensorProcedureID])
GO
CREATE INDEX [IX_Progress_PhenomenonOfferingID] ON [dbo].[Progress] ([PhenonmenonOfferingID])
GO
CREATE INDEX [IX_Progress_UserId] ON [dbo].[Progress] ([UserId])
--< Added 20160329 TimPN
