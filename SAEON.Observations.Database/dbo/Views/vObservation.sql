CREATE VIEW [dbo].[vObservation]
AS
SELECT 
  o.*,
  IsNull(SensorSchema.Name, DataSourceSchema.Name) DataSchemaName
FROM
  vObservationExpansion o
  inner join Sensor 
    on (o.SensorID = Sensor.ID)
  left join DataSchema SensorSchema
    on (Sensor.DataSchemaID = SensorSchema.ID)
  inner join DataSource
    on (Sensor.DataSourceID = DataSource.ID)
  left join DataSchema DataSourceSchema
    on (DataSource.DataSchemaID = DataSourceSchema.ID)
GO
