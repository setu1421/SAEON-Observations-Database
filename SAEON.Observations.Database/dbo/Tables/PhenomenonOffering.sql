CREATE TABLE [dbo].[PhenomenonOffering] (
    [ID]           UNIQUEIDENTIFIER CONSTRAINT [DF_PhenomenonOffering_ID] DEFAULT (newid()) NOT NULL,
    [PhenomenonID] UNIQUEIDENTIFIER NOT NULL,
    [OfferingID]   UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NULL, 
    [AddedAt] DATETIME NULL CONSTRAINT [DF_PhenomenonOffering_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_PhenomenonOffering_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_PhenomenonOffering] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_PhenomenonOffering_Offering] FOREIGN KEY ([OfferingID]) REFERENCES [dbo].[Offering] ([ID]),
    CONSTRAINT [FK_PhenomenonOffering_Phenomenon] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [UX_PhenomenonOffering] UNIQUE ([PhenomenonID],[OfferingID]),
    CONSTRAINT [FK_PhenomenonOffering_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
);
GO
CREATE INDEX [IX_PhenomenonOffering_PhenomenonID] ON [dbo].[PhenomenonOffering] ([PhenomenonID])
GO
CREATE INDEX [IX_PhenomenonOffering_OfferingID] ON [dbo].[PhenomenonOffering] ([OfferingID])
GO
CREATE INDEX [IX_PhenomenonOffering_UserId] ON [dbo].[PhenomenonOffering] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_PhenomenonOffering_Insert] ON [dbo].[PhenomenonOffering]
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
        PhenomenonOffering src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_PhenomenonOffering_Update] ON [dbo].[PhenomenonOffering]
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
        PhenomenonOffering src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END

