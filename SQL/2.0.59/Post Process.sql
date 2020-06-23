Insert Into Status
  (ID, Code, Name, Description, UserId)
values
  ('6489cd1a-0dd8-4f00-88f0-a7ad1009aff2','QA-96','Duplicate in batch','Value is already in batch',(Select UserId from aspnet_Users where UserName = 'TimPN')),
  ('9aa26462-1750-4ba0-8e63-62008946dcd2','QA-95','Queued for import','Import is queued for processing processing',(Select UserId from aspnet_Users where UserName = 'TimPN')),
  ('5b6c2125-2cf5-4857-a772-f2f8515c66d9','QA-94','Importing','Import being processed',(Select UserId from aspnet_Users where UserName = 'TimPN'))