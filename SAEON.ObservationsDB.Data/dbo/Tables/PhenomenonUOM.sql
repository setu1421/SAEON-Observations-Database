CREATE TABLE [dbo].[PhenomenonUOM] (
    [ID]              UNIQUEIDENTIFIER CONSTRAINT [DF_PhenomenonUOM_ID] DEFAULT (newid()) NOT NULL,
    [PhenomenonID]    UNIQUEIDENTIFIER NOT NULL,
    [UnitOfMeasureID] UNIQUEIDENTIFIER NOT NULL,
    [IsDefault]       BIT              CONSTRAINT [DF_PhenomenonUOM_IsDefault] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PhenomenonUOM] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_PhenomenonUOM_PhenomenonUOM] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [FK_PhenomenonUOM_UnitOfMeasure] FOREIGN KEY ([UnitOfMeasureID]) REFERENCES [dbo].[UnitOfMeasure] ([ID]),
    CONSTRAINT [IX_PhenomenonUOM] UNIQUE NONCLUSTERED ([PhenomenonID] ASC, [UnitOfMeasureID] ASC) WITH (FILLFACTOR = 80)
);

