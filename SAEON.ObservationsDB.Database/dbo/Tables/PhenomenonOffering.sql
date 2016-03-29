CREATE TABLE [dbo].[PhenomenonOffering] (
    [ID]           UNIQUEIDENTIFIER CONSTRAINT [DF_PhenomenonOffering_ID] DEFAULT (newid()) NOT NULL,
    [PhenomenonID] UNIQUEIDENTIFIER NOT NULL,
    [OfferingID]   UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [PK_PhenomenonOffering] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_PhenomenonOffering_Offering] FOREIGN KEY ([OfferingID]) REFERENCES [dbo].[Offering] ([ID]),
    CONSTRAINT [FK_PhenomenonOffering_Phenomenon] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
--> Added 20160329 TimPN
    CONSTRAINT [UX_PhenomenonOffering] UNIQUE ([PhenomenonID],[OfferingID]),
    CONSTRAINT [FK_PhenomenonOffering_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
--< Added 20160329 TimPN
);
--> Added 20160329 TimPN
GO
CREATE INDEX [IX_PhenomenonOffering_PhenomenonID] ON [dbo].[PhenomenonOffering] ([PhenomenonID])
GO
CREATE INDEX [IX_PhenomenonOffering_OfferingID] ON [dbo].[PhenomenonOffering] ([OfferingID])
GO
CREATE INDEX [IX_PhenomenonOffering_UserId] ON [dbo].[PhenomenonOffering] ([UserId])
--< Added 20160329 TimPN
