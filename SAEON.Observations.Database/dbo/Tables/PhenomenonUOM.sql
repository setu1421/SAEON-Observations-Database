CREATE TABLE [dbo].[PhenomenonUOM] (
    [ID]              UNIQUEIDENTIFIER CONSTRAINT [DF_PhenomenonUOM_ID] DEFAULT (newid()) NOT NULL,
    [PhenomenonID]    UNIQUEIDENTIFIER NOT NULL,
    [UnitOfMeasureID] UNIQUEIDENTIFIER NOT NULL,
    [IsDefault]       BIT              CONSTRAINT [DF_PhenomenonUOM_IsDefault] DEFAULT ((0)) NOT NULL,
    [UserId] UNIQUEIDENTIFIER NULL, 
    [AddedAt] DATETIME NULL CONSTRAINT [DF_PhenomenonUOM_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_PhenomenonUOM_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_PhenomenonUOM] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_PhenomenonUOM_PhenomenonUOM] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [FK_PhenomenonUOM_UnitOfMeasure] FOREIGN KEY ([UnitOfMeasureID]) REFERENCES [dbo].[UnitOfMeasure] ([ID]),
    CONSTRAINT [UX_PhenomenonUOM] UNIQUE ([PhenomenonID], [UnitOfMeasureID]),
    CONSTRAINT [FK_PhenomenonUOM_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
);
GO
CREATE INDEX [IX_PhenomenonUOM_PhenomenonID] ON [dbo].[PhenomenonUOM] ([PhenomenonID])
GO
CREATE INDEX [IX_PhenomenonUOM_UnitOfMeasureID] ON [dbo].[PhenomenonUOM] ([UnitOfMeasureID])
GO
CREATE INDEX [IX_PhenomenonUOM_UserId] ON [dbo].[PhenomenonUOM] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_PhenomenonUOM_Insert] ON [dbo].[PhenomenonUOM]
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
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        PhenomenonUOM src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
