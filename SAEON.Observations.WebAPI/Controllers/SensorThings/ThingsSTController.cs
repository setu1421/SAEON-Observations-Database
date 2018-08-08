using Newtonsoft.Json.Linq;
using SAEON.SensorThings;
using System.Linq;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.SensorThingsAPI
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

        [Route("~/SensorThings/Things({id:int})")]
        public override JToken GetById([FromUri] int id)
        {
            return base.GetById(id);
        }

        [Route("~/SensorThings/Things({id:int})/Location")]
        public JToken GetLocation([FromUri] int id)
        {
            return GetSingle(id, i => i.Location);
        }

        [Route("~/SensorThings/Things({id:int})/HistoricalLocations")]
        public JToken GetHistoricalLocations([FromUri] int id)
        {
            return GetMany(id, i => i.HistoricalLocations);
        }

        [Route("~/SensorThings/Things({id:int})/Datastreams")]
        public JToken GetDatastreams([FromUri] int id)
        {
            return GetMany(id, i => i.Datastreams);
        }

    }
}
