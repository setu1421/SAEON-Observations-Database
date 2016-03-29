CREATE TABLE [dbo].[ImportBatch] (
    [ID]           INT              IDENTITY (1, 1) NOT NULL,
--> Changed 20160329 TimPN
--    [Guid]         UNIQUEIDENTIFIER NOT NULL 
    [Guid]         UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_ImportBatch_Guid] DEFAULT (newid()),
--< Changed 20160329 TimPN
    [DataSourceID] UNIQUEIDENTIFIER NOT NULL,
    [ImportDate]   DATETIME         NOT NULL CONSTRAINT [DF_ImportBatch_ImportDate] DEFAULT (getdate()),
    [Status]       INT              NOT NULL,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [FileName]     VARCHAR (250)    NULL,
    [LogFileName]  VARCHAR (250)    NULL,
    CONSTRAINT [PK_ImportBatch] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_ImportBatch_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_ImportBatch_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID])
);
--> Added 20160329 TimPN
GO
CREATE INDEX [IX_ImportBatch_DataSourceID] ON [dbo].[ImportBatch] ([DataSourceID])
GO
CREATE INDEX [IX_ImportBatch_UserId] ON [dbo].[ImportBatch] ([UserId])
--< Added 20160329 TimPN

