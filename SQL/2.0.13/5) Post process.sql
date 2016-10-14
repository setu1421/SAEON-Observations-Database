USE [Observations]
--USE [ObservationsTest]
PRINT 'Create aspnet_Applications_Index'
GO
CREATE CLUSTERED INDEX [aspnet_Applications_Index]
    ON [dbo].[aspnet_Applications]([LoweredApplicationName])
  WITH(DROP_EXISTING=ON) ON [Authentication];
PRINT 'Create aspnet_Membership_index'
GO
CREATE CLUSTERED INDEX [aspnet_Membership_index]
    ON [dbo].[aspnet_Membership]([ApplicationId] ASC, [LoweredEmail])
  WITH(DROP_EXISTING=ON) ON [Authentication];
PRINT 'Create aspnet_Paths_index'
GO
CREATE UNIQUE CLUSTERED INDEX [aspnet_Paths_index]
    ON [dbo].[aspnet_Paths]([ApplicationId] ASC, [LoweredPath])
  WITH(DROP_EXISTING=ON) ON [Authentication];
PRINT 'Create aspnet_PersonalizationPerUser_index1'
GO
CREATE UNIQUE CLUSTERED INDEX [aspnet_PersonalizationPerUser_index1]
    ON [dbo].[aspnet_PersonalizationPerUser]([PathId] ASC, [UserId])
  WITH(DROP_EXISTING=ON) ON [Authentication];
PRINT 'Create aspnet_PersonalizationPerUser_ncindex2'
GO
CREATE UNIQUE INDEX [aspnet_PersonalizationPerUser_ncindex2]
    ON [dbo].[aspnet_PersonalizationPerUser]([UserId] ASC, [PathId])
  WITH(DROP_EXISTING=ON) ON [Authentication];
PRINT 'Create aspnet_Roles_index1'
GO
CREATE UNIQUE CLUSTERED INDEX [aspnet_Roles_index1]
    ON [dbo].[aspnet_Roles]([ApplicationId] ASC, [LoweredRoleName])
  WITH(DROP_EXISTING=ON) ON [Authentication];
PRINT 'Create aspnet_Users_Index'
GO
CREATE UNIQUE CLUSTERED INDEX [aspnet_Users_Index]
    ON [dbo].[aspnet_Users]([ApplicationId] ASC, [LoweredUserName])
  WITH(DROP_EXISTING=ON) ON [Authentication];
PRINT 'Create aspnet_Users_Index2'
GO
CREATE INDEX [aspnet_Users_Index2]
    ON [dbo].[aspnet_Users]([ApplicationId] ASC, [LastActivityDate])
  WITH(DROP_EXISTING=ON) ON [Authentication];
PRINT 'Create aspnet_UsersInRoles_index'
GO
CREATE INDEX [aspnet_UsersInRoles_index]
    ON [dbo].[aspnet_UsersInRoles]([RoleId])
  WITH(DROP_EXISTING=ON) ON [Authentication];
PRINT 'Create CX_Observation'
GO
CREATE CLUSTERED INDEX [CX_Observation] ON [dbo].[Observation] ([AddedAt])
  WITH(DROP_EXISTING=ON) ON [Observations];
PRINT 'Drop PK_Observation'
GO
ALTER TABLE [dbo].[Observation] DROP CONSTRAINT [PK_Observation]
PRINT 'Create PK_Observation'
GO
ALTER TABLE [dbo].[Observation] ADD CONSTRAINT [PK_Observation] PRIMARY KEY NONCLUSTERED ([ID]) on [Observations]
PRINT 'Drop UX_Observation'
GO
ALTER TABLE [dbo].[Observation] DROP CONSTRAINT [UX_Observation]
PRINT 'Create UX_Observation'
GO
ALTER TABLE [dbo].[Observation] ADD CONSTRAINT [UX_Observation] UNIQUE ([SensorID], [ImportBatchID], [ValueDate], [PhenomenonOfferingID], [PhenomenonUOMID]) ON [Observations]
PRINT 'Create IX_Observation'
GO
CREATE INDEX [IX_Observation] ON [dbo].[Observation]([SensorID] ASC, [ValueDate] ASC, [RawValue])
  WITH(DROP_EXISTING=ON) ON [Observations];
PRINT 'Create IX_Observation_ImportBatchID'
GO
CREATE INDEX [IX_Observation_ImportBatchID] ON [dbo].[Observation]([ImportBatchID])
  WITH(DROP_EXISTING=ON) ON [Observations];
PRINT 'Create IX_Observation_SensorID'
GO
CREATE INDEX [IX_Observation_SensorID] ON [dbo].[Observation] ([SensorID])
  WITH(DROP_EXISTING=ON) ON [Observations];
PRINT 'Create IX_Observation_PhenomenonOfferingID'
GO
CREATE INDEX [IX_Observation_PhenomenonOfferingID] ON [dbo].[Observation] ([PhenomenonOfferingID])
  WITH(DROP_EXISTING=ON) ON [Observations];
PRINT 'Create IX_Observation_PhenomenonUOMID'
GO
CREATE INDEX [IX_Observation_PhenomenonUOMID] ON [dbo].[Observation] ([PhenomenonUOMID])
  WITH(DROP_EXISTING=ON) ON [Observations];
PRINT 'Create IX_Observation_UserId'
GO
CREATE INDEX [IX_Observation_UserId] ON [dbo].[Observation] ([UserId])
  WITH(DROP_EXISTING=ON) ON [Observations];
PRINT 'Create IX_Observation_AddedDate'
GO
CREATE INDEX [IX_Observation_AddedDate] ON [dbo].[Observation] ([SensorID], [AddedDate])
  WITH(DROP_EXISTING=ON) ON [Observations];
PRINT 'Create IX_Observation_ValueDate'
GO
CREATE INDEX [IX_Observation_ValueDate] ON [dbo].[Observation] ([SensorID], [ValueDate])
  WITH(DROP_EXISTING=ON) ON [Observations];
PRINT 'Create IX_Observation_StatusID'
GO
CREATE INDEX [IX_Observation_StatusID] ON [dbo].[Observation] ([StatusID])
  WITH(DROP_EXISTING=ON) ON [Observations];
PRINT 'Create IX_Observation_StatusReasonID'
GO
CREATE INDEX [IX_Observation_StatusReasonID] ON [dbo].[Observation] ([StatusReasonID])
  WITH(DROP_EXISTING=ON) ON [Observations];
PRINT 'Done'
GO







