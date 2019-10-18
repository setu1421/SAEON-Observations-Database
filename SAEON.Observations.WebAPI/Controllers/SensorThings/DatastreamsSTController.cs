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
        protected override void CreateRelatedMappings(MapperConfigurationExpression cfg)
        {
            base.CreateRelatedMappings(cfg);
            cfg.CreateMap<db.SensorThingsDatastream, Datastream>();
        }

        protected override Datastream ConvertDbEntity(db.SensorThingsDatastream dbEntity)
        {
            using (Logging.MethodCall(GetType()))
            {
                var result = Converters.ConvertDatastream(Mapper, dbEntity);
                Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

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

        //[EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Location")]
        //public SingleResult<Location> GetLocation([FromUri] Guid id)
        //{
        //    return GetRelatedSingle(id, i => i.Location);
        //}

        //[EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/HistoricalLocations")]
        //public IQueryable<HistoricalLocation> GetHistoricalLocations([FromUri] Guid id)
        //{
        //    return GetRelatedMany(id, i => i.HistoricalLocations);
        //}
    }
}
/*
using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
using System;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [RoutePrefix("SensorThings/Datastreams")]
    public class DatastreamsSTController : BaseController<Datastream>
    {
        public DatastreamsSTController()
        {
            Entities.AddRange(SensorThingsFactory.Datastreams.OrderBy(i => i.Name));
        }

        public override JToken GetAll()
        {
            return base.GetAll();
        }

        [Route("~/SensorThings/Datastreams({id:guid})")]
        public override JToken GetById([FromUri] Guid id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/Datastreams({id:guid})/Sensor")]
        public JToken GetSensor([FromUri] Guid id)
        {
            return GetSingle(id, i => i.Sensor);
        }

        [Route("~/SensorThings/Datastreams({id:guid})/ObservedProperty")]
        public JToken GetObservedProperty([FromUri] Guid id)
        {
            return GetSingle(id, i => i.ObservedProperty);
        }

        [Route("~/SensorThings/Datastreams({id:guid})/Observations")]
        public JToken GetObservations([FromUri] Guid id)
        {
            return GetMany(id, i => i.Observations);
        }
    }
}
*/
