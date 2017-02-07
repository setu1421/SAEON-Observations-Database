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
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI.Controllers
{
    /// <summary>
    /// Users have to be logged in to download data in the QuerySite. Any downloads are saved for later re-downloads.
    /// </summary>
    [ODataRoutePrefix("UserDownloads")]
    public class UserDownloadsODataController : BaseODataController<UserDownload>
    {
        /// <summary>
        /// Filter only for logged in user
        /// </summary>
        /// <returns></returns>
        protected override Expression<Func<UserDownload, bool>> EntityFilter()
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("Logged in UserId");
            }
            return (i => i.UserId == userId);
        }

        /// <summary>
        /// Check UserId is logged in UserId
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsEntityOk(UserDownload item)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new NullReferenceException("Not logged in");
            }
            return base.IsEntityOk(item) && (item.UserId == userId);
        }

        /// <summary>
        /// Check UserId is logged in UserId
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override void SetEntity(ref UserDownload item)
        {
            base.SetEntity(ref item);
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("Logged in UserId");
            }
            item.UserId = userId;
        }

        /// <summary>
        /// Get a list of UserDownloads
        /// </summary>
        /// <returns>A list of UserDownload</returns>
        public override IQueryable<UserDownload> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/UserDownloads(5)
        /// <summary>
        /// Get a UserDownload by Id
        /// </summary>
        /// <param name="id">Id of UserDownload</param>
        /// <returns>UserDownload</returns>
        public override SingleResult<UserDownload> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }


        // GET: odata/UserDownloads(5)
        /// <summary>
        /// Get a UserDownload by Name
        /// </summary>
        /// <param name="name">Name of UserDownload</param>
        /// <returns>UserDownload</returns>
        public override SingleResult<UserDownload> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        //// POST: odata/UserDownloads
        //[ODataRoute]
        //[Authorize(Roles = "QuerySite")]
        //public async Task<IHttpActionResult> Post(UserDownload item)
        //{
        //    using (LogContext.PushProperty("Method", "Post"))
        //    {
        //        try
        //        {
        //            Log.Verbose("Adding {item.Name} {@item}", item);
        //            if (!ModelState.IsValid)
        //            {
        //                Log.Error("{item.Name} ModelState.Invalid", item);
        //                return BadRequest(ModelState);
        //            }
        //            if (item.UserId != User.Identity.GetUserId())
        //            {
        //                Log.Error("{item.Name} invalid user", item);
        //                return BadRequest();
        //            }
        //            item.UserId = User.Identity.GetUserId();
        //            db.UserDownloads.Add(item);
        //            try
        //            {
        //                await db.SaveChangesAsync();
        //            }
        //            catch (DbUpdateException)
        //            {
        //                if (db.UserDownloads.Any(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == item.Id)))
        //                {
        //                    Log.Error("{item.Name} conflict", item);
        //                    return Conflict();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }
        //            return Created(item);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(ex, "Unable to add {item.Name}", item);
        //            throw;
        //        }
        //    }
        //}

        //// PUT: odata/UserDownloads(5)
        //[ODataRoute("({id})")]
        //[Authorize(Roles = "QuerySite")]
        //public async Task<IHttpActionResult> PutById([FromODataUri] Guid id, Delta<UserDownload> patch)
        //{
        //    using (LogContext.PushProperty("Method", "PutById"))
        //    {
        //        try
        //        {
        //            Log.Verbose("Updating {id} {@patch}", id, patch);
        //            Validate(patch.GetEntity());
        //            if (!ModelState.IsValid)
        //            {
        //                Log.Error("{id} ModelState.Invalid", id);
        //                return BadRequest(ModelState);
        //            }
        //            var item = await db.UserDownloads.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
        //            if (item == null)
        //            {
        //                Log.Error("{id} not found", id);
        //                return NotFound();
        //            }
        //            patch.Put(item);
        //            await db.SaveChangesAsync();
        //            return Updated(item);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(ex, "Unable to update {id}", id);
        //            throw;
        //        }
        //    }
        //}

        //[ODataRoute("({name})")]
        //[Authorize(Roles = "QuerySite")]
        //public async Task<IHttpActionResult> PutByName([FromODataUri] string name, Delta<UserDownload> patch)
        //{
        //    using (LogContext.PushProperty("Method", "PutByName"))
        //    {
        //        try
        //        {
        //            Log.Verbose("Updating {name} {@patch}", name, patch);
        //            Validate(patch.GetEntity());
        //            if (!ModelState.IsValid)
        //            {
        //                Log.Error("{name} ModelState.Invalid", name);
        //                return BadRequest(ModelState);
        //            }
        //            var item = await db.UserDownloads.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
        //            if (item == null)
        //            {
        //                Log.Error("{name} not found", name);
        //                return NotFound();
        //            }
        //            patch.Put(item);
        //            await db.SaveChangesAsync();
        //            return Updated(item);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(ex, "Unable to update {name}", name);
        //            throw;
        //        }
        //    }
        //}

        //// PATCH: odata/UserDownloads(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //[ODataRoute("({id})")]
        //[Authorize(Roles = "QuerySite")]
        //public async Task<IHttpActionResult> PatchById([FromODataUri] Guid id, Delta<UserDownload> patch)
        //{
        //    using (LogContext.PushProperty("Method", "PatchById"))
        //    {
        //        try
        //        {
        //            Validate(patch.GetEntity());
        //            if (!ModelState.IsValid)
        //            {
        //                Log.Error("{id} ModelState.Invalid", id);
        //                return BadRequest(ModelState);
        //            }
        //            var item = await db.UserDownloads.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
        //            if (item == null)
        //            {
        //                Log.Error("{id} not found", id);
        //                return NotFound();
        //            }
        //            patch.Patch(item);
        //            await db.SaveChangesAsync();
        //            return Updated(item);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(ex, "Unable to patch {id}", id);
        //            throw;
        //        }
        //    }
        //}

        //[AcceptVerbs("PATCH", "MERGE")]
        //[ODataRoute("({name})")]
        //[Authorize(Roles = "QuerySite")]
        //public async Task<IHttpActionResult> PatchByName([FromODataUri] string name, Delta<UserDownload> patch)
        //{
        //    using (LogContext.PushProperty("Method", "PatchByName"))
        //    {
        //        try
        //        {
        //            Validate(patch.GetEntity());
        //            if (!ModelState.IsValid)
        //            {
        //                Log.Error("{name} ModelState.Invalid", name);
        //                return BadRequest(ModelState);
        //            }
        //            var item = await db.UserDownloads.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
        //            if (item == null)
        //            {
        //                Log.Error("{name} not found", name);
        //                return NotFound();
        //            }
        //            patch.Patch(item);
        //            await db.SaveChangesAsync();
        //            return Updated(item);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(ex, "Unable to patch {name}", name);
        //            throw;
        //        }
        //    }
        //}

        //// DELETE: odata/UserDownloads(5)
        //[ODataRoute("({id})")]
        //[Authorize(Roles = "QuerySite")]
        //public async Task<IHttpActionResult> DeleteById([FromODataUri] Guid id)
        //{
        //    using (LogContext.PushProperty("Method", "DeleteById"))
        //    {
        //        try
        //        {
        //            Log.Verbose("Deleting {id}", id);
        //            var item = await db.UserDownloads.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
        //            if (item == null)
        //            {
        //                Log.Error("{id} not found", id);
        //                return NotFound();
        //            }
        //            db.UserDownloads.Remove(item);
        //            await db.SaveChangesAsync();
        //            return StatusCode(HttpStatusCode.NoContent);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(ex, "Unable to delete {id}", id);
        //            throw;
        //        }
        //    }
        //}

        //[ODataRoute("({name})")]
        //[Authorize(Roles = "QuerySite")]
        //public async Task<IHttpActionResult> DeleteByName([FromODataUri] string name)
        //{
        //    using (LogContext.PushProperty("Method", "DeleteById"))
        //    {
        //        try
        //        {
        //            Log.Verbose("Deleting {name}", name);
        //            var item = await db.UserDownloads.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
        //            if (item == null)
        //            {
        //                Log.Error("{name} not found", name);
        //                return NotFound();
        //            }
        //            db.UserDownloads.Remove(item);
        //            await db.SaveChangesAsync();
        //            return StatusCode(HttpStatusCode.NoContent);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(ex, "Unable to delete {name}", name);
        //            throw;
        //        }
        //    }
        //}
    }
}
