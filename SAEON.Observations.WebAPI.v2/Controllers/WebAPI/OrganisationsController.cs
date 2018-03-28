using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.WebAPI.Controllers.WebAPI;

namespace SAEON.Observations.WebAPI.v2.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/Organisations")]
    public class OrganisationsController : BaseAPIController<Organisation>
    {
        public OrganisationsController(ObservationsDbContext context) : base(context) { }

        protected override IQueryable<Organisation> ApplyIncludes(IQueryable<Organisation> query)
        {
            return query
                .Include(i => i.OrganisationSites)
                    .ThenInclude(i => i.Site)
                .Include(i => i.OrganisationStations)
                    .ThenInclude(i => i.Station);
        }

        [HttpGet("{id}/Instruments")]
        public IEnumerable<Instrument> GetInstruments(Guid id)
        {
            return GetMany(id, sel => sel.OrganisationInstruments.Select(i => i.Instrument), inc => inc.OrganisationInstruments.Select(i => i.Organisation));
        }

        [HttpGet("{id}/Sites")]
        public IEnumerable<Site> GetSites(Guid id)
        {
            return GetMany(id, sel => sel.OrganisationSites.Select(i => i.Site), inc => inc.OrganisationSites.Select(i => i.Organisation));
        }

        [HttpGet("{id}/Stations")]
        public IEnumerable<Station> GetStations(Guid id)
        {
            return GetMany(id, sel => sel.OrganisationStations.Select(i => i.Station), inc => inc.OrganisationStations.Select(i => i.Organisation));
        }

    }
}