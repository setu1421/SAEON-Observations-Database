CREATE TABLE [dbo].[PhenomenonOffering] (
    [ID]           UNIQUEIDENTIFIER CONSTRAINT [DF_PhenomenonOffering_ID] DEFAULT (newid()) NOT NULL,
    [PhenomenonID] UNIQUEIDENTIFIER NOT NULL,
    [OfferingID]   UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NULL, 
--> Added 2.0.8 20160718 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_PhenomenonOffering_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_PhenomenonOffering_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160718 TimPN
--> Changed 2.0.8 20160718 TimPN
--    CONSTRAINT [PK_PhenomenonOffering] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_PhenomenonOffering] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.8 20160718 TimPN
    CONSTRAINT [FK_PhenomenonOffering_Offering] FOREIGN KEY ([OfferingID]) REFERENCES [dbo].[Offering] ([ID]),
    CONSTRAINT [FK_PhenomenonOffering_Phenomenon] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
--> Added 2.0.0 20160406 TimPN
    CONSTRAINT [UX_PhenomenonOffering] UNIQUE ([PhenomenonID],[OfferingID]),
    CONSTRAINT [FK_PhenomenonOffering_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
--< Added 2.0.0 20160406 TimPN
);
--> Added 2.0.8 20160718 TimPN
GO
CREATE CLUSTERED INDEX [CX_PhenomenonOffering] ON [dbo].[PhenomenonOffering] ([AddedAt])
--< Added 2.0.8 20160718 TimPN
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_PhenomenonOffering_PhenomenonID] ON [dbo].[PhenomenonOffering] ([PhenomenonID])
GO
CREATE INDEX [IX_PhenomenonOffering_OfferingID] ON [dbo].[PhenomenonOffering] ([OfferingID])
GO
CREATE INDEX [IX_PhenomenonOffering_UserId] ON [dbo].[PhenomenonOffering] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160718 TimPN
GO
CREATE TRIGGER [dbo].[TR_PhenomenonOffering_Insert] ON [dbo].[PhenomenonOffering]
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
        inner join PhenomenonOffering src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_PhenomenonOffering_Update] ON [dbo].[PhenomenonOffering]
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
        inner join PhenomenonOffering src
            on (ins.ID = src.ID)
END
--< Added 2.0.8 20160718 TimPN

