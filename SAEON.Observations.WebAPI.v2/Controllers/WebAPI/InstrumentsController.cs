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
    [Route("api/Instruments")]
    public class InstrumentsController : BaseAPIController<Instrument>
    {
        public InstrumentsController(ObservationsDbContext context) : base(context) { }

        protected override IQueryable<Instrument> ApplyIncludes(IQueryable<Instrument> query)
        {
            return query
                .Include(i => i.OrganisationInstruments)
                    .ThenInclude(i => i.Organisation)
                .Include(i => i.StationInstruments)
                    .ThenInclude(i => i.Instrument);
        }

        [HttpGet("{id}/Organisations")]
        public IEnumerable<Organisation> GetOrganisations(Guid id)
        {
            return GetMany(id, sel => sel.OrganisationInstruments.Select(i => i.Organisation), inc => inc.OrganisationInstruments.Select(i => i.Instrument));
            //return GetMany(id, sel => sel.OrganisationInstruments.Select(i => i.Organisation), "OrganisationInstruments.Organisation");
        }

        [HttpGet("{id}/Stations")]
        public IEnumerable<Station> GetStations(Guid id)
        {
            return GetMany(id, sel => sel.StationInstruments.Select(i => i.Station), inc => inc.StationInstruments.Select(i => i.Instrument));
            //return GetMany(id, sel => sel.StationInstruments.Select(i => i.Station), "StationInstruments.Station");
        }

    }
}