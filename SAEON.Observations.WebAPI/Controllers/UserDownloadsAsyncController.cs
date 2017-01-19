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
    [RoutePrefix("UserDownloadsAsync")]
    [Authorize(Roles = "Administrators,DataReaders")]
    public class UserDownloadsAsyncController : ApiController
    {
        ObservationsDbContext db = null;

        public UserDownloadsAsyncController()
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
        public async Task<IHttpActionResult> GetAsync()
        {
            return Ok(await db.UserDownloads.Where(d => d.UserId == User.Identity.GetUserId()).ToListAsync());
        }

        /// <summary>
        /// Return a UserDownload for for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserDownload</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        [ResponseType(typeof(UserDownloadDTO))]
        public async Task<IHttpActionResult> GetAsync(Guid id)
        {
            var item = await db.UserDownloads.FirstOrDefaultAsync(d => (d.UserId == User.Identity.GetUserId()) && (d.Id == id));
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        /// <summary>
        /// Return a UserDownload for for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserDownload</param>
        /// <returns></returns>
        [Route("{name}")]
        [ResponseType(typeof(UserDownloadDTO))]
        public async Task<IHttpActionResult> GetByNameAsync(string name)
        {
            var item = await db.UserDownloads.FirstOrDefaultAsync(d => (d.UserId == User.Identity.GetUserId()) && (d.Name == name));
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        /// <summary>
        /// Create a UserDownload
        /// </summary>
        /// <param name="itemDTO">The UserDownload to be created</param>
        [Route]
        [Authorize(Roles = "QuerySite")]
        [ResponseType(typeof(UserDownloadDTO))]
        public async Task<IHttpActionResult> PostAsync([FromBody]UserDownloadDTO itemDTO)
        {
            using (LogContext.PushProperty("Method", "PostAsync"))
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
                        db.UserDownloads.Add(Mapper.Map<UserDownloadDTO, UserDownload>(itemDTO));
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        if (!db.UserDownloads.Any(i => i.Id == itemDTO.Id))
                        {
                            Log.Error("{itemDTO.Name} Conflict", itemDTO);
                            return Conflict();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return CreatedAtRoute("UserDownloadsAsync", new { id = itemDTO.Id }, itemDTO);
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
        public async Task<IHttpActionResult> PutByIdAsync(Guid id, [FromBody]UserDownloadDTO itemDTO)
        {
            using (LogContext.PushProperty("Method", "PutByIdAsync"))
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
                    var item = await db.UserDownloads.FirstOrDefaultAsync(i => i.Id == id);
                    if (item == null)
                    {
                        Log.Error("{id} Not found", id);
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
        public async Task<IHttpActionResult> PutByNameAsync(string name, [FromBody]UserDownloadDTO itemDTO)
        {
            using (LogContext.PushProperty("Method", "PutByNameAsync"))
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
                        Log.Error("{name} Names not same", name);
                        return BadRequest();
                    }
                    var item = db.UserDownloads.FirstOrDefault(i => i.Name == name);
                    if (item == null)
                    {
                        Log.Error("{name} Not found", name);
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
        public async Task<IHttpActionResult> DeleteByIdAsync(Guid id)
        {
            using (LogContext.PushProperty("Method", "DeleteByIdAsync"))
            {
                try
                {
                    Log.Verbose("Deleting {id}", id);
                    var item = await db.UserDownloads.FirstOrDefaultAsync(d => (d.UserId == User.Identity.GetUserId()) && (d.Id == id));
                    if (item == null)
                    {
                        Log.Error("{id} Not found");
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
        public async Task<IHttpActionResult> DeleteByNameAsync(string name)
        {
            using (LogContext.PushProperty("Method", "DeleteByNameAsync"))
            {
                try
                {
                    Log.Verbose("Deleting {name}", name);
                    var item = await db.UserDownloads.FirstOrDefaultAsync(d => (d.UserId == User.Identity.GetUserId()) && (d.Name == name));
                    if (item == null)
                    {
                        Log.Error("{name} Not found", name);
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