Update 
  Offering
set
  Code = Replace(Code,'_1_Hours','_1_Hour'),
  Name = Replace(Name,' 1 Hours',' 1 Hour')
where
  Code like '_1_Hours'