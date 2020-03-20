using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.SensorThings;
using System;
using System.Linq;
using System.Web.Http;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{

    [ODataRoutePrefix("Datastreams")]
    public class DatastreamsSTController : BaseGuidIdController<Datastream, db.SensorThingsDatastream>
    {
        [ODataRoute]
        public override IQueryable<Datastream> GetAll() => base.GetAll();

        [ODataRoute("({id})")]
        public override SingleResult<Datastream> GetById([FromODataUri] Guid id) => base.GetById(id);

        [ODataRoute("({id})/Observations")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        [Authorize]
        public IQueryable<Observation> GetObservations([FromUri] Guid id) => GetRelatedManyIntId<Observation, db.SensorThingsObservation>(id);

        [ODataRoute("({id})/ObservedProperty")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public SingleResult<ObservedProperty> GetObservedProperty([FromUri] Guid id) => GetRelatedSingle<ObservedProperty, db.SensorThingsObservedProperty>(id);

        [ODataRoute("({id})/Sensor")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public SingleResult<Sensor> GetSensor([FromUri] Guid id) => GetRelatedSingle<Sensor, db.SensorThingsSensor>(id);

        [ODataRoute("({id})/Thing")]
        [EnableQuery(PageSize = PageSize, MaxTop = MaxTop)]
        public SingleResult<Thing> GetThing([FromUri] Guid id) => GetRelatedSingle<Thing, db.SensorThingsThing>(id);
    }
}
