CREATE TABLE [dbo].[PhenomenonUOM] (
    [ID]              UNIQUEIDENTIFIER CONSTRAINT [DF_PhenomenonUOM_ID] DEFAULT (newid()) NOT NULL,
    [PhenomenonID]    UNIQUEIDENTIFIER NOT NULL,
    [UnitOfMeasureID] UNIQUEIDENTIFIER NOT NULL,
    [IsDefault]       BIT              CONSTRAINT [DF_PhenomenonUOM_IsDefault] DEFAULT ((0)) NOT NULL,
    [UserId] UNIQUEIDENTIFIER NULL, 
--> Added 2.0.8 20160718 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_PhenomenonUOM_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_PhenomenonUOM_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160718 TimPN
--> Changed 2.0.8 20160718 TimPN
--    CONSTRAINT [PK_PhenomenonUOM] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_PhenomenonUOM] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.8 20160718 TimPN
    CONSTRAINT [FK_PhenomenonUOM_PhenomenonUOM] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [FK_PhenomenonUOM_UnitOfMeasure] FOREIGN KEY ([UnitOfMeasureID]) REFERENCES [dbo].[UnitOfMeasure] ([ID]),
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_PhenomenonUOM] UNIQUE ([PhenomenonID] ASC, [UnitOfMeasureID])
    CONSTRAINT [UX_PhenomenonUOM] UNIQUE ([PhenomenonID], [UnitOfMeasureID]),
--< Changed 20160329 TimPN
--> Added 2.0.0 20160406 TimPN
    CONSTRAINT [FK_PhenomenonUOM_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
--< Added 2.0.0 20160406 TimPN
);
--> Added 2.0.8 20160718 TimPN
GO
CREATE CLUSTERED INDEX [CX_PhenomenonUOM] ON [dbo].[PhenomenonUOM] ([AddedAt])
--< Added 2.0.8 20160718 TimPN
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_PhenomenonUOM_PhenomenonID] ON [dbo].[PhenomenonUOM] ([PhenomenonID])
GO
CREATE INDEX [IX_PhenomenonUOM_UnitOfMeasureID] ON [dbo].[PhenomenonUOM] ([UnitOfMeasureID])
GO
CREATE INDEX [IX_PhenomenonUOM_UserId] ON [dbo].[PhenomenonUOM] ([UserId])
--> Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160718 TimPN
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_PhenomenonUOM_Insert] ON [dbo].[PhenomenonUOM]
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
        PhenomenonUOM src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_PhenomenonUOM_Update] ON [dbo].[PhenomenonUOM]
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
        PhenomenonUOM src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160718 TimPN

