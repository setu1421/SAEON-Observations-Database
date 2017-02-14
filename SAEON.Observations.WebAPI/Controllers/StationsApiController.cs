using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Controllers;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Stations
    /// </summary>
    [RoutePrefix("Stations")]
    public class StationsApiController : BaseApiController<Station>
    {
        protected override List<Expression<Func<Station, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Site);
            list.Add(i => i.Instruments);
            return list;
        }

        /// <summary>
        /// Return a list of Stations
        /// </summary>
        /// <returns>A list of Station</returns>
        public override IQueryable<Station> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Return a Station by Id
        /// </summary>
        /// <param name="id">The Id of the Station</param>
        /// <returns>Station</returns>
        [ResponseType(typeof(Station))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Return a Station by Name
        /// </summary>
        /// <param name="name">The Name of the Station</param>
        /// <returns>Station</returns>
        [ResponseType(typeof(Station))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

        // GET: Stations/5/Site
        /// <summary>
        /// Return the Site of a Station
        /// </summary>
        /// <param name="id">The Id of the Station</param>
        /// <returns>Site</returns>
        [Route("{id:guid}/Site")]
        [ResponseType(typeof(Site))]
        public async Task<IHttpActionResult> GetSite(Guid id)
        {
            return await GetSingle<Site>(id, s => s.Site, i => i.Stations);
        }

        // GET: Stations/5/Instruments
        [Route("{id:guid}/Instruments")]
        public IQueryable<Instrument> GetInstruments([FromUri] Guid id)
        {
            return GetMany<Instrument>(id, s => s.Instruments, i => i.Stations);
        }
    }
}
