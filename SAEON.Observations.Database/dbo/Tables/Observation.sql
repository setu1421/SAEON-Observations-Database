CREATE TABLE [dbo].[Observation] (
--> Changed 2.0.8 20160720 TimPN
--    [ID]                    INT              IDENTITY (1, 1) NOT NULL,
    [ID]         UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Observation_ID] DEFAULT newid(),
--< Changed 2.0.8 20160720 TimPN
--> Changed 2.0.3 20160503 TimPN
--    [SensorProcedureID]     UNIQUEIDENTIFIER NOT NULL,
    [SensorID]     UNIQUEIDENTIFIER NOT NULL,
--< Changed 2.0.3 20160503 TimPN
    [ValueDate]             DATETIME         NOT NULL,
    [RawValue]              FLOAT (53)       NULL,
    [DataValue]             FLOAT (53)       NULL,
    [Comment]               VARCHAR (250)    NULL,
--> Changed 2.0.3 20160421 TimPN
--    [PhenonmenonOfferingID] UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonOfferingID] UNIQUEIDENTIFIER NOT NULL,
--< Changed 2.0.3 20160421 TimPN
--> Changed 2.0.3 20160421 TimPN
--    [PhenonmenonUOMID]      UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonUOMID]      UNIQUEIDENTIFIER NOT NULL,
--< Changed 2.0.3 20160421 TimPN
--> Changed 2.0.8 20160720 TimPN
--    [ImportBatchID]         INT              NOT NULL,
    [ImportBatchID]         UNIQUEIDENTIFIER              NOT NULL,
--< Changed 2.0.8 20160720 TimPN
    [UserId]                UNIQUEIDENTIFIER NOT NULL,
    [AddedDate]             DATETIME         CONSTRAINT [DF_Observation_AddedDate] DEFAULT getdate() NOT NULL,
--> Added 2.0.8 20160718 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Observation_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Observation_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160718 TimPN
--> Changed 2.0.8 20160718 TimPN
--    CONSTRAINT [PK_Observation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_Observation] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.8 20160718 TimPN
    CONSTRAINT [FK_Observation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_Observation_ImportBatch] FOREIGN KEY ([ImportBatchID]) REFERENCES [dbo].[ImportBatch] ([ID]),
    CONSTRAINT [FK_Observation_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_Observation_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
--> Changed 2.0.3 20160503 TimPN
--    CONSTRAINT [FK_Observation_SensorProcedure] FOREIGN KEY ([SensorProcedureID]) REFERENCES [dbo].[SensorProcedure] ([ID])
    CONSTRAINT [FK_Observation_Sensor] FOREIGN KEY ([SensorID]) REFERENCES [dbo].[Sensor] ([ID]),
--< Changed 2.0.3 20160503 TimPN
--> Added 2.0.8 20160726 TimPN
    CONSTRAINT [UX_Observation] UNIQUE ([SensorID], [ImportBatchID], [ValueDate], [PhenomenonOfferingID], [PhenomenonUOMID])
--< Added 2.0.8 20160726 TimPN
);
--> Added 2.0.8 20160718 TimPN
GO
CREATE CLUSTERED INDEX [CX_Observation] ON [dbo].[Observation] ([AddedAt])
--< Added 2.0.8 20160718 TimPN
--> Changed 2.0.3 20160503 TimPN
GO
--CREATE INDEX [IX_Observation] ON [dbo].[Observation]([SensorProcedureID] ASC, [ValueDate] ASC, [RawValue])
CREATE INDEX [IX_Observation] ON [dbo].[Observation]([SensorID] ASC, [ValueDate] ASC, [RawValue])
--< Changed 2.0.3 20160503 TimPN
GO
--> Changed 20160329 TimPN
--CREATE INDEX [IX_Observation_BatchID] ON [dbo].[Observation]([ImportBatchID])
CREATE INDEX [IX_Observation_ImportBatchID] ON [dbo].[Observation]([ImportBatchID])
--< Changed 20160329 TimPN
--> Added 2.0.0 20160406 TimPN
GO
--> Changed 2.0.3 20160503 TimPN
--CREATE INDEX [IX_Observation_SensorProcedureID] ON [dbo].[Observation] ([SensorProcedureID])
CREATE INDEX [IX_Observation_SensorID] ON [dbo].[Observation] ([SensorID])
--< Changed 2.0.3 20160503 TimPN
GO
CREATE INDEX [IX_Observation_PhenomenonOfferingID] ON [dbo].[Observation] ([PhenomenonOfferingID])
GO
CREATE INDEX [IX_Observation_PhenomenonUOMID] ON [dbo].[Observation] ([PhenomenonUOMID])
GO
CREATE INDEX [IX_Observation_UserId] ON [dbo].[Observation] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160726 TimPN
--< Added 2.0.8 20160726 TimPN
GO
CREATE INDEX [IX_Observation_AddedDate] ON [dbo].[Observation] ([SensorID], [AddedDate])
GO
CREATE INDEX [IX_Observation_ValueDate] ON [dbo].[Observation] ([SensorID], [ValueDate])
--> Added 2.0.8 20160718 TimPN
GO
CREATE TRIGGER [dbo].[TR_Observation_Insert] ON [dbo].[Observation]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        inserted ins
        inner join Observation src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Observation_Update] ON [dbo].[Observation]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    --if UPDATE(AddedAt) RAISERROR ('Cannot update AddedAt.', 16, 1)
    Update
        src
    set
        UpdatedAt = GETDATE()
    from
        inserted ins
        inner join Observation src
            on (ins.ID = src.ID)
END
--< Added 2.0.8 20160718 TimPN

