CREATE TABLE [dbo].[SchemaColumnType]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_SchemaColumnType_ID] DEFAULT (newid()), 
    [Name] VARCHAR(50) NOT NULL,
    [Description] VARCHAR(250) NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_SchemaColumnType_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_SchemaColumnType_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_SchemaColumnType] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_SchemaColumnType_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_SchemaColumnType] UNIQUE ([Name])
)
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
        AddedAt = GetDate(),
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
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        SchemaColumnType src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
