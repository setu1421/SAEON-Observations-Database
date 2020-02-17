using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.SensorThings;
using System;
using System.Linq;
using System.Web.Http;
using db = SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.Controllers.SensorThings
{
    [ODataRoutePrefix("ObservedProperties")]
    public class ObservedPropertiesSTController : BaseGuidIdController<ObservedProperty, db.SensorThingsObservedProperty>
    {
        [ODataRoute]
        public override IQueryable<ObservedProperty> GetAll() => base.GetAll();

        [ODataRoute("({id})")]
        public override SingleResult<ObservedProperty> GetById([FromODataUri] Guid id) => base.GetById(id);

        [ODataRoute("({id})/Datastreams")]
        [EnableQuery(PageSize = Config.PageSize, MaxTop = Config.MaxTop)]

        public IQueryable<Datastream> GetDatastreams([FromUri] Guid id) => GetRelatedMany<Datastream, db.SensorThingsDatastream>(id);
    }
}

