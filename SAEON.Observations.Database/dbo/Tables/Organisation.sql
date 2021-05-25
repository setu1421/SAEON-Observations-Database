CREATE TABLE [dbo].[Organisation] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Organisation_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (5000)   NULL,
    [Url] VARCHAR(250) NULL, 
	--[DigitalObjectIdentifierID] Int null,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Organisation_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Organisation_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_Organisation] PRIMARY KEY CLUSTERED ([ID]),
    --Constraint [FK_Organisation_DigitalObjectIdentifierID] Foreign Key ([DigitalObjectIdentifierID]) References [dbo].[DigitalObjectIdentifiers] ([ID]),
    CONSTRAINT [FK_Organisation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Organisation_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Organisation_Name] UNIQUE ([Name])
);
GO
--CREATE INDEX [IX_Organisation_DigitalObjectIdentifierID] ON [dbo].[Organisation]([DigitalObjectIdentifierID])
GO
CREATE INDEX [IX_Organisation_CodeName] ON [dbo].[Organisation] ([Code],[Name])
GO
CREATE INDEX [IX_Organisation_UserId] ON [dbo].[Organisation] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_Organisation_Insert] ON [dbo].[Organisation]
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
        Organisation src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Organisation_Update] ON [dbo].[Organisation]
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
        Organisation src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
