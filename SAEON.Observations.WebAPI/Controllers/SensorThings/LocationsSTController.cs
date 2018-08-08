using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
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

        [Route("~/SensorThings/Locations({id:int})")]
        public override JToken GetById([FromUri] int id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/Locations({id:int})/Things")]
        public JToken GetThings([FromUri] int id)
        {
            return GetMany(id, i => i.Things);
        }

        [Route("~/SensorThings/Locations({id:int})/HistoricalLocations")]
        public JToken GetHistoricalLocations([FromUri] int id)
        {
            return GetMany(id, i => i.HistoricalLocations);
        }

    }
}
