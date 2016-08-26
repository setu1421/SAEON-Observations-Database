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
    CONSTRAINT [PK_StatusReason] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [UX_StatusReason_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_StatusReason_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_StatusReason_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
)
GO
CREATE CLUSTERED INDEX [CX_StatusReason] ON [dbo].[StatusReason] ([AddedAt])
GO
CREATE INDEX [IX_StatusReason_UserId] ON [dbo].[StatusReason] ([UserId])
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
        inserted ins
        inner join StatusReason src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_StatusReason_Update] ON [dbo].[StatusReason]
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
        inner join StatusReason src
            on (ins.ID = src.ID)
END
--> Added 2.0.9 20160823 TimPN
