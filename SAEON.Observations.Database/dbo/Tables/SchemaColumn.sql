--> Added 2.0.11 20160908 TimPN
CREATE TABLE [dbo].[SchemaColumn]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_SchemaColumn_ID] DEFAULT newid(), 
    [DataSchemaID] UNIQUEIDENTIFIER NOT NULL, 
    [Number] INT NOT NULL,
    [Name] VARCHAR(100) NOT NULL,
    [SchemaColumnTypeID] UNIQUEIDENTIFIER NOT NULL,
    [Width] INT NULL,
    [Format] VARCHAR(50) NULL,
    [PhenomenonID] UNIQUEIDENTIFIER NULL,
    [PhenomenonOfferingID] UNIQUEIDENTIFIER NULL,
    [PhenomenonUOMID] UNIQUEIDENTIFIER NULL,
    [EmptyValue] VARCHAR(50) NULL,
    [FixedTime] VARCHAR(10) NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_SchemaColumn_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_SchemaColumn_UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_SchemaColumn] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [FK_SchemaColumn_DataSchema] FOREIGN KEY ([DataSchemaID]) REFERENCES [dbo].[DataSchema] ([ID]),
    CONSTRAINT [FK_SchemaColumn_SchemaColumnType] FOREIGN KEY ([SchemaColumnTypeID]) REFERENCES [dbo].[SchemaColumnType] ([ID]),
    CONSTRAINT [FK_SchemaColumn_Phenomenon] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [FK_SchemaColumn_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_SchemaColumn_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
    CONSTRAINT [FK_SchemaColumn_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_SchemaColumn_DataSchemaID_Number] UNIQUE ([DataSchemaID],[Number]),
    CONSTRAINT [UX_SchemaColumn_DataSchemaID_Name] UNIQUE ([DataSchemaID],[Name])
)
GO
--> Changed 2.0.23 20170112 TimPN
--CREATE CLUSTERED INDEX [CX_SchemaColumn] ON [dbo].[SchemaColumn] ([AddedAt])
CREATE UNIQUE CLUSTERED INDEX [CX_SchemaColumn] ON [dbo].[SchemaColumn] ([AddedAt])
--< Changed 2.0.23 20170112 TimPN
GO
CREATE INDEX [IX_SchemaColumn_DataSchemaID] ON [dbo].[SchemaColumn] ([DataSchemaID])
GO
CREATE INDEX [IX_SchemaColumn_SchemaColumnTypeID] ON [dbo].[SchemaColumn] ([SchemaColumnTypeID])
GO
CREATE INDEX [IX_SchemaColumn_PhenomenonID] ON [dbo].[SchemaColumn] ([PhenomenonID])
GO
CREATE INDEX [IX_SchemaColumn_PhenomenonOfferingID] ON [dbo].[SchemaColumn] ([PhenomenonOfferingID])
GO
CREATE INDEX [IX_SchemaColumn_PhenomenonUOMID] ON [dbo].[SchemaColumn] ([PhenomenonUOMID])
GO
CREATE INDEX [IX_SchemaColumn_UserId] ON [dbo].[SchemaColumn] ([UserId])
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_SchemaColumn_Insert] ON [dbo].[SchemaColumn]
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
        SchemaColumn src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_SchemaColumn_Update] ON [dbo].[SchemaColumn]
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
        SchemaColumn src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.11 20160908 TimPN
