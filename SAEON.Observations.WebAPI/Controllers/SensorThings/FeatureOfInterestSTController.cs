using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [RoutePrefix("SensorThings/FeaturesOfInterest")]
    public class FeatureOfInterestSTController : BaseController<FeatureOfInterest>
    {
        public FeatureOfInterestSTController() : base()
        {
            Entities.AddRange(SensorThingsFactory.FeaturesOfInterest.OrderBy(i => i.Name));
        }

        public override JToken GetAll()
        {
            return base.GetAll();
        }

        [Route("~/SensorThings/FeaturesOfInterest({id:int})")]
        public override JToken GetById([FromUri] int id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/FeaturesOfInterest({id:int})/Observations")]
        public JToken GetObservations([FromUri] int id)
        {
            return GetMany(id, i => i.Observations);
        }

    }
}
