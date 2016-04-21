CREATE TABLE [dbo].[RoleModule] (
    [ID]       UNIQUEIDENTIFIER CONSTRAINT [DF_RoleModule_ID] DEFAULT (newid()) NOT NULL,
    [RoleId]   UNIQUEIDENTIFIER NOT NULL,
    [ModuleID] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_RoleModule] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_RoleModule_aspnet_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[aspnet_Roles] ([RoleId]),
    CONSTRAINT [FK_RoleModule_Module] FOREIGN KEY ([ModuleID]) REFERENCES [dbo].[Module] ([ID])
);
--> Added 2.0.0.0 20160406 TimPN
GO
CREATE INDEX [IX_RoleModule_RoleId] ON [dbo].[RoleModule] ([RoleId])
GO
CREATE INDEX [IX_RoleModule_ModuleID] ON [dbo].[RoleModule] ([ModuleID])
--< Added 2.0.0.0 20160406 TimPN
