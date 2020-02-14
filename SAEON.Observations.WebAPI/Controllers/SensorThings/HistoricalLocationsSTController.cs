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
        [EnableQuery(PageSize = Config.PageSize), ODataRoute]
        public override IQueryable<HistoricalLocation> GetAll() => base.GetAll();

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})")]
        public override SingleResult<HistoricalLocation> GetById([FromODataUri] Guid id) => base.GetById(id);

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Thing")]
        public SingleResult<Thing> GetThing([FromUri] Guid id) => GetRelatedSingle<Thing, db.SensorThingsThing>(id);

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Locations")]
        public IQueryable<Location> GetLocations([FromUri] Guid id) => GetRelatedMany<Location, db.SensorThingsLocation>(id);

    }
}
