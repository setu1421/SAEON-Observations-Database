CREATE TABLE [dbo].[aspnet_Users] (
    [ApplicationId]    UNIQUEIDENTIFIER NOT NULL,
    [UserId]           UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [UserName]         NVARCHAR (256)   NOT NULL,
    [LoweredUserName]  NVARCHAR (256)   NOT NULL,
    [MobileAlias]      NVARCHAR (16)    DEFAULT (NULL) NULL,
    [IsAnonymous]      BIT              DEFAULT ((0)) NOT NULL,
    [LastActivityDate] DATETIME         NOT NULL,
--> Changed 2.0.13 20161011 TimPN
--    PRIMARY KEY NONCLUSTERED ([UserId]),
    CONSTRAINT [PK_aspnet_Users] PRIMARY KEY NONCLUSTERED ([UserId]) ON [Authentication],
--< Changed 2.0.13 20161011 TimPN
    FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
)
--> Added 2.0.13 20161010 TimPN
  ON [Authentication];
--< Added 2.0.13 20161010 TimPN

GO
CREATE UNIQUE CLUSTERED INDEX [aspnet_Users_Index]
    ON [dbo].[aspnet_Users]([ApplicationId] ASC, [LoweredUserName])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Authentication];
--< Added 2.0.13 20161010 TimPN


GO
CREATE INDEX [aspnet_Users_Index2]
    ON [dbo].[aspnet_Users]([ApplicationId] ASC, [LastActivityDate])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Authentication];
--< Added 2.0.13 20161010 TimPN

