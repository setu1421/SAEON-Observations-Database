CREATE TABLE [dbo].[RoleModule] (
    [ID]       UNIQUEIDENTIFIER CONSTRAINT [DF_RoleModule_ID] DEFAULT (newid()) NOT NULL,
    [RoleId]   UNIQUEIDENTIFIER NOT NULL,
    [ModuleID] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_RoleModule] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_RoleModule_aspnet_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[aspnet_Roles] ([RoleId]),
    CONSTRAINT [FK_RoleModule_Module] FOREIGN KEY ([ModuleID]) REFERENCES [dbo].[Module] ([ID])
);

