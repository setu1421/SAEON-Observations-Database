using SAEON.Observations.Core;
using System.Collections.Generic;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{

    public class LocationsController : InternalListController<LocationNode>
    {
        protected override List<LocationNode> GetList()
        {
            var result = base.GetList();
            LocationNode organisation = null;
            LocationNode programme = null;
            LocationNode project = null;
            LocationNode site = null;
            LocationNode station = null;
            foreach (var location in DbContext.VLocations.OrderBy(i => i.OrganisationName).ThenBy(i => i.ProgrammeName).ThenBy(i => i.ProjectName).ThenBy(i => i.SiteName).ThenBy(i => i.StationName))
            {
                if (organisation?.Id != location.OrganisationID)
                {
                    programme = null;
                    project = null;
                    site = null;
                    station = null;
                    organisation = new LocationNode
                    {
                        Id = location.OrganisationID,
                        Key = $"ORG~{location.OrganisationID}",
                        Text = location.OrganisationName,
                        Name = $"{location.OrganisationName}",
                        ToolTip = new LinkAttribute("Organisation"),
                        Url = location.OrganisationUrl,
                        HasChildren = true
                    };
                    result.Add(organisation);
                }
                if (programme?.Id != location.ProgrammeID)
                {
                    project = null;
                    site = null;
                    station = null;
                    programme = new LocationNode
                    {
                        Id = location.ProgrammeID,
                        ParentId = organisation.Id,
                        Key = $"PRG~{location.ProgrammeID}|{organisation.Key}",
                        ParentKey = organisation.Key,
                        Text = location.ProgrammeName,
                        Name = $"{organisation.Text}|{location.ProgrammeName}",
                        ToolTip = new LinkAttribute("Programme"),
                        Url = location.ProgrammeUrl,
                        HasChildren = true
                    };
                    result.Add(programme);
                }
                if (project?.Id != location.ProjectID)
                {
                    site = null;
                    station = null;
                    project = new LocationNode
                    {
                        Id = location.ProjectID,
                        ParentId = programme.Id,
                        Key = $"PRJ~{location.ProjectID}|{programme.Key}",
                        ParentKey = programme.Key,
                        Text = location.ProjectName,
                        Name = $"{organisation.Text}|{programme.Text}|{location.ProjectName}",
                        ToolTip = new LinkAttribute("Project"),
                        Url = location.ProjectUrl,
                        HasChildren = true
                    };
                    result.Add(project);
                }
                if (site?.Id != location.SiteID)
                {
                    station = null;
                    site = new LocationNode
                    {
                        Id = location.SiteID,
                        ParentId = project.Id,
                        Key = $"SIT~{location.SiteID}|{project.Key}",
                        ParentKey = project.Key,
                        Text = location.SiteName,
                        Name = $"{organisation.Text}|{programme.Text}|{project.Text}|{location.SiteName}",
                        ToolTip = new LinkAttribute("Site"),
                        Url = location.SiteUrl,
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
                        Key = $"STA~{location.StationID}|{site.Key}",
                        ParentKey = site.Key,
                        Text = location.StationName,
                        Name = $"{site.Text} | {location.StationName}",
                        ToolTip = new LinkAttribute("Station"),
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        Elevation = location.Elevation,
                        Url = location.StationUrl,
                        HasChildren = false
                    };
                    result.Add(station);
                }
            }
            return result;
        }
    }
}
