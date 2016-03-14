CREATE TABLE [dbo].[PhenomenonOffering] (
    [ID]           UNIQUEIDENTIFIER CONSTRAINT [DF_PhenomenonOffering_ID] DEFAULT (newid()) NOT NULL,
    [PhenomenonID] UNIQUEIDENTIFIER NOT NULL,
    [OfferingID]   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_PhenomenonOffering] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_PhenomenonOffering_Offering] FOREIGN KEY ([OfferingID]) REFERENCES [dbo].[Offering] ([ID]),
    CONSTRAINT [FK_PhenomenonOffering_Phenomenon] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID])
);

