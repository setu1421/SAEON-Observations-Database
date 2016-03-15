CREATE TABLE [dbo].[Site_Organisation]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Site_Organisation_ID] DEFAULT newid(), 
    [SiteID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationRoleID] UNIQUEIDENTIFIER NOT NULL, 
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Site_Organisation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Site_Organisation_Site] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]),
    CONSTRAINT [FK_Site_Organisation_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [FK_Site_Organisation_OrganisationRole] FOREIGN KEY ([OrganisationRoleID]) REFERENCES [dbo].[OrganisationRole] ([ID]),
    CONSTRAINT [FK_Site_Organisation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Site_Organisation_SiteIDOrganisation] UNIQUE ([SiteID],[OrganisationID])
)
