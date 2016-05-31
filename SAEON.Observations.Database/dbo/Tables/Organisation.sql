CREATE TABLE [dbo].[Organisation] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Table_1_Organisation] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (5000)   NULL,
--> Added 2.0.1 20160406 TimPN
    [Url] VARCHAR(250) NULL, 
--< Added 2.0.1 20160406 TimPN
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
--> Added 2.0.4 20160509 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Organisation_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Organisation_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.4 20160509 TimPN
    CONSTRAINT [PK_Organisation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Organisation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Organisation_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Organisation_Code] UNIQUE ([Code]),
--< Changed 20160329 TimPN
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Organisation_Name] UNIQUE ([Name])
    CONSTRAINT [UX_Organisation_Name] UNIQUE ([Name])
--< Changed 20160329 TimPN
);
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_Organisation_UserId] ON [dbo].[Organisation] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.4 20160508 TimPN
GO
CREATE TRIGGER [dbo].[TR_Organisation_Insert] ON [dbo].[Organisation]
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
        inner join Organisation src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Organisation_Update] ON [dbo].[Organisation]
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
        inner join Organisation src
            on (ins.ID = src.ID)
END
--< Added 2.0.4 20160508 TimPN
