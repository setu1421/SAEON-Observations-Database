using AutoMapper;
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
    [RoutePrefix("UserQueries")]
    [Authorize(Roles = "Administrators,DataReaders")]
    public class UserQueriesController : ApiController
    {
        ObservationsDbContext db = null;

        /// <summary>
        /// UserQueries constructor
        /// </summary>
        public UserQueriesController()
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
        [ResponseType(typeof(List<UserQueryDTO>))]
        public async Task<IHttpActionResult> Get()
        {
            using (LogContext.PushProperty("Method", "Get"))
            {
                try
                {
                    return Ok(await db.UserQueries.Where(i => i.UserId == User.Identity.GetUserId()).OrderBy(i => i.Name).ToListAsync());
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get");
                    throw;
                }
            }
        }

        /// <summary>
        /// Return a UserQuery for for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        [ResponseType(typeof(UserQueryDTO))]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            using (LogContext.PushProperty("Method", "Get"))
            {
                try
                {
                    var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
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
        /// Return a UserQuery for for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserQuery</param>
        /// <returns></returns>
        [Route("{name}")]
        [ResponseType(typeof(UserQueryDTO))]
        public async Task<IHttpActionResult> GetByName(string name)
        {
            using (LogContext.PushProperty("Method", "GetByName"))
            {
                try
                {
                    var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
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

        /// <summary>
        /// Create a UserQuery
        /// </summary>
        /// <param name="itemDTO">The UserQuery to be created</param>
        [Route]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(UserQueryDTO))]
        public async Task<IHttpActionResult> Post([FromBody]UserQueryDTO itemDTO)
        {
            using (LogContext.PushProperty("Method", "Post"))
            {
                try
                {
                    Log.Verbose("Adding {itemDTO.Name} {@itemDTO}", itemDTO);
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{itemDTO.Name} ModelState.Invalid", itemDTO);
                        return BadRequest(ModelState);
                    }
                    try
                    {
                        itemDTO.UserId = User.Identity.GetUserId();
                        db.UserQueries.Add(Mapper.Map<UserQueryDTO, UserQuery>(itemDTO));
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        if (!db.UserQueries.Any(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == itemDTO.Id)))
                        {
                            Log.Error("{itemDTO.Name} conflict", itemDTO);
                            return Conflict();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return CreatedAtRoute("UserQueriesA", new { id = itemDTO.Id }, itemDTO);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to add {itemDTO.Name}", itemDTO);
                    throw;
                }
            }
        }

        /// <summary>
        /// Update a UserQuery for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        /// <param name="itemDTO">The UserQuery values to be updated</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutById(Guid id, [FromBody]UserQueryDTO itemDTO)
        {
            using (LogContext.PushProperty("Method", "PutById"))
            {
                try
                {
                    Log.Verbose("Updating {id} {@itemDTO}", id, itemDTO);
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{id} ModelState.Invalid", id);
                        return BadRequest(ModelState);
                    }
                    if (id != itemDTO.Id)
                    {
                        Log.Error("{id} Ids not same", id);
                        return BadRequest();
                    }
                    var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} not found", id);
                        return NotFound();
                    }
                    Mapper.Map<UserQueryDTO, UserQuery>(itemDTO, item);
                    await db.SaveChangesAsync();
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
        /// <param name="itemDTO">The UserQuery values to be updated</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutByName(string name, [FromBody]UserQueryDTO itemDTO)
        {
            using (LogContext.PushProperty("Method", "PutByName"))
            {
                try
                {
                    Log.Verbose("Updating {name} {@itemDTO}", name, itemDTO);
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{name} ModelState.Invalid", name);
                        return BadRequest(ModelState);
                    }
                    if (name != itemDTO.Name)
                    {
                        Log.Error("{name} names not same", name);
                        return BadRequest();
                    }
                    var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
                    if (item == null)
                    {
                        Log.Error("{name} not found", name);
                        return NotFound();
                    }
                    Mapper.Map<UserQueryDTO, UserQuery>(itemDTO, item);
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
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteByIdAsync(Guid id)
        {
            using (LogContext.PushProperty("Method", "DeleteById"))
            {
                try
                {
                    Log.Verbose("Deleting {id}", id);
                    var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} not found");
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

        /// <summary>
        /// Delete a UserQuery for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserQuery</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteByNameAsync(string name)
        {
            using (LogContext.PushProperty("Method", "DeleteByName"))
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
    }
}