using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
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

        [Route("~/SensorThings/Observations({id:int})")]
        public override JToken GetById([FromUri] int id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/Observations({id:int})/Datastream")]
        public JToken GetDatastream([FromUri] int id)
        {
            return GetSingle(id, i => i.Datastream);
        }

        [Route("~/SensorThings/Observations({id:int})/FeatureOfInterest")]
        public JToken GetFeatureOfInterest([FromUri] int id)
        {
            return GetSingle(id, i => i.FeatureOfInterest);
        }

    }
}
