using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.SensorThings;
using System.Linq;
using System.Web.Http;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [ODataRoutePrefix("Observations")]
    public class ObservationsSTController : BaseIntIdController<Observation, db.SensorThingsObservation>
    {
        [ODataRoute]
        public override IQueryable<Observation> GetAll() => base.GetAll();

        [ODataRoute("({id})")]
        public override SingleResult<Observation> GetById([FromODataUri] int id) => base.GetById(id);

        //[EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/Datastream")]
        //public SingleResult<Datastream> GetDatastream([FromUri] int id) => GetRelatedSingle<Datastream, db.SensorThingsDatastream>(id);

        //[EnableQuery(PageSize = Config.PageSize), ODataRoute("({id})/FeatureOfInterest")]
        //public SingleResult<FeatureOfInterest> GetFeatureOfInterest([FromUri] int id) => GetRelatedSingle<FeatureOfInterest, db.SensorThingsFeatureOfInterest>(id);
    }
}
