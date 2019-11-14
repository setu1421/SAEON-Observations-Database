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
    public class UnitsController : CodedEntityController<UnitsController, Unit>
    {
        protected override IQueryable<Unit> AddIncludes(IQueryable<Unit> query)
        {
            return base.AddIncludes(query)
                .Include(s => s.PhenomenonUnits).ThenInclude(pu => pu.Phenomenon);
        }

        /// <summary>
        /// All Units
        /// </summary>
        /// <returns>List&lt;Unit&gt;</returns>
        public override Task<List<Unit>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Unit by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Unit</returns>
        public override Task<ActionResult<Unit>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Unit by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Unit</returns>
        public override Task<ActionResult<Unit>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Unit by Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Unit</returns>
        public override Task<ActionResult<Unit>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Phenomena linked to this Unit
        /// </summary>
        /// <param name="id">Id of the Unit</param>
        /// <returns>List&lt;Phenomenon&gt;</returns>
        [HttpGet("{id:guid}/Phenomena")]
        public async Task<List<Phenomenon>> GetPenomenaAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.PhenomenonUnits.Select(po => po.Phenomenon));
        }
    }
}