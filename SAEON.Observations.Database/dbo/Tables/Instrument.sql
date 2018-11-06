CREATE TABLE [dbo].[Instrument]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Instrument_ID] DEFAULT (newid()), 
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [Url] VARCHAR(250) NULL, 
    [StartDate]        DATE         NULL,
    [EndDate]        DATE         NULL,
    [Latitude] Float Null,
    [Longitude] Float Null,
    [Elevation] Float Null,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Instrument_AddedAt] DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Instrument_UpdatedAt] DEFAULT (getdate()), 
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_Instrument] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Instrument_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Instrument_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_Instrument_Name] UNIQUE ([Name])
)
GO
CREATE INDEX [IX_Instrument_StartDate] ON [dbo].[Instrument] ([StartDate])
GO
CREATE INDEX [IX_Instrument_EndDate] ON [dbo].[Instrument] ([EndDate])
GO
CREATE INDEX [IX_Instrument_UserId] ON [dbo].[Instrument] ([UserId])
GO
CREATE INDEX [IX_Instrument_Latitude] ON [dbo].[Instrument] ([Latitude])
GO
CREATE INDEX [IX_Instrument_Longitude] ON [dbo].[Instrument] ([Longitude])
GO
CREATE INDEX [IX_Instrument_Elevation] ON [dbo].[Instrument] ([Elevation])
GO
CREATE TRIGGER [dbo].[TR_Instrument_Insert] ON [dbo].[Instrument]
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
        AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
        UpdatedAt = GETDATE()
    from
        Instrument src
        inner join inserted ins 
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
