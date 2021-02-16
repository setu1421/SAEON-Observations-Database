using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core;
using System;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [ODataRoutePrefix("Instruments")]
    public class InstrumentsController : NamedODataController<Instrument>
    {
        [ODataRoute("({id})/Organisations")]
        public IQueryable<Organisation> GetOrganisations(Guid id)
        {
            var siteOrganisations = GetManyWithGuidId(id, s => s.Stations).Select(i => i.Site).SelectMany(i => i.Organisations);
            var stationOrganisations = GetManyWithGuidId(id, s => s.Stations).SelectMany(i => i.Organisations);
            return GetManyWithGuidId(id, s => s.Organisations).Union(stationOrganisations).Union(siteOrganisations);
        }

        [ODataRoute("({id})/Stations")]
        public IQueryable<Station> GetStations(Guid id)
        {
            return GetManyWithGuidId(id, s => s.Stations);
        }

        [ODataRoute("({id})/Sensors")]
        public IQueryable<Sensor> GetSensors(Guid id)
        {
            return GetManyWithGuidId(id, s => s.Sensors);
        }
    }
}
