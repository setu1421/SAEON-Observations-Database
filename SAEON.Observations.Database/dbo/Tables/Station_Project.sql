--> Added 2.0.0.5 20160527 TimPN
CREATE TABLE [dbo].[Station_Project]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Station_Project_ID] DEFAULT newid(), 
    [StationID] UNIQUEIDENTIFIER NOT NULL,
    [ProjectID] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Station_Project_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Station_Project__UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Station_Project] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [UX_Station_Project] UNIQUE ([StationID],[ProjectID],[StartDate],[EndDate]),
    CONSTRAINT [FK_Station_Project_StationID] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]),
    CONSTRAINT [FK_Station_Project_ProjectID] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project] ([ID]),
    CONSTRAINT [FK_Station_Project_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
CREATE CLUSTERED INDEX [CX_Station_Project] ON [dbo].[Station_Project] ([AddedAt])
GO
CREATE INDEX [IX_Station_Project_StationID] ON [dbo].[Station_Project] ([StationID])
GO
CREATE INDEX [IX_Station_Project_ProjectID] ON [dbo].[Station_Project] ([ProjectID])
GO
CREATE INDEX [IX_Station_Project_UserId] ON [dbo].[Station_Project] ([UserId])
GO
CREATE INDEX [IX_Station_Project_StartDate] ON [dbo].[Station_Project] ([StartDate])
GO
CREATE INDEX [IX_Station_Project_EndDate] ON [dbo].[Station_Project] ([EndDate])
GO
CREATE TRIGGER [dbo].[TR_Station_Project_Insert] ON [dbo].[Station_Project]
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
        inner join Station_Project src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Station_Project_Update] ON [dbo].[Station_Project]
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
        inner join Station_Project src
            on (ins.ID = src.ID)
END
--< Added 2.0.0.5 20160527 TimPN
