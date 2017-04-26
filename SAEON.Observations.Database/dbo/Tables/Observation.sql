CREATE TABLE [dbo].[Observation] (
--> Added 2.0.31 20170414 TimPN
    --[ID]		Int not null Constraint DF_Observation_ID Default (Next value for dbo.ObservationsSequence),
    [ID]		Int Identity(1,1) not null,
--< Added 2.0.31 20170414 TimPN
--> Removed 2.0.31 20170414 TimPN
--> Changed 2.0.8 20160720 TimPN
--    [ID]                    INT              IDENTITY (1, 1) NOT NULL,
--    [ID]         UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Observation_ID] DEFAULT newid(),
--< Changed 2.0.8 20160720 TimPN
--> Removed 2.0.31 20170414 TimPN
--> Changed 2.0.3 20160503 TimPN
--    [SensorProcedureID]     UNIQUEIDENTIFIER NOT NULL,
    [SensorID]     UNIQUEIDENTIFIER NOT NULL,
--< Changed 2.0.3 20160503 TimPN
    [ValueDate]             DATETIME         NOT NULL,
--> Added 2.0.31 20170423 TimPN
    [ValueDay]             as Cast(ValueDate as Date),
--< Added 2.0.31 20170423 TimPN
    [RawValue]              FLOAT (53)       NULL,
    [DataValue]             FLOAT (53)       NULL,
--> Changed 2.0.10 20160901 TimPN
--    [Comment]               VARCHAR (250)    NULL,
    [Comment]               VARCHAR (250)    SPARSE NULL,
--> Changed 2.0.10 20160901 TimPN
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
--> Added 2.0.9 20160823 TimPN
    [StatusID] UNIQUEIDENTIFIER NULL,
    [StatusReasonID] UNIQUEIDENTIFIER NULL,
--< Added 2.0.9 20160823 TimPN
--> Added 2.0.15 20161024 TimPN
    [CorrelationID] UNIQUEIDENTIFIER NULL,
--< Added 2.0.15 20161024 TimPN
    [UserId]                UNIQUEIDENTIFIER NOT NULL,
    [AddedDate]             DATETIME         CONSTRAINT [DF_Observation_AddedDate] DEFAULT getdate() NOT NULL,
--> Added 2.0.8 20160718 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Observation_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Observation_UpdatedAt] DEFAULT GetDate(), 
--> Added 2.0.31 20170414 TimPN
    [RowVersion] RowVersion not null
--< Added 2.0.31 20170414 TimPN
--< Added 2.0.8 20160718 TimPN
--> Changed 2.0.8 20160718 TimPN
--    CONSTRAINT [PK_Observation] PRIMARY KEY CLUSTERED ([ID]),
--> Changed 2.0.13 20161011 TimPN
--    CONSTRAINT [PK_Observation] PRIMARY KEY NONCLUSTERED ([ID]),
--> Changed 2.0.31 20170414 TimPN
--    CONSTRAINT [PK_Observation] PRIMARY KEY NONCLUSTERED ([ID]) on [Observations],
    CONSTRAINT [PK_Observation] PRIMARY KEY CLUSTERED ([ID]) on [Observations],
--< Changed 2.0.31 20170414 TimPN
--< Changed 2.0.13 20161011 TimPN
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
--> Removed 20170301 TimPN
--    CONSTRAINT [UX_Observation] UNIQUE ([SensorID], [ImportBatchID], [ValueDate], [PhenomenonOfferingID], [PhenomenonUOMID]) ON [Observations],
--< Removed 20170301 TimPN
--< Added 2.0.8 20160726 TimPN
--> Added 2.0.9 20160823 TimPN
    CONSTRAINT [FK_Observation_Status] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[Status] ([ID]),
    CONSTRAINT [FK_Observation_StatusReason] FOREIGN KEY ([StatusReasonID]) REFERENCES [dbo].[StatusReason] ([ID]),
--< Added 2.0.9 20160823 TimPN
);
--> Removed 2.0.31 20170414 TimPN
--> Added 2.0.8 20160718 TimPN
--GO
--CREATE CLUSTERED INDEX [CX_Observation] ON [dbo].[Observation] ([AddedAt])
--> Added 2.0.13 20161010 TimPN
--  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
--< Added 2.0.13 20161010 TimPN
--< Added 2.0.8 20160718 TimPN
--< Removed 2.0.31 20170414 TimPN
--> Changed 2.0.3 20160503 TimPN
GO
--CREATE INDEX [IX_Observation] ON [dbo].[Observation]([SensorProcedureID] ASC, [ValueDate] ASC, [RawValue])
CREATE INDEX [IX_Observation] ON [dbo].[Observation]([SensorID] ASC, [ValueDate] ASC, [RawValue])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
--< Added 2.0.13 20161010 TimPN
--< Changed 2.0.3 20160503 TimPN
GO
--> Changed 20160329 TimPN
--CREATE INDEX [IX_Observation_BatchID] ON [dbo].[Observation]([ImportBatchID])
CREATE INDEX [IX_Observation_ImportBatchID] ON [dbo].[Observation]([ImportBatchID])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
--< Added 2.0.13 20161010 TimPN
--< Changed 20160329 TimPN
--> Added 2.0.0 20160406 TimPN
GO
--> Changed 2.0.3 20160503 TimPN
--CREATE INDEX [IX_Observation_SensorProcedureID] ON [dbo].[Observation] ([SensorProcedureID])
CREATE INDEX [IX_Observation_SensorID] ON [dbo].[Observation] ([SensorID])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
--< Added 2.0.13 20161010 TimPN
--< Changed 2.0.3 20160503 TimPN
GO
CREATE INDEX [IX_Observation_PhenomenonOfferingID] ON [dbo].[Observation] ([PhenomenonOfferingID])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
--< Added 2.0.13 20161010 TimPN
GO
CREATE INDEX [IX_Observation_PhenomenonUOMID] ON [dbo].[Observation] ([PhenomenonUOMID])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
--< Added 2.0.13 20161010 TimPN
GO
CREATE INDEX [IX_Observation_UserId] ON [dbo].[Observation] ([UserId])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
--< Added 2.0.13 20161010 TimPN
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160726 TimPN
GO
CREATE INDEX [IX_Observation_AddedDate] ON [dbo].[Observation] ([SensorID], [AddedDate])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
--< Added 2.0.13 20161010 TimPN
--> Changed 2.0.31 20170423 TimPN
GO
--CREATE INDEX [IX_Observation_ValueDate] ON [dbo].[Observation] ([SensorID], [ValueDate])
CREATE INDEX [IX_Observation_SensorID_ValueDate] ON [dbo].[Observation] ([SensorID], [ValueDate])
--> Changed 2.0.31 20170423 TimPN
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
--< Added 2.0.13 20161010 TimPN
--> Added 2.0.31 20170423 TimPN
GO
CREATE INDEX [IX_Observation_ValueDate] ON [dbo].[Observation] ([ValueDate]) ON [Observations];
GO
CREATE INDEX [IX_Observation_ValueDay] ON [dbo].[Observation] ([ValueDay]);
--< Added 2.0.31 20170423 TimPN
--< Added 2.0.8 20160726 TimPN
--> Added 2.0.9 20160823 TimPN
GO
CREATE INDEX [IX_Observation_StatusID] ON [dbo].[Observation] ([StatusID])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
--< Added 2.0.13 20161010 TimPN
GO
CREATE INDEX [IX_Observation_StatusReasonID] ON [dbo].[Observation] ([StatusReasonID])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
--< Added 2.0.13 20161010 TimPN
--< Added 2.0.9 20160823 TimPN
--> Removed 2.0.12 2016107 TimPN
--> Added 2.0.10 20160901 TimPN
--GO
--CREATE INDEX [IX_Observation_Comment] ON [dbo].[Observation] ([Comment]) WHERE Comment is not null
--GO
--CREATE INDEX [IX_Observation_Comment_Null] ON [dbo].[Observation] ([Comment]) WHERE Comment is null
--< Added 2.0.10 20160901 TimPN
--> Removed 2.0.12 2016107 TimPN
--> Added 2.0.8 20160718 TimPN
--> Added 2.0.15 20161024 TimPN
GO
CREATE INDEX [IX_Observation_CorrelationID] ON [dbo].[Observation] ([CorrelationID]) ON [Observations];
--< Added 2.0.15 20161024 TimPN
--> Added 2.0.30 20170329 TimPN
GO
CREATE INDEX [IX_Observation_SensorIDPhenomenonOfferingID] ON [dbo].[Observation] ([SensorID],[PhenomenonOfferingID]) ON [Observations];
--< Added 2.0.30 20170329 TimPN
--> Changed 2.0.15 20161102 TimPN
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
        Observation src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Observation_Update] ON [dbo].[Observation]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
--> Changed 2.0.19 20161205 TimPN
--		AddedAt = del.AddedAt,
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate ()),
--< Changed 2.0.19 20161205 TimPN
        UpdatedAt = GETDATE()
    from
        Observation src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN

