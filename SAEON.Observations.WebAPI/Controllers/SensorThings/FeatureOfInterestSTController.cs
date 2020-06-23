using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.AspNet.WebApi;
using SAEON.Observations.Core;
using SAEON.Observations.SensorThings;
using System;
using System.Linq;
using System.Web.Http;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{

    [ODataRoutePrefix("FeaturesOfInterest")]
    public class FeaturesOfInterestSTController : BaseGuidIdController<FeatureOfInterest, db.SensorThingsFeatureOfInterest>
    {
        [ODataRoute]
        public override IQueryable<FeatureOfInterest> GetAll() => base.GetAll();

        [ODataRoute("({id})")]
        public override SingleResult<FeatureOfInterest> GetById([FromODataUri] Guid id) => base.GetById(id);

        [ODataRoute("({id})/Observations")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        [Authorize]
        [DenyClientAuthorization(Constants.ClientIdPostman, Constants.ClientIdSwagger)]
        public IQueryable<Observation> GetObservations([FromUri] Guid id) => GetRelatedManyIntId<Observation, db.SensorThingsObservation>(id);
    }
}
