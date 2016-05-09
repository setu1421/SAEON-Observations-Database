--> Added 2.0.0.4 20160508 TimPN
CREATE TABLE [dbo].[Instrument]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Instrument_ID] DEFAULT newid(), 
    [StationID] UNIQUEIDENTIFIER NULL,
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [Url] VARCHAR(250) NULL, 
    [StartDate] DATETIME NULL, 
    [EndDate] DATETIME NULL, 
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Instrument_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Instrument_UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Instrument] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_Instrument_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Instrument_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_Instrument_Station] FOREIGN KEY ([StationID]) REFERENCES [dbo].[Station] ([ID]),
    CONSTRAINT [FK_Instrument_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Instrument_StationID_Code] UNIQUE ([StationID],[Code]),
    CONSTRAINT [UX_Instrument_StationID_Name] UNIQUE ([StationID],[Name]),
)
GO
CREATE INDEX [IX_Instrument_StartDate] ON [dbo].[Site] ([StartDate])
GO
CREATE INDEX [IX_Instrument_EndDate] ON [dbo].[Site] ([EndDate])
GO
CREATE INDEX [IX_Instrument_UserId] ON [dbo].[Site] ([UserId])
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
        inserted ins 
        inner join Instrument src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Instrument_Update] ON [dbo].[Instrument]
FOR UPDATE
AS
BEGIN
    SET NoCount ON
    --if UPDATE(AddedAt) RAISERROR ('Cannot update AddedAt.', 16, 1)
    Update 
        src 
    set 
        UpdatedAt = GETDATE()
    from
        inserted ins 
        inner join Instrument src
            on (ins.ID = src.ID)
END
--< Added 2.0.0.4 20160508 TimPN
