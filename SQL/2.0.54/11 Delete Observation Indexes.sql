Use Observations
Alter database Observations set Single_User
ALTER TABLE [dbo].[Observation] DROP CONSTRAINT [UX_Observation]
Drop Index IX_Observation_Elevation on Observation
Drop Index IX_Observation_PhenomenonOfferingID on Observation
Drop Index IX_Observation_SensorID_PhenomenonOfferingID on Observation