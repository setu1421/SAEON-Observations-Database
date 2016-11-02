CREATE TABLE [dbo].[Phenomenon] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_Phenomenon_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (5000)   NULL,
    [Url]         VARCHAR (250)    NULL,
    [UserId]      UNIQUEIDENTIFIER NOT NULL,
--> Added 2.0.8 20160718 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Phenomenon_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Phenomenon_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160718 TimPN
--> Changed 2.0.8 20160718 TimPN
--    CONSTRAINT [PK_Phenomenon] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_Phenomenon] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.8 20160718 TimPN
    CONSTRAINT [FK_Phenomenon_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Phenomenon_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Phenomenon_Code] UNIQUE ([Code]),
--< Changed 20160329 TimPN
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_Phenomenon_Name] UNIQUE ([Name])
    CONSTRAINT [UX_Phenomenon_Name] UNIQUE ([Name])
--< Changed 20160329 TimPN
);
--> Added 2.0.8 20160718 TimPN
GO
CREATE CLUSTERED INDEX [CX_Phenomenon] ON [dbo].[Phenomenon] ([AddedAt])
--< Added 2.0.8 20160718 TimPN
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_Phenomenon_UserId] ON [dbo].[Phenomenon] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160718 TimPN
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_Phenomenon_Insert] ON [dbo].[Phenomenon]
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
        Phenomenon src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Phenomenon_Update] ON [dbo].[Phenomenon]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
		AddedAt = del.AddedAt,
        UpdatedAt = GETDATE()
    from
        Phenomenon src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN

