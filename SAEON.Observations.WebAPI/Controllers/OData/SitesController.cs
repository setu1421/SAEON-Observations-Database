using Microsoft.AspNet.OData.Routing;
using System;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [ODataRoutePrefix("Sites")]
    public class SitesController : NamedODataController<Site>
    {
        [ODataRoute("({id})/Organisations")]
        public IQueryable<Organisation> GetOrganisations(Guid id)
        {
            return GetManyWithGuidId<Organisation>(id, s => s.Organisations);
        }

        [ODataRoute("({id})/Stations")]
        public IQueryable<Station> GetStations(Guid id)
        {
            return GetManyWithGuidId<Station>(id, s => s.Stations);
        }
    }
}
