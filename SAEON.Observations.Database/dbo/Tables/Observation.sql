CREATE TABLE [dbo].[Observation] (
    [ID]		Int Identity(1,1) not null Constraint PK_Observation Primary Key Clustered (ID) on Observations,
    [SensorID]     UNIQUEIDENTIFIER NOT NULL,
    [ValueDate]             DATETIME         NOT NULL,
    [TextValue] VarChar(10) Null,
    [RawValue]              FLOAT            NULL,
    [DataValue]             FLOAT            NULL,
    [Comment]               VARCHAR (250)    SPARSE NULL,
    [PhenomenonOfferingID] UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonUOMID]      UNIQUEIDENTIFIER NOT NULL,
    [ImportBatchID]         UNIQUEIDENTIFIER              NOT NULL,
    [StatusID] UNIQUEIDENTIFIER NULL,
    [StatusReasonID] UNIQUEIDENTIFIER NULL,
    [CorrelationID] UNIQUEIDENTIFIER NULL,
    [Latitude] Float Null,
    [Longitude] Float Null,
    [Elevation] Float Null,
    [UserId]                UNIQUEIDENTIFIER NOT NULL,
    [AddedDate]             DATETIME         CONSTRAINT [DF_Observation_AddedDate] DEFAULT (getdate()) NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Observation_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Observation_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    [ValueDay]             as (CONVERT([date],[ValueDate])),
    [ValueYear]             as (Year([ValueDate])),
    [ValueDecade]           as (Year([ValueDate])/10*10),
    [VerifiedBy] UNIQUEIDENTIFIER NULL, 
    [VerifiedAt] DATETIME NULL, 
    [UnverifiedBy] UNIQUEIDENTIFIER NULL, 
    [UnverifiedAt] DATETIME NULL, 
    CONSTRAINT [FK_Observation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_Observation_ImportBatch] FOREIGN KEY ([ImportBatchID]) REFERENCES [dbo].[ImportBatch] ([ID]),
    CONSTRAINT [FK_Observation_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_Observation_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
    CONSTRAINT [FK_Observation_Sensor] FOREIGN KEY ([SensorID]) REFERENCES [dbo].[Sensor] ([ID]),
    CONSTRAINT [FK_Observation_Status] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[Status] ([ID]),
    CONSTRAINT [FK_Observation_StatusReason] FOREIGN KEY ([StatusReasonID]) REFERENCES [dbo].[StatusReason] ([ID]),
    CONSTRAINT [UX_Observation] UNIQUE ([SensorID], [ValueDate], [PhenomenonOfferingID], [PhenomenonUOMID], [Elevation]) on [Observations]
);
GO
CREATE INDEX [IX_Observation_ImportBatchID] ON [dbo].[Observation]([ImportBatchID])
  INCLUDE ([DataValue],[PhenomenonOfferingID],[PhenomenonUOMID],[SensorID],[StatusID],[StatusReasonID],[Elevation],[Latitude],[Longitude],[ValueDate],[ValueDay])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_SensorID] ON [dbo].[Observation] ([SensorID])
  --INCLUDE ([ValueDate],[DataValue],[PhenomenonOfferingID],[PhenomenonUOMID],[ImportBatchID],[StatusID],[StatusReasonID],[Elevation],[Latitude],[Longitude],[ValueDay])
  INCLUDE ([ValueDate],[PhenomenonOfferingID],[PhenomenonUOMID],[ImportBatchID],[StatusID],[StatusReasonID],[Elevation],[Latitude],[Longitude],[ValueDay])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_PhenomenonOfferingID] ON [dbo].[Observation] ([PhenomenonOfferingID])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_PhenomenonUOMID] ON [dbo].[Observation] ([PhenomenonUOMID])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_UserId] ON [dbo].[Observation] ([UserId])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_AddedDate] ON [dbo].[Observation] ([AddedDate])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_ValueDate] ON [dbo].[Observation] ([ValueDate])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_ValueDay] ON [dbo].[Observation] ([ValueDay])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_ValueYear] ON [dbo].[Observation] ([ValueYear])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_ValueDecade] ON [dbo].[Observation] ([ValueDecade])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_StatusID] ON [dbo].[Observation] ([StatusID])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_StatusReasonID] ON [dbo].[Observation] ([StatusReasonID])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_CorrelationID] ON [dbo].[Observation] ([CorrelationID])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_Latitude] ON [dbo].[Observation] ([Latitude])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_Longitude] ON [dbo].[Observation] ([Longitude])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE INDEX [IX_Observation_Elevation] ON [dbo].[Observation] ([Elevation])
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Observations];
GO
CREATE TRIGGER [dbo].[TR_Observation_Insert] ON [dbo].[Observation]
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
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        Observation src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
