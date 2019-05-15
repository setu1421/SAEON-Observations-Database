Create Table [dbo].[UserDownloads]
(
    [ID] UniqueIdentifier Constraint [DF_UserDownloads_ID] DEFAULT (newid()), 
    [UserId] NVarChar(128) not Null,
    [Name] VarChar(150) not Null, 
    [Description] VarChar(5000) not Null,
    [Title] VarChar(5000) not Null,
    [Input] VarChar(5000) not Null,
    [RequeryURL] VarChar(5000) not Null,
	[DOIId] int not null Identity,
    [DOI] as '10.15493/obsdb.'+Stuff(Convert(VarChar(20),Convert(VarBinary(4),DOIId),2),5,0,'.'),
	[DOIUrl] as 'https://doi.org/10.15493/obsdb.'+Stuff(Convert(VarChar(20),Convert(VarBinary(4),DOIId),2),5,0,'.'), 
    [MetadataURL] VarChar(2000) not Null,
    [DownloadURL] VarChar(2000) not Null,
	[ZipFullName] VarChar(2000) not Null,
	[ZipCheckSum] VarChar(64) not null,
    [Citation] VarChar(5000) not null,
	[Places] VarChar(5000) null,
    [LatitudeNorth] FLOAT NULL, 
    [LatitudeSouth] FLOAT NULL, 
    [LongitudeWest] FLOAT NULL, 
    [LongitudeEast] FLOAT NULL, 
    [ElevationMinimum] FLOAT NULL, 
    [ElevationMaximum] FLOAT NULL, 
    [StartDate] DATETIME NULL, 
    [EndDate] DATETIME NULL, 
    [AddedAt] DateTime null Constraint [DF_UserDownloads_AddedAt] DEFAULT (getdate()),
    [AddedBy] NVarChar(128) not Null,
    [UpdatedAt] DateTime null Constraint [DF_UserDownloads_UpdatedAt] DEFAULT (getdate()), 
    [UpdatedBy] NVarChar(128) not Null,
    [RowVersion] RowVersion not null,
    Constraint [PK_UserDownloads] Primary Key Clustered ([ID]),
    Constraint [UX_UserDownloads_UserId_Name] Unique ([UserId],[Name])
)
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
