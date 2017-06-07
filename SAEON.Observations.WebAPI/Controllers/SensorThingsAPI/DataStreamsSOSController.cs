using SAEON.Observations.Core.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ODataRoutePrefix("Datastreams")]
    public class DatastreamsSOSController : BaseSensorThingsControllers<Datastream>
    {
        protected override List<Datastream> GetList()
        {
            var result = base.GetList();
            result.AddRange(SensorThingsHelper.Datastreams);
            return result;
        }

        [EnableQuery, ODataRoute]
        public override IQueryable<Datastream> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Datastream> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery, ODataRoute("({id})/Thing")]
        public SingleResult<Thing> GetLocations([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.Thing, i => i.Datastreams);
        }

        [EnableQuery, ODataRoute("({id})/Sensor")]
        public SingleResult<Sensor> GetSensor([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.Sensor, i => i.Datastreams);
        }

        [EnableQuery, ODataRoute("({id})/ObservedProperty")]
        public SingleResult<ObservedProperty> GetObservedProperty([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.ObservedProperty, i => i.Datastream);
        }

        [EnableQuery, ODataRoute("({id})/Observations")]
        public IQueryable<Observation> GetObservationss([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Observations, i => i.Datastream);
        }

    }
}