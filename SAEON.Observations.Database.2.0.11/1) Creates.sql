Insert into SchemaColumnType
  (Name, Description, UserId)
values
  ('Date','A date column',(Select UserId from aspnet_Users where LoweredUserName = 'tim')),
  ('Time','A time column',(Select UserId from aspnet_Users where LoweredUserName = 'tim')),
  ('Ignore','A column that is ignored',(Select UserId from aspnet_Users where LoweredUserName = 'tim')),
  ('Phenomenon','An phenomenon column',(Select UserId from aspnet_Users where LoweredUserName = 'tim')),
  ('Fixed Time','An offering column with a fixed time',(Select UserId from aspnet_Users where LoweredUserName = 'tim')),
  ('Comment','A text column',(Select UserId from aspnet_Users where LoweredUserName = 'tim'))
  
