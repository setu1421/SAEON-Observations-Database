CREATE TABLE [dbo].[aspnet_Roles] (
    [ApplicationId]   UNIQUEIDENTIFIER NOT NULL,
    [RoleId]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [RoleName]        NVARCHAR (256)   NOT NULL,
    [LoweredRoleName] NVARCHAR (256)   NOT NULL,
    [Description]     NVARCHAR (256)   NULL,
--> Changed 2.0.13 20161011 TimPN
--    PRIMARY KEY NONCLUSTERED ([RoleId]),
    CONSTRAINT [PK_aspnet_Roles] PRIMARY KEY NONCLUSTERED ([RoleId]) ON [Authentication],
--< Changed 2.0.13 20161011 TimPN
    FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
)
--> Added 2.0.13 20161010 TimPN
  ON [Authentication];
--< Added 2.0.13 20161010 TimPN

GO
CREATE UNIQUE CLUSTERED INDEX [aspnet_Roles_index1]
    ON [dbo].[aspnet_Roles]([ApplicationId] ASC, [LoweredRoleName])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Authentication];
--< Added 2.0.13 20161010 TimPN


