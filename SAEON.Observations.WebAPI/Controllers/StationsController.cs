using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Controllers;
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
    /// Stations
    /// </summary>
    [RoutePrefix("Stations")]
    public class StationsController : BaseApiController<Station>
    {
        /// <summary>
        /// Return a list of Stations
        /// </summary>
        /// <returns>A list of Station</returns>
        [ResponseType(typeof(List<Station>))]
        public override async Task<IHttpActionResult> GetAll()
        {
            return await base.GetAll();
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
        [ResponseType(typeof(UserQuery))]
        public override async Task<IHttpActionResult> GetByName(string name)
        {
            return await base.GetByName(name);
        }

    }
}
