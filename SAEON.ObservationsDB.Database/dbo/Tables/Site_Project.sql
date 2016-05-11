--> Added 2.0.0.5 20160511 TimPN
CREATE TABLE [dbo].[Site_Project]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Site_Project_ID] DEFAULT newid(), 
    [ProjectID] UNIQUEIDENTIFIER NOT NULL,
    [SiteID] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Site_Project_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Site_Project__UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Site_Project] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [UX_Site_Project] UNIQUE ([ProjectID],[SiteID],[StartDate],[EndDate]),
    CONSTRAINT [FK_Site_Project_ProjectID] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project] ([ID]),
    CONSTRAINT [FK_Site_Project_SiteID] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[Site] ([ID]),
    CONSTRAINT [FK_Site_Project_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
CREATE CLUSTERED INDEX [CX_Site_Project] ON [dbo].[Site_Project] ([AddedAt])
GO
CREATE INDEX [IX_Site_Project_ProjectID] ON [dbo].[Site_Project] ([ProjectID])
GO
CREATE INDEX [IX_Site_Project_SiteID] ON [dbo].[Site_Project] ([SiteID])
GO
CREATE INDEX [IX_Site_Project_UserId] ON [dbo].[Site_Project] ([UserId])
GO
CREATE INDEX [IX_Site_Project_StartDate] ON [dbo].[Site_Project] ([StartDate])
GO
CREATE INDEX [IX_Site_Project_EndDate] ON [dbo].[Site_Project] ([EndDate])
GO
CREATE TRIGGER [dbo].[TR_Site_Project_Insert] ON [dbo].[Site_Project]
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
        inner join Site_Project src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Site_Project_Update] ON [dbo].[Site_Project]
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
        inner join Site_Project src
            on (ins.ID = src.ID)
END
--< Added 2.0.0.5 20160511 TimPN
