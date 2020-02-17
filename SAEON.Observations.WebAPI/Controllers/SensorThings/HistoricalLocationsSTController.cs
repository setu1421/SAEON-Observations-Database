using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.SensorThings;
using System;
using System.Linq;
using System.Web.Http;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [ODataRoutePrefix("HistoricalLocations")]
    public class HistoricalLocationsSTController : BaseGuidIdController<HistoricalLocation, db.SensorThingsHistoricalLocation>
    {
        [ODataRoute]
        [EnableQuery(PageSize = Config.PageSize, MaxTop = Config.MaxTop)]

        public override IQueryable<HistoricalLocation> GetAll() => base.GetAll();

        [ODataRoute("({id})")]

        public override SingleResult<HistoricalLocation> GetById([FromODataUri] Guid id) => base.GetById(id);

        [ODataRoute("({id})/Thing")]
        [EnableQuery(PageSize = Config.PageSize, MaxTop = Config.MaxTop)]

        public SingleResult<Thing> GetThing([FromUri] Guid id) => GetRelatedSingle<Thing, db.SensorThingsThing>(id);

        [ODataRoute("({id})/Locations")]
        [EnableQuery(PageSize = Config.PageSize, MaxTop = Config.MaxTop)]

        public IQueryable<Location> GetLocations([FromUri] Guid id) => GetRelatedMany<Location, db.SensorThingsLocation>(id);

    }
}
