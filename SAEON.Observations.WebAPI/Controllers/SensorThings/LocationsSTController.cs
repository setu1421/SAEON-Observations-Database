using AutoMapper.Configuration;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Logs;
using SAEON.Observations.SensorThings;
using System;
using System.Collections.Generic;
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
                foreach (var dbThing in DbContext.SensorThingsThings.Where(i => i.Id == dbEntity.Id))
                {
                    var thing = Converters.ConvertThing(Mapper, dbThing);
                    result.Things.Add(thing);
                    var historicalLocation = new HistoricalLocation(dbThing.StartDate)
                    {
                        Id = result.Id,
                        Thing = thing
                    };
                    historicalLocation.Locations.Add(result);
                    result.HistoricalLocations.Add(historicalLocation);

                }
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
        public IQueryable<Thing> GetThings([FromODataUri] Guid id)
        {
            return GetRelatedMany(id, i => i.Things);
        }

        [EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/HistoricalLocations")]
        public IQueryable<HistoricalLocation> GetHistoricalLocations([FromODataUri] Guid id)
        {
            return GetRelatedMany(id, i => i.HistoricalLocations);
        }
    }
}

