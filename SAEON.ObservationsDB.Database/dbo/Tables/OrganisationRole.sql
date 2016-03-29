--> Added 20160329 TimPN
CREATE TABLE [dbo].[OrganisationRole]
(
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_OrganisationRole_ID] DEFAULT newid() NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Description] VARCHAR (500)    NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [PK_OrganisationRole] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_OrganisationRole_aspnetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
    CONSTRAINT [UX_OrganisationRole_Code] UNIQUE ([Code])
);
GO
CREATE INDEX [IX_OrganisationRole_UserId] ON [dbo].[OrganisationRole] ([UserId])
--< Added 20160329 TimPN
