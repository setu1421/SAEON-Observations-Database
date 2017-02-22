using SAEON.Observations.Core;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.OData;

namespace SAEON.Observations.WebAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public abstract class BaseODataController<TEntity> : ODataController where TEntity : BaseEntity
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
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TEntity, bool>>> GetWheres()
        {
            return new List<Expression<Func<TEntity, bool>>>();
        }

        /// <summary>
        /// Overwrite for entity includes
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TEntity, bool>>> GetIncludes()
        {
            return new List<Expression<Func<TEntity, bool>>>();
        }

        /// <summary>
        /// Returns query for items
        /// </summary>
        /// <returns></returns>
        protected IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
        {
            var query = db.Set<TEntity>().AsQueryable().AsNoTracking();
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


        //[EnableQuery, ODataRoute] Required in derived class
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

        //[EnableQuery, ODataRoute("({id})")] Required in derived class
        public virtual SingleResult<TEntity> GetById([FromODataUri] Guid id)
        {
            using (LogContext.PushProperty("Method", "GetById"))
            {
                try
                {
                    return SingleResult.Create(GetQuery(i => (i.Id == id)));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        //[EnableQuery, ODataRoute("({name})")] Required in derived class 
        public virtual SingleResult<TEntity> GetByName([FromODataUri] string name)
        {
            using (LogContext.PushProperty("Method", "GetByName"))
            {
                try
                {
                    return SingleResult.Create(GetQuery(i => (i.Name == name)));
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
        /// <returns>SingleResultOf(TRelated)</returns>
        // GET: odata/TEntity(5)/TRelated
        //[EnableQuery, ODataRoute("({id})/TRelated")] Required in derived class
        protected SingleResult<TRelated> GetSingle<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select, Expression<Func<TRelated, IEnumerable<TEntity>>> include) where TRelated : BaseEntity
        {
            using (LogContext.PushProperty("Method", $"GetSingle<{nameof(TRelated)}>"))
            {
                try
                {
                    return SingleResult.Create(GetQuery(i => (i.Id == id)).Select(select).Include(include));
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
        /// <param name="include">Lambda to include TRelated.TEntity</param>
        /// <returns>IQueryableOf(TRelated)</returns>
        // GET: odata/TEntity(5)/TRelated
        //[EnableQuery, ODataRoute("({id})/TRelated")] Required in derived class
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
        // GET: odata/TEntity(5)/TRelated
        //[EnableQuery, ODataRoute("({id})/TRelated")] Required in derived class
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

        //// PUT: odata/Instruments(5)
        //public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Delta<Instrument> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Instrument instrument = await db.Instruments.FindAsync(key);
        //    if (instrument == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Put(instrument);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!InstrumentExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(instrument);
        //}

        //// POST: odata/Instruments
        //public async Task<IHttpActionResult> Post(Instrument instrument)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Instruments.Add(instrument);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (InstrumentExists(instrument.Id))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Created(instrument);
        //}

        //// PATCH: odata/Instruments(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        //public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<Instrument> patch)
        //{
        //    Validate(patch.GetEntity());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Instrument instrument = await db.Instruments.FindAsync(key);
        //    if (instrument == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.Patch(instrument);

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!InstrumentExists(key))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Updated(instrument);
        //}

        //// DELETE: odata/Instruments(5)
        //public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        //{
        //    Instrument instrument = await db.Instruments.FindAsync(key);
        //    if (instrument == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Instruments.Remove(instrument);
        //    await db.SaveChangesAsync();

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

    }


}