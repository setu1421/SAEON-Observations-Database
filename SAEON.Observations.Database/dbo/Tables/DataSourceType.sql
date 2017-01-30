CREATE TABLE [dbo].[DataSourceType] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_DataSourceType_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
--> Added 2.0.0 20160406 TimPN
    [UserId] UNIQUEIDENTIFIER NULL, 
--> Added 2.0.8 20160715 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_DataSourceType_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_DataSourceType_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160715 TimPN
--< Added 2.0.0 20160406 TimPN
    CONSTRAINT [PK_DataSourceType] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataSourceType_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_DataSourceType_Code] Unique ([Code])
--< Added 2.0.0 20160406 TimPN
);
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_DataSourceType_UserId] ON [dbo].[DataSourceType] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160715 TimPN
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_DataSourceType_Insert] ON [dbo].[DataSourceType]
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
--> Changed 2.0.19 20161205 TimPN
--		AddedAt = del.AddedAt,
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate ()),
--< Changed 2.0.19 20161205 TimPN
        UpdatedAt = GETDATE()
    from
        DataSourceType src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
--> Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160715 TimPN
