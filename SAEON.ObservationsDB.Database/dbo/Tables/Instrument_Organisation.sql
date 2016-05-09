--> Added 2.0.0.4 20160508 TimPN
CREATE TABLE [dbo].[Instrument_Organisation]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Instrument_Organisation_ID] DEFAULT newid(), 
    [InstrumentID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationRoleID] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AddedAt] DATETIME NULL CONSTRAINT [DF_Instrument_Organisation_AddedAt] DEFAULT GetDate(), 
    [UpdatedAt] DATETIME NULL CONSTRAINT [DF_Instrument_Organisation_UpdatedAt] DEFAULT GetDate(), 
    CONSTRAINT [PK_Instrument_Organisation] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_Instrument_Organisation_Instrument] FOREIGN KEY ([InstrumentID]) REFERENCES [dbo].[Instrument] ([ID]),
    CONSTRAINT [FK_Instrument_Organisation_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [FK_Instrument_Organisation_OrganisationRole] FOREIGN KEY ([OrganisationRoleID]) REFERENCES [dbo].[OrganisationRole] ([ID]),
    CONSTRAINT [FK_Instrument_Organisation_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_Instrument_Organisation] UNIQUE ([InstrumentID],[OrganisationID],[OrganisationRoleID],[StartDate],[EndDate])
)
GO
CREATE INDEX [IX_Instrument_Organisation_InstrumentID] ON [dbo].[Instrument_Organisation] ([InstrumentID])
GO
CREATE INDEX [IX_Instrument_Organisation_OrganisationID] ON [dbo].[Instrument_Organisation] ([OrganisationID])
GO
CREATE INDEX [IX_Instrument_Organisation_OrganisationRoleID] ON [dbo].[Instrument_Organisation] ([OrganisationRoleID])
GO
CREATE INDEX [IX_Instrument_Organisation_StartDate] ON [dbo].[Instrument_Organisation] ([StartDate])
GO
CREATE INDEX [IX_Instrument_Organisation_EndDate] ON [dbo].[Instrument_Organisation] ([EndDate])
GO
CREATE INDEX [IX_Instrument_Organisation_UserId] ON [dbo].[Instrument_Organisation] ([UserId])
GO
CREATE TRIGGER [dbo].[TR_Instrument_Organisation_Insert] ON [dbo].[Instrument_Organisation]
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
        inner join Instrument_Organisation src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Instrument_Organisation_Update] ON [dbo].[Instrument_Organisation]
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
        inner join Instrument_Organisation src
            on (ins.ID = src.ID)
END
--< Added 2.0.0.4 20160508 TimPN
