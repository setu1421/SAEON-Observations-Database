CREATE TABLE [dbo].[Datasets]
(
    [ID] UniqueIdentifier Constraint DF_Datasets_ID default NewId() not null,
	[Code] VARCHAR(200) not null,
	[Name] VARCHAR(500) not null,
    [Description] VARCHAR(5000) null,
    [Title] VARCHAR(5000) null,
    [StationID] UniqueIdentifier not null,
    [PhenomenonOfferingID] UniqueIdentifier not null,
    [PhenomenonUOMID] UniqueIdentifier not null,
    [DigitalObjectIdentifierID] int null,
    [Count] int null,
    [ValueCount] int null,
    [NullCount] int null,
    [VerifiedCount] int null,
    [UnverifiedCount] int null,
    [StartDate] DateTime null,
    [EndDate] DateTime null,
    [LatitudeNorth] Float null,
    [LatitudeSouth] Float null,
    [LongitudeWest] Float null,
    [LongitudeEast] Float null,
    [ElevationMinimum] Float null,
    [ElevationMaximum] Float null,
    [HashCode] int not null,
    [NeedsUpdate] bit null,
    [AddedAt] DateTime null Constraint [DF_Dataset_AddedAt] DEFAULT (getdate()),
    [AddedBy] VarChar(36) not Null,
    [UpdatedAt] DateTime null Constraint [DF_Datasets_UpdatedAt] DEFAULT (getdate()), 
    [UpdatedBy] VarChar(36) not Null,
    [UserId] UniqueIdentifier NOT NULL,
    [RowVersion] RowVersion not null,
	Constraint PK_Datasets Primary Key Clustered (ID),
    Constraint UX_Datasets_Code UNIQUE (Code),
    Constraint UX_Datasets_Name UNIQUE (Name),
    Constraint UX_Datasets_StationID_PhenomenonOfferingID_PhenomenonUOMID Unique (StationID,PhenomenonOfferingID,PhenomenonUOMID),
    Constraint FK_Datasets_StationID Foreign Key (StationID) references Station(ID),
    Constraint FK_Datasets_PhenomenonOfferingID Foreign Key (PhenomenonOfferingID) references PhenomenonOffering(ID),
    Constraint FK_Datasets_PhenomenonUOMID Foreign Key (PhenomenonUOMID) references PhenomenonUOM(ID),
    Constraint FK_Datasets_DigitalObjectIdentifierID Foreign Key (DigitalObjectIdentifierID) References DigitalObjectIdentifiers(ID)
);
GO
CREATE INDEX [IX_Datasets_DigitalObjectIdentifierID] ON [dbo].[Datasets] ([DigitalObjectIdentifierID])
GO
CREATE INDEX [IX_Datasets_StationID] ON [dbo].[Datasets] ([StationID])
GO
CREATE INDEX [IX_Datasets_PhenomenonOfferingID] ON [dbo].[Datasets] ([PhenomenonOfferingID])
GO
CREATE INDEX [IX_Datasets_PhenomenonUOMID] ON [dbo].[Datasets] ([PhenomenonUOMID])
GO
CREATE TRIGGER [dbo].[TR_Datasets_Insert] ON [dbo].[Datasets]
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
    Datasets src
    inner join inserted ins
      on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_Datasets_Update] ON [dbo].[Datasets]
FOR UPDATE
AS
BEGIN
  SET NoCount ON
  Update
      src
  set
    AddedAt = Coalesce(del.AddedAt, ins.AddedAt, GetDate()),
    UpdatedAt = GetDate()
  from
    Datasets src
    inner join inserted ins
      on (ins.ID = src.ID)
    inner join deleted del
      on (del.ID = src.ID)
END

