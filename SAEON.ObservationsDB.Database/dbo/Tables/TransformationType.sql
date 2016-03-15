CREATE TABLE [dbo].[TransformationType] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_TransformationType_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Name]        VARCHAR (150)    NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
    [iorder]      INT              NULL,
    CONSTRAINT [PK_TransformationType] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80)
);

