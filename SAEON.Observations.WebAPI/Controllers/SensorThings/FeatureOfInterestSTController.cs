/*
 * using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
using System;
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

        [Route("~/SensorThings/FeaturesOfInterest({id:guid})")]
        public override JToken GetById([FromUri] Guid id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/FeaturesOfInterest({id:guid})/Observations")]
        public JToken GetObservations([FromUri] Guid id)
        {
            return GetMany(id, i => i.Observations);
        }

    }
}
*/