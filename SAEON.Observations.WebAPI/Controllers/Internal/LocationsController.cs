using SAEON.Observations.Core;
using System.Collections.Generic;
using System.Linq;
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
            foreach (var location in db.Locations.Where(i => (i.Latitude != null) && (i.Longitude != null)).OrderBy(i => i.OrganisationName).ThenBy(i => i.SiteName).ThenBy(i => i.StationName))
            {
                if (organisation?.Id != location.OrganisationID)
                {
                    site = null;
                    station = null;
                    organisation = new LocationNode
                    {
                        Id = location.OrganisationID,
                        Key = $"ORG~{location.OrganisationID}~",
                        Text = location.OrganisationName,
                        Name = $"{location.OrganisationName}",
                        ToolTip = new LinkAttribute("Organisation"),
                        HasChildren = true
                    };
                    result.Add(organisation);
                }
                if (site?.Id != location.SiteID)
                {
                    station = null;
                    site = new LocationNode
                    {
                        Id = location.SiteID,
                        ParentId = organisation.Id,
                        Key = $"SIT~{location.SiteID}~{organisation.Key}",
                        ParentKey = organisation.Key,
                        Text = location.SiteName,
                        Name = $"{organisation.Text} | {location.SiteName}",
                        ToolTip = new LinkAttribute("Site"),
                        HasChildren = true
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
                        Text = location.StationName,
                        Name = $"{organisation.Text} | {site.Text} | {location.StationName}",
                        ToolTip = new LinkAttribute("Station"),
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        Elevation = location.Elevation,
                        Url = location.Url,
                        HasChildren = false
                    };
                    result.Add(station);
                }
            }
            return result;
        }
    }
}