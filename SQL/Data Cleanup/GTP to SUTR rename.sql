Update
  Project
set
  Code = 'ELW_NCTN_SUTR',
  Name = 'ELW, National Coastal Temperature Network, Shallow Underwater Temperature Recorders',
  Description = 'ELW, National Coastal Temperature Network, Shallow Underwater Temperature Recorders, Network of shallow coastal in situ moorings measuring water temperature'
where
  Code = 'ELW_NCTN_GTP'
Update
  Station
set
  Code = Replace(Replace(Code,'_GTP_','_SUTR_'),'_GUL','_SUTR'),
  Name = Replace(Name,', Gully Temperature Probes,',', Shallow Underwater Temperature Recorders,'),
  Description = Replace(Description, ', Gully Temperature Probes,',', Shallow Underwater Temperature Recorders,')
where
  Code like 'ELW_NCTN_GTP_%'
Update
  Datasets
set
  Code = Replace(Replace(Code,'_GTP_','_SUTR_'),'_GUL','_SUTR'),
  Name = Replace(Name,', Gully Temperature Probes,',', Shallow Underwater Temperature Recorders,'),
  Description = Replace(Description, ', Gully Temperature Probes,',', Shallow Underwater Temperature Recorders,')
where
  Code like 'ELW_NCTN_GTP_%'
Update
  DigitalObjectIdentifiers
set
  Code = Replace(Replace(Code,'_GTP_','_SUTR_'),'_GUL','_SUTR'),
  Name = Replace(Name,', Gully Temperature Probes,',', Shallow Underwater Temperature Recorders,')
where
  Code like 'ELW_NCTN_GTP_%'