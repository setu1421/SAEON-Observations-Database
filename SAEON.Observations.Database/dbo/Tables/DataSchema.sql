CREATE TABLE [dbo].[DataSchema] (
    [ID]               UNIQUEIDENTIFIER CONSTRAINT [DF_DataSchema_ID] DEFAULT (newid()) NOT NULL,
    [Code]             VARCHAR (50)     NOT NULL,
    [Name]             VARCHAR (100)    NOT NULL,
    [Description]      VARCHAR (5000)   NULL,
    [DataSourceTypeID] UNIQUEIDENTIFIER NOT NULL,
    [IgnoreFirst]      INT              CONSTRAINT [DF_DataSchema_IgnoreFirst] DEFAULT ((0)) NOT NULL,
    [HasColumnNames]	BIT				NULL,
    [IgnoreLast]       INT              CONSTRAINT [DF_DataSchema_IgnoreLast] DEFAULT ((0)) NOT NULL,
    [Condition]        VARCHAR (500)    NULL,
    [DataSchema]       TEXT             NULL,
    [UserId]           UNIQUEIDENTIFIER NOT NULL,
    [Delimiter]        VARCHAR (3)      NULL,
    [SplitSelector]    VARCHAR (50)     NULL,
    [SplitIndex]       INT              NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_DataSchema_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_DataSchema_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_DataSchema] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataSchema_DataSourceType] FOREIGN KEY ([DataSourceTypeID]) REFERENCES [dbo].[DataSourceType] ([ID]),
    CONSTRAINT [FK_DataSchema_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_DataSchema_Code] Unique ([Code]),
    CONSTRAINT [UX_DataSchema_Name] Unique ([Name])
);
GO
CREATE INDEX [IX_DataSchema_DataSourceTypeID] ON [dbo].[DataSchema] ([DataSourceTypeID])
GO
CREATE INDEX [IX_DataSchema_UserId] ON [dbo].[DataSchema] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_DataSchema_Insert] ON [dbo].[DataSchema]
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
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        DataSchema src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END

