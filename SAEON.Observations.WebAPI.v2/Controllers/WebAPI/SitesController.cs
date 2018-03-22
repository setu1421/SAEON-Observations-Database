using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Core.Entities;
using SAEON.Observations.WebAPI.Controllers.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.v2.Controllers.WebAPI
{
    [Produces("application/json")]
    [Route("api/Sites")]
    public class SitesController : BaseAPIController<Site>
    {
        public SitesController(ObservationsDbContext context) : base(context) { }

        protected override List<Expression<Func<Site, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            //list.Add(i => i.Organisations);
            list.Add(i => i.Stations);
            return list;
        }

        [HttpGet("{id}/Stations")]
        public IEnumerable<Station> GetStations(Guid id)
        {
            return GetMany(id, sel => sel.Stations, inc => inc.Site);
        }

    }
}