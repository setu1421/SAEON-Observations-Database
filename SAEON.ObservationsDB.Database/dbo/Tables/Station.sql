CREATE TABLE [dbo].[Station] (
    [ID]            UNIQUEIDENTIFIER CONSTRAINT [DF_Statuib_StationID] DEFAULT (newid()) NOT NULL,
    [Code]          VARCHAR (50)     NOT NULL,
    [Name]          VARCHAR (150)    NOT NULL,
    [Description]   VARCHAR (5000)   NULL,
    [Url]           VARCHAR (250)    NULL,
    [Latitude]      FLOAT (53)       NULL,
    [Longitude]     FLOAT (53)       NULL,
    [Elevation]     INT              NULL,
    [ProjectSiteID] UNIQUEIDENTIFIER NOT NULL,
    [UserId]        UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PKStation] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_Station_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_Station_ProjectSite] FOREIGN KEY ([ProjectSiteID]) REFERENCES [dbo].[ProjectSite] ([ID]),
    CONSTRAINT [IX_Station_Code] UNIQUE NONCLUSTERED ([Code] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [IX_Station_Name] UNIQUE NONCLUSTERED ([Name] ASC) WITH (FILLFACTOR = 80)
);

