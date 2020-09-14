Select
  Parent.ID ParentId, 
  case Parent.DOIType 
    when 0 then 'ObservationsDB'
	when 1 then 'Organisation'
	when 2 then 'Programme'
	when 3 then 'Project'
	when 4 then 'Site'
	when 5 then 'Station'
	when 6 then 'Dataset'
	when 7 then 'Periodic'
	when 8 then 'AdHoc'
	else null
  end ParentType, 
  Parent.Code ParentCode,
  Parent.Name ParentName,
  case Child.DOIType 
    when 0 then 'ObservationsDB'
	when 1 then 'Organisation'
	when 2 then 'Programme'
	when 3 then 'Project'
	when 4 then 'Site'
	when 5 then 'Station'
	when 6 then 'Dataset'
	when 7 then 'Periodic'
	when 8 then 'AdHoc'
	else null
  end ChildType, 
  Child.*
from
  DigitalObjectIdentifiers Child
  left join DigitalObjectIdentifiers Parent
    on (Child.ParentID = Parent.ID)