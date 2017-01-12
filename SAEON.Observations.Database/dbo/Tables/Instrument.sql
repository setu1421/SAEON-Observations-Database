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
    [StartDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
--> Changed 2.0.22 20170111 TimPN
--    [EndDate]        DATETIME         NULL,
    [EndDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Instrument_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Instrument_UpdatedAt] DEFAULT GetDate(), 
--> Changed 2.0.5 20160516 TimPN
--    CONSTRAINT [PK_Instrument] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [PK_Instrument] PRIMARY KEY NONCLUSTERED ([ID]),
--< Changed 2.0.5 20160516 TimPN
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
--> Added 2.0.5 20160516 TimPN
GO
--> Changed 2.0.23 20170112 TimPN
--CREATE CLUSTERED INDEX [CX_Instrument] ON [dbo].[Instrument] ([AddedAt])
CREATE UNIQUE CLUSTERED INDEX [CX_Instrument] ON [dbo].[Instrument] ([AddedAt])
--< Changed 2.0.23 20170112 TimPN
--< Added 2.0.5 20160516 TimPN
GO
CREATE INDEX [IX_Instrument_StartDate] ON [dbo].[Instrument] ([StartDate])
GO
CREATE INDEX [IX_Instrument_EndDate] ON [dbo].[Instrument] ([EndDate])
GO
CREATE INDEX [IX_Instrument_UserId] ON [dbo].[Instrument] ([UserId])
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
