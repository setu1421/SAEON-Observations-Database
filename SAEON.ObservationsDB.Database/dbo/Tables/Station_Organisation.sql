--> Added 2.0.0.1 20160406 TimPN
CREATE TABLE [dbo].[Station_Organisation]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Station_Organisation_ID] DEFAULT newid(), 
    [StationID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationRoleID] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Station_Organisation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Station_Organisation_Station] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]),
    CONSTRAINT [FK_Station_Organisation_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [FK_Station_Organisation_OrganisationRole] FOREIGN KEY ([OrganisationRoleID]) REFERENCES [dbo].[OrganisationRole] ([ID]),
    CONSTRAINT [FK_Station_Organisation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Station_Organisation] UNIQUE ([StationID],[OrganisationID],[OrganisationRoleID],[StartDate],[EndDate])
)
GO
CREATE INDEX [IX_Station_Organisation_StationID] ON [dbo].[Station_Organisation] ([StationID])
GO
CREATE INDEX [IX_Station_Organisation_OrganisationID] ON [dbo].[Station_Organisation] ([OrganisationID])
GO
CREATE INDEX [IX_Station_Organisation_OrganisationRoleID] ON [dbo].[Station_Organisation] ([OrganisationRoleID])
GO
CREATE INDEX [IX_Station_Organisation_StartDate] ON [dbo].[Station_Organisation] ([StartDate])
GO
CREATE INDEX [IX_Station_Organisation_EndDate] ON [dbo].[Station_Organisation] ([EndDate])
GO
CREATE INDEX [IX_Station_Organisation_UserId] ON [dbo].[Station_Organisation] ([UserId])
--< Added 2.0.0.1 20160406 TimPN
