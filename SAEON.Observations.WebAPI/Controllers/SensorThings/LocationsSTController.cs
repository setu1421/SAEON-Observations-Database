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
    public class LocationsSTController : BaseController<Location, db.SensorThingsLocation>
    {
        [EnableQuery(PageSize = Config.PageSize), ODataRoute]
        public override IQueryable<Location> GetAll() => base.GetAll();

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})")]
        public override SingleResult<Location> GetById([FromODataUri] Guid id) => base.GetById(id);

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Things")]
        public IQueryable<Thing> GetThings([FromUri] Guid id) => GetRelatedMany<Thing, db.SensorThingsThing>(id);

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/HistoricalLocations")]
        public IQueryable<HistoricalLocation> GetHistoricalLocations([FromUri] Guid id) => GetRelatedMany<HistoricalLocation, db.SensorThingsHistoricalLocation>(id);

    }
}
