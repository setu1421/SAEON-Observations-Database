CREATE TABLE [dbo].[DataSourceType] (
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_DataSourceType_ID] DEFAULT (newid()) NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
--> Added 2.0.0 20160406 TimPN
    [UserId] UNIQUEIDENTIFIER NULL, 
--< Added 2.0.0 20160406 TimPN
    CONSTRAINT [PK_DataSourceType] PRIMARY KEY CLUSTERED ([ID]),
--> Added 2.0.0 20160406 TimPN
    CONSTRAINT [FK_DataSourceType_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_DataSourceType_Code] Unique ([Code])
--< Added 2.0.0 20160406 TimPN
);
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_DataSourceType_UserId] ON [dbo].[DataSourceType] ([UserId])
--< Added 2.0.0 20160406 TimPN
