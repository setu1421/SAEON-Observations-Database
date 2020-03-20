using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.SensorThings;
using System;
using System.Linq;
using System.Web.Http;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{

    [ODataRoutePrefix("Sensors")]
    public class SensorsSTController : BaseGuidIdController<Sensor, db.SensorThingsSensor>
    {
        [ODataRoute]
        public override IQueryable<Sensor> GetAll() => base.GetAll();

        [ODataRoute("({id})")]
        public override SingleResult<Sensor> GetById([FromODataUri] Guid id) => base.GetById(id);

        [ODataRoute("({id})/Datastreams")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public IQueryable<Datastream> GetDatastreams([FromUri] Guid id) => GetRelatedMany<Datastream, db.SensorThingsDatastream>(id);

    }
}
