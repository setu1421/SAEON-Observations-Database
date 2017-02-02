CREATE TABLE [dbo].[ImportBatch] (
--> Changed 2.0.8 20160720 TimPN
--    [ID]           INT              IDENTITY (1, 1) NOT NULL,
--> Changed 2.0.18 20161130 TimPN
--    [ID]         UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_ImportBatch_ID] DEFAULT newid(),
    [ID]         UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL CONSTRAINT [DF_ImportBatch_ID] DEFAULT newid(),
--< Changed 2.0.18 20161130 TimPN
--< Changed 2.0.8 20160720 TimPN
--> Removed 2.0.8 20160720 TimPN
--> Changed 2.0.0 20160329 TimPN
--    [Guid]         UNIQUEIDENTIFIER NOT NULL 
--    [Guid]         UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_ImportBatch_ID] DEFAULT newid(),
--< Changed 2.0.0 20160329 TimPN
--> Removed 2.0.8 20160720 TimPN
--> Added 2.0.8 20160920 TimPN
    [Code]         INT              IDENTITY (1, 1) NOT NULL,
--< Added 2.0.8 20160920 TimPN
    [DataSourceID] UNIQUEIDENTIFIER NOT NULL,
    [ImportDate]   DATETIME         NOT NULL CONSTRAINT [DF_ImportBatch_ImportDate] DEFAULT getdate(),
    [Status]       INT              NOT NULL,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [FileName]     VARCHAR (250)    NULL,
    [LogFileName]  VARCHAR (250)    NULL,
--> Added 2.0.4 20160426 TimPN
    [Comment] VARCHAR(8000) NULL, 
--< Added 2.0.4 20160426 TimPN
--> Added 2.0.9 20160823 TimPN
    [StatusID] UNIQUEIDENTIFIER NULL,
    [StatusReasonID] UNIQUEIDENTIFIER NULL,
--< Added 2.0.9 20160823 TimPN
--> Added 2.0.21 20170106 TimPN
    [Problems] VARCHAR(1000) NULL,
--< Added 2.0.21 20170106 TimPN
--> Added 2.0.18 20161130 TimPN
    [SourceFile] VARBINARY(MAX) FILESTREAM NULL, 
    [Pass1File] VARBINARY(MAX) FILESTREAM NULL, 
    [Pass2File] VARBINARY(MAX) FILESTREAM NULL, 
    [Pass3File] VARBINARY(MAX) FILESTREAM NULL, 
    [Pass4File] VARBINARY(MAX) FILESTREAM NULL, 
--< Added 2.0.18 20161130 TimPN
--> Added 2.0.8 20160715 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_ImportBatch_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_ImportBatch_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160715 TimPN
    CONSTRAINT [PK_ImportBatch] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_ImportBatch_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_ImportBatch_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
--> Added 2.0.9 20160823 TimPN
    CONSTRAINT [FK_ImportBatch_Status] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[Status] ([ID]),
    CONSTRAINT [FK_ImportBatch_StatusReason] FOREIGN KEY ([StatusReasonID]) REFERENCES [dbo].[StatusReason] ([ID]),
--< Added 2.0.9 20160823 TimPN
--> Added 2.0.8 20160920 TimPN
    CONSTRAINT [UX_ImportBatch_Code] UNIQUE ([Code]),
--< Added 2.0.8 20160920 TimPN
--> Added 2.0.8 20160726 TimPN
    CONSTRAINT [UX_ImportBatch] UNIQUE ([DataSourceID], [ImportDate], [LogFileName])
--< Added 2.0.8 20160726 TimPN
);
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_ImportBatch_DataSourceID] ON [dbo].[ImportBatch] ([DataSourceID])
GO
CREATE INDEX [IX_ImportBatch_UserId] ON [dbo].[ImportBatch] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160715 TimPN
--> Added 2.0.8 20160726 TimPN
GO
CREATE INDEX [IX_ImportBatch_ImportDate] ON [dbo].[ImportBatch] ([DataSourceID], [ImportDate])
GO
CREATE INDEX [IX_ImportBatch_LogFileName] ON [dbo].[ImportBatch] ([DataSourceID], [LogFileName])
--< Added 2.0.8 20160726 TimPN
--> Added 2.0.9 20160823 TimPN
GO
CREATE INDEX [IX_ImportBatch_StatusID] ON [dbo].[ImportBatch] ([StatusID])
GO
CREATE INDEX [IX_ImportBatch_StatusReasonID] ON [dbo].[ImportBatch] ([StatusReasonID])
--< Added 2.0.9 20160823 TimPN
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_ImportBatch_Insert] ON [dbo].[ImportBatch]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GETDATE(),
        UpdatedAt = NULL
    from
        ImportBatch src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_ImportBatch_Update] ON [dbo].[ImportBatch]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
--> Changed 2.0.19 20161205 TimPN
--		AddedAt = del.AddedAt,
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate ()),
--< Changed 2.0.19 20161205 TimPN
        UpdatedAt = GETDATE()
    from
        ImportBatch src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160715 TimPN

