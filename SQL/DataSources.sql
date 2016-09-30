Select
--  sp.Name Sensor, ds.Name DataSource, dsh.Name DataSchema,
  ds.Code DataSourceCode, ds.Name DataSourceName, ds.Description DataSourceDescription, 
  dsh.Code SchemaCode, dsh.Name SchemaName, dsh.Description SchemaDescription
from
  DataSource ds
  inner join DataSchema dsh
    on (ds.DataSchemaID = dsh.ID)
order by
  ds.Name, dsh.Name
