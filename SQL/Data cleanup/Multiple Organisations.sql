Select InstrumentName, Count(OrganisationName) from vInstrumentOrganisation group by InstrumentName  having Count(OrganisationName) > 1 order by InstrumentName
Select StationName, Count(OrganisationName) from vStationOrganisation group by StationName having Count(OrganisationName) > 1 order by StationName
Select SiteName, Count(OrganisationName) from vSiteOrganisation group by SiteName having Count(OrganisationName) > 1 order by SiteName
