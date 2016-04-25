CREATE TABLE [dbo].[DataSource] (
    [ID]               UNIQUEIDENTIFIER CONSTRAINT [DF_DataSource_ID] DEFAULT (newid()) NOT NULL,
    [Code]             VARCHAR (50)     NOT NULL,
    [Name]             VARCHAR (150)    NOT NULL,
    [Description]      VARCHAR (5000)   NULL,
    [Url]              VARCHAR (250)    NOT NULL,
    [DefaultNullValue] FLOAT (53)       NULL,
    [ErrorEstimate]    FLOAT (53)       NULL,
    [UpdateFreq]       INT              NOT NULL,
    [StartDate]        DATETIME         NULL,
    [LastUpdate]       DATETIME         NOT NULL,
    [DataSchemaID]     UNIQUEIDENTIFIER NULL,
    [UserId]           UNIQUEIDENTIFIER NOT NULL,
--> Added 2.0.0.2 20160419 TimPN
--    [StationID] UNIQUEIDENTIFIER NOT NULL,
    [StationID] UNIQUEIDENTIFIER NULL, -- Must be NOT NULL once all Stations have Sites
--< Added 2.0.0.2 20160419 TimPN
--> Added 2.0.0.3 20160421 TimPN
    [AddedAt] DATETIME NULL CONSTRAINT [DF_DataSource_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_DataSource_UpdatedAt] DEFAULT GetDate(), 
--< Added 2.0.0.3 20160421 TimPN
    CONSTRAINT [PK_DataSource] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_DataSource_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_DataSource_DataSchema] FOREIGN KEY ([DataSchemaID]) REFERENCES [dbo].[DataSchema] ([ID]),
--> Added 2.0.0.2 20160419 TimPN
    CONSTRAINT [FK_DataSource_Station] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]),
--    CONSTRAINT [UX_DataSource_StationID_Code] UNIQUE ([StationID],[Code]), -- Must be added once all DataSources have Stations
--    CONSTRAINT [UX_DataSource_StationID_Name] UNIQUE ([StationID],[Name]), -- Must be added once all DataSources have Stations
--< Added 2.0.0.2 20160419 TimPN
--> Added 2.0.0.0 20160406 TimPN
    CONSTRAINT [UX_DataSource_Code] Unique ([Code]),
    CONSTRAINT [UX_DataSource_Name] Unique ([Name])
--< Added 2.0.0.0 20160406 TimPN
);
GO
--> Added 2.0.0.0 20160406 TimPN
CREATE INDEX [IX_DataSource_DataSchemaID] ON [dbo].[DataSource] ([DataSchemaID])
GO
CREATE INDEX [IX_DataSource_UserId] ON [dbo].[DataSource] ([UserId])
--< Added 2.0.0.0 20160406 TimPN
--> Added 2.0.0.0 20160406 TimPN
GO
CREATE INDEX [IX_DataSource_StationID] ON [dbo].Station ([ID])
--< Added 2.0.0.0 20160406 TimPN
--> Added 2.0.0.3 20160421 TimPN
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
        inserted ins
        inner join DataSource src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_DataSource_Update] ON [dbo].[DataSource]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    if UPDATE(AddedAt) RAISERROR ('Cannot update AddedAt.', 16, 1)
    if UPDATE(UpdatedAt) RAISERROR ('Cannot update UpdatedAt.', 16, 1)
    if not UPDATE(AddedAt) and not UPDATE(UpdatedAt)
        Update
            src
        set
            UpdatedAt = GETDATE()
        from
            inserted ins
            inner join DataSource src
                on (ins.ID = src.ID)
END
--< Added 2.0.0.3 20160421 TimPN
