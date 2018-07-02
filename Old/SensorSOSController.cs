using SAEON.Observations.Core.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ODataRoutePrefix("Sensors")]
    public class SensorsSOSController : BaseSensorThingsControllers<Sensor>
    {
        protected override List<Sensor> GetList()
        {
            var result = base.GetList();
            result.AddRange(SensorThingsHelper.Sensors);
            return result;
        }

        [EnableQuery, ODataRoute]
        public override IQueryable<Sensor> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<Sensor> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery, ODataRoute("({id})/DataSteams")]
        public IQueryable<Datastream> GetDatastreams([FromODataUri] Guid id)
        {
            return GetMany(id, s => s.Datastreams, i => i.Sensor);
        }

    }
}