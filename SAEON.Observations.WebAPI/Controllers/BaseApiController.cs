using AutoMapper;
using SAEON.Observations.Core;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{
    [Route("{action=index}")]
    [Authorize(Roles = "Administrators, DataReaders")]
    public abstract class BaseApiController<TEntity> : ApiController where TEntity : BaseEntity
    {
        protected ObservationsDbContext db = new ObservationsDbContext();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Overwrite to filter entities
        /// </summary>
        /// <returns>PredicateOf(TEntity)</returns>
        protected virtual Expression<Func<TEntity, bool>> EntityFilter()
        {
            return null;
        }

        /// <summary>
        /// Overwrite to do additional checks before Post or Put
        /// </summary>
        /// <param name="item">TEntity</param>
        /// <returns>True if TEntity is Ok else False</returns>
        protected virtual bool IsEntityOk(TEntity item)
        {
            return true;
        }

        /// <summary>
        /// Overwrite to do additional checks before Post or Put
        /// </summary>
        /// <param name="item">TEntity</param>
        protected virtual void SetEntity(ref TEntity item)
        { }

        /// <summary>
        /// Return all TEntity
        /// </summary>
        /// <returns>List of TEntity</returns>
        [Route]
        public virtual async Task<IHttpActionResult> GetAll()
        {
            using (LogContext.PushProperty("Method", "GetAll"))
            {
                try
                {
                    var filter = EntityFilter();
                    if (filter == null)
                        return Ok(await db.Set<TEntity>().OrderBy(i => i.Name).ToListAsync());
                    else
                        return Ok(await db.Set<TEntity>().Where(filter).OrderBy(i => i.Name).ToListAsync());
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get all");
                    throw;
                }
            }
        }

        /// <summary>
        /// Return an TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        /// <returns>TEntity</returns>
        [Route("{id:guid}")]
        public virtual async Task<IHttpActionResult> GetById(Guid id)
        {
            using (LogContext.PushProperty("Method", "GetById"))
            {
                try
                {
                    var filter = EntityFilter();
                    TEntity item;
                    if (filter == null)
                        item = await db.Set<TEntity>().FirstOrDefaultAsync(i => (i.Id == id));
                    else
                        item = await db.Set<TEntity>().Where(filter).FirstOrDefaultAsync(i => (i.Id == id));
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
        /// Return an TEntity by Name
        /// </summary>
        /// <param name="name">The Name of the TEntity</param>
        /// <returns>TEntity</returns>
        [Route("{name}")]
        public virtual async Task<IHttpActionResult> GetByName(string name)
        {
            using (LogContext.PushProperty("Method", "GetByName"))
            {
                try
                {
                    var filter = EntityFilter();
                    TEntity item;
                    if (filter == null)
                        item = await db.Set<TEntity>().FirstOrDefaultAsync(i => (i.Name == name));
                    else
                        item = await db.Set<TEntity>().Where(filter).FirstOrDefaultAsync(i => (i.Name == name));
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
        /// Create a TEntity
        /// </summary>
        /// <param name="item">The new TEntity </param>
        [Route]
        [Authorize(Roles = "Administrators, DataWriters")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual async Task<IHttpActionResult> Post([FromBody]TEntity item)
        {
            using (LogContext.PushProperty("Method", "Post"))
            {
                try
                {
                    Log.Verbose("Adding {Name} {@item}", item.Name, item);
                    if (item == null)
                    {
                        Log.Error("item cannot be null");
                        return BadRequest("item cannot be null");

                    }
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{Name} ModelState.Invalid", item.Name);
                        return BadRequest(ModelState);
                    }
                    if (!IsEntityOk(item))
                    {
                        Log.Error("{Name} invalid", item.Name);
                        return BadRequest($"{item.Name} invalid");
                    }
                    try
                    {
                        Log.Verbose("1: {@item}", item);
                        SetEntity(ref item);
                        Log.Verbose("2: {@item}", item);
                        item = Mapper.Map<TEntity, TEntity>(item);
                        Log.Verbose("3: {@item}", item);
                        db.Set<TEntity>().Add(item);
                        Log.Verbose("4: {@item}", item);
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        var filter = EntityFilter();
                        IQueryable<TEntity> query;
                        query = db.Set<TEntity>().Where(i => i.Id == item.Id);
                        if (filter != null)
                            query = query.Where(filter);
                        if (await query.AnyAsync())
                        {
                            Log.Error("{Name} conflict", item.Name);
                            return Conflict();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        Log.Error(ex, "Unable to add {Name} {EntityValidationErrors}", item.Name, ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(m => m.PropertyName+": "+m.ErrorMessage)).ToList());
                        return BadRequest($"Unable to add {item.Name} EntityValidationErrors");

                    }
                    return CreatedAtRoute(nameof(TEntity), new { id = item.Id }, item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to add {Name}", item.Name);
                    throw;
                }
            }
        }

        /// <summary>
        /// Update a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEnity</param>
        /// <param name="delta">The new TEntity</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "Administrators, DataWriters")]
        [ResponseType(typeof(void))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual async Task<IHttpActionResult> PutById(Guid id, [FromBody]TEntity delta)
        {
            using (LogContext.PushProperty("Method", "PutById"))
            {
                try
                {
                    Log.Verbose("Updating {id} {@delta}", id, delta);
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{id} ModelState invalid", id);
                        return BadRequest(ModelState);
                    }
                    if (id != delta.Id)
                    {
                        Log.Error("{id} Id not same", id);
                        return BadRequest($"{id} Id not same");
                    }
                    if (!IsEntityOk(delta))
                    {
                        Log.Error("{delta.Name} invalid", delta);
                        return BadRequest($"{delta.Name} invalid");
                    }
                    var filter = EntityFilter();
                    IQueryable<TEntity> query;
                    query = db.Set<TEntity>().Where(i => i.Id == id);
                    if (filter != null)
                    {
                        query = query.Where(filter);
                    }
                    var item = await query.FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Log.Error("{id} not found", id);
                        return NotFound();
                    }
                    SetEntity(ref delta);
                    Mapper.Map<TEntity, TEntity>(delta, item);
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
        /// Update a TEntity by Name
        /// </summary>
        /// <param name="name">The Name of the TEnity</param>
        /// <param name="delta">The new TEntity</param>
        [Route("{name}")]
        [Authorize(Roles = "Administrators, DataWriters")]
        [ResponseType(typeof(void))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual async Task<IHttpActionResult> PutByName(string name, [FromBody]TEntity delta)
        {
            using (LogContext.PushProperty("Method", "PutByName"))
            {
                try
                {
                    Log.Verbose("Updating {id} {@delta}", name, delta);
                    if (!ModelState.IsValid)
                    {
                        Log.Error("{id} ModelState Invalid", name);
                        return BadRequest(ModelState);
                    }
                    if (name != delta.Name)
                    {
                        Log.Error("{name} Name not same", name);
                        return BadRequest($"{name} Name not same");
                    }
                    if (!IsEntityOk(delta))
                    {
                        Log.Error("{delta.Name} invalid", delta);
                        return BadRequest($"{delta.Name} invalid");
                    }
                    var filter = EntityFilter();
                    IQueryable<TEntity> query;
                    query = db.Set<TEntity>().Where(i => i.Name == name);
                    if (filter != null)
                    {
                        query = query.Where(filter);
                    }
                    var item = await query.FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Log.Error("{name} not found", name);
                        return NotFound();
                    }
                    SetEntity(ref delta);
                    Mapper.Map<TEntity, TEntity>(delta, item);
                    await db.SaveChangesAsync();
                    return StatusCode(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to update {id}", name);
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        [Route("{id:guid}")]
        [Authorize(Roles = "Administrators, DataWriters")]
        [ResponseType(typeof(void))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual async Task<IHttpActionResult> DeleteById(Guid id)
        {
            using (LogContext.PushProperty("Method", "DeleteById"))
            {
                try
                {
                    Log.Verbose("Deleting {id}", id);
                    var filter = EntityFilter();
                    IQueryable<TEntity> query;
                    query = db.Set<TEntity>().Where(i => i.Id == id);
                    if (filter != null)
                    {
                        query = query.Where(filter);
                    }
                    var item = await query.FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Log.Error("{id} not found", id);
                        return NotFound();
                    }
                    db.Set<TEntity>().Remove(item);
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
        /// Delete a TEntity by Name
        /// </summary>
        /// <param name="name">The Name of the TEntity</param>
        [Route("{name}")]
        [Authorize(Roles = "Administrators, DataWriters")]
        [ResponseType(typeof(void))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual async Task<IHttpActionResult> DeleteByName(string name)
        {
            using (LogContext.PushProperty("Method", "DeleteByName"))
            {
                try
                {
                    Log.Verbose("Deleting {name}", name);
                    var filter = EntityFilter();
                    IQueryable<TEntity> query;
                    query = db.Set<TEntity>().Where(i => i.Name == name);
                    if (filter != null)
                    {
                        query = query.Where(filter);
                    }
                    var item = await query.FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Log.Error("{name} not found", name);
                        return NotFound();
                    }
                    db.Set<TEntity>().Remove(item);
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