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
                    result.Location = Converters.ConvertLocation(Mapper, dbLocation);
                    var historicalLocation = new HistoricalLocation(dbEntity.StartDate)
                    {
                        Id = result.Id,
                        Thing = result
                    };
                    historicalLocation.Locations.Add(result.Location);
                    result.HistoricalLocations.Add(historicalLocation);
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

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Location")]
        public SingleResult<Location> GetLocation([FromUri] Guid id)
        {
            return GetRelatedSingle(id, i => i.Location);
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/HistoricalLocations")]
        public IQueryable<HistoricalLocation> GetHistoricalLocations([FromUri] Guid id)
        {
            return GetRelatedMany(id, i => i.HistoricalLocations);
        }
    }
}

