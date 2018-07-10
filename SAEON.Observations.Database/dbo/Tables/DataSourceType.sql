CREATE TABLE [dbo].[DataSourceType] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_DataSourceType_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
    [UserId] UNIQUEIDENTIFIER NULL, 
    [AddedAt] DATETIME NULL CONSTRAINT [DF_DataSourceType_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_DataSourceType_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_DataSourceType] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataSourceType_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_DataSourceType_Code] Unique ([Code])
);
GO
CREATE INDEX [IX_DataSourceType_UserId] ON [dbo].[DataSourceType] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_DataSourceType_Insert] ON [dbo].[DataSourceType]
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
        DataSourceType src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_DataSourceType_Update] ON [dbo].[DataSourceType]
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
        DataSourceType src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
