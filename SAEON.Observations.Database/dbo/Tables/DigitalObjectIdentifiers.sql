CREATE TABLE [dbo].[DigitalObjectIdentifiers]
(
    [ID] Int Identity(1,1) not null,
    [AlternateID] UniqueIdentifier Constraint DF_DigitalObjectIdentifiers_AlternateID default NewId(),
    [ParentID] int null,
    [DOIType] tinyint not null,
	[DOI] as '10.15493/obsdb.'+Stuff(Convert(VarChar(20),Convert(VarBinary(4),ID),2),5,0,'.'),
	[DOIUrl] as 'https://doi.org/10.15493/obsdb.'+Stuff(Convert(VarChar(20),Convert(VarBinary(4),ID),2),5,0,'.'),
	[Code] VARCHAR(200) not null,
	[Name] VARCHAR(500) not null,
    [Title] VARCHAR(5000) not null,
    [Description] VARCHAR(5000) not null,
    [DescriptionHtml] VARCHAR(5000) not null,
    [Citation] VarChar(5000) null,
    [CitationHtml] VarChar(5000) null,
    [MetadataJson] VARCHAR(MAX) not null,
    [MetadataJsonSha256] Binary(32) not null,
    [MetadataHtml] VARCHAR(MAX) not null,
    [MetadataUrl] VARCHAR(250) not null,
    [ObjectStoreUrl] VARCHAR(250) null,
    [QueryUrl] VARCHAR(250) null,
	[ODPMetadataID] UniqueIdentifier null,
    [ODPMetadataNeedsUpdate] Bit null,
    [ODPMetadataIsValid] Bit null,
    [ODPMetadataErrors] VarChar(Max) null,
    [ODPMetadataIsPublished] Bit null,
    [ODPMetadataPublishErrors] VarChar(Max) null,
    [AddedAt] DateTime null Constraint [DF_DigitalObjectIdentifiers_AddedAt] DEFAULT (getdate()),
    [AddedBy] VarChar(128) not Null,
    [UpdatedAt] DateTime null Constraint [DF_DigitalObjectIdentifiers_UpdatedAt] DEFAULT (getdate()), 
    [UpdatedBy] VarChar(128) not Null,
    [RowVersion] RowVersion not null,
	Constraint PK_DigitalObjectIdentifiers Primary Key Clustered (ID),
    Constraint UX_DigitalObjectIdentifiers_DOIType_Code UNIQUE (DOIType, Code),
    Constraint UX_DigitalObjectIdentifiers_DOIType_Name UNIQUE (DOIType, Name),
    Constraint FK_DigitalObjectIdentifiers_ParentID Foreign Key (ParentID) References DigitalObjectIdentifiers(ID)
);
GO
CREATE INDEX [IX_DigitalObjectIdentifiers_ParentID] ON [dbo].[DigitalObjectIdentifiers] ([ParentID])
GO
CREATE INDEX [IX_DigitalObjectIdentifiers_DOIType] ON [dbo].[DigitalObjectIdentifiers] ([DOIType])
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

