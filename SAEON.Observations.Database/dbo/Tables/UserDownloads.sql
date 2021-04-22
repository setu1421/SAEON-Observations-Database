Create Table [dbo].[UserDownloads]
(
    [ID] UniqueIdentifier Constraint [DF_UserDownloads_ID] DEFAULT (newid()), 
    [Name] VarChar(150) not Null, 
    [Description] VarChar(5000) not Null,
    [IsDeleted] Bit Null,
    --[Title] VarChar(5000) not Null,
    --[Keywords] VarChar(1000) not Null,
	[Date] DateTime not null,
    [Input] VarChar(5000) not Null,
    [RequeryURL] VarChar(5000) not Null,
	[DigitalObjectIdentifierID] Int not null,
 --   [MetadataJson] VarChar(Max) not Null,
 --   [MetadataURL] VarChar(2000) not Null,
	--[OpenDataPlatformID] UniqueIdentifier not null,
    [DownloadURL] VarChar(2000) not Null,
	[ZipFullName] VarChar(2000) not Null,
	[ZipCheckSum] VarChar(64) not null,
    [ZipURL] VarChar(2000) not Null,
    --[Citation] VarChar(5000) not null,
	--[Places] VarChar(5000) null,
 --   [LatitudeNorth] FLOAT NULL, 
 --   [LatitudeSouth] FLOAT NULL, 
 --   [LongitudeWest] FLOAT NULL, 
 --   [LongitudeEast] FLOAT NULL, 
 --   [ElevationMinimum] FLOAT NULL, 
 --   [ElevationMaximum] FLOAT NULL, 
 --   [StartDate] DATETIME NULL, 
 --   [EndDate] DATETIME NULL, 
    [UserId] VarChar(128) not Null,
    [AddedAt] DateTime null Constraint [DF_UserDownloads_AddedAt] DEFAULT (getdate()),
    [AddedBy] VarChar(128) not Null,
    [UpdatedAt] DateTime null Constraint [DF_UserDownloads_UpdatedAt] DEFAULT (getdate()), 
    [UpdatedBy] VarChar(128) not Null,
    [RowVersion] RowVersion not null,
    Constraint [PK_UserDownloads] Primary Key Clustered ([ID]),
    Constraint [UX_UserDownloads_UserId_Name] Unique ([UserId],[Name]),
	Constraint [FK_UserDownloads_DigitalObjectIdentifiers] FOREIGN KEY ([DigitalObjectIdentifierID]) REFERENCES [dbo].[DigitalObjectIdentifiers] ([ID])
)
GO
CREATE INDEX [IX_UserDownloads_DOI] ON [dbo].[UserDownloads]([DigitalObjectIdentifierID])
GO
CREATE TRIGGER [dbo].[TR_UserDownloads_Insert] ON [dbo].[UserDownloads]
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
    UserDownloads src
    inner join inserted ins
      on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_UserDownloads_Update] ON [dbo].[UserDownloads]
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
    UserDownloads src
    inner join inserted ins
      on (ins.ID = src.ID)
    inner join deleted del
      on (del.ID = src.ID)
END
