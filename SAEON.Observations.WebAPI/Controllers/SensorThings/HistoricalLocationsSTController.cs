using AutoMapper.Configuration;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Logs;
using SAEON.Observations.SensorThings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [ODataRoutePrefix("HistoricalLocations")]
    public class HistoricalLocationsSTController : BaseController<HistoricalLocation, db.SensorThingsLocation>
    {
        protected override void CreateRelatedMappings(MapperConfigurationExpression cfg)
        {
            base.CreateRelatedMappings(cfg);
            cfg.CreateMap<db.SensorThingsLocation, Location>();
            cfg.CreateMap<db.SensorThingsThing, Thing>();
        }

        protected override HistoricalLocation ConvertDbEntity(db.SensorThingsLocation dbEntity)
        {
            using (Logging.MethodCall(GetType()))
            {
                var dbThing = DbContext.SensorThingsThings.Where(i => i.Id == dbEntity.Id).First();
                return Converters.ConvertHistoricalLocation(Mapper, dbEntity, dbThing);
            }
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute]
        public override IQueryable<HistoricalLocation> GetAll()
        {
            return base.GetAll();
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})")]
        public override SingleResult<HistoricalLocation> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Thing")]
        public SingleResult<Thing> GetThing([FromUri] Guid id)
        {
            return GetRelatedSingle(id, i => i.Thing);
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Locations")]
        public IQueryable<Location> GetLocations([FromUri] Guid id)
        {
            return GetRelatedMany(id, i => i.Locations);
        }

    }
}
