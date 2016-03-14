CREATE TABLE [dbo].[DataSourceType] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_DataSourceType_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
    CONSTRAINT [PK_DataSourceType] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80)
);

