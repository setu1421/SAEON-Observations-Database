--> Added 2.0.0.1 20160406 TimPN
CREATE TABLE [dbo].[OrganisationRole]
(
    [ID] UNIQUEIDENTIFIER CONSTRAINT [DF_OrganisationRole_ID] DEFAULT newid(), 
    [Code] VARCHAR(50) NOT NULL, 
    [Name] VARCHAR(150) NOT NULL, 
    [Description] VARCHAR(5000) NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_OrganisationRole] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [UX_OrganisationRole_Code] UNIQUE ([Code]),
    CONSTRAINT [UX_OrganisationRole_Name] UNIQUE ([Name]),
    CONSTRAINT [FK_OrganisationRole_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)

GO
CREATE INDEX [IX_OrganisationRole_UserId] ON [dbo].[OrganisationRole] ([UserId])
--< Added 2.0.0.1 20160406 TimPN
