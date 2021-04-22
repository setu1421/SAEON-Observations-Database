Select
  Parent.ID ParentId, 
  case Parent.DOIType 
    when 0 then 'ObservationsDB'
	when 1 then 'Collection'
	when 2 then 'Organisation'
	when 3 then 'Programme'
	when 4 then 'Project'
	when 5 then 'Site'
	when 6 then 'Station'
	when 7 then 'Dataset'
	when 8 then 'Periodic'
	when 9 then 'AdHoc'
	else null
  end ParentType, 
  Parent.Code ParentCode,
  Parent.Name ParentName,
  case Child.DOIType 
    when 0 then 'ObservationsDB'
	when 1 then 'Collection'
	when 2 then 'Organisation'
	when 3 then 'Programme'
	when 4 then 'Project'
	when 5 then 'Site'
	when 6 then 'Station'
	when 7 then 'Dataset'
	when 8 then 'Periodic'
	when 9 then 'AdHoc'
	else null
  end Type, 
  Child.*
from
  DigitalObjectIdentifiers Child
  left join DigitalObjectIdentifiers Parent
    on (Child.ParentID = Parent.ID)