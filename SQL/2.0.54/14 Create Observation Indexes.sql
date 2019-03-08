Use Observations
ALTER TABLE [dbo].[Observation] ADD  CONSTRAINT [UX_Observation] UNIQUE 
([SensorID],[ValueDate],[DataValue],[PhenomenonOfferingID],[PhenomenonUOMID],[Elevation])
ON [Observations];
CREATE INDEX [IX_Observation_Elevation] ON [dbo].[Observation] ([Elevation]) ON [Observations];
CREATE INDEX [IX_Observation_PhenomenonOfferingID] ON [dbo].[Observation] ([PhenomenonOfferingID]) ON [Observations];
CREATE INDEX [IX_Observation_SensorID_PhenomenonOfferingID] ON [dbo].[Observation] ([SensorID],[PhenomenonOfferingID]) ON [Observations];
CheckPoint;
