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
    public class OfferingsController : CodedEntityController<OfferingsController, Offering>
    {
        protected override IQueryable<Offering> AddIncludes(IQueryable<Offering> query)
        {
            return base.AddIncludes(query)
                .Include(s => s.PhenomenonOfferings).ThenInclude(po => po.Phenomenon);
        }

        /// <summary>
        /// All Offerings
        /// </summary>
        /// <returns>List&lt;Offering&gt;</returns>
        public override Task<List<Offering>> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Offering by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Offering</returns>
        public override Task<ActionResult<Offering>> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        /// <summary>
        /// Offering by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Offering</returns>
        public override Task<ActionResult<Offering>> GetByNameAsync(string name)
        {
            return base.GetByNameAsync(name);
        }

        /// <summary>
        /// Offering by Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Offering</returns>
        public override Task<ActionResult<Offering>> GetByCodeAsync(string code)
        {
            return base.GetByCodeAsync(code);
        }

        /// <summary>
        /// Phenomena linked to this Offering
        /// </summary>
        /// <param name="id">Id of the Offering</param>
        /// <returns>List&lt;Phenomenon&gt;</returns>
        [HttpGet("{id:guid}/Phenomena")]
        public async Task<List<Phenomenon>> GetPenomenaAsync(Guid id)
        {
            return await GetManyAsync(id, s => s.PhenomenonOfferings.Select(po => po.Phenomenon));
        }
    }
}