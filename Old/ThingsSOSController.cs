using SAEON.Observations.Core.SensorThings;
using System.Collections.Generic;
using System.Linq;
using System.Web.OData;
using System.Web.OData.Routing;
using System;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ODataRoutePrefix("Things")]
    public class ThingsSOSController : BaseSensorThingsControllers<Thing>
    {
        protected override List<Thing> GetList()
        {
            var result = base.GetList();
            result.AddRange(SensorThingsHelper.Things);
            return result;
        }

        [EnableQuery, ODataRoute]
        public override IQueryable<Thing> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Thing> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery, ODataRoute("({id})/Locations")]
        public IQueryable<Location> GetLocations([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Locations, i => i.Things);
        }

        [EnableQuery, ODataRoute("({id})/HistoricalLocations")]
        public IQueryable<HistoricalLocation> GetHistoricalLocations([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.HistoricalLocations, i => i.Thing);
        }

        [EnableQuery, ODataRoute("({id})/Datastreams")]
        public IQueryable<Datastream> GetDatastreams([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Datastreams, i => i.Thing);
        }
    }
}