--> Added 2.0.9 20160727 TimPN
CREATE TABLE [dbo].[Instrument_DataSource]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Instrument_DataSource_ID] DEFAULT newid(), 
    [InstrumentID] UNIQUEIDENTIFIER NOT NULL, 
    [DataSourceID] UNIQUEIDENTIFIER NOT NULL, 
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Instrument_DataSource_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Instrument_DataSource_UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Instrument_DataSource] PRIMARY KEY NONCLUSTERED ([ID]),
    CONSTRAINT [FK_Instrument_DataSource_Instrument] FOREIGN KEY ([InstrumentID]) REFERENCES [dbo].[Instrument] ([ID]),
    CONSTRAINT [FK_Instrument_DataSource_DataSource] FOREIGN KEY ([DataSourceID]) REFERENCES [dbo].[DataSource] ([ID]),
    CONSTRAINT [FK_Instrument_DataSource_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Instrument_DataSource] UNIQUE ([InstrumentID],[DataSourceID],[StartDate],[EndDate])
)
GO
CREATE CLUSTERED INDEX [CX_Instrument_DataSource] ON [dbo].[Instrument_DataSource] ([AddedAt])
GO
CREATE INDEX [IX_Instrument_DataSource_InstrumentID] ON [dbo].[Instrument_DataSource] ([InstrumentID])
GO
CREATE INDEX [IX_Instrument_DataSource_DataSourceID] ON [dbo].[Instrument_DataSource] ([DataSourceID])
GO
CREATE INDEX [IX_Instrument_DataSource_StartDate] ON [dbo].[Instrument_DataSource] ([StartDate])
GO
CREATE INDEX [IX_Instrument_DataSource_EndDate] ON [dbo].[Instrument_DataSource] ([EndDate])
GO
CREATE INDEX [IX_Instrument_DataSource_UserId] ON [dbo].[Instrument_DataSource] ([UserId])
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_Instrument_DataSource_Insert] ON [dbo].[Instrument_DataSource]
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
        Instrument_DataSource src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Instrument_DataSource_Update] ON [dbo].[Instrument_DataSource]
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
        Instrument_DataSource src
        inner join inserted ins
            on (ins.ID = src.ID)
		inner join deleted del
			on (del.ID = src.ID)
END
--> Changed 2.0.15 20161102 TimPN
--< Added 2.0.9 20160727 TimPN
