CREATE TABLE [dbo].[Organisation_Station]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Organisation_Station_ID] DEFAULT (newid()), 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [StationID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationRoleID] UNIQUEIDENTIFIER NOT NULL,
    [StartDate]        DATE         NULL,
    [EndDate]        DATE         NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Organisation_Station_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Organisation_Station_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
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
CREATE INDEX [IX_Organisation_Station_StartDateEndDate] ON [dbo].[Organisation_Station] ([StartDate],[EndDate])
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
        AddedAt = GetDate(),
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
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        Organisation_Station src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
