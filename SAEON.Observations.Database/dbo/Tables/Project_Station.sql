--> Added 2.0.5 20160527 TimPN
CREATE TABLE [dbo].[Project_Station]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Project_Station_ID] DEFAULT newid(), 
    [ProjectID] UNIQUEIDENTIFIER NOT NULL,
    [StationID] UNIQUEIDENTIFIER NOT NULL,
--> Changed 2.0.22 20170111 TimPN
--    [StartDate]        DATETIME         NULL,
    [StartDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
--> Changed 2.0.22 20170111 TimPN
--    [EndDate]        DATETIME         NULL,
    [EndDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Project_Station_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Project_Station__UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Project_Station] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [UX_Project_Station] UNIQUE ([ProjectID],[StationID],[StartDate],[EndDate]),
    CONSTRAINT [FK_Project_Station_ProjectID] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project] ([ID]),
    CONSTRAINT [FK_Project_Station_StationID] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]),
    CONSTRAINT [FK_Project_Station_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
CREATE CLUSTERED INDEX [CX_Project_Station] ON [dbo].[Project_Station] ([AddedAt])
GO
CREATE INDEX [IX_Project_Station_ProjectID] ON [dbo].[Project_Station] ([ProjectID])
GO
CREATE INDEX [IX_Project_Station_StationID] ON [dbo].[Project_Station] ([StationID])
GO
CREATE INDEX [IX_Project_Station_UserId] ON [dbo].[Project_Station] ([UserId])
GO
CREATE INDEX [IX_Project_Station_StartDate] ON [dbo].[Project_Station] ([StartDate])
GO
CREATE INDEX [IX_Project_Station_EndDate] ON [dbo].[Project_Station] ([EndDate])
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_Project_Station_Insert] ON [dbo].[Project_Station]
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
        Project_Station src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Project_Station_Update] ON [dbo].[Project_Station]
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
        Project_Station src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160527 TimPN
