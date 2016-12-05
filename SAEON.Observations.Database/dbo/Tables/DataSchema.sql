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
--> Added 2.0.8 20160715 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_DataSchema_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_DataSchema_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.8 20160715 TimPN
--> Changed 2.0.8 20160715 TimPN
--    CONSTRAINT [PK_DataSchema] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_DataSchema] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.8 20160715 TimPN
    CONSTRAINT [FK_DataSchema_DataSourceType] FOREIGN KEY ([DataSourceTypeID]) REFERENCES [dbo].[DataSourceType] ([ID]),
--> Added 2.0.0 20160406 TimPN
--> Added 2.0.9 20160727 TimPN
    CONSTRAINT [FK_DataSchema_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--> Added 2.0.9 20160727 TimPN
    CONSTRAINT [UX_DataSchema_Code] Unique ([Code]),
    CONSTRAINT [UX_DataSchema_Name] Unique ([Name])
--< Added 2.0.0 20160406 TimPN
);
--> Added 2.0.8 20160715 TimPN
GO
CREATE CLUSTERED INDEX [CX_DataSchema] ON [dbo].[DataSchema] ([AddedAt])
--< Added 2.0.8 20160715 TimPN
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_DataSchema_DataSourceTypeID] ON [dbo].[DataSchema] ([DataSourceTypeID])
GO
CREATE INDEX [IX_DataSchema_UserId] ON [dbo].[DataSchema] ([UserId])
--< Added 2.0.0 20160406 TimPN
--> Added 2.0.8 20160716 TimPN
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_DataSchema_Insert] ON [dbo].[DataSchema]
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
        DataSchema src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_DataSchema_Update] ON [dbo].[DataSchema]
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
        DataSchema src
        inner join inserted ins 
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.8 20160715 TimPN

