CREATE TABLE [dbo].[DataSourceTransformation] (
    [ID]                      UNIQUEIDENTIFIER CONSTRAINT [DF_DataSourceTransformation_ID] DEFAULT (newid()) NOT NULL,
    [TransformationTypeID]    UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonID]            UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonOfferingID]    UNIQUEIDENTIFIER NULL,
    [PhenomenonUOMID]         UNIQUEIDENTIFIER NULL,
--> Changed 2.0.22 20170111 TimPN
--    [StartDate]        DATETIME         NULL,
    [StartDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
--> Changed 2.0.22 20170111 TimPN
--    [EndDate]        DATETIME         NULL,
    [EndDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
    [DataSourceID]            UNIQUEIDENTIFIER NOT NULL,
    [Definition]              TEXT             NOT NULL,
    [NewPhenomenonOfferingID] UNIQUEIDENTIFIER NULL,
    [NewPhenomenonUOMID]      UNIQUEIDENTIFIER NULL,
    [Rank]                    INT              CONSTRAINT [DF_DataSourceTransformation_Rank] DEFAULT ((0)) NULL,
--> Added 2.0.4 20160506 TimPN
    [SensorID]				  UNIQUEIDENTIFIER NULL,
--< Added 2.0.4 20160506 TimPN
--> Added 2.0.0 20160406 TimPN
    [UserId] UNIQUEIDENTIFIER NULL, 
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160715 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_DataSourceTransformation_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_DataSourceTransformation_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160715 TimPN
--> Changed 2.0.8 20160715 TimPN
--    CONSTRAINT [PK_DataSourceTransformation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_DataSourceTransformation] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.8 20160715 TimPN
    CONSTRAINT [FK_DataSourceTransformation_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_Phenomenon] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_TransformationType] FOREIGN KEY ([TransformationTypeID]) REFERENCES [dbo].[TransformationType] ([ID]),
--> Added 2.0.4 20160506 TimPN
--> Added 2.0.8 20160715 TimPN
    CONSTRAINT [FK_DataSourceTransformation_NewPhenomenonOffering] FOREIGN KEY ([NewPhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_NewPhenomenonUOM] FOREIGN KEY ([NewPhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
--< Added 2.0.8 20160715 TimPN
    CONSTRAINT [FK_DataSourceTransformation_Sensor] FOREIGN KEY ([SensorID]) REFERENCES [dbo].[Sensor] ([ID]),
--< Added 2.0.4 20160506 TimPN
--> Added 2.0.0 20160406 TimPN
    CONSTRAINT [FK_DataSourceTransformation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160726 TimPN
    CONSTRAINT [UX_DataSourceTransformation] UNIQUE ([DataSourceID], [SensorID], [Rank], [TransformationTypeID], [PhenomenonID], [PhenomenonOfferingID], [PhenomenonUOMID], [NewPhenomenonOfferingID], [NewPhenomenonUOMID], [StartDate], [EndDate])
--< Added 2.0.8 20160726 TimPN
);
--> Added 2.0.8 20160715 TimPN
GO
--> Changed 2.0.23 20170112 TimPN
--CREATE CLUSTERED INDEX [CX_DataSourceTransformation] ON [dbo].[DataSourceTransformation] ([AddedAt])
CREATE UNIQUE CLUSTERED INDEX [CX_DataSourceTransformation] ON [dbo].[DataSourceTransformation] ([AddedAt])
--< Changed 2.0.23 20170112 TimPN
--< Added 2.0.8 20160715 TimPN
--> Added 2.0.0 20160406 TimPN
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
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.4 20160506 TimPN
GO
CREATE INDEX [IX_DataSourceTransformation_SensorID] ON [dbo].[DataSourceTransformation] ([SensorID])
--< Added 2.0.4 20160506 TimPN
GO
CREATE INDEX [IX_DataSourceTransformation_Rank] ON [dbo].[DataSourceTransformation] ([DataSourceID], [Rank])
GO
CREATE INDEX [IX_DataSourceTransformation_StartDate] ON [dbo].[DataSourceTransformation] ([DataSourceID], [StartDate])
GO
CREATE INDEX [IX_DataSourceTransformation_EndDate] ON [dbo].[DataSourceTransformation] ([DataSourceID], [EndDate])
--> Added 2.0.8 20160726 TimPN
--< Added 2.0.8 20160726 TimPN
--> Added 2.0.8 20160715 TimPN
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_DataSourceTransformation_Insert] ON [dbo].[DataSourceTransformation]
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
--> Changed 2.0.19 20161205 TimPN
--		AddedAt = del.AddedAt,
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate ()),
--< Changed 2.0.19 20161205 TimPN
        UpdatedAt = GETDATE()
    from
        DataSourceTransformation src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--> Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160715 TimPN
