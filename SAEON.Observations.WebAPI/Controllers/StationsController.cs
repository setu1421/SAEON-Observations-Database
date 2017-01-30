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
    /// Stations
    /// </summary>
    [RoutePrefix("Stations")]
    [Authorize(Roles = "Administrators,DataReaders")]
    public class StationsController : ApiController
    {
        ObservationsDbContext db = null;

        /// <summary>
        /// Stations construtor
        /// </summary>
        public StationsController()
        {
            db = new ObservationsDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Return all Stations
        /// </summary>
        /// <returns></returns>
        [Route]
        [ResponseType(typeof(List<StationDTO>))]
        public async Task<IHttpActionResult> Get()
        {
            using (LogContext.PushProperty("Method", "Get"))
            {
                try
                {
                    return Ok(await db.Stations.OrderBy(i => i.Name).ToListAsync());
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get");
                    throw;
                }
            }
        }

        /// <summary>
        /// Return a Station
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        [ResponseType(typeof(StationDTO))]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            using (LogContext.PushProperty("Method", "Get"))
            {
                try
                {
                    var item = await db.Stations.FirstOrDefaultAsync(i => (i.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} not found", id);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Return a Station
        /// </summary>
        /// <param name="name">The name of the Station</param>
        /// <returns></returns>
        [Route("{name}")]
        [ResponseType(typeof(UserQueryDTO))]
        public async Task<IHttpActionResult> GetByName(string name)
        {
            using (LogContext.PushProperty("Method", "GetByName"))
            {
                try
                {
                    var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.Name == name));
                    if (item == null)
                    {
                        Log.Error("{name} not found", name);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {name}", name);
                    throw;
                }
            }
        }

    }
}
