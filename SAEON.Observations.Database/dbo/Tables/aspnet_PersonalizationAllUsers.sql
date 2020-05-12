CREATE TABLE [dbo].[aspnet_PersonalizationAllUsers] (
    [PathId]          UNIQUEIDENTIFIER NOT NULL,
    [PageSettings]    IMAGE            NOT NULL,
    [LastUpdatedDate] DATETIME         NOT NULL,
    CONSTRAINT [PK_aspnet_PersonalizationAllUsers] PRIMARY KEY CLUSTERED ([PathId]) ON [Primary],
    FOREIGN KEY ([PathId]) REFERENCES [dbo].[aspnet_Paths] ([PathId])
)
  ON [Primary];

GO
EXECUTE sp_tableoption @TableNamePattern = N'[dbo].[aspnet_PersonalizationAllUsers]', @OptionName = N'text in row', @OptionValue = N'6000';

