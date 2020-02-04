using AutoMapper.Configuration;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Logs;
using SAEON.Observations.SensorThings;
using System;
using System.Linq;
using System.Web.Http;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{

    [ODataRoutePrefix("Datastreams")]
    public class DatastreamsSTController : BaseController<Datastream, db.SensorThingsDatastream>
    {
        [EnableQuery(PageSize = Config.PageSize), ODataRoute]
        public override IQueryable<Datastream> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})")]
        public override SingleResult<Datastream> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/ObservedProperty")]
        public SingleResult<ObservedProperty> GetObservedProperty([FromUri] Guid id)
        {
            return GetRelatedSingle(id, i => i.ObservedProperty);
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Sensor")]
        public SingleResult<Sensor> GetSensor([FromUri] Guid id)
        {
            return GetRelatedSingle(id, i => i.Sensor);
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Thing")]
        public SingleResult<Thing> GetThing([FromUri] Guid id)
        {
            return GetRelatedSingle(id, i => i.Thing);
        }
    }
}
