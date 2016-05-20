CREATE TABLE [dbo].[PhenomenonUOM] (
    [ID]              UNIQUEIDENTIFIER CONSTRAINT [DF_PhenomenonUOM_ID] DEFAULT (newid()) NOT NULL,
    [PhenomenonID]    UNIQUEIDENTIFIER NOT NULL,
    [UnitOfMeasureID] UNIQUEIDENTIFIER NOT NULL,
    [IsDefault]       BIT              CONSTRAINT [DF_PhenomenonUOM_IsDefault] DEFAULT ((0)) NOT NULL,
    [UserId] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [PK_PhenomenonUOM] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_PhenomenonUOM_PhenomenonUOM] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [FK_PhenomenonUOM_UnitOfMeasure] FOREIGN KEY ([UnitOfMeasureID]) REFERENCES [dbo].[UnitOfMeasure] ([ID]),
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_PhenomenonUOM] UNIQUE ([PhenomenonID] ASC, [UnitOfMeasureID])
    CONSTRAINT [UX_PhenomenonUOM] UNIQUE ([PhenomenonID], [UnitOfMeasureID]),
--< Changed 20160329 TimPN
--> Added 2.0.0.0 20160406 TimPN
    CONSTRAINT [FK_PhenomenonUOM_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
--< Added 2.0.0.0 20160406 TimPN
);
--> Added 2.0.0.0 20160406 TimPN
GO
CREATE INDEX [IX_PhenomenonUOM_PhenomenonID] ON [dbo].[PhenomenonUOM] ([PhenomenonID])
GO
CREATE INDEX [IX_PhenomenonUOM_UnitOfMeasureID] ON [dbo].[PhenomenonUOM] ([UnitOfMeasureID])
GO
CREATE INDEX [IX_PhenomenonUOM_UserId] ON [dbo].[PhenomenonUOM] ([UserId])
--> Added 2.0.0.0 20160406 TimPN
