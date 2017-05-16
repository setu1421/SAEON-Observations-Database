using SAEON.Observations.Core.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ODataRoutePrefix("Locations")]
    public class LocationsSOSController : BaseSensorThingsControllers<Location>
    {
        protected override List<Location> GetList()
        {
            var result = base.GetList();
            result.AddRange(SensorThingsHelper.Locations);
            return result;
        }

        [EnableQuery, ODataRoute]
        public override IQueryable<Location> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Location> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery, ODataRoute("({id})/Locations")]
        public IQueryable<Thing> GetLocations([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Things, i => i.Locations);
        }

        [EnableQuery, ODataRoute("({id})/HistoricalLocations")]
        public IQueryable<HistoricalLocation> GetHistoricalLocations([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.HistoricalLocations, i => i.Locations);
        }

    }
}