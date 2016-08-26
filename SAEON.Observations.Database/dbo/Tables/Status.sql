CREATE TABLE [dbo].[Status] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Status_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
--> Added 2.0.0 20160406 TimPN
    [UserId] UNIQUEIDENTIFIER NULL, 
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160718 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Status_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Status_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160718 TimPN
--> Changed 2.0.8 20160718 TimPN
--    CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_Status] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.8 20160718 TimPN
--> Changed 20160329 TimPN
--	CONSTRAINT [IX_Status_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Status_Code] UNIQUE ([Code]),
--< Changed 20160329 TimPN
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Status_Name] UNIQUE ([Name])
    CONSTRAINT [UX_Status_Name] UNIQUE ([Name]),
--< Changed 20160329 TimPN
--> Added 2.0.0 20160406 TimPN
    CONSTRAINT [FK_Status_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
--< Added 2.0.0 20160406 TimPN
);
--> Added 2.0.8 20160718 TimPN
GO
CREATE CLUSTERED INDEX [CX_Status] ON [dbo].[Status] ([AddedAt])
--< Added 2.0.8 20160718 TimPN
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_Status_UserId] ON [dbo].[Status] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160718 TimPN
GO
CREATE TRIGGER [dbo].[TR_Status_Insert] ON [dbo].[Status]
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
        inner join Status src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Status_Update] ON [dbo].[Status]
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
        inner join Status src
            on (ins.ID = src.ID)
END
--< Added 2.0.8 20160718 TimPN


