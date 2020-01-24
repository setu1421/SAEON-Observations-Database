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
    [ODataRoutePrefix("Locations")]
    public class LocationsSTController : BaseController<Location, db.SensorThingsLocation>
    {
        protected override void CreateRelatedMappings(MapperConfigurationExpression cfg)
        {
            base.CreateRelatedMappings(cfg);
            cfg.CreateMap<db.SensorThingsThing, Thing>();
        }

        protected override Location ConvertDbEntity(db.SensorThingsLocation dbEntity)
        {
            using (Logging.MethodCall(GetType()))
            {
                var result = Converters.ConvertLocation(Mapper, dbEntity);
                var dbThing = DbContext.SensorThingsThings.Where(i => i.Id == dbEntity.Id).First();
                result.Things.Add(Converters.ConvertThing(Mapper, dbThing));
                result.HistoricalLocations.Add(Converters.ConvertHistoricalLocation(Mapper, dbEntity, dbThing));
                Logging.Information("Result: {@Result}", result);
                return result;
            }
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute]
        public override IQueryable<Location> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})")]
        public override SingleResult<Location> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Things")]
        public IQueryable<Thing> GetThings([FromUri] Guid id)
        {
            return GetRelatedMany(id, i => i.Things);
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
    [RoutePrefix("SensorThings/Locations")]
    public class LocationsSTController : BaseController<Location>
    {
        public LocationsSTController()
        {
            Entities.AddRange(SensorThingsFactory.Locations.OrderBy(i => i.Name));
        }

        public override JToken GetAll()
        {
            return base.GetAll();
        }

        [Route("~/SensorThings/Locations({id:guid})")]
        public override JToken GetById([FromUri] Guid id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/Locations({id:guid})/Things")]
        public JToken GetThings([FromUri] Guid id)
        {
            return GetMany(id, i => i.Things);
        }

        [Route("~/SensorThings/Locations({id:guid})/HistoricalLocations")]
        public JToken GetHistoricalLocations([FromUri] Guid id)
        {
            return GetMany(id, i => i.HistoricalLocations);
        }

    }
}
*/
