--> Added 2.0.0.2 20160419 TimPN
CREATE TABLE [dbo].[AuditLog]
(
    [ID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_AuditLog_ID] DEFAULT NewID(), 
    [Description] VARCHAR(500) NOT NULL, 
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_AuditLog_AddedAt] DEFAULT GetDate(), 
--> Added 2.0.0.3 20160421 TimPN
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_AuditLog_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.0.3 20160421 TimPN
    CONSTRAINT [PK_AuditLog] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [FK_AuditLog_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
CREATE CLUSTERED INDEX [IX_AuditLog_AddedAt] ON [dbo].AuditLog ([AddedAt])
GO
CREATE INDEX [IX_AuditLog_UserId] ON [dbo].AuditLog ([UserId])
--< Added 2.0.0.2 20160419 TimPN
--> Added 2.0.0.3 20160421 TimPN
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
        inserted ins 
        inner join AuditLog src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_AuditLog_Update] ON [dbo].[AuditLog]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    if UPDATE(AddedAt) RAISERROR ('Cannot update AddedAt.', 16, 1)
    if UPDATE(UpdatedAt) RAISERROR ('Cannot update UpdatedAt.', 16, 1)
    if not UPDATE(AddedAt) and not UPDATE(UpdatedAt)
        Update 
            src 
        set 
            UpdatedAt = GETDATE()
        from
            inserted ins 
            inner join AuditLog src
                on (ins.ID = src.ID)
END
--< Added 2.0.0.3 20160421 TimPN
