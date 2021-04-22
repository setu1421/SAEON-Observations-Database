CREATE TABLE [dbo].[Sensor] (
    [ID]           UNIQUEIDENTIFIER CONSTRAINT [DF_Sensor_ID] DEFAULT (newid()) NOT NULL,
    [Code]         VARCHAR (75)     NOT NULL,
    [Name]         VARCHAR (150)    NOT NULL,
    [Description]  VARCHAR (5000)   NULL,
    [Url]          VARCHAR (250)    NULL,
    [PhenomenonID] UNIQUEIDENTIFIER NOT NULL,
    [DataSourceID] UNIQUEIDENTIFIER NOT NULL,
    [DataSchemaID] UNIQUEIDENTIFIER NULL,
    [Latitude] Float Null,
    [Longitude] Float Null,
    [Elevation] Float Null,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Sensor_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Sensor_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_Sensor] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Sensor_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_Sensor_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
    CONSTRAINT [FK_Sensor_DataSchema] FOREIGN KEY ([DataSchemaID]) REFERENCES [dbo].[DataSchema] ([ID]),
    CONSTRAINT [FK_Sensor_Phenomenon] FOREIGN KEY ([PhenomenonID]) REFERENCES [dbo].[Phenomenon] ([ID]),
    CONSTRAINT [UX_Sensor_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Sensor_Name] UNIQUE ([Name])
);
GO
CREATE INDEX [IX_Sensor_CodeName] ON [dbo].[Sensor] ([Code],[Name])
GO
CREATE INDEX [IX_Sensor_PhenomenonID] ON [dbo].[Sensor] ([PhenomenonID])
GO
CREATE INDEX [IX_Sensor_DataSourceID] ON [dbo].[Sensor] ([DataSourceID])
GO
CREATE INDEX [IX_Sensor_DataSchemaID] ON [dbo].[Sensor] ([DataSchemaID])
GO
CREATE INDEX [IX_Sensor_UserId] ON [dbo].[Sensor] ([UserId])
GO
CREATE INDEX [IX_Sensor_Latitude] ON [dbo].[Sensor] ([Latitude])
GO
CREATE INDEX [IX_Sensor_Longitude] ON [dbo].[Sensor] ([Longitude])
GO
CREATE INDEX [IX_Sensor_Elevation] ON [dbo].[Sensor] ([Elevation])
GO
CREATE TRIGGER [dbo].[TR_Sensor_Insert] ON [dbo].[Sensor]
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
        Sensor src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Sensor_Update] ON [dbo].[Sensor]
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
        Sensor src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END

