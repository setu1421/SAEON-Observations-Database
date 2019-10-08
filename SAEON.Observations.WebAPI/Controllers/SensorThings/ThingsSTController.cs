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
                var result = SensorThingsConverters.ConvertThing(Mapper, dbEntity);
                var dbLocation = DbContext.SensorThingsLocations.Where(i => i.Id == dbEntity.Id).FirstOrDefault();
                if (dbLocation != null)
                {
                    result.Location = SensorThingsConverters.ConvertLocation(Mapper, dbLocation);
                }
                Logging.Verbose("Result: {@Result}", result);
                return result;
            }
        }

        //protected override TRelatedSensorThingsEntity ConvertRelated<TRelatedSensorThingsEntity, TRelatedDbEntity>(TRelatedDbEntity dbEnitity)
        //{
        //    var result = base.ConvertRelated<TRelatedSensorThingsEntity, TRelatedDbEntity>(dbEnitity);
        //    result.location = new GeometryLocation(dbEnitity.Latitude, dbEnitity.Longitude, dbEnitity.Elevation);
        //    return result;
        //}

        [EnableQuery(PageSize = SensorThingsConfig.PageSize), ODataRoute]
        public override IQueryable<Thing> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery(PageSize = SensorThingsConfig.PageSize), ODataRoute("({id})")]
        public override SingleResult<Thing> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery(PageSize = SensorThingsConfig.PageSize), ODataRoute("({id})/Location")]
        public SingleResult<Location> GetLocation([FromUri] Guid id)
        {
            return GetRelatedSingle(id, i => i.Location);
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
