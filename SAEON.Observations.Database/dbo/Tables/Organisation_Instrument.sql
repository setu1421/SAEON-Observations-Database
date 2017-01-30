--> Added 2.0.5 20160530 TimPN
CREATE TABLE [dbo].[Organisation_Instrument]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Organisation_Instrument_ID] DEFAULT newid(), 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [InstrumentID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationRoleID] UNIQUEIDENTIFIER NOT NULL,
--> Changed 2.0.22 20170111 TimPN
--    [StartDate]        DATETIME         NULL,
    [StartDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
--> Changed 2.0.22 20170111 TimPN
--    [EndDate]        DATETIME         NULL,
    [EndDate]        DATE         NULL,
--< Changed 2.0.22 20170111 TimPN
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Organisation_Instrument_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Organisation_Instrument_UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Organisation_Instrument] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Organisation_Instrument_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [FK_Organisation_Instrument_Instrument] FOREIGN KEY ([InstrumentID]) REFERENCES [dbo].[Instrument] ([ID]),
    CONSTRAINT [FK_Organisation_Instrument_OrganisationRole] FOREIGN KEY ([OrganisationRoleID]) REFERENCES [dbo].[OrganisationRole] ([ID]),
    CONSTRAINT [FK_Organisation_Instrument_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Organisation_Instrument] UNIQUE ([OrganisationID],[InstrumentID],[OrganisationRoleID],[StartDate],[EndDate])
)
GO
CREATE INDEX [IX_Organisation_Instrument_InstrumentID] ON [dbo].[Organisation_Instrument] ([InstrumentID])
GO
CREATE INDEX [IX_Organisation_Instrument_OrganisationRoleID] ON [dbo].[Organisation_Instrument] ([OrganisationRoleID])
GO
CREATE INDEX [IX_Organisation_Instrument_OrganisationID] ON [dbo].[Organisation_Instrument] ([OrganisationID])
GO
CREATE INDEX [IX_Organisation_Instrument_StartDate] ON [dbo].[Organisation_Instrument] ([StartDate])
GO
CREATE INDEX [IX_Organisation_Instrument_EndDate] ON [dbo].[Organisation_Instrument] ([EndDate])
GO
CREATE INDEX [IX_Organisation_Instrument_UserId] ON [dbo].[Organisation_Instrument] ([UserId])
--> Changed 2.0.15 20161102 TimPN
GO
CREATE TRIGGER [dbo].[TR_Organisation_Instrument_Insert] ON [dbo].[Organisation_Instrument]
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
        Organisation_Instrument src
        inner join inserted ins
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Organisation_Instrument_Update] ON [dbo].[Organisation_Instrument]
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
        Organisation_Instrument src
        inner join inserted ins
            on (ins.ID = src.ID)
        inner join deleted del
            on (del.ID = src.ID)
END
--< Changed 2.0.15 20161102 TimPN
--< Added 2.0.5 20160530 TimPN
