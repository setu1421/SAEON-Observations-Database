Insert Into Status
  (ID, Code, Name, Description, UserId)
values
  ('6489cd1a-0dd8-4f00-88f0-a7ad1009aff2','QA-96','Duplicate in batch','Value is already in batch',(Select UserId from aspnet_Users where UserName = 'TimPN'))