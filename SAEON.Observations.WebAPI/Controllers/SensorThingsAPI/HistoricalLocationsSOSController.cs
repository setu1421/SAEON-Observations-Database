using SAEON.Observations.Core.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ODataRoutePrefix("HistoricalLocations")]
    public class HistoricalLocationsSOSController : BaseSensorThingsControllers<HistoricalLocation>
    {
        protected override List<HistoricalLocation> GetList()
        {
            var result = base.GetList();
            result.AddRange(SensorThingsHelper.HistoricalLocations);
            return result;
        }

        [EnableQuery, ODataRoute]
        public override IQueryable<HistoricalLocation> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<HistoricalLocation> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery, ODataRoute("({id})/Thing")]
        public SingleResult<Thing> GetThing([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.Thing, i => i.HistoricalLocations);
        }

        [EnableQuery, ODataRoute("({id})/Locations")]
        public IQueryable<Location> GetLocations([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Locations, i => i.HistoricalLocations);
        }

    }
}