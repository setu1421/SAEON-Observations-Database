--> Added 2.0.0.1 20160406 TimPN
CREATE TABLE [dbo].[Site_Organisation]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Site_Organisation_ID] DEFAULT newid(), 
    [SiteID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationRoleID] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
--> Added 2.0.0.3 20160426 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Site_Organisation_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Site_Organisation_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.0.3 20160426 TimPN
    CONSTRAINT [PK_Site_Organisation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Site_Organisation_Site] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]),
    CONSTRAINT [FK_Site_Organisation_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [FK_Site_Organisation_OrganisationRole] FOREIGN KEY ([OrganisationRoleID]) REFERENCES [dbo].[OrganisationRole] ([ID]),
    CONSTRAINT [FK_Site_Organisation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Site_Organisation] UNIQUE ([SiteID],[OrganisationID],[OrganisationRoleID],[StartDate],[EndDate])
)
GO
CREATE INDEX [IX_Site_Organisation_SiteID] ON [dbo].[Site_Organisation] ([SiteID])
GO
CREATE INDEX [IX_Site_Organisation_OrganisationID] ON [dbo].[Site_Organisation] ([OrganisationID])
GO
CREATE INDEX [IX_Site_Organisation_OrganisationRoleID] ON [dbo].[Site_Organisation] ([OrganisationRoleID])
GO
CREATE INDEX [IX_Site_Organisation_StartDate] ON [dbo].[Site_Organisation] ([StartDate])
GO
CREATE INDEX [IX_Site_Organisation_EndDate] ON [dbo].[Site_Organisation] ([EndDate])
GO
CREATE INDEX [IX_Site_Organisation_UserId] ON [dbo].[Site_Organisation] ([UserId])
--< Added 2.0.0.1 20160406 TimPN
--> Added 2.0.0.3 20160426 TimPN
GO
CREATE TRIGGER [dbo].[TR_Site_Organisation_Insert] ON [dbo].[Site_Organisation]
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
        inserted ins
        inner join Site_Organisation src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Site_Organisation_Update] ON [dbo].[Site_Organisation]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    --if UPDATE(AddedAt) RAISERROR ('Cannot update AddedAt.', 16, 1)
    Update
        src
    set
        UpdatedAt = GETDATE()
    from
        inserted ins
        inner join Site_Organisation src
            on (ins.ID = src.ID)
END
--< Added 2.0.0.3 20160426 TimPN
