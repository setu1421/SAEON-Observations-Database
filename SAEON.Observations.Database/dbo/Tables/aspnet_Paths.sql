CREATE TABLE [dbo].[aspnet_Paths] (
    [ApplicationId] UNIQUEIDENTIFIER NOT NULL,
    [PathId]        UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Path]          NVARCHAR (256)   NOT NULL,
    [LoweredPath]   NVARCHAR (256)   NOT NULL,
--> Changed 2.0.13 20161011 TimPN
--    PRIMARY KEY NONCLUSTERED ([PathId]),
    CONSTRAINT [PK_aspnet_Paths] PRIMARY KEY NONCLUSTERED ([PathId]) ON [Authentication],
--< Changed 2.0.13 20161011 TimPN
    FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
)
--> Added 2.0.13 20161010 TimPN
  ON [Authentication];
--< Added 2.0.13 20161010 TimPN

GO
CREATE UNIQUE CLUSTERED INDEX [aspnet_Paths_index]
    ON [dbo].[aspnet_Paths]([ApplicationId] ASC, [LoweredPath])
--> Added 2.0.13 20161010 TimPN
  WITH(DROP_EXISTING=ON,ONLINE=ON) ON [Authentication];
--< Added 2.0.13 20161010 TimPN

