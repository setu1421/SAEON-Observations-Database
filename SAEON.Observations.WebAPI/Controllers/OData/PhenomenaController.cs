using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core;
using System;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [ODataRoutePrefix("Phenomena")]
    public class PhenomenaController : NamedODataController<Phenomenon>
    {
        [ODataRoute("({id})/Offerings")]
        public IQueryable<Offering> GetOfferings(Guid id)
        {
            return GetManyWithGuidId(id, i => i.Offerings);
        }

        [ODataRoute("({id})/Units")]
        public IQueryable<Unit> GetUnits(Guid id)
        {
            return GetManyWithGuidId(id, i => i.Units);
        }

        [ODataRoute("({id})/Sensors")]
        public IQueryable<Sensor> GetSensors(Guid id)
        {
            return GetManyWithGuidId(id, s => s.Sensors);
        }
    }
}
