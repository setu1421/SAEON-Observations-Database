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
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers
{

    //[Authorize]
    public abstract class BaseApiController<TEntity> : ApiController where TEntity : BaseEntity
    {
        protected ObservationsDbContext db = new ObservationsDbContext();

        protected bool TrackChanges { get; set; } = false;

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
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TEntity, bool>>> GetWheres()
        {
            return new List<Expression<Func<TEntity, bool>>>();
        }

        /// <summary>
        /// Overwrite for entity includes
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TEntity, object>>> GetIncludes() 
        {
            return new List<Expression<Func<TEntity, object>>>();
        }

        /// <summary>
        /// Returns query for items
        /// </summary>
        /// <returns></returns>
        protected IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
        {
            var query = db.Set<TEntity>().AsQueryable();
            if (!TrackChanges)
            {
                query = query.AsNoTracking();
            }
            foreach (var include in GetIncludes())
            {
                query = query.Include(include);
            }
            foreach (var where in GetWheres())
            {
                query = query.Where(where);
            }
            if (extraWhere != null)
            {
                query = query.Where(extraWhere);
            }
            return query;
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
        /// <returns>ListOf(TEntity)</returns>
        [Route]
        public virtual IQueryable<TEntity> GetAll()
        {
            using (LogContext.PushProperty("Method", "GetAll"))
            {
                try
                {
                    return GetQuery().OrderBy(i => i.Name);
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
        //[ResponseType(typeof(TEntity))] required in derived classes
        public virtual async Task<IHttpActionResult> GetById(Guid id)
        {
            using (LogContext.PushProperty("Method", "GetById"))
            {
                try
                {
                    TEntity item = await GetQuery(i => (i.Id == id)).FirstOrDefaultAsync();
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
        [Route]
        //[ResponseType(typeof(TEntity))] required in derived classes
        public virtual async Task<IHttpActionResult> GetByName(string name)
        {
            using (LogContext.PushProperty("Method", "GetByName"))
            {
                try
                {
                    TEntity item = await GetQuery(i => (i.Name == name)).FirstOrDefaultAsync();
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
        /// Get a Related Entity TEntity.TRelated
        /// </summary>
        /// <typeparam name="TRelated"></typeparam>
        /// <param name="id">Id of TEntity</param>
        /// <param name="select">Lambda to select TRelated</param>
        /// <param name="include">Lamda to include TRelated.ListOf(TEntrity)</param>
        /// <returns>TaskOf(IHttpActionResult)</returns>
        //[ResponseType(typeof(TRelated))] Required in derived classes
        //[Route("{id:guid}/TRelated")] Required in derived classes
        protected async Task<IHttpActionResult> GetSingle<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select, Expression<Func<TRelated, IEnumerable<TEntity>>> include) where TRelated : BaseEntity
        {
            using (LogContext.PushProperty("Method", $"GetSingle<{nameof(TRelated)}>"))
            {
                try
                {
                    if (!await GetQuery(i => (i.Id == id)).AnyAsync())
                    {
                        Log.Error("{id} not found", id);
                        return NotFound();
                    }
                    return Ok(await GetQuery(i => (i.Id == id)).Select(select).Include(include).FirstOrDefaultAsync());
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        ///// <summary>
        ///// Get IQueryableOf(TRelated)
        ///// </summary>
        ///// <typeparam name="TRelated"></typeparam>
        ///// <param name="id">Id of TEntity</param>
        ///// <param name="select">Lambda to select ListOf(TRelated)</param>
        ///// <returns>IQueryableOf(TRelated)</returns>
        ////[Route("{id:guid}/TRelated")] Required in derived classes
        //protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select) where TRelated : BaseEntity
        //{
        //    using (LogContext.PushProperty("Method", $"GetMany<{nameof(TRelated)}>"))
        //    {
        //        try
        //        {
        //            return GetQuery(i => i.Id == id).SelectMany(select).OrderBy(i => i.Name);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(ex, "Unable to get {id}", id);
        //            throw;
        //        }
        //    }
        //}

        /// <summary>
        /// Get IQueryableOf(TRelated)
        /// </summary>
        /// <typeparam name="TRelated"></typeparam>
        /// <param name="id">Id of TEntity</param>
        /// <param name="select">Lambda to select ListOf(TRelated)</param>
        /// <param name="include">Lambda to include TRelated.TEntity</param>
        /// <returns>IQueryableOf(TRelated)</returns>
        //[Route("{id:guid}/TRelated")] Required in derived classes
        protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select, Expression<Func<TRelated, TEntity>> include) where TRelated : BaseEntity
        {
            using (LogContext.PushProperty("Method", $"GetMany<{nameof(TRelated)}>"))
            {
                try
                {
                    return GetQuery(i => i.Id == id).SelectMany(select).Include(include).OrderBy(i => i.Name);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Get IQueryableOf(TRelated)
        /// </summary>
        /// <typeparam name="TRelated"></typeparam>
        /// <param name="id">Id of TEntity</param>
        /// <param name="select">Lambda to select ListOf(TRelated)</param>
        /// <param name="include">Lambda to include TRelated.ListOf(TEntity)</param>
        /// <returns>IQueryableOf(TRelated)</returns>
        //[Route("{id:guid}/TRelated")] Required in derived classes
        protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select, Expression<Func<TRelated, IEnumerable<TEntity>>> include) where TRelated : BaseEntity
        {
            using (LogContext.PushProperty("Method", $"GetMany<{nameof(TRelated)}>"))
            {
                try
                {
                    return GetQuery(i => i.Id == id).SelectMany(select).Include(include).OrderBy(i => i.Name);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }
    }

    [Authorize]
    public abstract class BaseApiWriteController<TEntity> : BaseApiController<TEntity> where TEntity : BaseEntity
    {
        public BaseApiWriteController() : base()
        {
            TrackChanges = true;
        }

        /// <summary>
        /// Create a TEntity
        /// </summary>
        /// <param name="item">The new TEntity </param>
        //[Route] Required in derived classes
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
                        SetEntity(ref item);
                        item = Mapper.Map<TEntity, TEntity>(item);
                        db.Set<TEntity>().Add(item);
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        if (await GetQuery().Where(i => i.Id == item.Id).AnyAsync())
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
                        var validationErrors = ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(m => m.PropertyName + ": " + m.ErrorMessage)).ToList();
                        Log.Error(ex, "Unable to add {Name} {EntityValidationErrors}", item.Name, validationErrors);
                        return BadRequest($"Unable to add {item.Name} EntityValidationErrors: {string.Join("; ", validationErrors)}");

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
        //[Route("{id:guid}")] Required in derived classes
        [ResponseType(typeof(void))]
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
                    var item = await GetQuery().Where(i => i.Id == id).FirstOrDefaultAsync();
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
        //[Route] Required in derived classes
        [ResponseType(typeof(void))]
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
                    var item = await GetQuery().Where(i => i.Name == name).FirstOrDefaultAsync();
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
        //[Route("{id:guid}")] Required in derived classes
        [ResponseType(typeof(void))]
        public virtual async Task<IHttpActionResult> DeleteById(Guid id)
        {
            using (LogContext.PushProperty("Method", "DeleteById"))
            {
                try
                {
                    Log.Verbose("Deleting {id}", id);
                    var item = await GetQuery().Where(i => i.Id == id).FirstOrDefaultAsync();
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
        //[Route] Required in derived classes
        [ResponseType(typeof(void))]
        public virtual async Task<IHttpActionResult> DeleteByName(string name)
        {
            using (LogContext.PushProperty("Method", "DeleteByName"))
            {
                try
                {
                    Log.Verbose("Deleting {name}", name);
                    var item = await GetQuery().Where(i => i.Name == name).FirstOrDefaultAsync();
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