CREATE TABLE [dbo].[RequestLogs]
(
    [ID] INT NOT NULL Identity,
    [Time] DateTime NOT null,
    [Method] VarChar(50) not null,
    [Path] VarChar(1024) not null,
    [QueryString] VarChar(1024) null,
    [Headers] VarChar(Max) null,
    [Body] VarChar(Max) null,
    [IPAddress] VarChar(45) null,
    [Description] VarChar(8000) null,
    --[BytesSent] BigInt not null,
    --[UserId] UNIQUEIDENTIFIER null,
    [RowVersion] RowVersion not null,
    CONSTRAINT [PK_RequestLogs] PRIMARY KEY CLUSTERED ([ID])
)
GO
CREATE INDEX [IX_RequestLogs_Time] ON [dbo].RequestLogs ([Time])
GO
CREATE INDEX [IX_RequestLogs_Method] ON [dbo].RequestLogs ([Method])
GO
CREATE INDEX [IX_RequestLogs_Path] ON [dbo].RequestLogs ([Path])
GO
--CREATE INDEX [IX_RequestLogs_UserId] ON [dbo].RequestLogs ([UserId])
--GO
