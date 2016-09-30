Update
  DataSource
set
  StationID = sp.StationID
from
  SensorProcedure sp
  inner join DataSource i
    on (sp.DataSourceID = i.ID)

