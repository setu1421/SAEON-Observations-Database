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
    [DataSchemaID]     UNIQUEIDENTIFIER NOT NULL,
    [UserId]           UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_DataSource] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataSource_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_DataSource_DataSchema] FOREIGN KEY ([DataSchemaID]) REFERENCES [dbo].[DataSchema] ([ID]),
--> Added 20160329 TimPN
    CONSTRAINT [UX_DataSource_Code] Unique ([Code]),
    CONSTRAINT [UX_DataSource_Name] Unique ([Name])
--< Added 20160329 TimPN
);
GO
--> Added 20160329 TimPN
CREATE INDEX [IX_DataSource_DataSchemaID] ON [dbo].[DataSource] ([DataSchemaID])
GO
CREATE INDEX [IX_DataSource_UserId] ON [dbo].[DataSource] ([UserId])
--< Added 20160329 TimPN
