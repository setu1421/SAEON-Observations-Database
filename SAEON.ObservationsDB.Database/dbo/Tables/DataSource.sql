CREATE TABLE [dbo].[DataSource] (
    [ID]               UNIQUEIDENTIFIER CONSTRAINT [DF_DataSource_ID] DEFAULT (newid()) NOT NULL,
    [Code]             VARCHAR (50)     NOT NULL,
    [Name]             VARCHAR (150)    NOT NULL,
    [Description]      VARCHAR (5000)   NULL,
    [Url]              VARCHAR (250)    NOT NULL,
    [DefaultNullValue] FLOAT (53)       NULL,
    [ErrorEstimate]    FLOAT (53)       NULL,
    [UpdateFreq]       INT              NOT NULL,
    [StartDate]        DATETIME         NULL,
    [LastUpdate]       DATETIME         NOT NULL,
    [DataSchemaID]     UNIQUEIDENTIFIER NULL,
    [UserId]           UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_DataSource] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_DataSource_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_DataSource_DataSchema] FOREIGN KEY ([DataSchemaID]) REFERENCES [dbo].[DataSchema] ([ID])
);

