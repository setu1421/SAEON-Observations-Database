--> Added 2.0.5 20160530 TimPN
CREATE TABLE [dbo].[Organisation_Site]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Organisation_Site_ID] DEFAULT newid(), 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [SiteID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationRoleID] UNIQUEIDENTIFIER NOT NULL,
--> Changed 2.0.22 20170111 TimPN
--    [StartDate]        DATETIME         NULL,
    [StartDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
--> Changed 2.0.22 20170111 TimPN
--    [EndDate]        DATETIME         NULL,
    [EndDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Organisation_Site_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Organisation_Site_UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Organisation_Site] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Organisation_Site_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [FK_Organisation_Site_Site] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]),
    CONSTRAINT [FK_Organisation_Site_OrganisationRole] FOREIGN KEY ([OrganisationRoleID]) REFERENCES [dbo].[OrganisationRole] ([ID]),
    CONSTRAINT [FK_Organisation_Site_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Organisation_Site] UNIQUE ([OrganisationID],[SiteID],[OrganisationRoleID],[StartDate],[EndDate])
)
GO
CREATE INDEX [IX_Organisation_Site_OrganisationID] ON [dbo].[Organisation_Site] ([OrganisationID])
GO
CREATE INDEX [IX_Organisation_Site_SiteID] ON [dbo].[Organisation_Site] ([SiteID])
GO
CREATE INDEX [IX_Organisation_Site_OrganisationRoleID] ON [dbo].[Organisation_Site] ([OrganisationRoleID])
GO
CREATE INDEX [IX_Organisation_Site_StartDate] ON [dbo].[Organisation_Site] ([StartDate])
GO
CREATE INDEX [IX_Organisation_Site_EndDate] ON [dbo].[Organisation_Site] ([EndDate])
GO
CREATE INDEX [IX_Organisation_Site_UserId] ON [dbo].[Organisation_Site] ([UserId])
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_Organisation_Site_Insert] ON [dbo].[Organisation_Site]
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
        Organisation_Site src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Organisation_Site_Update] ON [dbo].[Organisation_Site]
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
        Organisation_Site src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160530 TimPN
