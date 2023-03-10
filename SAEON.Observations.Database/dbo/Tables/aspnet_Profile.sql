CREATE TABLE [dbo].[aspnet_Profile] (
    [UserId]               UNIQUEIDENTIFIER NOT NULL,
    [PropertyNames]        NTEXT            NOT NULL,
    [PropertyValuesString] NTEXT            NOT NULL,
    [PropertyValuesBinary] IMAGE            NOT NULL,
    [LastUpdatedDate]      DATETIME         NOT NULL,
    CONSTRAINT [PK_aspnet_Profile] PRIMARY KEY CLUSTERED ([UserId]) ON [PRIMARY],
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
)
  ON [PRIMARY];

GO
EXECUTE sp_tableoption @TableNamePattern = N'[dbo].[aspnet_Profile]', @OptionName = N'text in row', @OptionValue = N'6000';

