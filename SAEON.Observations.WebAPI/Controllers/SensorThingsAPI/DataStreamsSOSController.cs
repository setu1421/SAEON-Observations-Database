using SAEON.Observations.Core.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ODataRoutePrefix("DataStreams")]
    public class DataStreamsSOSController : BaseSensorThingsControllers<DataStream>
    {
        protected override List<DataStream> GetList()
        {
            var result = base.GetList();
            result.AddRange(SensorThingsHelper.DataStreams);
            return result;
        }

        [EnableQuery, ODataRoute]
        public override IQueryable<DataStream> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<DataStream> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery, ODataRoute("({id})/Thing")]
        public SingleResult<Thing> GetLocations([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.Thing, i => i.DataStreams);
        }

        [EnableQuery, ODataRoute("({id})/Sensor")]
        public SingleResult<Sensor> GetSensor([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.Sensor, i => i.DataStreams);
        }

        [EnableQuery, ODataRoute("({id})/ObservedProperty")]
        public SingleResult<ObservedProperty> GetObservedProperty([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.ObservedProperty, i => i.DataStream);
        }

        [EnableQuery, ODataRoute("({id})/Observations")]
        public IQueryable<Observation> GetObservationss([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Observations, i => i.DataStream);
        }

    }
}