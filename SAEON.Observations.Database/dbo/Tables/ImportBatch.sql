CREATE TABLE [dbo].[ImportBatch] (
    [ID]           INT              IDENTITY (1, 1) NOT NULL,
--> Changed 2.0.0 20160329 TimPN
--    [Guid]         UNIQUEIDENTIFIER NOT NULL 
    [Guid]         UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_ImportBatch_Guid] DEFAULT newid(),
--< Changed 2.0.0 20160329 TimPN
    [DataSourceID] UNIQUEIDENTIFIER NOT NULL,
    [ImportDate]   DATETIME         NOT NULL CONSTRAINT [DF_ImportBatch_ImportDate] DEFAULT getdate(),
    [Status]       INT              NOT NULL,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [FileName]     VARCHAR (250)    NULL,
    [LogFileName]  VARCHAR (250)    NULL,
--> Added 2.0.4 20160426 TimPN
    [Comment] VARCHAR(8000) NULL, 
--< Added 2.0.4 20160426 TimPN
--> Added 2.0.8 20160715 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_ImportBatch_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_ImportBatch_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160715 TimPN
--> Changed 2.0.8 20160715 TimPN
--    CONSTRAINT [PK_ImportBatch] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_ImportBatch] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.8 20160715 TimPN
    CONSTRAINT [FK_ImportBatch_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_ImportBatch_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
--> Added 2.0.8 20160719 TimPN
    CONSTRAINT [UX_ImportBatch_Guid] UNIQUE ([Guid])
--< Added 2.0.8 20160719 TimPN
);
--> Added 2.0.8 20160715 TimPN
GO
CREATE CLUSTERED INDEX [CX_ImportBatch] ON [dbo].[ImportBatch] ([AddedAt])
--< Added 2.0.8 20160715 TimPN
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_ImportBatch_DataSourceID] ON [dbo].[ImportBatch] ([DataSourceID])
GO
CREATE INDEX [IX_ImportBatch_UserId] ON [dbo].[ImportBatch] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160715 TimPN
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
        inserted ins
        inner join ImportBatch src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_ImportBatch_Update] ON [dbo].[ImportBatch]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    --if UPDATE(AddedAt) RAISERROR ('Cannot update AddedAt.', 16, 1)
    Update
        src
    set
        UpdatedAt = GETDATE()
    from
        inserted ins
        inner join ImportBatch src
            on (ins.ID = src.ID)
END
--< Added 2.0.8 20160715 TimPN

