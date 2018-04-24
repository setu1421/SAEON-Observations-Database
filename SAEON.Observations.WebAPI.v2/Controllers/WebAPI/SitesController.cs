using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.WebAPI.Controllers.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAEON.Observations.WebAPI.v2.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/Sites")]
    public class SitesController : BaseAPIController<Site>
    {
        public SitesController(ObservationsDbContext context) : base(context) { }

        protected override IQueryable<Site> ApplyIncludes(IQueryable<Site> query)
        {
            return query
                .Include(i => i.OrganisationSites)
                    .ThenInclude(i => i.Organisation)
                .Include(i => i.Stations);
        }

        [HttpGet("{id}/Organisations")]
        public IEnumerable<Organisation> GetOrganisations(Guid id)
        {
            return GetMany(id, sel => sel.OrganisationSites.Select(i => i.Organisation), inc => inc.OrganisationSites.Select(i => i.Site));
        }

        [HttpGet("{id}/Stations")]
        public IEnumerable<Station> GetStations(Guid id)
        {
            return GetMany(id, sel => sel.Stations, inc => inc.Site);
        }

    }
}