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
    [ODataRoutePrefix("Things")]
    public class ThingsSTController : BaseController<Thing, db.SensorThingsThing>
    {
        protected override void CreateRelatedMappings(MapperConfigurationExpression cfg)
        {
            base.CreateRelatedMappings(cfg);
            cfg.CreateMap<db.SensorThingsLocation, Location>();
        }

        protected override Thing ConvertDbEntity(db.SensorThingsThing dbEntity)
        {
            using (Logging.MethodCall(GetType()))
            {
                var result = Converters.ConvertThing(Mapper, dbEntity);
                var dbLocation = DbContext.SensorThingsLocations.Where(i => i.Id == dbEntity.Id).FirstOrDefault();
                if (dbLocation != null)
                {
                    result.Locations.Add(Converters.ConvertLocation(Mapper, dbLocation));
                    result.HistoricalLocations.Add(Converters.ConvertHistoricalLocation(Mapper, dbLocation, dbEntity));
                }
                Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute]
        public override IQueryable<Thing> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})")]
        public override SingleResult<Thing> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Locations")]
        public IQueryable<Location> GetLocations([FromUri] Guid id)
        {
            return GetRelatedMany(id, i => i.Locations);
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/HistoricalLocations")]
        public IQueryable<HistoricalLocation> GetHistoricalLocations([FromUri] Guid id)
        {
            return GetRelatedMany(id, i => i.HistoricalLocations);
        }


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
    [RoutePrefix("SensorThings/Things")]
    public class ThingsSTController : BaseController<Thing>
    {
        public ThingsSTController()
        {
            Entities.AddRange(SensorThingsFactory.Things.OrderBy(i => i.Name));
        }

        public override JToken GetAll()
        {
            return base.GetAll();
        }

        [Route("~/SensorThings/Things({id:guid})")]
        public override JToken GetById([FromUri] Guid id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/Things({id:guid})/Location")]
        public JToken GetLocation([FromUri] Guid id)
        {
            return GetSingle(id, i => i.Location);
        }

        [Route("~/SensorThings/Things({id:guid})/HistoricalLocations")]
        public JToken GetHistoricalLocations([FromUri] Guid id)
        {
            return GetMany(id, i => i.HistoricalLocations);
        }

        [Route("~/SensorThings/Things({id:guid})/Datastreams")]
        public JToken GetDatastreams([FromUri] Guid id)
        {
            return GetMany(id, i => i.Datastreams);
        }

    }
}
*/
