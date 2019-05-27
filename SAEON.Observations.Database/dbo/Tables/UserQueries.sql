Create Table [dbo].[UserQueries]
(
    [ID] UniqueIdentifier Constraint [DF_UserQueries_ID] DEFAULT (newid()), 
    [UserId] VarChar(128) not Null,
    [Name] VarChar(150) not Null, 
    [Description] VarChar(500) Null,
    [QueryInput] VarChar(5000) not Null,
    [AddedAt] DateTime null Constraint [DF_UserQueries_AddedAt] DEFAULT (getdate()),
    [AddedBy] VarChar(128) not Null,
    [UpdatedAt] DateTime null Constraint [DF_UserQueries_UpdatedAt] DEFAULT (getdate()), 
    [UpdatedBy] VarChar(128) not Null,
    [RowVersion] RowVersion not null,
    Constraint [PK_UserQueries] Primary Key Clustered ([ID]),
    Constraint [UX_UserQueries_UserId_Name] Unique ([UserId],[Name])
)
GO
CREATE TRIGGER [dbo].[TR_UserQueries_Insert] ON [dbo].[UserQueries]
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
    UserQueries src
    inner join inserted ins
      on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_UserQueries_Update] ON [dbo].[UserQueries]
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
    UserQueries src
    inner join inserted ins
      on (ins.ID = src.ID)
    inner join deleted del
      on (del.ID = src.ID)
END
