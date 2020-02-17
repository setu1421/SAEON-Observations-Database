using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.SensorThings;
using System;
using System.Linq;
using System.Web.Http;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [ODataRoutePrefix("Locations")]
    public class LocationsSTController : BaseGuidIdController<Location, db.SensorThingsLocation>
    {
        [ODataRoute]
        public override IQueryable<Location> GetAll() => base.GetAll();

        [ODataRoute("({id})")]
        public override SingleResult<Location> GetById([FromODataUri] Guid id) => base.GetById(id);

        [ODataRoute("({id})/Things")]
        [EnableQuery(PageSize = Config.PageSize, MaxTop = Config.MaxTop)]

        public IQueryable<Thing> GetThings([FromUri] Guid id) => GetRelatedMany<Thing, db.SensorThingsThing>(id);

        [ODataRoute("({id})/HistoricalLocations")]
        [EnableQuery(PageSize = Config.PageSize, MaxTop = Config.MaxTop)]

        public IQueryable<HistoricalLocation> GetHistoricalLocations([FromUri] Guid id) => GetRelatedMany<HistoricalLocation, db.SensorThingsHistoricalLocation>(id);

    }
}
