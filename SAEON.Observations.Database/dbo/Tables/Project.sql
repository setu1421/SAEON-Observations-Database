--> Added 2.0.5 20160511 TimPN
CREATE TABLE [dbo].[Project]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Project_ID] DEFAULT newid(), 
    [ProgrammeID] UNIQUEIDENTIFIER NULL,
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [Url] VARCHAR(250) NULL, 
--> Changed 2.0.22 20170111 TimPN
--    [StartDate]        DATETIME         NULL,
    [StartDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
--> Changed 2.0.22 20170111 TimPN
--    [EndDate]        DATETIME         NULL,
    [EndDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Project_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Project_UpdatedAt] DEFAULT GetDate(), 
--> Added 2.0.33 20170628 TimPN
    [RowVersion] RowVersion not null,
--< Added 2.0.33 20170628 TimPN
    CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_Project_ProgramID_Code] UNIQUE ([ProgrammeID],[Code]),
    CONSTRAINT [UX_Project_ProgramID_Name] UNIQUE ([ProgrammeID],[Name]),
    CONSTRAINT [FK_Project_Programme] FOREIGN KEY ([ProgrammeID]) REFERENCES [dbo].[Programme] ([ID]),
    CONSTRAINT [FK_Project_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
CREATE INDEX [IX_Project_ProgrammeID] ON [dbo].[Project] ([ProgrammeID])
GO
CREATE INDEX [IX_Project_UserId] ON [dbo].[Project] ([UserId])
GO
CREATE INDEX [IX_Project_StartDate] ON [dbo].[Project] ([StartDate])
GO
CREATE INDEX [IX_Project_EndDate] ON [dbo].[Project] ([EndDate])
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_Project_Insert] ON [dbo].[Project]
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
        Project src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Project_Update] ON [dbo].[Project]
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
        Project src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160511 TimPN
