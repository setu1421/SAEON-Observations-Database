using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI.v2.Controllers.WebApi
{
    public class SensorsController : CodedEntityController<SensorsController, Sensor>
    {
        protected override IQueryable<Sensor> AddIncludes(IQueryable<Sensor> query)
        {
            return base.AddIncludes(query)
                .Include(s => s.InstrumentSensors).ThenInclude(_is => _is.Instrument)
                .Include(s => s.Phenomenon);
        }

        /// <summary>
        /// All Sensors
        /// </summary>
        /// <returns>List&lt;Sensor&gt;</returns>
        public override Task<List<Sensor>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Sensor by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Sensor</returns>
        public override Task<ActionResult<Sensor>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Sensor by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Sensor</returns>
        public override Task<ActionResult<Sensor>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Sensor by Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Sensor</returns>
        public override Task<ActionResult<Sensor>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Instruments linked to this Sensor
        /// </summary>
        /// <param name="id">Id of the Sensor</param>
        /// <returns>List&lt;Instrument&gt;</returns>
        [HttpGet("{id:guid}/Instruments")]
        public async Task<List<Instrument>> GetInstrumentsAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.InstrumentSensors.Select(oi => oi.Instrument));
        }

        /// <summary>
        /// This Sensor's Phenomenon
        /// </summary>
        /// <param name="id">Id of the Sensor</param>
        /// <returns>Phenomenon</returns>
        [HttpGet("{id:guid}/Phenomenon")]
        public async Task<ActionResult<Phenomenon>> GetPhenomenonAsync(Guid id)
        {
            return await GetSingleAsync(id, s => s.Phenomenon);
        }
    }
}