CREATE TABLE [dbo].[DataSourceTransformation] (
    [ID]                      UNIQUEIDENTIFIER CONSTRAINT [DF_DataSourceTransformation_ID] DEFAULT (newid()) NOT NULL,
    [TransformationTypeID]    UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonID]            UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonOfferingID]    UNIQUEIDENTIFIER NULL,
    [PhenomenonUOMID]         UNIQUEIDENTIFIER NULL,
    [StartDate]				  DATETIME         NULL,
    [EndDate]				  DATETIME         NULL,
    [DataSourceID]            UNIQUEIDENTIFIER NOT NULL,
    [Definition]              TEXT             NOT NULL,
    [ParamA] FLOAT NULL, 
    [ParamB] FLOAT NULL, 
    [ParamC] FLOAT NULL, 
    [ParamD] FLOAT NULL, 
    [ParamE] FLOAT NULL, 
    [ParamF] FLOAT NULL, 
    [ParamG] FLOAT NULL, 
    [ParamH] FLOAT NULL, 
    [ParamI] FLOAT NULL, 
    [ParamJ] FLOAT NULL, 
    [ParamK] FLOAT NULL, 
    [ParamL] FLOAT NULL, 
    [ParamM] FLOAT NULL, 
    [ParamN] FLOAT NULL, 
    [ParamO] FLOAT NULL, 
    [ParamP] FLOAT NULL, 
    [ParamQ] FLOAT NULL, 
    [ParamR] FLOAT NULL, 
    [ParamS] FLOAT NULL, 
    [ParamT] FLOAT NULL, 
    [ParamU] FLOAT NULL, 
    [ParamV] FLOAT NULL, 
    [ParamW] FLOAT NULL, 
    [ParamX] FLOAT NULL, 
    [ParamY] FLOAT NULL, 
    [NewPhenomenonOfferingID] UNIQUEIDENTIFIER NULL,
    [NewPhenomenonUOMID]      UNIQUEIDENTIFIER NULL,
    [Rank]                    INT              CONSTRAINT [DF_DataSourceTransformation_Rank] DEFAULT ((0)) NULL,
    [SensorID]				  UNIQUEIDENTIFIER NULL,
    [UserId] UNIQUEIDENTIFIER NULL, 
    [AddedAt] DATETIME NULL CONSTRAINT [DF_DataSourceTransformation_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_DataSourceTransformation_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_DataSourceTransformation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_Phenomenon] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_TransformationType] FOREIGN KEY ([TransformationTypeID]) REFERENCES [dbo].[TransformationType] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_NewPhenomenonOffering] FOREIGN KEY ([NewPhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_NewPhenomenonUOM] FOREIGN KEY ([NewPhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_Sensor] FOREIGN KEY ([SensorID]) REFERENCES [dbo].[Sensor] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_DataSourceTransformation] UNIQUE ([DataSourceID], [SensorID], [Rank], [TransformationTypeID], [PhenomenonID], [PhenomenonOfferingID], [PhenomenonUOMID], [NewPhenomenonOfferingID], [NewPhenomenonUOMID], [StartDate], [EndDate])
);
GO
CREATE INDEX [IX_DataSourceTransformation_DataSourceID] ON [dbo].[DataSourceTransformation] ([DataSourceID])
GO
CREATE INDEX [IX_DataSourceTransformation_TransformationTypeID] ON [dbo].[DataSourceTransformation] ([TransformationTypeID])
GO
CREATE INDEX [IX_DataSourceTransformation_PhenomenonID] ON [dbo].[DataSourceTransformation] ([PhenomenonID])
GO
CREATE INDEX [IX_DataSourceTransformation_PhenomenonOfferingID] ON [dbo].[DataSourceTransformation] ([PhenomenonOfferingID])
GO
CREATE INDEX [IX_DataSourceTransformation_PhenomenonUOMID] ON [dbo].[DataSourceTransformation] ([PhenomenonUOMID])
GO
CREATE INDEX [IX_DataSourceTransformation_NewPhenomenonOfferingID] ON [dbo].[DataSourceTransformation] ([NewPhenomenonOfferingID])
GO
CREATE INDEX [IX_DataSourceTransformation_NewPhenomenonUOMID] ON [dbo].[DataSourceTransformation] ([NewPhenomenonUOMID])
GO
CREATE INDEX [IX_DataSourceTransformation_UserId] ON [dbo].[DataSourceTransformation] ([UserId])
GO
CREATE INDEX [IX_DataSourceTransformation_SensorID] ON [dbo].[DataSourceTransformation] ([SensorID])
GO
CREATE INDEX [IX_DataSourceTransformation_Rank] ON [dbo].[DataSourceTransformation] ([DataSourceID], [Rank])
GO
CREATE INDEX [IX_DataSourceTransformation_StartDate] ON [dbo].[DataSourceTransformation] ([DataSourceID], [StartDate])
GO
CREATE INDEX [IX_DataSourceTransformation_EndDate] ON [dbo].[DataSourceTransformation] ([DataSourceID], [EndDate])
GO
CREATE TRIGGER [dbo].[TR_DataSourceTransformation_Insert] ON [dbo].[DataSourceTransformation]
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
        DataSourceTransformation src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_DataSourceTransformation_Update] ON [dbo].[DataSourceTransformation]
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
        DataSourceTransformation src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
