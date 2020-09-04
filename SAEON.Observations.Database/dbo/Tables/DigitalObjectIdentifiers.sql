CREATE TABLE [dbo].[DigitalObjectIdentifiers]
(
    [ID] Int Identity(1,1) not null,
    [ParentID] int null,
    [DOIType] tinyint not null,
    --[DOI] VarChar(100) not null,
	--[DOIUrl]  VarChar(100) not null,
	[DOI] as '10.15493/obsdb.'+Convert(VarChar(20),Convert(VarBinary(1),DOIType),2)+'.'+Stuff(Convert(VarChar(20),Convert(VarBinary(4),ID),2),5,0,'.'),
	[DOIUrl] as 'https://doi.org/10.15493/obsdb.'+Convert(VarChar(20),Convert(VarBinary(1),DOIType),2)+'.'+Stuff(Convert(VarChar(20),Convert(VarBinary(4),ID),2),5,0,'.'),
	[Name] VARCHAR(1000) null,
    [MetadataJson] VARCHAR(5000) not null,
    [MetadataJsonSha256] Binary(32) not null,
    --[MetadataJsonSha256] as HASHBYTES('SHA2_256', MetadataJson),
    [MetadataUrl] VARCHAR(250) not null,
	[OpenDataPlatformID] UniqueIdentifier not null,
    [AddedAt] DateTime null Constraint [DF_DigitalObjectIdentifiers_AddedAt] DEFAULT (getdate()),
    [AddedBy] VarChar(128) not Null,
    [UpdatedAt] DateTime null Constraint [DF_DigitalObjectIdentifiers_UpdatedAt] DEFAULT (getdate()), 
    [UpdatedBy] VarChar(128) not Null,
    [RowVersion] RowVersion not null,
	Constraint PK_DigitalObjectIdentifiers Primary Key Clustered (ID),
    Constraint UX_DigitalObjectIdentifiers_DOIType_Name UNIQUE (DOIType, Name),
    Constraint FK_DigitalObjectIdentifiers_ParentID Foreign Key (ParentID) References DigitalObjectIdentifiers(ID)
);
GO
CREATE INDEX [IX_DigitalObjectIdentifiers_ParentID] ON [dbo].[DigitalObjectIdentifiers] ([ParentID])
GO
CREATE INDEX [IX_DigitalObjectIdentifiers_DOIType] ON [dbo].[DigitalObjectIdentifiers] ([DOIType])
GO
CREATE INDEX [IX_DigitalObjectIdentifiers_Name] ON [dbo].[DigitalObjectIdentifiers] ([Name])
GO
CREATE TRIGGER [dbo].[TR_DigitalObjectIdentifiers_Insert] ON [dbo].[DigitalObjectIdentifiers]
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
    DigitalObjectIdentifiers src
    inner join inserted ins
      on (ins.ID = src.ID)
END
GO
CREATE TRIGGER [dbo].[TR_DigitalObjectIdentifiers_Update] ON [dbo].[DigitalObjectIdentifiers]
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
    DigitalObjectIdentifiers src
    inner join inserted ins
      on (ins.ID = src.ID)
    inner join deleted del
      on (del.ID = src.ID)
END

