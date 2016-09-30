Select
--  sp.Name Sensor, ds.Name DataSource, dsh.Name DataSchema,
  sp.Code SensorCode, sp.Name SensorName, sp.Description SensorDescription, 
  ds.Code DataSourceCode, ds.Name DataSourceName, ds.Description DataSourceDescription, 
  dsh.Code SchemaCode, dsh.Name SchemaName, dsh.Description SchemaDescription
from
  SensorProcedure sp
  inner join DataSource ds
    on (sp.DataSourceID = ds.ID)
  inner join DataSchema dsh
    on (ds.DataSchemaID = dsh.ID)
order by
  sp.Name, ds.Name, dsh.Name
