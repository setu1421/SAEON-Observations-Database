CREATE TABLE [dbo].[aspnet_Profile] (
    [UserId]               UNIQUEIDENTIFIER NOT NULL,
    [PropertyNames]        NTEXT            NOT NULL,
    [PropertyValuesString] NTEXT            NOT NULL,
    [PropertyValuesBinary] IMAGE            NOT NULL,
    [LastUpdatedDate]      DATETIME         NOT NULL,
--> Changed 2.0.13 20161011 TimPN
--    PRIMARY KEY CLUSTERED ([UserId]),
    CONSTRAINT [PK_aspnet_Profile] PRIMARY KEY CLUSTERED ([UserId]) ON [Authentication],
--< Changed 2.0.13 20161011 TimPN
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
)
--> Added 2.0.13 20161010 TimPN
  ON [Authentication];
--< Added 2.0.13 20161010 TimPN

GO
EXECUTE sp_tableoption @TableNamePattern = N'[dbo].[aspnet_Profile]', @OptionName = N'text in row', @OptionValue = N'6000';

