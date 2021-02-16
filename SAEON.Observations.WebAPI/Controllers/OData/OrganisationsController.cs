using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using SAEON.Observations.Core;
using System;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.OData
{
    [ODataRoutePrefix("Organisations")]
    public class OrganisationsController : NamedODataController<Organisation>
    {
        [ODataRoute("({id})/Sites")]
        public IQueryable<Site> GetSites(Guid id)
        {
            return GetManyWithGuidId<Site>(id, s => s.Sites);
        }

        [ODataRoute("({id})/Stations")]
        public IQueryable<Station> GetStations([FromODataUri] Guid id)
        {
            var siteStations = GetManyWithGuidId<Site>(id, s => s.Sites).SelectMany(i => i.Stations);
            return GetManyWithGuidId<Station>(id, s => s.Stations).Union(siteStations);
        }

        [ODataRoute("({id})/Instruments")]
        public IQueryable<Instrument> GetInstruments([FromODataUri] Guid id)
        {
            var siteInstruments = GetManyWithGuidId<Site>(id, s => s.Sites).SelectMany(i => i.Stations).SelectMany(i => i.Instruments);
            var stationInstruments = GetManyWithGuidId<Station>(id, s => s.Stations).SelectMany(i => i.Instruments);
            return GetManyWithGuidId<Instrument>(id, s => s.Instruments).Union(siteInstruments).Union(stationInstruments);
        }
    }
}
