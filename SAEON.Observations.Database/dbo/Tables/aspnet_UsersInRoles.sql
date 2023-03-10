CREATE TABLE [dbo].[aspnet_UsersInRoles] (
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_aspnet_UsersInRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId]) ON [PRIMARY],
    FOREIGN KEY ([RoleId]) REFERENCES [dbo].[aspnet_Roles] ([RoleId]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
)
  ON [PRIMARY];


GO
CREATE INDEX [aspnet_UsersInRoles_index] ON [dbo].[aspnet_UsersInRoles]([RoleId]) ON [PRIMARY];

