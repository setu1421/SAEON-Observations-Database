Select
  d.Code, s.Code 
from
  DataSource d
  inner join DataSchema s
    on (d.DataSchemaID = s.ID)
order by
  d.Code, s.code
