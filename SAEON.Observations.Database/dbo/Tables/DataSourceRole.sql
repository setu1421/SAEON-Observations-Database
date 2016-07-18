CREATE TABLE [dbo].[DataSourceRole] (
    [ID]             UNIQUEIDENTIFIER CONSTRAINT [DF_DataSourceRole_ID] DEFAULT (newid()) NOT NULL,
    [DataSourceID]   UNIQUEIDENTIFIER NOT NULL,
    [RoleId]         UNIQUEIDENTIFIER NOT NULL,
    [DateStart]      DATETIME         NULL,
    [DateEnd]        DATETIME         NULL,
    [RoleName]       VARCHAR (256)    NULL,
    [IsRoleReadOnly] BIT              NULL,
--> Added 2.0.0 20160406 TimPN
    [UserId] UNIQUEIDENTIFIER NULL, 
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160715 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_DataSourceRole_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_DataSourceRole_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160715 TimPN
--> Changed 2.0.8 20160715 TimPN
--    CONSTRAINT [PK_DataSourceRole] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_DataSourceRole] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.8 20160715 TimPN
    CONSTRAINT [FK_DataSourceRole_aspnet_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[aspnet_Roles] ([RoleId]),
    CONSTRAINT [FK_DataSourceRole_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
--> Added 2.0.0 20160406 TimPN
    CONSTRAINT [FK_DataSourceRole_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--< Added 2.0.0 20160406 TimPN
);
--> Added 2.0.8 20160715 TimPN
GO
CREATE CLUSTERED INDEX [CX_DataSourceRole] ON [dbo].[DataSourceRole] ([AddedAt])
--< Added 2.0.8 20160715 TimPN
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_DataSourceRole_DataSourceID] ON [dbo].[DataSourceRole] ([DataSourceID])
GO
CREATE INDEX [IX_DataSourceRole_RoleId] ON [dbo].[DataSourceRole] ([RoleId])
GO
CREATE INDEX [IX_DataSourceRole_UserId] ON [dbo].[DataSourceRole] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160715 TimPN
GO
CREATE TRIGGER [dbo].[TR_DataSourceRole_Insert] ON [dbo].[DataSourceRole]
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
        inner join DataSourceRole src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_DataSourceRole_Update] ON [dbo].[DataSourceRole]
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
        inner join DataSourceRole src
            on (ins.ID = src.ID)
END
--< Added 2.0.8 20160715 TimPN
