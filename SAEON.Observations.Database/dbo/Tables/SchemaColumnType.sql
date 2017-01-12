--> Added 2.0.11 20160908 TimPN
-- Date, Time, Ignore, Offering, FixedTime, Comment
CREATE TABLE [dbo].[SchemaColumnType]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_SchemaColumnType_ID] DEFAULT newid(), 
    [Name] VARCHAR(50) NOT NULL,
    [Description] VARCHAR(250) NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_SchemaColumnType_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_SchemaColumnType_UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_SchemaColumnType] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [FK_SchemaColumnType_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_SchemaColumnType] UNIQUE ([Name])
)
GO
--> Changed 2.0.23 20170112 TimPN
--CREATE CLUSTERED INDEX [CX_SchemaColumnType] ON [dbo].[SchemaColumnType] ([AddedAt])
CREATE UNIQUE CLUSTERED INDEX [CX_SchemaColumnType] ON [dbo].[SchemaColumnType] ([AddedAt])
--< Changed 2.0.23 20170112 TimPN
GO
CREATE INDEX [IX_SchemaColumnType_UserId] ON [dbo].[SchemaColumnType] ([UserId])
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_SchemaColumnType_Insert] ON [dbo].[SchemaColumnType]
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
        SchemaColumnType src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_SchemaColumnType_Update] ON [dbo].[SchemaColumnType]
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
        SchemaColumnType src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.11 20160908 TimPN
