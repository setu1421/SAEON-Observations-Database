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
using Serilog.Context;
using Serilog;
using Microsoft.AspNet.Identity;

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
    [ODataRoutePrefix("UserQueries")]
    [Authorize(Roles = "Administrators,DataReaders")]
    public class UserQueriesODataController : ODataController
    {
        private ObservationsDbContext db = new ObservationsDbContext();

        // GET: odata/UserQueriesOData
        [EnableQuery]
        [ODataRoute]
        public IQueryable<UserQuery> GetUserQueries()
        {
            using (LogContext.PushProperty("Method", "Get"))
            {
                try
                {
                    return db.UserQueries.Where(i => (i.UserId == User.Identity.GetUserId())).OrderBy(i => i.Name);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get");
                    throw;
                }
            }
        }

        // GET: odata/UserQueriesOData(5)
        [EnableQuery]
        [ODataRoute("({id})")]
        public SingleResult<UserQuery> GetUserQueryById([FromODataUri] Guid id)
        {
            using (LogContext.PushProperty("Method", "GetUserQueryById"))
            {
                try
                {
                    return SingleResult.Create(db.UserQueries.Where(i => (i.UserId == User.Identity.GetUserId()) && i.Id == id));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        [EnableQuery]
        [ODataRoute("({name})")]
        public SingleResult<UserQuery> GetUserQueryByName([FromODataUri] string name)
        {
            using (LogContext.PushProperty("Method", "GetUserQueryByName"))
            {
                try
                {
                    return SingleResult.Create(db.UserQueries.Where(i => (i.UserId == User.Identity.GetUserId()) && i.Name == name));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {name}", name);
                    throw;
                }
            }
        }

        // POST: odata/UserQueriesOData
        [ODataRoute]
        [Authorize(Roles = "QuerySite")]
        public async Task<IHttpActionResult> Post(UserQuery item)
        {
            using (LogContext.PushProperty("Method", "Post"))
            {
                try
                {
                    Log.Verbose("Adding {item.Name} {@item}", item);
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{item.Name} ModelState.Invalid", item);
                        return BadRequest(ModelState);
                    }
                    if (item.UserId != User.Identity.GetUserId())
                    {
                        Log.Error("{item.Name} invalid user", item);
                        return BadRequest();
                    }
                    item.UserId = User.Identity.GetUserId();
                    db.UserQueries.Add(item);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        if (db.UserQueries.Any(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == item.Id)))
                        {
                            Log.Error("{item.Name} conflict", item);
                            return Conflict();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return Created(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to add {item.Name}", item);
                    throw;
                }
            }
        }

        // PUT: odata/UserQueriesOData(5)
        [ODataRoute("({id})")]
        [Authorize(Roles = "QuerySite")]
        public async Task<IHttpActionResult> PutById([FromODataUri] Guid id, Delta<UserQuery> patch)
        {
            using (LogContext.PushProperty("Method", "PutById"))
            {
                try
                {
                    Log.Verbose("Updating {id} {@patch}", id, patch);
                    Validate(patch.GetEntity());
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{id} ModelState.Invalid", id);
                        return BadRequest(ModelState);
                    }
                    var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} not found", id);
                        return NotFound();
                    }
                    patch.Put(item);
                    await db.SaveChangesAsync();
                    return Updated(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to update {id}", id);
                    throw;
                }
            }
        }

        [ODataRoute("({name})")]
        [Authorize(Roles = "QuerySite")]
        public async Task<IHttpActionResult> PutByName([FromODataUri] string name, Delta<UserQuery> patch)
        {
            using (LogContext.PushProperty("Method", "PutByName"))
            {
                try
                {
                    Log.Verbose("Updating {name} {@patch}", name, patch);
                    Validate(patch.GetEntity());
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{name} ModelState.Invalid", name);
                        return BadRequest(ModelState);
                    }
                    var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
                    if (item == null)
                    {
                        Log.Error("{name} not found", name);
                        return NotFound();
                    }
                    patch.Put(item);
                    await db.SaveChangesAsync();
                    return Updated(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to update {name}", name);
                    throw;
                }
            }
        }

        // PATCH: odata/UserQueriesOData(5)
        [AcceptVerbs("PATCH", "MERGE")]
        [ODataRoute("({id})")]
        [Authorize(Roles = "QuerySite")]
        public async Task<IHttpActionResult> PatchById([FromODataUri] Guid id, Delta<UserQuery> patch)
        {
            using (LogContext.PushProperty("Method", "PatchById"))
            {
                try
                {
                    Validate(patch.GetEntity());
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{id} ModelState.Invalid", id);
                        return BadRequest(ModelState);
                    }
                    var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} not found", id);
                        return NotFound();
                    }
                    patch.Patch(item);
                    await db.SaveChangesAsync();
                    return Updated(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to patch {id}", id);
                    throw;
                }
            }
        }

        [AcceptVerbs("PATCH", "MERGE")]
        [ODataRoute("({name})")]
        [Authorize(Roles = "QuerySite")]
        public async Task<IHttpActionResult> PatchByName([FromODataUri] string name, Delta<UserQuery> patch)
        {
            using (LogContext.PushProperty("Method", "PatchByName"))
            {
                try
                {
                    Validate(patch.GetEntity());
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{name} ModelState.Invalid", name);
                        return BadRequest(ModelState);
                    }
                    var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
                    if (item == null)
                    {
                        Log.Error("{name} not found", name);
                        return NotFound();
                    }
                    patch.Patch(item);
                    await db.SaveChangesAsync();
                    return Updated(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to patch {name}", name);
                    throw;
                }
            }
        }

        // DELETE: odata/UserQueriesOData(5)
        [ODataRoute("({id})")]
        [Authorize(Roles = "QuerySite")]
        public async Task<IHttpActionResult> DeleteById([FromODataUri] Guid id)
        {
            using (LogContext.PushProperty("Method", "DeleteById"))
            {
                try
                {
                    Log.Verbose("Deleting {id}", id);
                    var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} not found", id);
                        return NotFound();
                    }
                    db.UserQueries.Remove(item);
                    await db.SaveChangesAsync();
                    return StatusCode(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to delete {id}", id);
                    throw;
                }
            }
        }

        [ODataRoute("({name})")]
        [Authorize(Roles = "QuerySite")]
        public async Task<IHttpActionResult> DeleteByName([FromODataUri] string name)
        {
            using (LogContext.PushProperty("Method", "DeleteById"))
            {
                try
                {
                    Log.Verbose("Deleting {name}", name);
                    var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
                    if (item == null)
                    {
                        Log.Error("{name} not found", name);
                        return NotFound();
                    }
                    db.UserQueries.Remove(item);
                    await db.SaveChangesAsync();
                    return StatusCode(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to delete {name}", name);
                    throw;
                }
            }
        }

        // GET: odata/UserQueriesOData(5)/User
        [EnableQuery]
        public SingleResult<ApplicationUser> GetUser([FromODataUri] Guid id)
        {
            using (LogContext.PushProperty("Method", "PutByName"))
            {
                try
                {
                    return SingleResult.Create(db.UserQueries.Where(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id)).Select(i => i.User));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
