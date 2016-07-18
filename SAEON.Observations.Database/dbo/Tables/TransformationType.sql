CREATE TABLE [dbo].[TransformationType] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_TransformationType_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
    [iorder]      INT              NULL,
--> Added 2.0.0 20160406 TimPN
    [UserId] UNIQUEIDENTIFIER NULL, 
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160718 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_TransformationType_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_TransformationType_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160718 TimPN
--> Changed 2.0.8 20160718 TimPN
--    CONSTRAINT [PK_TransformationType] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_TransformationType] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.8 20160718 TimPN
--> Added 2.0.0 20160406 TimPN
    CONSTRAINT [UX_TransformationType_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_TransformationType_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_TransformationType_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--< Added 2.0.0 20160406 TimPN
);
--> Added 2.0.8 20160718 TimPN
GO
CREATE CLUSTERED INDEX [CX_TransformationType] ON [dbo].[TransformationType] ([AddedAt])
--< Added 2.0.8 20160718 TimPN
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_TransformationType_UserId] ON [dbo].[TransformationType] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160718 TimPN
GO
CREATE TRIGGER [dbo].[TR_TransformationType_Insert] ON [dbo].[TransformationType]
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
        inner join TransformationType src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_TransformationType_Update] ON [dbo].[TransformationType]
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
        inner join TransformationType src
            on (ins.ID = src.ID)
END
--< Added 2.0.8 20160718 TimPN


