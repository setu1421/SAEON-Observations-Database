CREATE TABLE [dbo].[aspnet_Applications] (
    [ApplicationName]        NVARCHAR (256)   NOT NULL,
    [LoweredApplicationName] NVARCHAR (256)   NOT NULL,
    [ApplicationId]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Description]            NVARCHAR (256)   NULL,
--> Changed 2.0.13 20161011 TimPN
--    PRIMARY KEY NONCLUSTERED ([ApplicationId]),
--    UNIQUE ([ApplicationName]),
--    UNIQUE ([LoweredApplicationName])
    CONSTRAINT [PK_aspnet_Applications] PRIMARY KEY NONCLUSTERED ([ApplicationId]) ON [Authentication],
    CONSTRAINT [UX_aspnet_Applications_ApplicationName] UNIQUE ([ApplicationName]) ON [Authentication],
    CONSTRAINT [UX_aspnet_Applications_LoweredApplicationName] UNIQUE ([LoweredApplicationName]) ON [Authentication]
--< Changed 2.0.13 20161011 TimPN
)
--> Added 2.0.13 20161010 TimPN
  ON [Authentication];
--< Added 2.0.13 20161010 TimPN


GO
CREATE CLUSTERED INDEX [aspnet_Applications_Index]
    ON [dbo].[aspnet_Applications]([LoweredApplicationName])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Authentication];
--< Added 2.0.13 20161010 TimPN

