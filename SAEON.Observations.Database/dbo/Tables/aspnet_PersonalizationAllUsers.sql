CREATE TABLE [dbo].[aspnet_PersonalizationAllUsers] (
    [PathId]          UNIQUEIDENTIFIER NOT NULL,
    [PageSettings]    IMAGE            NOT NULL,
    [LastUpdatedDate] DATETIME         NOT NULL,
--> Changed 2.0.13 20161011 TimPN
--    PRIMARY KEY CLUSTERED ([PathId]),
    CONSTRAINT [PK_aspnet_PersonalizationAllUsers] PRIMARY KEY CLUSTERED ([PathId]) ON [Authentication],
--< Changed 2.0.13 20161011 TimPN
    FOREIGN KEY ([PathId]) REFERENCES [dbo].[aspnet_Paths] ([PathId])
)
--> Added 2.0.13 20161010 TimPN
  ON [Authentication];
--< Added 2.0.13 20161010 TimPN

GO
EXECUTE sp_tableoption @TableNamePattern = N'[dbo].[aspnet_PersonalizationAllUsers]', @OptionName = N'text in row', @OptionValue = N'6000';

