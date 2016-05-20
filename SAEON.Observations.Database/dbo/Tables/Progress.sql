CREATE TABLE [dbo].[Progress] (
    [ImportBatchID]         INT              NULL,
    [StartDate]             DATETIME         NULL,
    [EndDate]               DATETIME         NULL,
    [DateUploaded]          DATETIME         NULL,
    [Observations]          BIGINT           NULL,
    [UserId]                UNIQUEIDENTIFIER NULL,
--> Changed 2.0.0.3 20160503 TimPN
--    [SensorProcedureID]     UNIQUEIDENTIFIER NULL,
    [SensorID]     UNIQUEIDENTIFIER NULL,
--< Changed 2.0.0.3 20160503 TimPN
--> Changed 2.0.0.3 20160421 TimPN
    [PhenomenonOfferingID] UNIQUEIDENTIFIER NULL,
--< Changed 2.0.0.3 20160421 TimPN
--< Changed 20160329 TimPN
--> Added 2.0.0.0 20160406 TimPN
    CONSTRAINT [FK_Progress_ImportBatch] FOREIGN KEY ([ImportBatchID]) REFERENCES [ImportBatch]([ID]),
--> Changed 2.0.0.3 20160503 TimPN
--    CONSTRAINT [FK_Progress_SensorProcedure] FOREIGN KEY ([SensorID]) REFERENCES [SensorProcedure]([ID]),
    CONSTRAINT [FK_Progress_Sensor] FOREIGN KEY ([SensorID]) REFERENCES [Sensor]([ID]),
--< Changed 2.0.0.3 20160503 TimPN
    CONSTRAINT [FK_Progress_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [PhenomenonOffering]([ID]),
    CONSTRAINT [FK_Progress_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
--< Added 2.0.0.0 20160406 TimPN
);
--> Added 2.0.0.0 20160406 TimPN
GO
CREATE INDEX [IX_Progress_ImportBatchID] ON [dbo].[Progress] ([ImportBatchID])
GO
--> Changed 2.0.0.3 20160503 TimPN
--CREATE INDEX [IX_Progress_SensorProcedureID] ON [dbo].[Progress] ([SensorProcedureID])
CREATE INDEX [IX_Progress_SensorID] ON [dbo].[Progress] ([SensorID])
--< Changed 2.0.0.3 20160503 TimPN
GO
CREATE INDEX [IX_Progress_PhenomenonOfferingID] ON [dbo].[Progress] ([PhenomenonOfferingID])
GO
CREATE INDEX [IX_Progress_UserId] ON [dbo].[Progress] ([UserId])
--< Added 2.0.0.0 20160406 TimPN
