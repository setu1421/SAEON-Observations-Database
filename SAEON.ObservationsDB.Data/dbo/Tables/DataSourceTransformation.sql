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
    CONSTRAINT [PK_DataSourceTransformation] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_DataSourceTransformation_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_Phenomenon] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_PhenomenonOffering] FOREIGN KEY ([PhenomenonOfferingID]) REFERENCES [dbo].[PhenomenonOffering] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_PhenomenonUOM] FOREIGN KEY ([PhenomenonUOMID]) REFERENCES [dbo].[PhenomenonUOM] ([ID]),
    CONSTRAINT [FK_DataSourceTransformation_TransformationType] FOREIGN KEY ([TransformationTypeID]) REFERENCES [dbo].[TransformationType] ([ID])
);

