using SAEON.Observations.Core;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Sites
    /// </summary>
    [RoutePrefix("Sites")]
    public class SitesApiController : BaseApiController<Site>
    {
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

    }
}
