using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.WebAPI.v2.Controllers.WebApi;

namespace SAEON.Observations.WebAPI.Controllers.WebApi
{
    public class InstrumentsController : CodedEntityController<InstrumentsController, Instrument>
    {
        protected override IQueryable<Instrument> AddIncludes(IQueryable<Instrument> query)
        {
            return base.AddIncludes(query)
                .Include(i => i.OrganisationInstruments).ThenInclude(oi => oi.Organisation)
                .Include(i => i.StationInstruments).ThenInclude(si => si.Station)
                .Include(i => i.InstrumentSensors).ThenInclude(i => i.Sensor);
        }

        /// <summary>
        /// All Instruments
        /// </summary>
        /// <returns>List&lt;Instrument&gt;</returns>
        public override Task<List<Instrument>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Instrument by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Instrument</returns>
        public override Task<ActionResult<Instrument>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Instrument by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Instrument</returns>
        public override Task<ActionResult<Instrument>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Instrument by Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Instrument</returns>
        public override Task<ActionResult<Instrument>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Organisations linked to this Instrument
        /// </summary>
        /// <param name="id">Id of the Instrument</param>
        /// <returns>List&lt;Organisation&gt;</returns>
        [HttpGet("{id:guid}/Organisations")]
        public async Task<List<Organisation>> GetOrganisationsAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.OrganisationInstruments.Select(oi => oi.Organisation));
        }

        /// <summary>
        /// Stations linked to this Instrument
        /// </summary>
        /// <param name="id">Id of the Instrument</param>
        /// <returns>List&lt;Station&gt;</returns>
        [HttpGet("{id:guid}/Stations")]
        public async Task<List<Station>> GetStationsAsync(Guid id)
        {
            return await GetManyAsync<Station>(id, s => s.StationInstruments.Select(si => si.Station));
        }

        /// <summary>
        /// Sensors linked to this Instrument
        /// </summary>
        /// <param name="id">Id of the Instrument</param>
        /// <returns>List&lt;Sensor&gt;</returns>
        [HttpGet("{id:guid}/Sensors")]
        public async Task<List<Sensor>> GetSensorsAsync(Guid id)
        {
            return await GetManyAsync<Sensor>(id, s => s.InstrumentSensors.Select(i => i.Sensor));
        }
    }
}
