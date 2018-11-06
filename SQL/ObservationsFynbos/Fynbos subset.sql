declare @List Table (Code VarChar(100))
Insert into 
  @List (Code)
Values
  ('CONSTB_CWS_W'),('ENG CED AWS'),('HSB_BAB_SM'),('HSB_BAT_SM'),('HSB_BDB_SM'),('HSB_BDT_SM'),('JNHK_DWS_W'),
  ('JNHK_LANGFOG_500'),('JNHK_LANGFOG_600'),('JNHK_LANGFOG_700'),('JNHK_LANGFOG_800')

Select 
  --Sum(Count)
  *
From
  vImportBatchSummary
where
  StationCode in (Select Code from @List)