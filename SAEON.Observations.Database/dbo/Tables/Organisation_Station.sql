--> Added 2.0.5 201605306 TimPN
CREATE TABLE [dbo].[Organisation_Station]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Organisation_Station_ID] DEFAULT newid(), 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [StationID] UNIQUEIDENTIFIER NOT NULL, 
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
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Organisation_Station_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Organisation_Station_UpdatedAt] DEFAULT GetDate(), 
--> Added 2.0.33 20170628 TimPN
    [RowVersion] RowVersion not null,
--< Added 2.0.33 20170628 TimPN
    CONSTRAINT [PK_Organisation_Station] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Organisation_Station_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [FK_Organisation_Station_Station] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]),
    CONSTRAINT [FK_Organisation_Station_OrganisationRole] FOREIGN KEY ([OrganisationRoleID]) REFERENCES [dbo].[OrganisationRole] ([ID]),
    CONSTRAINT [FK_Organisation_Station_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Organisation_Station] UNIQUE ([OrganisationID],[StationID],[OrganisationRoleID],[StartDate],[EndDate])
)
GO
CREATE INDEX [IX_Organisation_Station_OrganisationID] ON [dbo].[Organisation_Station] ([OrganisationID])
GO
CREATE INDEX [IX_Organisation_Station_StationID] ON [dbo].[Organisation_Station] ([StationID])
GO
CREATE INDEX [IX_Organisation_Station_OrganisationRoleID] ON [dbo].[Organisation_Station] ([OrganisationRoleID])
GO
CREATE INDEX [IX_Organisation_Station_StartDate] ON [dbo].[Organisation_Station] ([StartDate])
GO
CREATE INDEX [IX_Organisation_Station_EndDate] ON [dbo].[Organisation_Station] ([EndDate])
GO
CREATE INDEX [IX_Organisation_Station_UserId] ON [dbo].[Organisation_Station] ([UserId])
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_Organisation_Station_Insert] ON [dbo].[Organisation_Station]
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
        Organisation_Station src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Organisation_Station_Update] ON [dbo].[Organisation_Station]
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
        Organisation_Station src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160530 TimPN
