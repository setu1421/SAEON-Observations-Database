CREATE TABLE [dbo].[DataSourceRole] (
    [ID]             UNIQUEIDENTIFIER CONSTRAINT [DF_DataSourceRole_ID] DEFAULT (newid()) NOT NULL,
    [DataSourceID]   UNIQUEIDENTIFIER NOT NULL,
    [RoleId]         UNIQUEIDENTIFIER NOT NULL,
    [DateStart]      DATETIME         NULL,
    [DateEnd]        DATETIME         NULL,
    [RoleName]       VARCHAR (256)    NULL,
    [IsRoleReadOnly] BIT              NULL,
    CONSTRAINT [PK_DataSourceRole] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_DataSourceRole_aspnet_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[aspnet_Roles] ([RoleId]),
    CONSTRAINT [FK_DataSourceRole_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID])
);

