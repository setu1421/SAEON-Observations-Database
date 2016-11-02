CREATE TABLE [dbo].[UnitOfMeasure] (
    [ID]         UNIQUEIDENTIFIER CONSTRAINT [DF_UnitOfMeasure_ID] DEFAULT (newid()) NOT NULL,
    [Code]       VARCHAR (50)     NOT NULL,
    [Unit]       VARCHAR (100)    NOT NULL,
    [UnitSymbol] VARCHAR (20)     NOT NULL,
    [UserId]     UNIQUEIDENTIFIER NOT NULL,
--> Added 2.0.8 20160718 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_UnitOfMeasure_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_UnitOfMeasure_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160718 TimPN
--> Changed 2.0.8 20160718 TimPN
--    CONSTRAINT [PK_UnitOfMeasure] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_UnitOfMeasure] PRIMARY KEY NONCLUSTERED ([ID]),
--> Changed 2.0.8 20160718 TimPN
    CONSTRAINT [FK_UnitOfMeasure_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_UnitOfMeasure_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_UnitOfMeasure_Code] UNIQUE ([Code]),
--< Changed 20160329 TimPN
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_UnitOfMeasure_Unit] UNIQUE ([Unit])
    CONSTRAINT [UX_UnitOfMeasure_Unit] UNIQUE ([Unit])
--< Changed 20160329 TimPN
);
--> Added 2.0.8 20160718 TimPN
GO
CREATE CLUSTERED INDEX [CX_UnitOfMeasure] ON [dbo].[UnitOfMeasure] ([AddedAt])
--< Added 2.0.8 20160718 TimPN
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_UnitOfMeasure_UserId] ON [dbo].[UnitOfMeasure] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160718 TimPN
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_UnitOfMeasure_Insert] ON [dbo].[UnitOfMeasure]
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
        UnitOfMeasure src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_UnitOfMeasure_Update] ON [dbo].[UnitOfMeasure]
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
        UnitOfMeasure src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN


