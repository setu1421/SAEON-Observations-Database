CREATE TABLE [dbo].[SensorProcedure] (
    [ID]           UNIQUEIDENTIFIER CONSTRAINT [DF_SensorProcedure_ID] DEFAULT (newid()) NOT NULL,
    [Code]         VARCHAR (50)     NOT NULL,
    [Name]         VARCHAR (150)    NOT NULL,
    [Description]  VARCHAR (5000)   NULL,
    [Url]          VARCHAR (250)    NULL,
    [StationID]    UNIQUEIDENTIFIER NOT NULL,
    [PhenomenonID] UNIQUEIDENTIFIER NOT NULL,
    [DataSourceID] UNIQUEIDENTIFIER NOT NULL,
    [DataSchemaID] UNIQUEIDENTIFIER NULL,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_SensorProcedure] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_SensorProcedure_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_SensorProcedure_DataSchema] FOREIGN KEY ([DataSchemaID]) REFERENCES [dbo].[DataSchema] ([ID]),
    CONSTRAINT [FK_SensorProcedure_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
    CONSTRAINT [FK_SensorProcedure_Phenomenon] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [FK_SensorProcedure_Station] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]),
    CONSTRAINT [IX_SensorProcedure_Code] UNIQUE NONCLUSTERED ([Code] ASC) WITH (FILLFACTOR = 80)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_SensorProcedure_Name]
    ON [dbo].[SensorProcedure]([Name] ASC) WITH (FILLFACTOR = 80);

