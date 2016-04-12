--> Added 20160406 TimPN
CREATE TABLE [dbo].[Site_Organisation]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Site_Organisation_ID] DEFAULT newid(), 
    [SiteID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationRoleID] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Site_Organisation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Site_Organisation_Site] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]),
    CONSTRAINT [FK_Site_Organisation_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [FK_Site_Organisation_OrganisationRole] FOREIGN KEY ([OrganisationRoleID]) REFERENCES [dbo].[OrganisationRole] ([ID]),
    CONSTRAINT [FK_Site_Organisation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Site_Organisation_SiteIDOrganisationIDOOrganisationRoleID] UNIQUE ([SiteID],[OrganisationID],[OrganisationRoleID])
)
GO
CREATE INDEX [IX_Site_Organisation_SiteID] ON [dbo].[Site_Organisation] ([SiteID])
GO
CREATE INDEX [IX_Site_Organisation_OrganisationID] ON [dbo].[Site_Organisation] ([OrganisationID])
GO
CREATE INDEX [IX_Site_Organisation_OrganisationRoleID] ON [dbo].[Site_Organisation] ([OrganisationRoleID])
GO
CREATE INDEX [IX_Site_Organisation_UserId] ON [dbo].[Site_Organisation] ([UserId])
--< Added 20160406 TimPN
