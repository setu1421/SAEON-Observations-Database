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
    public class PhenomenonaController : CodedEntityController<PhenomenonaController, Phenomenon>
    {
        protected override IQueryable<Phenomenon> AddIncludes(IQueryable<Phenomenon> query)
        {
            return base.AddIncludes(query)
                .Include(s => s.PhenomenonOfferings).ThenInclude(po => po.Offering)
                .Include(s => s.PhenomenonUnits).ThenInclude(pu => pu.Unit)
                .Include(s => s.Sensors);
        }

        /// <summary>
        /// All Phenomenona
        /// </summary>
        /// <returns>List&lt;Phenomenon&gt;</returns>
        public override Task<List<Phenomenon>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Phenomenon by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Phenomenon</returns>
        public override Task<ActionResult<Phenomenon>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Phenomenon by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Phenomenon</returns>
        public override Task<ActionResult<Phenomenon>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Phenomenon by Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Phenomenon</returns>
        public override Task<ActionResult<Phenomenon>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Offerings linked to this Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>List&lt;Offering&gt;</returns>
        [HttpGet("{id:guid}/Offerings")]
        public async Task<List<Offering>> GetOfferingsAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.PhenomenonOfferings.Select(oi => oi.Offering));
        }

        /// <summary>
        /// Units linked to this Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>List&lt;Unit&gt;</returns>
        [HttpGet("{id:guid}/Units")]
        public async Task<List<Unit>> GetUnitsAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.PhenomenonUnits.Select(oi => oi.Unit));
        }

        /// <summary>
        /// Sensors linked to this Phenomenon
        /// </summary>
        /// <param name="id">Id of the Phenomenon</param>
        /// <returns>List&lt;Sensor&gt;</returns>
        [HttpGet("{id:guid}/Sensors")]
        public async Task<List<Sensor>> GetSensorsAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.Sensors);
        }

    }
}