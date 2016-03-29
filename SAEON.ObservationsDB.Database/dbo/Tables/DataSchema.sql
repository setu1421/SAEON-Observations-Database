CREATE TABLE [dbo].[DataSchema] (
    [ID]               UNIQUEIDENTIFIER CONSTRAINT [DF_DataSchema_ID] DEFAULT (newid()) NOT NULL,
    [Code]             VARCHAR (50)     NOT NULL,
    [Name]             VARCHAR (100)    NOT NULL,
    [Description]      VARCHAR (5000)   NULL,
    [DataSourceTypeID] UNIQUEIDENTIFIER NOT NULL,
    [IgnoreFirst]      INT              CONSTRAINT [DF_DataSchema_IgnoreFirst] DEFAULT ((0)) NOT NULL,
    [IgnoreLast]       INT              CONSTRAINT [DF_DataSchema_IgnoreLast] DEFAULT ((0)) NOT NULL,
    [Condition]        VARCHAR (500)    NULL,
    [DataSchema]       TEXT             NULL,
    [UserId]           UNIQUEIDENTIFIER NOT NULL,
    [Delimiter]        VARCHAR (3)      NULL,
    [SplitSelector]    VARCHAR (50)     NULL,
    [SplitIndex]       INT              NULL,
    CONSTRAINT [PK_DataSchema] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataSchema_DataSourceType] FOREIGN KEY ([DataSourceTypeID]) REFERENCES [dbo].[DataSourceType] ([ID]),
--> Added 20160329 TimPN
    CONSTRAINT [UX_DataSchema_Code] Unique ([Code]),
    CONSTRAINT [UX_DataSchema_Name] Unique ([Name])
--< Added 20160329 TimPN
);

--> Added 20160329 TimPN
GO
CREATE INDEX [IX_DataSchema_DataSourceTypeID] ON [dbo].[DataSchema] ([DataSourceTypeID])
GO
CREATE INDEX [IX_DataSchema_UserId] ON [dbo].[DataSchema] ([UserId])
--< Added 20160329 TimPN
