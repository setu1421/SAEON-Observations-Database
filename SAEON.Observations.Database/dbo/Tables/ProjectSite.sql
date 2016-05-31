CREATE TABLE [dbo].[ProjectSite] (
    [ID]             UNIQUEIDENTIFIER CONSTRAINT [DF_ProjectSite_ID] DEFAULT (newid()) NOT NULL,
    [Code]           VARCHAR (50)     NOT NULL,
    [Name]           VARCHAR (150)    NOT NULL,
    [Description]    VARCHAR (5000)   NOT NULL,
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL,
    [UserId]         UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ProjectSite] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_ProjectSite_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_ProjectSite_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_ProjectSite_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_ProjectSite_Code] UNIQUE ([Code]),
--< Changed 20160329 TimPN
--> Changed 20160329 TimPN
--    CONSTRAINT [IX_ProjectSite_Name] UNIQUE ([Name])
    CONSTRAINT [UX_ProjectSite_Name] UNIQUE ([Name])
--< Changed 20160329 TimPN
);
--> Added 2.0.0 20160406 TimPN
GO
CREATE INDEX [IX_ProjectSite_OrganisationID] ON [dbo].[ProjectSite] ([OrganisationID])
GO
CREATE INDEX [IX_ProjectSite_UserID] ON [dbo].[ProjectSite] ([UserId])
--< Added 2.0.0 20160406 TimPN

