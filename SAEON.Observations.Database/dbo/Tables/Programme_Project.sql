--> Added 2.0.0.5 20160511 TimPN
CREATE TABLE [dbo].[Programme_Project]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Programme_Project_ID] DEFAULT newid(), 
    [ProgrammeID] UNIQUEIDENTIFIER NOT NULL,
    [ProjectID] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Programme_Project_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Programme_Project__UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Programme_Project] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [UX_Programme_Project] UNIQUE ([ProjectID],[ProgrammeID],[StartDate],[EndDate]),
    CONSTRAINT [FK_Programme_Project_ProjectID] FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Project] ([ID]),
    CONSTRAINT [FK_Programme_Project_ProgrammeID] FOREIGN KEY ([ProgrammeID]) REFERENCES [dbo].[Programme] ([ID]),
    CONSTRAINT [FK_Programme_Project_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
CREATE CLUSTERED INDEX [CX_Programme_Project] ON [dbo].[Programme_Project] ([AddedAt])
GO
CREATE INDEX [IX_Programme_Project_ProjectID] ON [dbo].[Programme_Project] ([ProjectID])
GO
CREATE INDEX [IX_Programme_Project_ProgrammeID] ON [dbo].[Programme_Project] ([ProgrammeID])
GO
CREATE INDEX [IX_Programme_Project_UserId] ON [dbo].[Programme_Project] ([UserId])
GO
CREATE INDEX [IX_Programme_Project_StartDate] ON [dbo].[Programme_Project] ([StartDate])
GO
CREATE INDEX [IX_Programme_Project_EndDate] ON [dbo].[Programme_Project] ([EndDate])
GO
CREATE TRIGGER [dbo].[TR_Programme_Project_Insert] ON [dbo].[Programme_Project]
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
        inner join Programme_Project src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Programme_Project_Update] ON [dbo].[Programme_Project]
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
        inner join Programme_Project src
            on (ins.ID = src.ID)
END
--< Added 2.0.0.5 20160511 TimPN
