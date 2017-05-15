using Newtonsoft.Json.Linq;
using SAEON.Observations.Core.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ODataRoutePrefix("Things")]
    public class ThingsSOSController : BaseSensorThingsControllers<Thing>
    {
        protected override List<Thing> GetList()
        {
            var result = base.GetList();
            result.AddRange(SensorThingsHelper.Things);
            return result;
        }

        [EnableQuery, ODataRoute]
        public override IQueryable<Thing> GetAll()
        {
            return base.GetAll();
        }
    }
}