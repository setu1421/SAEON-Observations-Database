Drop Index if exists [IX_Observation_StatusID] on Observation;
Drop Index if exists [IX_Observation_ImportBatchID] on Observation;
Drop Index if exists [IX_Observation_SensorID] on Observation;
Drop Index if exists [IX_Observation_SensorID_PhenomenonOfferingID] on Observation;
Drop Index if exists [IX_Observation_SensorID_PhenomenonUOMID] on Observation;
Drop Index if exists [IX_Observation_SensorID_PhenomenonOfferingID_PhenomenonUOMID_ImportBatchID] on Observation;
Drop Index if exists [IX_Observation_SensorID_ValueDate_Latitude] on Observation;
Drop Index if exists [IX_Observation_SensorID_ValueDate_Longitude] on Observation;
Drop Index if exists [IX_Observation_SensorID_ValueDate_Elevation] on Observation;
CheckPoint;

--CREATE INDEX [IX_Observation_ImportBatchID] ON [dbo].[Observation]([ImportBatchID])
--  INCLUDE ([DataValue],[PhenomenonOfferingID],[PhenomenonUOMID],[SensorID],[StatusID],[StatusReasonID],[Elevation],[Latitude],[Longitude],[ValueDate],[ValueDay])
--  --WITH(DROP_EXISTING=ON,ONLINE=ON) 
--  ON [Observations];
--CREATE INDEX [IX_Observation_SensorID] ON [dbo].[Observation] ([SensorID])
--  --INCLUDE ([DataValue],[PhenomenonOfferingID],[PhenomenonUOMID],[ImportBatchID],[StatusID],[StatusReasonID],[Elevation],[Latitude],[Longitude],[ValueDate],[ValueDay])
--  --WITH(DROP_EXISTING=ON,ONLINE=ON) 
--  ON [Observations];
--CheckPoint;


