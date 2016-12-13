--> Added 2.0.20 20161213 TimPN
Create function fnObservations(@UserID UniqueIdentifier)
Returns Table
As
return
  Select
	vObservation.*
  from
	vObservation
  where
	Exists(
	  Select 
	    * 
	  from 
		DataSourceRole 
	  inner join  aspnet_UsersInRoles 
		on (DataSourceRole.RoleId = aspnet_UsersInRoles.RoleId)
	  where
		(vObservation.ValueDate >= DataSourceRole.DateStart) and
		(vObservation.ValueDate <= DataSourceRole.DateEnd))
--< Added 2.0.20 20161213 TimPN

