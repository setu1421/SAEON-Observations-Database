--> Added 20160329 TimPN
CREATE TABLE [dbo].[Project_Site]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Project_Site_ID] DEFAULT newid(), 
    [ProjectID] UNIQUEIDENTIFIER NOT NULL, 
    [SiteID] UNIQUEIDENTIFIER NOT NULL, 
    [StartDate] DATE NULL, 
    [EndDate] DATE NULL, 
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Project_Site] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Project_Site_Project] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project] ([ID]),
    CONSTRAINT [FK_Project_Site_Site] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]),
    CONSTRAINT [FK_Project_Site_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Project_Site_ProjectIDSiteID] UNIQUE ([ProjectID], [SiteID])
)

GO
CREATE INDEX [IX_Project_Site_UserId] ON [dbo].[Project_Site] ([UserId])
--< Added 20160329 TimPN
