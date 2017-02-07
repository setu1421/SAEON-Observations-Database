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
    /// Logged in users can save frequently used queries in the QueryUserQuery for later use
    /// </summary>
    [ODataRoutePrefix("UserQueries")]
    public class UserQueriesODataController : BaseODataController<UserQuery>
    {
        /// <summary>
        /// Filter only for logged in user
        /// </summary>
        /// <returns></returns>
        protected override Expression<Func<UserQuery, bool>> EntityFilter()
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
        protected override bool IsEntityOk(UserQuery item)
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
        protected override void SetEntity(ref UserQuery item)
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
        /// Get a list of UserQueries
        /// </summary>
        /// <returns>A list of UserQuery</returns>
        public override IQueryable<UserQuery> GetAll()
        {
            return base.GetAll();
        }

        // GET: odata/UserQueries(5)
        /// <summary>
        /// Get a UserQuery by Id
        /// </summary>
        /// <param name="id">Id of UserQuery</param>
        /// <returns>UserQuery</returns>
        public override SingleResult<UserQuery> GetById([FromODataUri] Guid id)
        {
            return base.GetById(id);
        }


        // GET: odata/UserQueries(5)
        /// <summary>
        /// Get a UserQuery by Name
        /// </summary>
        /// <param name="name">Name of UserQuery</param>
        /// <returns>UserQuery</returns>
        public override SingleResult<UserQuery> GetByName([FromODataUri] string name)
        {
            return base.GetByName(name);
        }

        //// POST: odata/UserQueries
        //[ODataRoute]
        //[Authorize(Roles = "QuerySite")]
        //public async Task<IHttpActionResult> Post(UserQuery item)
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
        //            db.UserQueries.Add(item);
        //            try
        //            {
        //                await db.SaveChangesAsync();
        //            }
        //            catch (DbUpdateException)
        //            {
        //                if (db.UserQueries.Any(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == item.Id)))
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

        //// PUT: odata/UserQueries(5)
        //[ODataRoute("({id})")]
        //[Authorize(Roles = "QuerySite")]
        //public async Task<IHttpActionResult> PutById([FromODataUri] Guid id, Delta<UserQuery> patch)
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
        //            var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
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
        //public async Task<IHttpActionResult> PutByName([FromODataUri] string name, Delta<UserQuery> patch)
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
        //            var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
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

        //// PATCH: odata/UserQueries(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //[ODataRoute("({id})")]
        //[Authorize(Roles = "QuerySite")]
        //public async Task<IHttpActionResult> PatchById([FromODataUri] Guid id, Delta<UserQuery> patch)
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
        //            var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
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
        //public async Task<IHttpActionResult> PatchByName([FromODataUri] string name, Delta<UserQuery> patch)
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
        //            var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
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

        //// DELETE: odata/UserQueries(5)
        //[ODataRoute("({id})")]
        //[Authorize(Roles = "QuerySite")]
        //public async Task<IHttpActionResult> DeleteById([FromODataUri] Guid id)
        //{
        //    using (LogContext.PushProperty("Method", "DeleteById"))
        //    {
        //        try
        //        {
        //            Log.Verbose("Deleting {id}", id);
        //            var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Id == id));
        //            if (item == null)
        //            {
        //                Log.Error("{id} not found", id);
        //                return NotFound();
        //            }
        //            db.UserQueries.Remove(item);
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
        //            var item = await db.UserQueries.FirstOrDefaultAsync(i => (i.UserId == User.Identity.GetUserId()) && (i.Name == name));
        //            if (item == null)
        //            {
        //                Log.Error("{name} not found", name);
        //                return NotFound();
        //            }
        //            db.UserQueries.Remove(item);
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
