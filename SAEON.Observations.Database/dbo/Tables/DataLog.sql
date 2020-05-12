CREATE TABLE [dbo].[DataLog] (
    [ID]         UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_DataLog_ID] DEFAULT (newid()),
    [SensorID]          UNIQUEIDENTIFIER NULL,
    [ImportDate]                 DATETIME         CONSTRAINT [DF_DataLog_ImportDate] DEFAULT (getdate()) NOT NULL,
    [ValueDate]                  DATETIME         NULL,
    [ValueTime]                  DATETIME         NULL,
    [ValueText]                  VARCHAR (50)     NOT NULL,
    [TransformValueText]         VARCHAR (50)     NULL,
    [RawValue]                   FLOAT            NULL,
    [DataValue]                  FLOAT            NULL,
    [Comment]                    VARCHAR (250)    NULL,
    [Latitude] Float Null,
    [Longitude] Float Null,
    [Elevation] Float Null,
    [InvalidDateValue]           VARCHAR (50)     NULL,
    [InvalidTimeValue]           VARCHAR (50)     NULL,
    [InvalidOffering]            VARCHAR (50)     NULL,
    [InvalidUOM]                 VARCHAR (50)     NULL,
    [DataSourceTransformationID] UNIQUEIDENTIFIER NULL,
    [StatusID]                   UNIQUEIDENTIFIER NOT NULL,
    [StatusReasonID] UNIQUEIDENTIFIER NULL,
    [ImportStatus]               VARCHAR (500)    NOT NULL,
    [UserId]                     UNIQUEIDENTIFIER NULL,
    [PhenomenonOfferingID]       UNIQUEIDENTIFIER NULL,
    [PhenomenonUOMID]           UNIQUEIDENTIFIER NULL,
    [ImportBatchID]           UNIQUEIDENTIFIER NOT NULL,
    [RawRecordData]              VARCHAR (500)    NULL,
    [RawFieldValue]              VARCHAR (50)     NOT NULL,
    [CorrelationID] UNIQUEIDENTIFIER NULL,
    [ValueDay]             as (CONVERT([date],[ValueDate])),
    [AddedAt] DATETIME NULL CONSTRAINT [DF_DataLog_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_DataLog_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_DataLog] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataLog_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_DataLog_DataSourceTransformation] FOREIGN KEY ([DataSourceTransformationID]) REFERENCES [dbo].[DataSourceTransformation] ([ID]),
    CONSTRAINT [FK_DataLog_ImportBatch] FOREIGN KEY ([ImportBatchID]) REFERENCES [dbo].[ImportBatch] ([ID]),
    CONSTRAINT [FK_DataLog_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_DataLog_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
    CONSTRAINT [FK_DataLog_Sensor] FOREIGN KEY ([SensorID]) REFERENCES [dbo].[Sensor] ([ID]),
    CONSTRAINT [FK_DataLog_Status] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[Status] ([ID]),
    CONSTRAINT [FK_DataLog_StatusReason] FOREIGN KEY ([StatusReasonID]) REFERENCES [dbo].[StatusReason] ([ID]),
    CONSTRAINT [UX_DataLog] Unique ([ImportBatchID], [SensorID], [ValueDate], [RawValue], [PhenomenonOfferingID], [PhenomenonUOMID])
);
GO
CREATE INDEX [IX_DataLog_ImportBatchID] ON [dbo].[DataLog]([ImportBatchID]);
GO
CREATE INDEX [IX_DataLog_SensorID] ON [dbo].[DataLog] ([SensorID]);
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
GO
CREATE INDEX [IX_DataLog_StatusReasonID] ON [dbo].[DataLog] ([StatusReasonID])
GO
CREATE TRIGGER [dbo].[TR_DataLog_Insert] ON [dbo].[DataLog]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
        AddedAt = GetDate(),
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
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        DataLog src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END

