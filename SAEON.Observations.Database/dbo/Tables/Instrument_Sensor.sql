CREATE TABLE [dbo].[Instrument_Sensor]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Instrument_Sensor_ID] DEFAULT (newid()), 
    [InstrumentID] UNIQUEIDENTIFIER NOT NULL, 
    [SensorID] UNIQUEIDENTIFIER NOT NULL, 
    [StartDate]        DATETIME         NULL,
    [EndDate]        DATETIME         NULL,
    [Latitude] Float Null,
    [Longitude] Float Null,
    [Elevation] Float Null,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Instrument_Sensor_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Instrument_Sensor_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_Instrument_Sensor] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Instrument_Sensor_Instrument] FOREIGN KEY ([InstrumentID]) REFERENCES [dbo].[Instrument] ([ID]),
    CONSTRAINT [FK_Instrument_Sensor_Sensor] FOREIGN KEY ([SensorID]) REFERENCES [dbo].[Sensor] ([ID]),
    CONSTRAINT [FK_Instrument_Sensor_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Instrument_Sensor] UNIQUE ([InstrumentID],[SensorID],[StartDate],[EndDate])
)
GO
CREATE INDEX [IX_Instrument_Sensor_InstrumentID] ON [dbo].[Instrument_Sensor] ([InstrumentID])
GO
CREATE INDEX [IX_Instrument_Sensor_SensorID] ON [dbo].[Instrument_Sensor] ([SensorID])
GO
CREATE INDEX [IX_Instrument_Sensor_StartDate] ON [dbo].[Instrument_Sensor] ([StartDate])
GO
CREATE INDEX [IX_Instrument_Sensor_EndDate] ON [dbo].[Instrument_Sensor] ([EndDate])
GO
CREATE INDEX [IX_Instrument_Sensor_StartDateEndDate] ON [dbo].[Instrument_Sensor] ([StartDate],[EndDate])
GO
CREATE INDEX [IX_Instrument_Sensor_Latitude] ON [dbo].[Instrument_Sensor] ([Latitude])
GO
CREATE INDEX [IX_Instrument_Sensor_Longitude] ON [dbo].[Instrument_Sensor] ([Longitude])
GO
CREATE INDEX [IX_Instrument_Sensor_Elevation] ON [dbo].[Instrument_Sensor] ([Elevation])
GO
CREATE INDEX [IX_Instrument_Sensor_UserId] ON [dbo].[Instrument_Sensor] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_Instrument_Sensor_Insert] ON [dbo].[Instrument_Sensor]
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
        Instrument_Sensor src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Instrument_Sensor_Update] ON [dbo].[Instrument_Sensor]
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
        Instrument_Sensor src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
