using SAEON.Observations.Core.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
{
    [ODataRoutePrefix("ObservedProperties")]
    public class ObservedPropertiesSOSController : BaseSensorThingsControllers<ObservedProperty>
    {
        protected override List<ObservedProperty> GetList()
        {
            var result = base.GetList();
            result.AddRange(SensorThingsHelper.ObservedProperties);
            return result;
        }

        [EnableQuery, ODataRoute]
        public override IQueryable<ObservedProperty> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery, ODataRoute("({id})")]
        public override SingleResult<ObservedProperty> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery, ODataRoute("({id})/DataSteam")]
        public SingleResult<DataStream> GetDataStream([FromODataUri] Guid id)
        {
            return GetSingle(id, s => s.DataStream, i => i.ObservedProperty);
        }

    }
}