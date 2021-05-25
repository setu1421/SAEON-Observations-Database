CREATE TABLE [dbo].[Site]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Site_ID] DEFAULT (newid()), 
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [Url] VARCHAR(250) NULL, 
	--[DigitalObjectIdentifierID] Int null,
    [StartDate]        DATE         NULL,
    [EndDate]        DATE         NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Site_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Site_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_Site_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Site_Name] UNIQUE ([Name]),
    --Constraint [FK_Site_DigitalObjectIdentifierID] Foreign Key ([DigitalObjectIdentifierID]) References [dbo].[DigitalObjectIdentifiers] ([ID]),
    CONSTRAINT [FK_Site_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
--CREATE INDEX [IX_Site_DigitalObjectIdentifierID] ON [dbo].[Site]([DigitalObjectIdentifierID])
GO
CREATE INDEX [IX_Site_CodeName] ON [dbo].[Site] ([Code],[Name])
GO
CREATE INDEX [IX_Site_UserId] ON [dbo].[Site] ([UserId])
GO
CREATE INDEX [IX_Site_StartDate] ON [dbo].[Site] ([StartDate])
GO
CREATE INDEX [IX_Site_EndDate] ON [dbo].[Site] ([EndDate])
GO
CREATE INDEX [IX_Site_StartDateEndDate] ON [dbo].[Site] ([StartDate],[EndDate])
GO
CREATE TRIGGER [dbo].[TR_Site_Insert] ON [dbo].[Site]
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
        Site src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Site_Update] ON [dbo].[Site]
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
        Site src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
