using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
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

        [Route("~/SensorThings/Datastreams({id:int})")]
        public override JToken GetById([FromUri] int id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/Datastreams({id:int})/Sensor")]
        public JToken GetSensor([FromUri] int id)
        {
            return GetSingle(id, i => i.Sensor);
        }

        [Route("~/SensorThings/Datastreams({id:int})/ObservedProperty")]
        public JToken GetObservedProperty([FromUri] int id)
        {
            return GetSingle(id, i => i.ObservedProperty);
        }

        [Route("~/SensorThings/Datastreams({id:int})/Observations")]
        public JToken GetObservations([FromUri] int id)
        {
            return GetMany(id, i => i.Observations);
        }
    }
}
