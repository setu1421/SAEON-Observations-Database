using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.WebAPI.Controllers.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.v2.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/Stations")]
    public class StationsController : BaseAPIController<Station>
    {
        public StationsController(ObservationsDbContext context) : base(context) { }

        protected override List<Expression<Func<Station, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Site);
            //list.Add(i => i.Instruments);
            return list;
        }

        [HttpGet("{id}/Site")]
        public async Task<IActionResult> GetSite(Guid id)
        {
            return await GetSingle(id, sel => sel.Site, inc => inc.Stations);
        }

    }
}