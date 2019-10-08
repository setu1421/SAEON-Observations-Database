/*
 * using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [RoutePrefix("SensorThings/ObservedProperties")]
    public class ObservedPropertySTController : BaseController<ObservedProperty>
    {
        public ObservedPropertySTController() : base()
        {
            Entities.AddRange(SensorThingsFactory.ObservedProperties.OrderBy(i => i.Name));
        }

        public override JToken GetAll()
        {
            return base.GetAll();
        }

        [Route("~/SensorThings/ObservedProperties({id:guid})")]
        public override JToken GetById([FromUri] Guid id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/ObservedProperties({id:guid})/Datastream")]
        public JToken GetDatastream([FromUri] Guid id)
        {
            return GetSingle(id, i => i.Datastream);
        }

    }
}
*/