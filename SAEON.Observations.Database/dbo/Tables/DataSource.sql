CREATE TABLE [dbo].[DataSource] (
    [ID]               UNIQUEIDENTIFIER CONSTRAINT [DF_DataSource_ID] DEFAULT (newid()) NOT NULL,
    [Code]             VARCHAR (50)     NOT NULL,
    [Name]             VARCHAR (150)    NOT NULL,
    [Description]      VARCHAR (5000)   NULL,
    [Url]              VARCHAR (250)    NULL,
    [DefaultNullValue] FLOAT            NULL,
    [ErrorEstimate]    FLOAT            NULL,
    [UpdateFreq]       INT              NOT NULL,
    [StartDate]        DATE         NULL,
    [EndDate]        DATE         NULL,
    [LastUpdate]       DATETIME         NOT NULL,
    [DataSchemaID]     UNIQUEIDENTIFIER NULL,
    [UserId]           UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_DataSource_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_DataSource_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_DataSource] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataSource_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_DataSource_DataSchema] FOREIGN KEY ([DataSchemaID]) REFERENCES [dbo].[DataSchema] ([ID]),
    CONSTRAINT [UX_DataSource_Code] Unique ([Code]),
    CONSTRAINT [UX_DataSource_Name] Unique ([Name])
);
GO
CREATE INDEX [IX_DataSource_DataSchemaID] ON [dbo].[DataSource] ([DataSchemaID])
GO
CREATE INDEX [IX_DataSource_UserId] ON [dbo].[DataSource] ([UserId])
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
        AddedAt = GetDate(),
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
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        DataSource src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
