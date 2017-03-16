--> Added 2.0.2 20160419 TimPN
CREATE TABLE [dbo].[AuditLog]
(
    [ID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_AuditLog_ID] DEFAULT NewID(), 
--> Changed 20170316 TimPN
--    [Description] VARCHAR(500) NOT NULL, 
    [Description] VARCHAR(5000) NOT NULL, 
--< Changed 20170316 TimPN
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_AuditLog_AddedAt] DEFAULT GetDate(), 
--> Added 2.0.3 20160421 TimPN
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_AuditLog_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.3 20160421 TimPN
    CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_AuditLog_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
CREATE INDEX [IX_AuditLog_UserId] ON [dbo].AuditLog ([UserId])
--< Added 2.0.2 20160419 TimPN
--> Added 2.0.3 20160421 TimPN
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_AuditLog_Insert] ON [dbo].[AuditLog]
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
        AuditLog src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_AuditLog_Update] ON [dbo].[AuditLog]
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
        AuditLog src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
--> Changed 2.0.15 20161102 TimPN
--< Added 2.0.3 20160421 TimPN
