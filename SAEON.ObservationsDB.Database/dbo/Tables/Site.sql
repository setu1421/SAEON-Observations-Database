--> Added 20160329 TimPN
CREATE TABLE [dbo].[Site]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_SiteRole_ID] DEFAULT newid(), 
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_Site_Name] UNIQUE ([Name]),
    CONSTRAINT [UX_Site_Code] UNIQUE ([Code]),
    CONSTRAINT [FK_Site_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)

GO
CREATE INDEX [IX_Site_UserId] ON [dbo].[Site] ([UserId])
--< Added 20160329 TimPN
