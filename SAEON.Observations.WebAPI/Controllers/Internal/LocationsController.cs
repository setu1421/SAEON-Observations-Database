using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [RoutePrefix("Internal/Locations")]
    public class LocationsController : BaseListController<LocationNode>
    {
        protected override List<LocationNode> GetList()
        {
            var result = base.GetList();
            LocationNode organisation = null;
            LocationNode site = null;
            LocationNode station = null;
            foreach (var location in db.Locations
                .Include(i => i.Organisation).Include(i => i.Site).Include(i => i.Station)
                .OrderBy(i => i.Organisation.Name).ThenBy(i => i.Site.Name).ThenBy(i => i.Station.Name))
            {
                if (organisation?.Id != location.OrganisationID)
                {
                    organisation = new LocationNode
                    {
                        Id = location.OrganisationID,
                        Key = $"ORG~{location.OrganisationID}~",
                        Text = location.Organisation.Name,
                        HasChildren = true,
                        Name = $"Organisation - {location.Organisation.Name}"
                    };
                    result.Add(organisation);
                }
                if (site?.Id != location.SiteID)
                {
                    site = new LocationNode
                    {
                        Id = location.SiteID,
                        ParentId = organisation.Id,
                        Key = $"SIT~{location.SiteID}~{organisation.Key}",
                        ParentKey = organisation.Key,
                        Text = location.Site.Name,
                        HasChildren = true,
                        Name = $"Site - {location.Site.Name}"
                    };
                    result.Add(site);
                }
                if (station?.Id != location.StationID)
                {
                    station = new LocationNode
                    {
                        Id = location.StationID,
                        ParentId = site.Id,
                        Key = $"STA~{location.StationID}~{site.Key}",
                        ParentKey = site.Key,
                        Text = location.Station.Name,
                        Name = $"{location.Site.Name} - {location.Station.Name}",
                        HasChildren = false,
                        Latitude = location.Station.Latitude,
                        Longitude = location.Station.Longitude,
                        Elevation = location.Station.Elevation,
                        Url = location.Station.Url,
                    };
                    result.Add(station);
                }
            }
            return result;
        }
    }
}