Delete from [Sensor] where Code like '_Test_%'
Delete [Organisation_Instrument] from [Organisation_Instrument] inner join [Instrument] on ([Organisation_Instrument].InstrumentID = [Instrument].ID) where [Instrument].Code like '_Test_%'
Delete [Station_Instrument] from [Station_Instrument] inner join [Instrument] on ([Station_Instrument].InstrumentID = [Instrument].ID) where [Instrument].Code like '_Test_%'
Delete from [Instrument] where Code like '_Test_%'
Delete [Organisation_Station] from [Organisation_Station] inner join [Station] on ([Organisation_Station].StationID = [Station].ID) where [Station].Code like '_Test_%'
Delete [Project_Station] from [Project_Station] inner join [Station] on ([Project_Station].StationID = [Station].ID) where [Station].Code like '_Test_%'
Delete [Organisation_Site] from [Organisation_Site] inner join [Site] on ([Organisation_Site].SiteID = [Site].ID) where [Site].Code like '_Test_%'
Update [Station] set SiteID = null from [Station] inner join [Site] on ([Station].SiteID = [Site].ID) where [Site].Code like '_Test_%' 
Delete from [Station] where Code like '_Test_%'
Delete from [Site] where Code like '_Test_%'
Delete from [Project] where Code like '_Test_%'
Delete from [Programme] where Code like '_Test_%'
Delete from [Organisation] where Code like '_Test_%'
