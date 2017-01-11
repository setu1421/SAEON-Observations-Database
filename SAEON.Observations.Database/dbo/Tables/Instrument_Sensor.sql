--> Added 2.0.5 20160530 TimPN
CREATE TABLE [dbo].[Instrument_Sensor]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Instrument_Sensor_ID] DEFAULT newid(), 
    [InstrumentID] UNIQUEIDENTIFIER NOT NULL, 
    [SensorID] UNIQUEIDENTIFIER NOT NULL, 
--> Changed 2.0.22 20170111 TimPN
--    [StartDate]        DATETIME         NULL,
    [StartDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
--> Changed 2.0.22 20170111 TimPN
--    [EndDate]        DATETIME         NULL,
    [EndDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Instrument_Sensor_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Instrument_Sensor_UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Instrument_Sensor] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [FK_Instrument_Sensor_Instrument] FOREIGN KEY ([InstrumentID]) REFERENCES [dbo].[Instrument] ([ID]),
    CONSTRAINT [FK_Instrument_Sensor_Sensor] FOREIGN KEY ([SensorID]) REFERENCES [dbo].[Sensor] ([ID]),
    CONSTRAINT [FK_Instrument_Sensor_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Instrument_Sensor] UNIQUE ([InstrumentID],[SensorID],[StartDate],[EndDate])
)
GO
CREATE CLUSTERED INDEX [CX_Instrument_Sensor] ON [dbo].[Instrument_Sensor] ([AddedAt])
GO
CREATE INDEX [IX_Instrument_Sensor_InstrumentID] ON [dbo].[Instrument_Sensor] ([InstrumentID])
GO
CREATE INDEX [IX_Instrument_Sensor_SensorID] ON [dbo].[Instrument_Sensor] ([SensorID])
GO
CREATE INDEX [IX_Instrument_Sensor_StartDate] ON [dbo].[Instrument_Sensor] ([StartDate])
GO
CREATE INDEX [IX_Instrument_Sensor_EndDate] ON [dbo].[Instrument_Sensor] ([EndDate])
GO
CREATE INDEX [IX_Instrument_Sensor_UserId] ON [dbo].[Instrument_Sensor] ([UserId])
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_Instrument_Sensor_Insert] ON [dbo].[Instrument_Sensor]
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
--> Changed 2.0.19 20161205 TimPN
--		AddedAt = del.AddedAt,
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate ()),
--< Changed 2.0.19 20161205 TimPN
        UpdatedAt = GETDATE()
    from
        Instrument_Sensor src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--> Added 2.0.5 20160530 TimPN
