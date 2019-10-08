/*
 * using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [RoutePrefix("SensorThings/Sensors")]
    public class SensorSTController : BaseController<Sensor>
    {
        public SensorSTController() : base()
        {
            Entities.AddRange(SensorThingsFactory.Sensors.OrderBy(i => i.Name));
        }

        public override JToken GetAll()
        {
            return base.GetAll();
        }

        [Route("~/SensorThings/Sensors({id:guid})")]
        public override JToken GetById([FromUri] Guid id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/Sensors({id:guid})/Datastream")]
        public JToken GetDatastream([FromUri] Guid id)
        {
            return GetSingle(id, i => i.Datastream);
        }

    }
}
*/