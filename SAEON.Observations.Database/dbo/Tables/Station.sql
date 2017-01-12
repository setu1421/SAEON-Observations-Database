CREATE TABLE [dbo].[Station] (
    [ID]            UNIQUEIDENTIFIER CONSTRAINT [DF_Station_ID] DEFAULT (newid()) NOT NULL,
    [Code]          VARCHAR (50)     NOT NULL,
    [Name]          VARCHAR (150)    NOT NULL,
    [Description]   VARCHAR (5000)   NULL,
    [Url]           VARCHAR (250)    NULL,
    [Latitude]      FLOAT (53)       NULL,
    [Longitude]     FLOAT (53)       NULL,
    [Elevation]     INT              NULL,
--> Changed 2.0.3 20160426 TimPN
--    [ProjectSiteID] UNIQUEIDENTIFIER NOT NULL,
    [ProjectSiteID] UNIQUEIDENTIFIER NULL,
--< Changed 2.0.3 20160426 TimPN
    [UserId]        UNIQUEIDENTIFIER NOT NULL,
--> Added 2.0.2 20160407 TimPN
    [SiteID] UNIQUEIDENTIFIER NULL, 
--> Changed 2.0.22 20170111 TimPN
--    [StartDate]        DATETIME         NULL,
    [StartDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
--> Changed 2.0.22 20170111 TimPN
--    [EndDate]        DATETIME         NULL,
    [EndDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
--< Added 2.0.2 20160407 TimPN
--> Added 2.0.3 20160421 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Station_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Station_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.3 20160421 TimPN
--> Changed 2.0.5 20160411 TimPN
--    CONSTRAINT [PKStation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PKStation] PRIMARY KEY NONCLUSTERED ([ID]),
--> Changed 2.0.5 20160411 TimPN
    CONSTRAINT [FK_Station_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_Station_ProjectSite] FOREIGN KEY ([ProjectSiteID]) REFERENCES [dbo].[ProjectSite] ([ID]),
--> Added 2.0.2 20160407 TimPN
    CONSTRAINT [FK_Station_Site] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]),
--< Added 2.0.2 20160407 TimPN
--> Changed 2.0.0 20160329 TimPN
--    CONSTRAINT [IX_Station_Code] UNIQUE ([Code]),
--> Changed 2.0.5 20160411 TimPN
--    CONSTRAINT [UX_Station_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Station_SiteID_Code] UNIQUE ([SiteID], [Code]),
--< Changed 2.0.5 20160411 TimPN
--< Changed 2.0.0 20160329 TimPN
--> Changed 2.0.0 20160329 TimPN
--    CONSTRAINT [IX_Station_Name] UNIQUE ([Name])
--> Changed 2.0.5 20160411 TimPN
--    CONSTRAINT [UX_Station_Name] UNIQUE ([Name])
    CONSTRAINT [UX_Station_SiteID_Name] UNIQUE ([SiteID],[Name])
--< Changed 2.0.5 20160411 TimPN
--< Changed 2.0.0 20160329 TimPN
);
--> Added 2.0.5 20160411 TimPN
GO
--> Changed 2.0.23 20170112 TimPN
--CREATE CLUSTERED INDEX [CX_Station] ON [dbo].[Station] ([AddedAt])
CREATE UNIQUE CLUSTERED INDEX [CX_Station] ON [dbo].[Station] ([AddedAt])
--< Changed 2.0.23 20170112 TimPN
--< Added 2.0.5 20160411 TimPN
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_Station_ProjectSiteID] ON [dbo].[Station] ([ProjectSiteID])
GO
CREATE INDEX [IX_Station_UserId] ON [dbo].[Station] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.2 20160407 TimPN
GO
CREATE INDEX [IX_Station_SiteID] ON [dbo].[Station] ([SiteID])
--< Added 2.0.2 20160407 TimPN
--> Added 2.0.3 20160421 TimPN
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_Station_Insert] ON [dbo].[Station]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        Station src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Station_Update] ON [dbo].[Station]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
--> Changed 2.0.19 20161205 TimPN
--		AddedAt = del.AddedAt,
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate ()),
--< Changed 2.0.19 20161205 TimPN
        UpdatedAt = GETDATE()
    from
        Station src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.3 20160421 TimPN
