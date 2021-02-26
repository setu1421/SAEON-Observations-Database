using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core;
using System;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [ODataRoutePrefix("Sensors")]
    public class SensorsController : CodedNamedODataController<Sensor>
    {
        [ODataRoute("({id})/Phenomenon")]
        public Phenomenon GetPhenomenon(Guid id)
        {
            return GetSingle(id, s => s.Phenomenon);
        }

        [ODataRoute("({id})/Instruments")]
        public IQueryable<Instrument> GetInstruments(Guid id)
        {
            return GetManyWithGuidId<Instrument>(id, s => s.Instruments);
        }
    }
}
