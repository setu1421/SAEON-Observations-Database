--> Added 20160419 TimPN
CREATE TABLE [dbo].[AuditLog]
(
    [ID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_AuditLog_ID] DEFAULT NewID(), 
    [When] DATETIME NOT NULL CONSTRAINT [DF_AuditLog_When] DEFAULT GetDate(), 
    [Where] VARCHAR(100) NOT NULL, 
    [What] VARCHAR(500) NOT NULL, 
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED ([ID]),
    CONSTRAINT [FK_AuditLog_aspnet_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId]),
)
GO
CREATE INDEX [IX_AuditLog_UserId] ON [dbo].AuditLog ([UserId])
--< Added 20160419 TimPN
