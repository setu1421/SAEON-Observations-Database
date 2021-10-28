CREATE TABLE [dbo].[InventorySnapshots]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_InventorySnapshots_ID] DEFAULT newid() NOT NULL, 
    [When] DATETIME NOT NULL CONSTRAINT DF_InventorySnapshots_When DEFAULT getdate(),
    [Organisations] INT NOT NULL, 
    [Programmes] INT NOT NULL, 
    [Projects] INT NOT NULL, 
    [Sites] INT NOT NULL, 
    [Stations] INT NOT NULL, 
    [Instruments] INT NOT NULL, 
    [Sensors] INT NOT NULL, 
    [Phenomena] INT NOT NULL, 
    [Offerings] INT NOT NULL, 
    [UnitsOfMeasure] INT NOT NULL, 
    [Variables] INT NOT NULL, 
    [Datasets] INT NOT NULL, 
    [Observations] INT NOT NULL,
    [Downloads] INT NOT NULL, 
    CONSTRAINT [PK_InventorySnapshots] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_InventorySnapshots_ID_When] UNIQUE ([ID],[When]),
)
GO
CREATE INDEX [IX_InventorySnapshots_When] ON [dbo].[InventorySnapshots] ([When])
