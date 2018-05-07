insert into SchemaColumnType
  (Name, Description, UserId)
values
  ('Latitude','A latitude column',(Select UserID from aspnet_Users where (UserName = 'TimPN'))),
  ('Longitude','A longitude column',(Select UserID from aspnet_Users where (UserName = 'TimPN'))),
  ('Elevation','An elevation column, negative for below sea level',(Select UserID from aspnet_Users where (UserName = 'TimPN')))
Print 'Done'
