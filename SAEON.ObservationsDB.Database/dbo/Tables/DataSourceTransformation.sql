CREATE TABLE [dbo].[DataSourceTransformation] (
    [ID]                      UNIQUEIDENTIFIER CONSTRAINT [DF_DataSourceTransformation_ID] DEFAULT (newid()) NOT NULL,
    [TransformationTypeID]    UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonID]            UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonOfferingID]    UNIQUEIDENTIFIER NULL,
    [PhenomenonUOMID]         UNIQUEIDENTIFIER NULL,
    [StartDate]               DATETIME         NOT NULL,
    [EndDate]                 DATETIME         NULL,
    [DataSourceID]            UNIQUEIDENTIFIER NOT NULL,
    [Definition]              TEXT             NOT NULL,
    [NewPhenomenonOfferingID] UNIQUEIDENTIFIER NULL,
    [NewPhenomenonUOMID]      UNIQUEIDENTIFIER NULL,
    [Rank]                    INT              CONSTRAINT [DF_DataSourceTransformation_Rank] DEFAULT ((0)) NOT NULL,
--> Added 20160329 TimPN
    [UserId] UNIQUEIDENTIFIER NULL, 
--< Added 20160329 TimPN
    CONSTRAINT [PK_DataSourceTransformation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_Phenomenon] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_TransformationType] FOREIGN KEY ([TransformationTypeID]) REFERENCES [dbo].[TransformationType] ([ID]),
--> Added 20160329 TimPN
    CONSTRAINT [FK_DataSourceTransformation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--< Added 20160329 TimPN
);
--> Added 20160329 TimPN
GO
CREATE INDEX [IX_DataSourceTransformation_DataSourceID] ON [dbo].[DataSourceTransformation] ([DataSourceID])
GO
CREATE INDEX [IX_DataSourceTransformation_TransformationTypeID] ON [dbo].[DataSourceTransformation] ([TransformationTypeID])
GO
CREATE INDEX [IX_DataSourceTransformation_PhenomenonID] ON [dbo].[DataSourceTransformation] ([PhenomenonID])
GO
CREATE INDEX [IX_DataSourceTransformation_PhenomenonOfferingID] ON [dbo].[DataSourceTransformation] ([PhenomenonOfferingID])
GO
CREATE INDEX [IX_DataSourceTransformation_PhenomenonUOMID] ON [dbo].[DataSourceTransformation] ([PhenomenonUOMID])
GO
CREATE INDEX [IX_DataSourceTransformation_NewPhenomenonOfferingID] ON [dbo].[DataSourceTransformation] ([NewPhenomenonOfferingID])
GO
CREATE INDEX [IX_DataSourceTransformation_NewPhenomenonUOMID] ON [dbo].[DataSourceTransformation] ([NewPhenomenonUOMID])
GO
CREATE INDEX [IX_DataSourceTransformation_UserId] ON [dbo].[DataSourceTransformation] ([UserId])
--< Added 20160329 TimPN
