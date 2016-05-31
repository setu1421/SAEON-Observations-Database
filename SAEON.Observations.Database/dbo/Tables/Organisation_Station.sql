--> Added 2.0.5 201605306 TimPN
CREATE TABLE [dbo].[Organisation_Station]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Organisation_Station_ID] DEFAULT newid(), 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [StationID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationRoleID] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Organisation_Station_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Organisation_Station_UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Organisation_Station] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [FK_Organisation_Station_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [FK_Organisation_Station_Station] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]),
    CONSTRAINT [FK_Organisation_Station_OrganisationRole] FOREIGN KEY ([OrganisationRoleID]) REFERENCES [dbo].[OrganisationRole] ([ID]),
    CONSTRAINT [FK_Organisation_Station_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Organisation_Station] UNIQUE ([OrganisationID],[StationID],[OrganisationRoleID],[StartDate],[EndDate])
)
GO
CREATE CLUSTERED INDEX [CX_Organisation_Station] ON [dbo].[Organisation_Station] ([AddedAt])
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
        inserted ins
        inner join Organisation_Station src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Organisation_Station_Update] ON [dbo].[Organisation_Station]
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
        inner join Organisation_Station src
            on (ins.ID = src.ID)
END
--< Added 2.0.5 20160530 TimPN
