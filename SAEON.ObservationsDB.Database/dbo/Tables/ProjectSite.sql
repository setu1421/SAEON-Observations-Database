CREATE TABLE [dbo].[ProjectSite] (
    [ID]             UNIQUEIDENTIFIER CONSTRAINT [DF_ProjectSite_ID] DEFAULT (newid()) NOT NULL,
    [Code]           VARCHAR (50)     NOT NULL,
    [Name]           VARCHAR (150)    NOT NULL,
    [Description]    VARCHAR (5000)   NOT NULL,
    [OrganisationID] UNIQUEIDENTIFIER NOT NULL,
    [UserId]         UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ProjectSite] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_ProjectSite_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [FK_ProjectSite_Organisation] FOREIGN KEY ([OrganisationID]) REFERENCES [dbo].[Organisation] ([ID]),
    CONSTRAINT [IX_ProjectSite_Code] UNIQUE NONCLUSTERED ([Code] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [IX_ProjectSite_Name] UNIQUE NONCLUSTERED ([Name] ASC) WITH (FILLFACTOR = 80)
);

