/*
using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [RoutePrefix("SensorThings/Observations")]
    public class ObservationSTController : BaseController<Observation>
    {
        public ObservationSTController() : base()
        {
            Entities.AddRange(SensorThingsFactory.Observations.OrderBy(i => i.ResultTime));
        }

        public override JToken GetAll()
        {
            return base.GetAll();
        }

        [Route("~/SensorThings/Observations({id:guid})")]
        public override JToken GetById([FromUri] Guid id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/Observations({id:guid})/Datastream")]
        public JToken GetDatastream([FromUri] Guid id)
        {
            return GetSingle(id, i => i.Datastream);
        }

        [Route("~/SensorThings/Observations({id:guid})/FeatureOfInterest")]
        public JToken GetFeatureOfInterest([FromUri] Guid id)
        {
            return GetSingle(id, i => i.FeatureOfInterest);
        }

    }
}
*/