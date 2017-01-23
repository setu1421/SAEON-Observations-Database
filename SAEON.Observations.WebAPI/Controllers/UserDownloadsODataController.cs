using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using SAEON.Observations.Core;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using SAEON.Observations.Core;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<UserDownload>("UserDownloadsOData");
    builder.EntitySet<ApplicationUser>("Users"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UserDownloadsODataController : ODataController
    {
        private ObservationsDbContext db = new ObservationsDbContext();

        // GET: odata/UserDownloadsOData
        [EnableQuery]
        public IQueryable<UserDownload> GetUserDownloads()
        {
            return db.UserDownloads;
        }

        // GET: odata/UserDownloadsOData(5)
        [EnableQuery]
        public SingleResult<UserDownload> GetUserDownload([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.UserDownloads.Where(userDownload => userDownload.Id == key));
        }

        // PUT: odata/UserDownloadsOData(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Delta<UserDownload> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserDownload userDownload = await db.UserDownloads.FindAsync(key);
            if (userDownload == null)
            {
                return NotFound();
            }

            patch.Put(userDownload);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDownloadExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(userDownload);
        }

        // POST: odata/UserDownloadsOData
        public async Task<IHttpActionResult> Post(UserDownload userDownload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UserDownloads.Add(userDownload);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserDownloadExists(userDownload.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(userDownload);
        }

        // PATCH: odata/UserDownloadsOData(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<UserDownload> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserDownload userDownload = await db.UserDownloads.FindAsync(key);
            if (userDownload == null)
            {
                return NotFound();
            }

            patch.Patch(userDownload);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDownloadExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(userDownload);
        }

        // DELETE: odata/UserDownloadsOData(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            UserDownload userDownload = await db.UserDownloads.FindAsync(key);
            if (userDownload == null)
            {
                return NotFound();
            }

            db.UserDownloads.Remove(userDownload);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/UserDownloadsOData(5)/User
        [EnableQuery]
        public SingleResult<ApplicationUser> GetUser([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.UserDownloads.Where(m => m.Id == key).Select(m => m.User));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserDownloadExists(Guid key)
        {
            return db.UserDownloads.Any(e => e.Id == key);
        }
    }
}
