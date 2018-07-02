using SAEON.Observations.Core.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ODataRoutePrefix("FeaturesOfInterest")]
    public class FeaturesOfInterestSOSController : BaseSensorThingsControllers<FeatureOfInterest>
    {
        protected override List<FeatureOfInterest> GetList()
        {
            var result = base.GetList();
            result.AddRange(SensorThingsHelper.FeaturesOfInterest);
            return result;
        }

        [EnableQuery, ODataRoute]
        public override IQueryable<FeatureOfInterest> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<FeatureOfInterest> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery, ODataRoute("({id})/Observations")]
        public IQueryable<Observation> GetObservations([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Observations, i => i.FeatureOfInterest);
        }


    }
}