CREATE TABLE [dbo].[Project]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Project_ID] DEFAULT (newid()), 
    [ProgrammeID] UNIQUEIDENTIFIER NOT NULL,
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [Url] VARCHAR(250) NULL, 
	--[DigitalObjectIdentifierID] Int null,
    [StartDate]        DATE         NULL,
    [EndDate]        DATE         NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Project_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Project_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_Project_ProgramID_Code] UNIQUE ([ProgrammeID],[Code]),
    CONSTRAINT [UX_Project_ProgramID_Name] UNIQUE ([ProgrammeID],[Name]),
    --Constraint [FK_Project_DigitalObjectIdentifierID] Foreign Key ([DigitalObjectIdentifierID]) References [dbo].[DigitalObjectIdentifiers] ([ID]),
    CONSTRAINT [FK_Project_Programme] FOREIGN KEY ([ProgrammeID]) REFERENCES [dbo].[Programme] ([ID]),
    CONSTRAINT [FK_Project_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
--CREATE INDEX [IX_Project_DigitalObjectIdentifierID] ON [dbo].[Project]([DigitalObjectIdentifierID])
GO
CREATE INDEX [IX_Project_ProgrammeID] ON [dbo].[Project] ([ProgrammeID])
GO
CREATE INDEX [IX_Project_CodeName] ON [dbo].[Project] ([Code],[Name])
GO
CREATE INDEX [IX_Project_UserId] ON [dbo].[Project] ([UserId])
GO
CREATE INDEX [IX_Project_StartDate] ON [dbo].[Project] ([StartDate])
GO
CREATE INDEX [IX_Project_EndDate] ON [dbo].[Project] ([EndDate])
GO
CREATE INDEX [IX_Project_StartDateEndDate] ON [dbo].[Project] ([StartDate],[EndDate])
GO
CREATE TRIGGER [dbo].[TR_Project_Insert] ON [dbo].[Project]
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
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        Project src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
