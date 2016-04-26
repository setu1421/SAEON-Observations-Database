--> Added 2.0.0.3 20160426 TimPN
CREATE TABLE [dbo].[DataSource_Organisation]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_DataSource_Organisation_ID] DEFAULT newid(), 
    [DataSourceID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationRoleID] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_DataSource_Organisation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataSource_Organisation_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
    CONSTRAINT [FK_DataSource_Organisation_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [FK_DataSource_Organisation_OrganisationRole] FOREIGN KEY ([OrganisationRoleID]) REFERENCES [dbo].[OrganisationRole] ([ID]),
    CONSTRAINT [FK_DataSource_Organisation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_DataSource_Organisation] UNIQUE ([DataSourceID],[OrganisationID],[OrganisationRoleID],[StartDate],[EndDate])
)
GO
CREATE INDEX [IX_DataSource_Organisation_DataSourceID] ON [dbo].[DataSource_Organisation] ([DataSourceID])
GO
CREATE INDEX [IX_DataSource_Organisation_OrganisationID] ON [dbo].[DataSource_Organisation] ([OrganisationID])
GO
CREATE INDEX [IX_DataSource_Organisation_OrganisationRoleID] ON [dbo].[DataSource_Organisation] ([OrganisationRoleID])
GO
CREATE INDEX [IX_DataSource_Organisation_StartDate] ON [dbo].[DataSource_Organisation] ([StartDate])
GO
CREATE INDEX [IX_DataSource_Organisation_EndDate] ON [dbo].[DataSource_Organisation] ([EndDate])
GO
CREATE INDEX [IX_DataSource_Organisation_UserId] ON [dbo].[DataSource_Organisation] ([UserId])
--< Added 2.0.0.1 20160406 TimPN
