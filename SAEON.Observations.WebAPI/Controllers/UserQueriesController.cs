using Microsoft.AspNet.Identity;
using Microsoft.Web.Http;
using SAEON.Observations.Entities;
using SAEON.Observations.WebAPI.Models;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
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
        public IEnumerable<UserQuery> Get()
        {
            return db.UserQueries.Where(d => d.UserId == User.Identity.GetUserId());
        }

        /// <summary>
        /// Return a UserQuery for for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        public UserQuery Get(Guid id)
        {
            var item = db.UserQueries.FirstOrDefault(d => (d.UserId == User.Identity.GetUserId()) && (d.Id == id));
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return item;
        }

        /// <summary>
        /// Return a UserQuery for for the logged in user
        /// </summary>
        /// <param name="name">The name of the UserQuery</param>
        /// <returns></returns>
        [Route("{name}")]
        public UserQuery GetByName(string name)
        {
            var item = db.UserQueries.FirstOrDefault(d => (d.UserId == User.Identity.GetUserId()) && (d.Name == name));
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return item;
        }

        /// <summary>
        /// Create a UserQuery
        /// </summary>
        /// <param name="item">The UserQuery to be created</param>
        [Route]
        [Authorize(Roles="QuerySite")]
        public UserQuery Post([FromBody]UserQuery item)
        {
            using (LogContext.PushProperty("Method", "Post"))
            {
                try
                {
                    Log.Verbose("Adding {item.Name} {@Item}", item);
                    if (!ModelState.IsValid)
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    try
                    {
                        db.UserQueries.Add(item);
                        db.SaveChanges();
                    }
                    catch (DbUpdateException)
                    {
                        if (!db.UserQueries.Any(i => i.Id == item.Id))
                        {
                            throw new HttpResponseException(HttpStatusCode.Conflict);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return item;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to add {item.Name}", item);
                    throw;
                }
            }
        }

        /// <summary>
        /// Update a UserQuery for the logged in user
        /// </summary>
        /// <param name="id">The id of the UserQuery</param>
        /// <param name="item">The UserQuery to be updated</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "QuerySite")]
        public void PutById(Guid id, [FromBody]UserQuery item)
        {
            using (LogContext.PushProperty("Method", "PutById"))
            {
                try
                {
                    Log.Verbose("Updating {id} to {@item}", id, item);
                    if (!ModelState.IsValid || (id != item.Id))
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    try
                    {
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!db.UserQueries.Any(i => i.Id == item.Id))
                        {
                            throw new HttpResponseException(HttpStatusCode.NotFound);
                        }
                        else
                        {
                            throw;
                        }
                    }
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
        /// <param name="item">The UserQuery to be updated</param>
        [Route("{name}")]
        [Authorize(Roles = "QuerySite")]
        public void PutByName(string name, [FromBody]UserQuery item)
        {
            using (LogContext.PushProperty("Method", "PutByName"))
            {
                try
                {
                    Log.Verbose("Updating {name} to {@item}", name);
                    if (!ModelState.IsValid || (name != item.Name))
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                    try
                    {
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!db.UserQueries.Any(i => i.Name == item.Name))
                        {
                            throw new HttpResponseException(HttpStatusCode.NotFound);
                        }
                        else
                        {
                            throw;
                        }
                    }
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
        public void DeleteById(Guid id)
        {
            using (LogContext.PushProperty("Method", "DeleteById"))
            {
                try
                {
                    Log.Verbose("Deleting {id}", id);
                    var item = db.UserQueries.FirstOrDefault(d => (d.UserId == User.Identity.GetUserId()) && (d.Id == id));
                    if (item == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                    db.UserQueries.Remove(item);
                    db.SaveChanges();
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
        public void DeleteByName(string name)
        {
            using (LogContext.PushProperty("Method", "DeleteByName"))
            {
                try
                {
                    Log.Verbose("Deleting {name}", name);
                    var item = db.UserQueries.FirstOrDefault(d => (d.UserId == User.Identity.GetUserId()) && (d.Name == name));
                    if (item == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                    db.UserQueries.Remove(item);
                    db.SaveChanges();
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