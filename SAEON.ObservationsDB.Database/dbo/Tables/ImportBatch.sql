CREATE TABLE [dbo].[ImportBatch] (
    [ID]           INT              IDENTITY (1, 1) NOT NULL,
    [Guid]         UNIQUEIDENTIFIER NOT NULL,
    [DataSourceID] UNIQUEIDENTIFIER NOT NULL,
    [ImportDate]   DATETIME         CONSTRAINT [DF_ImportBatch_ImportDate] DEFAULT (getdate()) NOT NULL,
    [Status]       INT              NOT NULL,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [FileName]     VARCHAR (250)    NULL,
    [LogFileName]  VARCHAR (250)    NULL,
    CONSTRAINT [PK_ImportBatch] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_ImportBatch_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_ImportBatch_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID])
);

