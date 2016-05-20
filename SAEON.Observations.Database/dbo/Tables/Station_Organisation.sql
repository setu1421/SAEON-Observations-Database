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
--> Added 2.0.0.3 20160426 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Station_Organisation_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Station_Organisation_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.0.3 20160426 TimPN
--> Changed 2.0.0.5 20160411 TimPN
--    CONSTRAINT [PK_Station_Organisation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_Station_Organisation] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.0.5 20160411 TimPN
    CONSTRAINT [FK_Station_Organisation_Station] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]),
    CONSTRAINT [FK_Station_Organisation_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [FK_Station_Organisation_OrganisationRole] FOREIGN KEY ([OrganisationRoleID]) REFERENCES [dbo].[OrganisationRole] ([ID]),
    CONSTRAINT [FK_Station_Organisation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Station_Organisation] UNIQUE ([StationID],[OrganisationID],[OrganisationRoleID],[StartDate],[EndDate])
)
--> Added 2.0.0.5 20160411 TimPN
GO
CREATE CLUSTERED INDEX [CX_Station_Organisation] ON [dbo].[Station_Organisation] ([AddedAt])
--< Added 2.0.0.5 20160411 TimPN
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
--> Added 2.0.0.3 20160426 TimPN
GO
CREATE TRIGGER [dbo].[TR_Station_Organisation_Insert] ON [dbo].[Station_Organisation]
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
        inner join Station_Organisation src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Station_Organisation_Update] ON [dbo].[Station_Organisation]
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
        inner join Station_Organisation src
            on (ins.ID = src.ID)
END
--< Added 2.0.0.3 20160426 TimPN
