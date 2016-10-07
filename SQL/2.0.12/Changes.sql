PRINT 'Dropping IX_Observation_Comment'
GO
DROP INDEX [IX_Observation_Comment] ON [dbo].[Observation]
PRINT 'Dropping IX_Observation_Comment_Null'
GO
DROP INDEX [IX_Observation_Comment_Null] ON [dbo].[Observation]
PRINT 'Creating IX_Observation_Comment'
GO
CREATE INDEX [IX_Observation_Comment] ON [dbo].[Observation] ([Comment])
PRINT 'Done'
GO
