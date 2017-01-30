--> Added 2.0.9 20160823 TimPN
CREATE TABLE [dbo].[StatusReason]
(
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_StatusReason_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
    [AddedAt] DATETIME NULL CONSTRAINT [DF_StatusReason_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_StatusReason_UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_StatusReason] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_StatusReason_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_StatusReason_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_StatusReason_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
)
GO
CREATE INDEX [IX_StatusReason_UserId] ON [dbo].[StatusReason] ([UserId])
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_StatusReason_Insert] ON [dbo].[StatusReason]
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
        StatusReason src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_StatusReason_Update] ON [dbo].[StatusReason]
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
        StatusReason src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--> Added 2.0.9 20160823 TimPN
