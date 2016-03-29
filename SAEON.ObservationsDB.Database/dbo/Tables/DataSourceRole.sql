CREATE TABLE [dbo].[DataSourceRole] (
    [ID]             UNIQUEIDENTIFIER CONSTRAINT [DF_DataSourceRole_ID] DEFAULT (newid()) NOT NULL,
    [DataSourceID]   UNIQUEIDENTIFIER NOT NULL,
    [RoleId]         UNIQUEIDENTIFIER NOT NULL,
    [DateStart]      DATETIME         NULL,
    [DateEnd]        DATETIME         NULL,
    [RoleName]       VARCHAR (256)    NULL,
    [IsRoleReadOnly] BIT              NULL,
--> Added 20160329 TimPN
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
--< Added 20160329 TimPN
    CONSTRAINT [PK_DataSourceRole] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataSourceRole_aspnet_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[aspnet_Roles] ([RoleId]),
    CONSTRAINT [FK_DataSourceRole_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
--> Added 20160329 TimPN
    CONSTRAINT [FK_DataSourceRole_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--< Added 20160329 TimPN


);
--> Added 20160329 TimPN
GO
CREATE INDEX [IX_DataSourceRole_DataSourceID] ON [dbo].[DataSourceRole] ([DataSourceID])
GO
CREATE INDEX [IX_DataSourceRole_RoleId] ON [dbo].[DataSourceRole] ([RoleId])
GO
CREATE INDEX [IX_DataSourceRole_UserId] ON [dbo].[DataSourceRole] ([UserId])
--< Added 20160329 TimPN
