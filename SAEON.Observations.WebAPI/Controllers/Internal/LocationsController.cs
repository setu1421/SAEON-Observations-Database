using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [RoutePrefix("Internal/Locations")]
    public class LocationsController : BaseListController<Location>
    {

        protected override List<Location> GetList()
        {
            var result = base.GetList();
            foreach (var organisation in db.Organisations
                .Where(org => org.Sites.Any(site => db.Inventory.Any(inv => inv.SiteCode == site.Code)))
                .Include(org => org.Sites.Select(site => site.Stations))
                .OrderBy(org => org.Name))
            {
                //Logging.Verbose("Organisation: {organisation}", organisation.Name);
                var organisationNode = new Location
                {
                    Id = organisation.Id,
                    Key = $"ORG~{organisation.Id}~",
                    Text = organisation.Name,
                    HasChildren = true,
                    LinkProperty = "Organisation"
                };
                result.Add(organisationNode);
                foreach (var site in organisation.Sites
                    .Where(site => db.Inventory.Any(inv => inv.SiteCode == site.Code))
                    .OrderBy(i => i.Name))
                {
                    //Logging.Verbose("Site: {site}", site.Name);
                    var siteNode = new Location
                    {
                        Id = site.Id,
                        ParentId = organisationNode.Id,
                        Key = $"SIT~{site.Id}~{organisationNode.Key}",
                        ParentKey = organisationNode.Key,
                        Text = site.Name,
                        HasChildren = true,
                        LinkProperty = "Site"
                    };
                    result.Add(siteNode);
                    foreach (var station in site.Stations
                        .Where(station => db.Inventory.Any(inv => inv.StationCode == station.Code && inv.SiteCode == station.Site.Code))
                        .OrderBy(station => station.Name))
                    {
                        var stationNode = new Location
                        {
                            Id = station.Id,
                            ParentId = siteNode.Id,
                            Key = $"STA~{station.Id}~{siteNode.Key}",
                            ParentKey = siteNode.Key,
                            Text = station.Name,
                            Name = $"{site.Name} - {station.Name}",
                            HasChildren = false,
                            Latitude = station.Latitude,
                            Longitude = station.Longitude,
                            Elevation = station.Elevation,
                            Url = station.Url,
                            LinkProperty = "Station"
                        };
                        result.Add(stationNode);
                    }

                }
            }
            return result;
        }
    }
}