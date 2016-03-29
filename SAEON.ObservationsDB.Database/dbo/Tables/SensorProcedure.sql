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
    CONSTRAINT [PK_SensorProcedure] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_SensorProcedure_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_SensorProcedure_DataSchema] FOREIGN KEY ([DataSchemaID]) REFERENCES [dbo].[DataSchema] ([ID]),
    CONSTRAINT [FK_SensorProcedure_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
    CONSTRAINT [FK_SensorProcedure_Phenomenon] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [FK_SensorProcedure_Station] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]),
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_SensorProcedure_Code] UNIQUE ([Code])
    CONSTRAINT [UX_SensorProcedure_Code] UNIQUE ([Code]),
--< Changed 20160329 TimPN
--> Added 20160329 TimPN
    CONSTRAINT [UX_SensorProcedure_Name] UNIQUE ([Name])
--< Added 20160329 TimPN
);
--> Removed 20160329 TimPN
--GO
--CREATE UNIQUE INDEX [IX_SensorProcedure_Name] ON [dbo].[SensorProcedure]([Name]);
--< Removed 20160329 TimPN
--> Added 20160329 TimPN
GO
CREATE INDEX [IX_SensorProcedure_StationID] ON [dbo].[SensorProcedure] ([StationID])
GO
CREATE INDEX [IX_SensorProcedure_PhenomenonID] ON [dbo].[SensorProcedure] ([PhenomenonID])
GO
CREATE INDEX [IX_SensorProcedure_DataSourceID] ON [dbo].[SensorProcedure] ([DataSourceID])
GO
CREATE INDEX [IX_SensorProcedure_DataSchemaID] ON [dbo].[SensorProcedure] ([DataSchemaID])
GO
CREATE INDEX [IX_SensorProcedure_UserId] ON [dbo].[SensorProcedure] ([UserId])
--< Added 20160329 TimPN
