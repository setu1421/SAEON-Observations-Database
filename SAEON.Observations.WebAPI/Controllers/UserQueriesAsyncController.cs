using Microsoft.AspNet.Identity;
using Microsoft.Web.Http;
using SAEON.Observations.Core;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Logged in users can save frequently used queries in the QuerySite for later use
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("UserQueriesAsync")]
    [Authorize(Roles = "Administrators,DataReaders")]
    public class UserQueriesAsyncController : ApiController
    {
        ObservationsDbContext db = null;

        public UserQueriesAsyncController()
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
        /// Return all UserQueries for the logged in user
        /// </summary>
        /// <returns></returns>
        [Route]
        [ResponseType(typeof(List<UserQuery>))]
        public async Task<IHttpActionResult> GetAsync()
        {
            return Ok(await db.UserQueries.Where(d => d.UserId == User.Identity.GetUserId()).ToListAsync());
        }

        /// <summary>
        /// Return a UserQuery for for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        [ResponseType(typeof(UserQuery))]
        public async Task<IHttpActionResult> GetAsync(Guid id)
        {
            var item = await db.UserQueries.FirstOrDefaultAsync(d => (d.UserId == User.Identity.GetUserId()) && (d.Id == id));
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        /// <summary>
        /// Return a UserQuery for for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserQuery</param>
        /// <returns></returns>
        [Route("{name}")]
        [ResponseType(typeof(UserQuery))]
        public async Task<IHttpActionResult> GetByNameAsync(string name)
        {
            var item = await db.UserQueries.FirstOrDefaultAsync(d => (d.UserId == User.Identity.GetUserId()) && (d.Name == name));
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        /// <summary>
        /// Create a UserQuery
        /// </summary>
        /// <param name="item">The UserQuery to be created</param>
        [Route]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(UserQuery))]
        public async Task<IHttpActionResult> PostAsync([FromBody]UserQuery item)
        {
            using (LogContext.PushProperty("Method", "PostAsync"))
            {
                try
                {
                    Log.Verbose("Adding {item.Name} {@Item}", item);
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{item.Name} ModelState.Invalid",item);
                        return BadRequest(ModelState);
                    }
                    try
                    {
                        db.UserQueries.Add(item);
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        if (!db.UserQueries.Any(i => i.Id == item.Id))
                        {
                            Log.Error("{item.Name} Conflict",item);
                            return Conflict();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return CreatedAtRoute("UserQueriesAsync", new { id = item.Id }, item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to add {item.Name}",item);
                    throw;
                }
            }
        }

        /// <summary>
        /// Update a UserQuery for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        /// <param name="item">The UserQuery values to be updated</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutByIdAsync(Guid id, [FromBody]UserQuery item)
        {
            using (LogContext.PushProperty("Method", "PutByIdAsync"))
            {
                try
                {
                    Log.Verbose("Updating {id} to {@item}", id, item);
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{id} ModelState.Invalid",id);
                        return BadRequest(ModelState);
                    }
                    if (id != item.Id)
                    {
                        Log.Error("{id} Ids not same",id);
                        return BadRequest();
                    }
                    try
                    {
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        if (!db.UserQueries.Any(i => i.Id == item.Id))
                        {
                            Log.Error("{id} Not found",id);
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return StatusCode(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to update {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Update a UserQuery for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserQuery</param>
        /// <param name="item">The UserQuery values to be updated</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutByNameAsync(string name, [FromBody]UserQuery item)
        {
            using (LogContext.PushProperty("Method", "PutByNameAsync"))
            {
                try
                {
                    Log.Verbose("Updating {name} to {@item}", name);
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{name} ModelState.Invalid", name);
                        return BadRequest(ModelState);
                    }
                    if (name != item.Name)
                    {
                        Log.Error("{name} Names not same", name);
                        return BadRequest();
                    }
                    try
                    {
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        if (!db.UserQueries.Any(i => i.Name == item.Name))
                        {
                            Log.Error("{name} Not found", name);
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return StatusCode(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to update {name}", name);
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete a UserQuery for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(UserQuery))]
        public async Task<IHttpActionResult> DeleteByIdAsync(Guid id)
        {
            using (LogContext.PushProperty("Method", "DeleteByIdAsync"))
            {
                try
                {
                    Log.Verbose("Deleting {id}", id);
                    var item = await db.UserQueries.FirstOrDefaultAsync(d => (d.UserId == User.Identity.GetUserId()) && (d.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} Not found");
                        return NotFound();
                    }
                    db.UserQueries.Remove(item);
                    await db.SaveChangesAsync();
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to delete {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete a UserQuery for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserQuery</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(UserQuery))]
        public async Task<IHttpActionResult> DeleteByNameAsync(string name)
        {
            using (LogContext.PushProperty("Method", "DeleteByNameAsync"))
            {
                try
                {
                    Log.Verbose("Deleting {name}", name);
                    var item = await db.UserQueries.FirstOrDefaultAsync(d => (d.UserId == User.Identity.GetUserId()) && (d.Name == name));
                    if (item == null)
                    {
                        Log.Error("{name} Not found", name);
                        return NotFound();
                    }
                    db.UserQueries.Remove(item);
                    await db.SaveChangesAsync();
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to delete {name}", name);
                    throw;
                }
            }
        }
    }
}