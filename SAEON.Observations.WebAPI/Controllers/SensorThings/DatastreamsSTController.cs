/*
using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [RoutePrefix("SensorThings/Datastreams")]
    public class DatastreamsSTController : BaseController<Datastream>
    {
        public DatastreamsSTController()
        {
            Entities.AddRange(SensorThingsFactory.Datastreams.OrderBy(i => i.Name));
        }

        public override JToken GetAll()
        {
            return base.GetAll();
        }

        [Route("~/SensorThings/Datastreams({id:guid})")]
        public override JToken GetById([FromUri] Guid id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/Datastreams({id:guid})/Sensor")]
        public JToken GetSensor([FromUri] Guid id)
        {
            return GetSingle(id, i => i.Sensor);
        }

        [Route("~/SensorThings/Datastreams({id:guid})/ObservedProperty")]
        public JToken GetObservedProperty([FromUri] Guid id)
        {
            return GetSingle(id, i => i.ObservedProperty);
        }

        [Route("~/SensorThings/Datastreams({id:guid})/Observations")]
        public JToken GetObservations([FromUri] Guid id)
        {
            return GetMany(id, i => i.Observations);
        }
    }
}
*/