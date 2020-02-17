using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.SensorThings;
using System;
using System.Linq;
using System.Web.Http;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [ODataRoutePrefix("Things")]
    public class ThingsSTController : BaseGuidIdController<Thing, db.SensorThingsThing>
    {
        [ODataRoute]
        public override IQueryable<Thing> GetAll() => base.GetAll();

        [ODataRoute("({id})")]
        public override SingleResult<Thing> GetById([FromODataUri] Guid id) => base.GetById(id);

        [ODataRoute("({id})/Datastreams")]
        [EnableQuery(PageSize = Config.PageSize, MaxTop = Config.MaxTop)]
        public IQueryable<Datastream> GetDatastreams([FromUri] Guid id) => GetRelatedMany<Datastream, db.SensorThingsDatastream>(id);

        [ODataRoute("({id})/Locations")]
        [EnableQuery(PageSize = Config.PageSize, MaxTop = Config.MaxTop)]
        public IQueryable<Location> GetLocations([FromUri] Guid id) => GetRelatedMany<Location, db.SensorThingsLocation>(id);

        [ODataRoute("({id})/HistoricalLocations")]
        [EnableQuery(PageSize = Config.PageSize, MaxTop = Config.MaxTop)]
        public IQueryable<HistoricalLocation> GetHistoricalLocations([FromUri] Guid id) => GetRelatedMany<HistoricalLocation, db.SensorThingsHistoricalLocation>(id);
    }
}
