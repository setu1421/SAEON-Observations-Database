using SAEON.Observations.Core.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ODataRoutePrefix("Observations")]
    public class ObservationsSOSController : BaseSensorThingsControllers<Observation>
    {
        protected override List<Observation> GetList()
        {
            var result = base.GetList();
            result.AddRange(SensorThingsHelper.Observations);
            return result;
        }

        [EnableQuery, ODataRoute]
        public override IQueryable<Observation> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Observation> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery, ODataRoute("({id})/DataSteam")]
        public SingleResult<Datastream> GetDatastream([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.Datastream, i => i.Observations);
        }

        [EnableQuery, ODataRoute("({id})/FeatureOfInterest")]
        public SingleResult<FeatureOfInterest> GetFeatureOfInterest([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.FeatureOfInterest, i => i.Observations);
        }

    }
}