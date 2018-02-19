CREATE TABLE [dbo].[DataLog] (
--> Changed 2.0.8 20160720 TimPN
--    [ID]                         INT              IDENTITY (1, 1) NOT NULL,
    [ID]         UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_DataLog_ID] DEFAULT newid(),
--< Changed 2.0.8 20160720 TimPN
--> Changed 2.0.3 20160503 TimPN
--    [SensorProcedureID]          UNIQUEIDENTIFIER NULL,
    [SensorID]          UNIQUEIDENTIFIER NULL,
--< Changed 2.0.3 20160503 TimPN
    [ImportDate]                 DATETIME         CONSTRAINT [DF_DataLog_ImportDate] DEFAULT (getdate()) NOT NULL,
    [ValueDate]                  DATETIME         NULL,
    [ValueTime]                  DATETIME         NULL,
--> Added 2.0.31 20170423 TimPN
    [ValueDay]             as Cast(ValueDate as Date),
--< Added 2.0.31 20170423 TimPN
    [ValueText]                  VARCHAR (50)     NOT NULL,
    [TransformValueText]         VARCHAR (50)     NULL,
    [RawValue]                   FLOAT (53)       NULL,
    [DataValue]                  FLOAT (53)       NULL,
    [Comment]                    VARCHAR (250)    NULL,
--> Added 2.0.37 20180215 TimPN
    [Latitude] Float Null,
    [Longitude] Float Null,
    [Elevation] Float Null,
--< Added 2.0.37 20180215 TimPN
    [InvalidDateValue]           VARCHAR (50)     NULL,
    [InvalidTimeValue]           VARCHAR (50)     NULL,
    [InvalidOffering]            VARCHAR (50)     NULL,
    [InvalidUOM]                 VARCHAR (50)     NULL,
    [DataSourceTransformationID] UNIQUEIDENTIFIER NULL,
    [StatusID]                   UNIQUEIDENTIFIER NOT NULL,
--> Added 2.0.9 20160823 TimPN
    [StatusReasonID] UNIQUEIDENTIFIER NULL,
--< Added 2.0.9 20160823 TimPN
    [ImportStatus]               VARCHAR (500)    NOT NULL,
    [UserId]                     UNIQUEIDENTIFIER NULL,
    [PhenomenonOfferingID]       UNIQUEIDENTIFIER NULL,
--> Changed 2.0.3 20160421 TimPN
--    [PhenonmenonUOMID]           UNIQUEIDENTIFIER NULL,
    [PhenomenonUOMID]           UNIQUEIDENTIFIER NULL,
--< Changed 2.0.3 20160421 TimPN
--> Changed 2.0.8 20160720 TimPN
--    [ImportBatchID]              INT              NOT NULL,
    [ImportBatchID]           UNIQUEIDENTIFIER NOT NULL,
--< Changed 2.0.8 20160720 TimPN
    [RawRecordData]              VARCHAR (500)    NULL,
    [RawFieldValue]              VARCHAR (50)     NOT NULL,
--> Added 2.0.15 20161024 TimPN
    [CorrelationID] UNIQUEIDENTIFIER NULL,
--< Added 2.0.15 20161024 TimPN
--> Added 2.0.8 20160708 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_DataLog_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_DataLog_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160708 TimPN
--> Added 2.0.33 20170628 TimPN
    [RowVersion] RowVersion not null,
--< Added 2.0.33 20170628 TimPN
    CONSTRAINT [PK_DataLog] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataLog_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_DataLog_DataSourceTransformation] FOREIGN KEY ([DataSourceTransformationID]) REFERENCES [dbo].[DataSourceTransformation] ([ID]),
    CONSTRAINT [FK_DataLog_ImportBatch] FOREIGN KEY ([ImportBatchID]) REFERENCES [dbo].[ImportBatch] ([ID]),
    CONSTRAINT [FK_DataLog_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_DataLog_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
--> Changed 2.0.3 20160503 TimPN
--    CONSTRAINT [FK_DataLog_SensorProcedure] FOREIGN KEY ([SensorProcedureID]) REFERENCES [dbo].[SensorProcedure] ([ID]),
    CONSTRAINT [FK_DataLog_Sensor] FOREIGN KEY ([SensorID]) REFERENCES [dbo].[Sensor] ([ID]),
--< Changed 2.0.3 20160503 TimPN
    CONSTRAINT [FK_DataLog_Status] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[Status] ([ID]),
--> Added 2.0.9 20160823 TimPN
    CONSTRAINT [FK_DataLog_StatusReason] FOREIGN KEY ([StatusReasonID]) REFERENCES [dbo].[StatusReason] ([ID]),
--< Added 2.0.9 20160823 TimPN
--> Added 2.0.9 20160905 TimPN
    CONSTRAINT [UX_DataLog] Unique ([ImportBatchID], [SensorID], [ValueDate], [RawValue], [PhenomenonOfferingID], [PhenomenonUOMID])
--> Added 2.0.9 20160905 TimPN
);
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_DataLog_ImportBatchID] ON [dbo].[DataLog]([ImportBatchID]);
--> Changed 2.0.3 20160503 TimPN
GO
--CREATE INDEX [IX_DataLog_SensorProcedureID] ON [dbo].[DataLog] ([SensorProcedureID]);
CREATE INDEX [IX_DataLog_SensorID] ON [dbo].[DataLog] ([SensorID]);
--< Changed 2.0.3 20160503 TimPN
--> Added 2.0.31 20170423 TimPN
GO
CREATE INDEX [IX_DataLog_ValueDay] ON [dbo].[DataLog] ([ValueDay]);
--< Added 2.0.31 20170423 TimPN
GO
CREATE INDEX [IX_DataLog_DataSourceTransformationID] ON [dbo].[DataLog] ([DataSourceTransformationID])
GO
CREATE INDEX [IX_DataLog_PhenomenonOfferingID] ON [dbo].[DataLog] ([PhenomenonOfferingID])
GO
CREATE INDEX [IX_DataLog_PhenomenonUOMID] ON [dbo].[DataLog] ([PhenomenonUOMID])
GO
CREATE INDEX [IX_DataLog_StatusID] ON [dbo].[DataLog] ([StatusID])
GO
CREATE INDEX [IX_DataLog_UserId] ON [dbo].[DataLog] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.9 20160823 TimPN
GO
CREATE INDEX [IX_DataLog_StatusReasonID] ON [dbo].[DataLog] ([StatusReasonID])
--< Added 2.0.9 20160823 TimPN
--> Added 2.0.8 20160708 TimPN
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_DataLog_Insert] ON [dbo].[DataLog]
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
        DataLog src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_DataLog_Update] ON [dbo].[DataLog]
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
        DataLog src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160708 TimPN

