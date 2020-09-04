CREATE TABLE [dbo].[Programme]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Programme_ID] DEFAULT (newid()), 
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [Url] VARCHAR(250) NULL, 
	[DigitalObjectIdentifierID] Int null,
    [StartDate]        DATE         NULL,
    [EndDate]        DATE         NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Programme_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Programme_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_Programme] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_Programme_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Programme_Name] UNIQUE ([Name]),
    Constraint [FK_Programme_DigitalObjectIdentifierID] Foreign Key ([DigitalObjectIdentifierID]) References [dbo].[DigitalObjectIdentifiers] ([ID]),
    CONSTRAINT [FK_Programme_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
CREATE INDEX [IX_Programme_DigitalObjectIdentifierID] ON [dbo].[Programme]([DigitalObjectIdentifierID])
GO
CREATE INDEX [IX_Programme_CodeName] ON [dbo].[Programme] ([Code],[Name])
GO
CREATE INDEX [IX_Programme_UserId] ON [dbo].[Programme] ([UserId])
GO
CREATE INDEX [IX_Programme_StartDate] ON [dbo].[Programme] ([StartDate])
GO
CREATE INDEX [IX_Programme_EndDate] ON [dbo].[Programme] ([EndDate])
GO
CREATE INDEX [IX_Programme_StartDateEndDate] ON [dbo].[Programme] ([StartDate],[EndDate])
GO
CREATE TRIGGER [dbo].[TR_Programme_Insert] ON [dbo].[Programme]
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
        Programme src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Programme_Update] ON [dbo].[Programme]
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
        Programme src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
