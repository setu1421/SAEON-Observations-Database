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
    builder.EntitySet<UserQuery>("UserQueriesOData");
    builder.EntitySet<ApplicationUser>("Users"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UserQueriesODataController : ODataController
    {
        private ObservationsDbContext db = new ObservationsDbContext();

        // GET: odata/UserQueriesOData
        [EnableQuery]
        public IQueryable<UserQuery> GetUserQueries()
        {
            return db.UserQueries;
        }

        // GET: odata/UserQueriesOData(5)
        [EnableQuery]
        public SingleResult<UserQuery> GetUserQuery([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.UserQueries.Where(userQuery => userQuery.Id == key));
        }

        // PUT: odata/UserQueriesOData(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Delta<UserQuery> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserQuery userQuery = await db.UserQueries.FindAsync(key);
            if (userQuery == null)
            {
                return NotFound();
            }

            patch.Put(userQuery);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserQueryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(userQuery);
        }

        // POST: odata/UserQueriesOData
        public async Task<IHttpActionResult> Post(UserQuery userQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UserQueries.Add(userQuery);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserQueryExists(userQuery.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(userQuery);
        }

        // PATCH: odata/UserQueriesOData(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<UserQuery> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserQuery userQuery = await db.UserQueries.FindAsync(key);
            if (userQuery == null)
            {
                return NotFound();
            }

            patch.Patch(userQuery);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserQueryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(userQuery);
        }

        // DELETE: odata/UserQueriesOData(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            UserQuery userQuery = await db.UserQueries.FindAsync(key);
            if (userQuery == null)
            {
                return NotFound();
            }

            db.UserQueries.Remove(userQuery);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/UserQueriesOData(5)/User
        [EnableQuery]
        public SingleResult<ApplicationUser> GetUser([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.UserQueries.Where(m => m.Id == key).Select(m => m.User));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserQueryExists(Guid key)
        {
            return db.UserQueries.Any(e => e.Id == key);
        }
    }
}
