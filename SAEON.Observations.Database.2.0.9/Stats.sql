Select
  count(*)
from
  ImportBatch  
  inner join DataSource
    on (ImportBatch.DataSourceID = DataSource.ID)
where
  (DataSource.Name like '%CSIR%') or
  (DataSource.Name like '%ACSYS%')
Select
  Count(*)
from
  Observation
  inner join Sensor
    on (Observation.SensorID = Sensor.ID)
  inner join Station
    on (Sensor.StationID = Station.ID)
  inner join ProjectSite
    on (Station.ProjectSiteID = ProjectSite.ID)
where
  (Observation.StatusID is null) and
  ((Station.Name like '%CSIR%') or (Station.Name like '%ACSYS%') or
   (ProjectSite.Name like '%CSIR%') or (ProjectSite.Name like '%ACSYS%'))
Select
  Count(*)
from
  Observation
  inner join ImportBatch
    on (Observation.ImportBatchID = ImportBatch.ID)
  inner join DataSource
    on (ImportBatch.DataSourceID = DataSource.ID)
where
  (Observation.StatusID is null) and
  ((DataSource.Name like '%CSIR%') or (DataSource.Name like '%ACSYS%'))
Select count(*) from Observation