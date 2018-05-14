--> Added 2.0.4 20160508 TimPN
CREATE TABLE [dbo].[Instrument]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Instrument_ID] DEFAULT newid(), 
--> Removed 2.0.9 20160824 TimPN
--    [StationID] UNIQUEIDENTIFIER NULL,
--< Removed 2.0.9 20160824 TimPN
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [Url] VARCHAR(250) NULL, 
--> Changed 2.0.22 20170111 TimPN
--    [StartDate]        DATETIME         NULL,
    [StartDate]        DATETIMEOFFSET         NULL,
--< Changed 2.0.22 20170111 TimPN
--> Changed 2.0.22 20170111 TimPN
--    [EndDate]        DATETIME         NULL,
    [EndDate]        DATETIMEOFFSET         NULL,
--< Changed 2.0.22 20170111 TimPN
--> Added 2.0.33 20170628 TimPN
    [Latitude] Float Null,
    [Longitude] Float Null,
    [Elevation] Float Null,
--< Added 2.0.33 20170628 TimPN
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Instrument_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Instrument_UpdatedAt] DEFAULT GetDate(), 
--> Added 2.0.33 20170628 TimPN
    [RowVersion] RowVersion not null,
--< Added 2.0.33 20170628 TimPN
    CONSTRAINT [PK_Instrument] PRIMARY KEY CLUSTERED ([ID]),
--> Removed 2.0.9 20160824 TimPN
--    CONSTRAINT [FK_Instrument_Station] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]),
--> Removed 2.0.9 20160824 TimPN
    CONSTRAINT [FK_Instrument_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
--> Removed 2.0.9 20160824 TimPN
--    CONSTRAINT [UX_Instrument_StationID_Code] UNIQUE ([StationID],[Code]),
--    CONSTRAINT [UX_Instrument_StationID_Name] UNIQUE ([StationID],[Name]),
--< Removed 2.0.9 20160824 TimPN
--> Added 2.0.9 20160824 TimPN
    CONSTRAINT [UX_Instrument_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Instrument_Name] UNIQUE ([Name])
--< Added 2.0.9 20160824 TimPN
)
GO
CREATE INDEX [IX_Instrument_StartDate] ON [dbo].[Instrument] ([StartDate])
GO
CREATE INDEX [IX_Instrument_EndDate] ON [dbo].[Instrument] ([EndDate])
GO
CREATE INDEX [IX_Instrument_UserId] ON [dbo].[Instrument] ([UserId])
--> Added 2.0.37 20180201 TimPN
GO
CREATE INDEX [IX_Instrument_Latitude] ON [dbo].[Instrument] ([Latitude])
GO
CREATE INDEX [IX_Instrument_Longitude] ON [dbo].[Instrument] ([Longitude])
GO
CREATE INDEX [IX_Instrument_Elevation] ON [dbo].[Instrument] ([Elevation])
--< Added 2.0.37 20180201 TimPN
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_Instrument_Insert] ON [dbo].[Instrument]
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
        Instrument src
        inner join inserted ins 
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Instrument_Update] ON [dbo].[Instrument]
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
        Instrument src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
--> Changed 2.0.15 20161102 TimPN
--< Added 2.0.4 20160508 TimPN
