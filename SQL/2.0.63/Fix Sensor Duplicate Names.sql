with NameDups
as
(
Select 
  Name, Count(*) Count
from 
  Sensor
group by 
  Name
having
  (Count(*) > 1)
)

Update
  Sensor
set
  Name = Sensor.Name + '~1'
from
  NameDups
  inner join Sensor
    on (Sensor.Name = NameDups.Name) and
	   (Sensor.Code = (Select Top 1 Code from Sensor s where (s.Name = Sensor.Name)))

