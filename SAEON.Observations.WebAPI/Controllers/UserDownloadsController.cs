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
    /// Users have to be logged in to download data in the QuerySite. Any downloads are saved for later re-downloads.
    /// </summary>
    [ApiVersion("1.0")]
    [RoutePrefix("UserDownloads")]
    [Authorize(Roles = "Administrators,DataReaders")]
    public class UserDownloadsController : ApiController
    {
        ObservationsDbContext db = null;

        /// <summary>
        /// UserDownloads constructor
        /// </summary>
        public UserDownloadsController()
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
        /// Return all UserDownloads for the logged in user
        /// </summary>
        /// <returns></returns>
        [Route]
        [ResponseType(typeof(List<UserDownloadDTO>))]
        public async Task<IHttpActionResult> GetAll()
        {
            using (LogContext.PushProperty("Method", "GetAll"))
            {
                try
                {
                    return Ok(await db.UserDownloads.Where(i => i.UserId == User.Identity.GetUserId()).OrderBy(i => i.Name).ToListAsync());
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get all");
                    throw;
                }
            }
        }

        /// <summary>
        /// Return a UserDownload for for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserDownload</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        [ResponseType(typeof(UserDownloadDTO))]
        public async Task<IHttpActionResult> GetById(Guid id)
        {
            using (LogContext.PushProperty("Method", "GetById"))
            {
                try
                {
                    var item = await db.UserDownloads.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
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
        /// Return a UserDownload for for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserDownload</param>
        /// <returns></returns>
        [Route("{name}")]
        [ResponseType(typeof(UserDownloadDTO))]
        public async Task<IHttpActionResult> GetByName(string name)
        {
            using (LogContext.PushProperty("Method", "GetByName"))
            {
                try
                {
                    var item = await db.UserDownloads.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
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
        /// Create a UserDownload
        /// </summary>
        /// <param name="itemDTO">The UserDownload to be created</param>
        [Route]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(UserDownloadDTO))]
        public async Task<IHttpActionResult> Post([FromBody]UserDownloadDTO itemDTO)
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
                    if (itemDTO.UserId != User.Identity.GetUserId())
                    {
                        Log.Error("{itemDTO.Name} invalid user", itemDTO);
                        return BadRequest();
                    }
                    try
                    {
                        itemDTO.UserId = User.Identity.GetUserId();
                        db.UserDownloads.Add(Mapper.Map<UserDownloadDTO, UserDownload>(itemDTO));
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        if (!db.UserDownloads.Any(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == itemDTO.Id)))
                        {
                            Log.Error("{itemDTO.Name} conflict", itemDTO);
                            return Conflict();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return CreatedAtRoute("UserDownloads", new { id = itemDTO.Id }, itemDTO);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to add {itemDTO.Name}", itemDTO);
                    throw;
                }
            }
        }

        /// <summary>
        /// Update a UserDownload for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserDownload</param>
        /// <param name="itemDTO">The UserDownload to be updated</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutById(Guid id, [FromBody]UserDownloadDTO itemDTO)
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
                    var item = await db.UserDownloads.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} not found", id);
                        return NotFound();
                    }
                    Mapper.Map<UserDownloadDTO, UserDownload>(itemDTO, item);
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
        /// Update a UserDownload for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserDownload</param>
        /// <param name="itemDTO">The UserDownload to be updated</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutByName(string name, [FromBody]UserDownloadDTO itemDTO)
        {
            using (LogContext.PushProperty("Method", "PutByName"))
            {
                try
                {
                    Log.Verbose("Updating {name} to {@itemDTO}", name, itemDTO);
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
                    var item = db.UserDownloads.FirstOrDefault(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
                    if (item == null)
                    {
                        Log.Error("{name} not found", name);
                        return NotFound();
                    }
                    await db.SaveChangesAsync();
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
        /// Delete a UserDownload for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserDownload</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteById(Guid id)
        {
            using (LogContext.PushProperty("Method", "DeleteById"))
            {
                try
                {
                    Log.Verbose("Deleting {id}", id);
                    var item = await db.UserDownloads.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} not found");
                        return NotFound();
                    }
                    db.UserDownloads.Remove(item);
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
        /// Delete a UserDownload for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserDownload</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteByName(string name)
        {
            using (LogContext.PushProperty("Method", "DeleteByName"))
            {
                try
                {
                    Log.Verbose("Deleting {name}", name);
                    var item = await db.UserDownloads.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
                    if (item == null)
                    {
                        Log.Error("{name} not found", name);
                        return NotFound();
                    }
                    db.UserDownloads.Remove(item);
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