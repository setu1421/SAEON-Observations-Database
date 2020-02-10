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
    public class ThingsSTController : BaseController<Thing, db.SensorThingsThing>
    {
        [EnableQuery(PageSize = Config.PageSize), ODataRoute]
        public override IQueryable<Thing> GetAll() => base.GetAll();

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})")]
        public override SingleResult<Thing> GetById([FromODataUri] Guid id) => base.GetById(id);

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Datastreams")]
        public IQueryable<Datastream> GetDatastreams([FromUri] Guid id) => GetRelatedMany<Datastream, db.SensorThingsDatastream>(id);

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Locations")]
        public IQueryable<Location> GetLocations([FromUri] Guid id) => GetRelatedMany<Location, db.SensorThingsLocation>(id);

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/HistoricalLocations")]
        public IQueryable<HistoricalLocation> GetHistoricalLocations([FromUri] Guid id) => GetRelatedMany<HistoricalLocation, db.SensorThingsHistoricalLocation>(id);
    }
}
