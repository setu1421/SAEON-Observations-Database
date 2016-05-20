--> Added 2.0.0.5 20160512 TimPN
CREATE TABLE [dbo].[Site_Station]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Site_Station_ID] DEFAULT newid(), 
    [SiteID] UNIQUEIDENTIFIER NOT NULL,
    [StationID] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Site_Station_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Site_Station__UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Site_Station] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [UX_Site_Station] UNIQUE ([SiteID],[StationID],[StartDate],[EndDate]),
    CONSTRAINT [FK_Site_Station_SiteID] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]),
    CONSTRAINT [FK_Site_Station_StationID] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]),
    CONSTRAINT [FK_Site_Station_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
CREATE CLUSTERED INDEX [CX_Site_Station] ON [dbo].[Site_Station] ([AddedAt])
GO
CREATE INDEX [IX_Site_Station_SiteID] ON [dbo].[Site_Station] ([SiteID])
GO
CREATE INDEX [IX_Site_Station_StationID] ON [dbo].[Site_Station] ([StationID])
GO
CREATE INDEX [IX_Site_Station_UserId] ON [dbo].[Site_Station] ([UserId])
GO
CREATE INDEX [IX_Site_Station_StartDate] ON [dbo].[Site_Station] ([StartDate])
GO
CREATE INDEX [IX_Site_Station_EndDate] ON [dbo].[Site_Station] ([EndDate])
GO
CREATE TRIGGER [dbo].[TR_Site_Station_Insert] ON [dbo].[Site_Station]
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
        inner join Site_Station src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Site_Station_Update] ON [dbo].[Site_Station]
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
        inner join Site_Station src
            on (ins.ID = src.ID)
END
--< Added 2.0.0.5 20160512 TimPN
