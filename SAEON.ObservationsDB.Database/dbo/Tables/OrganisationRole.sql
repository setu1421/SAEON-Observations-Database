CREATE TABLE [dbo].[OrganisationRole]
(
    [ID]          UNIQUEIDENTIFIER CONSTRAINT [DF_OrganisationRole_ID] DEFAULT newid() NOT NULL,
    [Code]        VARCHAR (50)     NOT NULL,
    [Description] VARCHAR (500)    NOT NULL,
    CONSTRAINT [PK_OrganisationRole] PRIMARY KEY CLUSTERED ([ID] ASC)
);
