Create Table [dbo].[UserDownloads]
(
    [ID] UniqueIdentifier Constraint [DF_UserDownloads_ID] DEFAULT (newid()), 
    [UserId] NVarChar(128) not Null,
    [Name] VarChar(150) not Null, 
    [Description] VarChar(5000) Null,
    [QueryInput] VarChar(5000) not Null,
    [DownloadURI] VarChar(500) not Null,
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
