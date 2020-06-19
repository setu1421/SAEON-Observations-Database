CREATE TABLE [dbo].[aspnet_Applications] (
    [ApplicationName]        NVARCHAR (256)   NOT NULL,
    [LoweredApplicationName] NVARCHAR (256)   NOT NULL,
    [ApplicationId]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Description]            NVARCHAR (256)   NULL,
    CONSTRAINT [PK_aspnet_Applications] PRIMARY KEY NONCLUSTERED ([ApplicationId]) ON [PRIMARY],
    CONSTRAINT [UX_aspnet_Applications_ApplicationName] UNIQUE ([ApplicationName]) ON [PRIMARY],
    CONSTRAINT [UX_aspnet_Applications_LoweredApplicationName] UNIQUE ([LoweredApplicationName]) ON [PRIMARY]
)
  ON [PRIMARY];


GO
CREATE CLUSTERED INDEX [aspnet_Applications_Index] ON [dbo].[aspnet_Applications]([LoweredApplicationName]) ON [PRIMARY];

