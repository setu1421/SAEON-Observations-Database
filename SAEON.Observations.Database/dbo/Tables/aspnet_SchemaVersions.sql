CREATE TABLE [dbo].[aspnet_SchemaVersions] (
    [Feature]                 NVARCHAR (128) NOT NULL,
    [CompatibleSchemaVersion] NVARCHAR (128) NOT NULL,
    [IsCurrentVersion]        BIT            NOT NULL,
--> Changed 2.0.13 20161011 TimPN
--    PRIMARY KEY CLUSTERED ([Feature] ASC, [CompatibleSchemaVersion])
    CONSTRAINT [PK_aspnet_SchemaVersions] PRIMARY KEY CLUSTERED ([Feature] ASC, [CompatibleSchemaVersion]) ON [Authentication]
--< Changed 2.0.13 20161011 TimPN
)
--> Added 2.0.13 20161010 TimPN
  ON [Authentication];
--< Added 2.0.13 20161010 TimPN
