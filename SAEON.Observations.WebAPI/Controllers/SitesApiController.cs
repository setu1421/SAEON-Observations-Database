using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.Controllers
{

    /// <summary>
    /// Sites
    /// </summary>
    [RoutePrefix("Sites")]
    public class SitesApiController : BaseApiController<Site>
    {
        protected override List<Expression<Func<Site, object>>> GetIncludes()
        {
            var list = base.GetIncludes();
            list.Add(i => i.Stations);
            return list;
        }

        /// <summary>
        /// Return a list of Sites
        /// </summary>
        /// <returns>A list of Site</returns>
        public override IQueryable<Site> GetAll()
        {
            return base.GetAll();
        }

        /// <summary>
        /// Return a Site by Id
        /// </summary>
        /// <param name="id">The Id of the Site</param>
        /// <returns>Site</returns>
        [ResponseType(typeof(Site))]
        public override async Task<IHttpActionResult> GetById(Guid id)
        {
            return await base.GetById(id);
        }

        /// <summary>
        /// Return a Site by Name
        /// </summary>
        /// <param name="name">The Name of the Site</param>
        /// <returns>Site</returns>
        [ResponseType(typeof(Site))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

        //GET: Sites/5/Stations
        /// <summary>
        /// Return a list of Stations for the Site
        /// </summary>
        /// <param name="id">Id of Site</param>
        /// <returns>List of Station</returns>
        [Route("{id:guid}/Stations")]
        public IQueryable<Station> GetStations([FromUri] Guid id)
        {
            return GetMany<Station>(id, s => s.Stations, i => i.Site);
        }

    }
}
