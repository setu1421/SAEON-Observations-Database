--> Added 20160329 TimPN
CREATE TABLE [dbo].[SiteOrganisation]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_SiteOrganisation_ID] DEFAULT newid(), 
    [SiteID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationRoleID] UNIQUEIDENTIFIER NOT NULL, 
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_SiteOrganisation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_SiteOrganisation_Site] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]),
    CONSTRAINT [FK_SiteOrganisation_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [FK_SiteOrganisation_OrganisationRole] FOREIGN KEY ([OrganisationRoleID]) REFERENCES [dbo].[OrganisationRole] ([ID]),
    CONSTRAINT [FK_SiteOrganisation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_SiteOrganisation_SiteIDOrganisationID] UNIQUE ([SiteID],[OrganisationID])
)
GO
CREATE INDEX [IX_SiteOrganisation_SiteID] ON [dbo].[SiteOrganisation] ([SiteID])
GO
CREATE INDEX [IX_SiteOrganisation_OrganisationID] ON [dbo].[SiteOrganisation] ([OrganisationID])
GO
CREATE INDEX [IX_SiteOrganisation_OrganisationRoleID] ON [dbo].[SiteOrganisation] ([OrganisationRoleID])
GO
CREATE INDEX [IX_SiteOrganisation_UserId] ON [dbo].[SiteOrganisation] ([UserId])
--< Added 20160329 TimPN
