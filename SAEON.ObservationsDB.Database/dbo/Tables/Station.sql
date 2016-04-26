CREATE TABLE [dbo].[Station] (
    [ID]            UNIQUEIDENTIFIER CONSTRAINT [DF_Statuib_StationID] DEFAULT (newid()) NOT NULL,
    [Code]          VARCHAR (50)     NOT NULL,
    [Name]          VARCHAR (150)    NOT NULL,
    [Description]   VARCHAR (5000)   NULL,
    [Url]           VARCHAR (250)    NULL,
    [Latitude]      FLOAT (53)       NULL,
    [Longitude]     FLOAT (53)       NULL,
    [Elevation]     INT              NULL,
--> Changed 2.0.0.3 20160426 TimPN
--    [ProjectSiteID] UNIQUEIDENTIFIER NOT NULL,
    [ProjectSiteID] UNIQUEIDENTIFIER NULL,
--< Changed 2.0.0.3 20160426 TimPN
    [UserId]        UNIQUEIDENTIFIER NOT NULL,
--> Added 2.0.0.2 20160407 TimPN
--    [SiteID] UNIQUEIDENTIFIER NOT NULL, -- Must be NOT NULL once all Stations have Sites
    [SiteID] UNIQUEIDENTIFIER NULL, 
    [StartDate] DATETIME NULL, 
    [EndDate] DATETIME NULL, 
    --< Added 2.0.0.2 20160407 TimPN
--> Added 2.0.0.3 20160421 TimPN
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Station_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.0.3 20160421 TimPN
    CONSTRAINT [PKStation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Station_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_Station_ProjectSite] FOREIGN KEY ([ProjectSiteID]) REFERENCES [dbo].[ProjectSite] ([ID]),
--> Added 2.0.0.2 20160407 TimPN
    CONSTRAINT [FK_Station_Site] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]),
--    CONSTRAINT [UX_Station_SiteID_Code] UNIQUE ([SiteID],[Code]), -- Must be added once all Stations have Sites
--    CONSTRAINT [UX_Station_SiteID_Name] UNIQUE ([SiteID],[Name]), -- Must be added once all Stations have Sites
--< Added 2.0.0.2 20160407 TimPN
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Station_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Station_Code] UNIQUE ([Code]),
--< Changed 20160329 TimPN
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Station_Name] UNIQUE ([Name])
    CONSTRAINT [UX_Station_Name] UNIQUE ([Name])
--< Changed 20160329 TimPN
);
--> Added 2.0.0.0 20160406 TimPN
GO
CREATE INDEX [IX_Station_ProjectSiteID] ON [dbo].[Station] ([ProjectSiteID])
GO
CREATE INDEX [IX_Station_UserId] ON [dbo].[Station] ([UserId])
--< Added 2.0.0.0 20160406 TimPN
--> Added 2.0.0.2 20160407 TimPN
GO
CREATE INDEX [IX_Station_SiteID] ON [dbo].[Station] ([SiteID])
--< Added 2.0.0.2 20160407 TimPN
--> Added 2.0.0.3 20160421 TimPN
GO
CREATE TRIGGER [dbo].[TR_Station_InsertUpdate] ON [dbo].[Station]
FOR INSERT, UPDATE
AS
BEGIN
    SET NoCount ON
    Update 
        src 
    set 
        UpdatedAt = GETDATE()
    from
        inserted ins 
        inner join Site src
            on (ins.ID = src.ID)
END
--< Added 2.0.0.3 20160421 TimPN
