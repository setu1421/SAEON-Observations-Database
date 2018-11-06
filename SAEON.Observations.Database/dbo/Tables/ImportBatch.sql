CREATE TABLE [dbo].[ImportBatch] (
    [ID]         UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL CONSTRAINT [DF_ImportBatch_ID] DEFAULT (newid()),
    [Code]         INT              IDENTITY (1, 1) NOT NULL,
    [DataSourceID] UNIQUEIDENTIFIER NOT NULL,
    [ImportDate]   DATETIME         NOT NULL CONSTRAINT [DF_ImportBatch_ImportDate] DEFAULT (getdate()),
    [Status]       INT              NOT NULL,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [FileName]     VARCHAR (250)    NULL,
    [LogFileName]  VARCHAR (250)    NULL,
    [Comment] VARCHAR(8000) NULL, 
    [StatusID] UNIQUEIDENTIFIER NULL,
    [StatusReasonID] UNIQUEIDENTIFIER NULL,
    [Issues] VARCHAR(1000) NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_ImportBatch_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_ImportBatch_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_ImportBatch] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_ImportBatch_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_ImportBatch_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
    CONSTRAINT [FK_ImportBatch_Status] FOREIGN KEY ([StatusID]) REFERENCES [dbo].[Status] ([ID]),
    CONSTRAINT [FK_ImportBatch_StatusReason] FOREIGN KEY ([StatusReasonID]) REFERENCES [dbo].[StatusReason] ([ID]),
    CONSTRAINT [UX_ImportBatch_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_ImportBatch] UNIQUE ([DataSourceID], [ImportDate], [LogFileName])
);
GO
CREATE INDEX [IX_ImportBatch_DataSourceID] ON [dbo].[ImportBatch] ([DataSourceID])
GO
CREATE INDEX [IX_ImportBatch_UserId] ON [dbo].[ImportBatch] ([UserId])
GO
CREATE INDEX [IX_ImportBatch_ImportDate] ON [dbo].[ImportBatch] ([DataSourceID], [ImportDate])
GO
CREATE INDEX [IX_ImportBatch_LogFileName] ON [dbo].[ImportBatch] ([DataSourceID], [LogFileName])
GO
CREATE INDEX [IX_ImportBatch_StatusID] ON [dbo].[ImportBatch] ([StatusID])
GO
CREATE INDEX [IX_ImportBatch_StatusReasonID] ON [dbo].[ImportBatch] ([StatusReasonID])
GO
CREATE TRIGGER [dbo].[TR_ImportBatch_Insert] ON [dbo].[ImportBatch]
FOR INSERT
AS
BEGIN
    SET NoCount ON
    Update
        src
    set
        AddedAt = GetDate(),
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
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        ImportBatch src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END

