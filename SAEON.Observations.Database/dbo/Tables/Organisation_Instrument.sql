--> Added 2.0.5 20160530 TimPN
CREATE TABLE [dbo].[Organisation_Instrument]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_Organisation_Instrument_ID] DEFAULT newid(), 
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL, 
    [InstrumentID] UNIQUEIDENTIFIER NOT NULL, 
    [OrganisationRoleID] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
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
        inserted ins
        inner join Organisation_Instrument src
            on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Organisation_Instrument_Update] ON [dbo].[Organisation_Instrument]
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
        inner join Organisation_Instrument src
            on (ins.ID = src.ID)
END
--< Added 2.0.5 20160530 TimPN
