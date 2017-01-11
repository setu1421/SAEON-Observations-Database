CREATE TABLE [dbo].[DataSource] (
    [ID]               UNIQUEIDENTIFIER CONSTRAINT [DF_DataSource_ID] DEFAULT (newid()) NOT NULL,
    [Code]             VARCHAR (50)     NOT NULL,
    [Name]             VARCHAR (150)    NOT NULL,
    [Description]      VARCHAR (5000)   NULL,
    [Url]              VARCHAR (250)    NOT NULL,
    [DefaultNullValue] FLOAT (53)       NULL,
    [ErrorEstimate]    FLOAT (53)       NULL,
    [UpdateFreq]       INT              NOT NULL,
--> Changed 2.0.22 20170111 TimPN
--    [StartDate]        DATETIME         NULL,
    [StartDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
--> Added 2.0.2 20160419 TimPN
--> Changed 2.0.22 20170111 TimPN
--    [EndDate]        DATETIME         NULL,
    [EndDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
--< Added 2.0.2 20160419 TimPN
    [LastUpdate]       DATETIME         NOT NULL,
    [DataSchemaID]     UNIQUEIDENTIFIER NULL,
    [UserId]           UNIQUEIDENTIFIER NOT NULL,
--> Added 2.0.3 20160421 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_DataSource_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_DataSource_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.3 20160421 TimPN
--> Changed 2.0.8 20160715 TimPN
--    CONSTRAINT [PK_DataSource] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_DataSource] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.8 20160715 TimPN
    CONSTRAINT [FK_DataSource_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_DataSource_DataSchema] FOREIGN KEY ([DataSchemaID]) REFERENCES [dbo].[DataSchema] ([ID]),
--> Added 2.0.0 20160406 TimPN
    CONSTRAINT [UX_DataSource_Code] Unique ([Code]),
    CONSTRAINT [UX_DataSource_Name] Unique ([Name])
--< Added 2.0.0 20160406 TimPN
);
--> Added 2.0.8 20160715 TimPN
GO
CREATE CLUSTERED INDEX [CX_DataSource] ON [dbo].[DataSource] ([AddedAt])
--< Added 2.0.8 20160715 TimPN
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_DataSource_DataSchemaID] ON [dbo].[DataSource] ([DataSchemaID])
GO
CREATE INDEX [IX_DataSource_UserId] ON [dbo].[DataSource] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.3 20160421 TimPN
--> Changed 2.0.15 20161102 TimPN
GO
CREATE INDEX [IX_DataSource_StartDate] ON [dbo].DataSource ([StartDate])
GO
CREATE INDEX [IX_DataSource_EndDate] ON [dbo].DataSource ([EndDate])
GO
CREATE TRIGGER [dbo].[TR_DataSource_Insert] ON [dbo].[DataSource]
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
        DataSource src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_DataSource_Update] ON [dbo].[DataSource]
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
        DataSource src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.3 20160421 TimPN
