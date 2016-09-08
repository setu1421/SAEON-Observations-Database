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
CREATE CLUSTERED INDEX [CX_SchemaColumnType] ON [dbo].[SchemaColumnType] ([AddedAt])
GO
CREATE INDEX [IX_SchemaColumnType_UserId] ON [dbo].[SchemaColumnType] ([UserId])
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
        inserted ins
        inner join SchemaColumnType src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_SchemaColumnType_Update] ON [dbo].[SchemaColumnType]
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
        inner join SchemaColumnType src
            on (ins.ID = src.ID)
END
--< Added 2.0.11 20160908 TimPN
