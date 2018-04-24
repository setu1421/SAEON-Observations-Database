using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [Route("[controller] /[action]")]
    public abstract class BaseAPIController<TEntity> : BaseController where TEntity : BaseEntity
    {
        public BaseAPIController(ObservationsDbContext context) : base(context)
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// Overwrite for entity includes 
        /// </summary>
        protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query)
        {
            return query;
        }

        /// <summary>
        /// Overwrite for entity wheres 
        /// </summary>
        protected virtual IQueryable<TEntity> ApplyWheres(IQueryable<TEntity> query)
        {
            return query;
        }

        /// <summary>
        /// query for items
        /// </summary>
        /// <returns></returns>
        protected IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
        {
            var query = db.Set<TEntity>().AsQueryable();
            query = ApplyIncludes(query);
            query = ApplyWheres(query);
            if (extraWhere != null)
            {
                query = query.Where(extraWhere);
            }
            return query;
        }

        /// <summary>
        /// all TEntity
        /// </summary>
        /// <returns>ListOf(TEntity)</returns>
        [HttpGet]
        public virtual IEnumerable<TEntity> GetAll()
        {
            using (Logging.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    var q = GetQuery().OrderBy(i => i.Name);
                    //Logging.Verbose("SQL: {SQL}", q.ToSql());
                    return q.ToList();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }

        /// <summary>
        /// an TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet("{id:guid}")]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Id", id } }))
            {
                try
                {
                    var q = GetQuery(i => (i.Id == id));
                    //Logging.Verbose("SQL: {SQL}", q.ToSql());
                    TEntity item = await q.FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Logging.Error("{id} not found", id);
                        return NotFound();
                    }
                    return new ObjectResult(item);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// an TEntity by Name
        /// </summary>
        /// <param name="name">The Name of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet("{name}")]
        public virtual async Task<IActionResult> GetByName(string name)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Name", name } }))
            {
                try
                {
                    var q = GetQuery(i => (i.Name == name));
                    //Logging.Verbose("SQL: {SQL}", q.ToSql());
                    TEntity item = await q.FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Logging.Error("{name} not found", name);
                        return NotFound();
                    }
                    return new ObjectResult(item);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {name}", name);
                    throw;
                }
            }
        }

        /// <summary>
        /// Related Entity TEntity.TRelated
        /// </summary>
        /// <typeparam name="TRelated"></typeparam>
        /// <param name="id">Id of TEntity</param>
        /// <param name="select">Lambda to select TRelated</param>
        /// <param name="include">Lamda to include TRelated.ListOf(TEntrity)</param>
        /// <returns>TaskOf(IHttpActionResult)</returns>
        //[HttpGet("{id: guid}/TRelated")] Required in derived classes
        protected async Task<IActionResult> GetSingle<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select, Expression<Func<TRelated, IEnumerable<TEntity>>> include) where TRelated : BaseEntity
        {
            using (Logging.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    if (!await GetQuery(i => (i.Id == id)).AnyAsync())
                    {
                        Logging.Error("{id} not found", id);
                        return NotFound();
                    }
                    var q = GetQuery(i => (i.Id == id)).Select(select).Include(include);
                    //Logging.Verbose("SQL: {SQL}", q.ToSql());
                    return new ObjectResult(await q.FirstOrDefaultAsync());
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
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
        //[HttpGet("{id: guid}/TRelated")] Required in derived classes
        protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select, Expression<Func<TRelated, TEntity>> include) where TRelated : BaseEntity
        {
            using (Logging.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    var q = GetQuery(i => i.Id == id).SelectMany(select).Include(include).OrderBy(i => i.Name);
                    //Logging.Verbose("SQL: {SQL}", q.ToSql());
                    return q;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
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
        //[HttpGet("{id: guid}/TRelated")] Required in derived classes
        protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select, Expression<Func<TRelated, IEnumerable<TEntity>>> include) where TRelated : BaseEntity
        {
            using (Logging.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    var q = GetQuery(i => i.Id == id).SelectMany(select).Include(include).OrderBy(i => i.Name);
                    //Logging.Verbose("SQL: {SQL}", q.ToSql());
                    return q;
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }
    }

    /*
    public abstract class BaseAPIWriteController<TEntity> : BaseAPIController<TEntity> where TEntity : BaseEntity
    {
        public BaseAPIWriteController() : base()
        {
            db.Configuration.AutoDetectChangesEnabled = true;
        }

        /// <summary>
        /// Overwrite to do additional checks before Post or Put
        /// </summary>
        /// <param name="item">TEntity</param>
        /// <param name="isPost"></param>
        /// <returns>True if TEntity is Ok else False</returns>
        protected virtual bool IsEntityOk(TEntity item, bool isPost)
        {
            return true;
        }

        /// <summary>
        /// Overwrite to do additional checks before Post or Put
        /// </summary>
        /// <param name="item">TEntity</param>
        /// <param name="isPost"></param>
        protected virtual void SetEntity(ref TEntity item, bool isPost)
        { }

        protected List<string> ModelStateErrors
        {
            get
            {
                return ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception).ToList();
            }
        }

        protected List<string> GetValidationErrors(DbEntityValidationException ex)
        {
            return ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(m => m.PropertyName + ": " + m.ErrorMessage)).ToList();
        }

        /// <summary>
        /// Create a TEntity
        /// </summary>
        /// <param name="item">The new TEntity </param>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize]
        //[ClientAuthorization("SAEON.Observations.QuerySite")]
        public virtual async Task<IActionResult> Post([FromBody]TEntity item)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "item", item } }))
            {
                try
                {
                    //Logging.Verbose("Adding {Name} {@item}", item.Name, item);
                    if (item == null)
                    {
                        Logging.Error("item cannot be null");
                        return BadRequest("item cannot be null");
                    }
                    if (!ModelState.IsValid)
                    {
                        Logging.Error("ModelState.Invalid {ModelStateErrors}", ModelStateErrors);
                        return BadRequest(ModelState);
                    }
                    if (!IsEntityOk(item, true))
                    {
                        Logging.Error("{Name} invalid", item.Name);
                        return BadRequest($"{item.Name} invalid");
                    }
                    try
                    {
                        SetEntity(ref item, true);
                        Logging.Verbose("Add {@item}", item);
                        db.Set<TEntity>().Add(item);
                        await db.SaveChangesAsync();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        var validationErrors = GetValidationErrors(ex);
                        Logging.Exception(ex, "Unable to add {Name} {EntityValidationErrors}", item.Name, validationErrors);
                        return BadRequest($"Unable to add {item.Name} EntityValidationErrors: {string.Join("; ", validationErrors)}");
                    }
                    catch (DbUpdateException ex)
                    {
                        if (await GetQuery().Where(i => i.Name == item.Name).AnyAsync())
                        {
                            Logging.Error("{Name} conflict", item.Name);
                            return Conflict();
                        }
                        else
                        {
                            Logging.Exception(ex, "Unable to add {Name}", item.Name);
                            return BadRequest(ex.Message);
                        }
                    }
                    var attr = (RoutePrefixAttribute)GetType().GetCustomAttributes(typeof(RoutePrefixAttribute), true)?[0];
                    var location = $"{attr?.Prefix ?? typeof(TEntity).Name}/{item.Id}";
                    Logging.Verbose("Location: {location} Id: {Id} Item: {@item}", location, item.Id, item);
                    return Created<TEntity>(location, item);
                    //return CreatedAtRoute(name, new { id = item.Id }, item);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to add {Name}", item.Name);
                    throw;
                }
            }
        }

        /// <summary>
        /// Update a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEnity</param>
        /// <param name="delta">The new TEntity</param>
        [HttpPut("{id:guid}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize]
        //[ClientAuthorization("SAEON.Observations.QuerySite")]
        public virtual async Task<IActionResult> PutById(Guid id, [FromBody]TEntity delta)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "id", id }, { "delta", delta } }))
            {
                try
                {
                    Logging.Verbose("Updating {id} {@delta}", id, delta);
                    if (!ModelState.IsValid)
                    {
                        Logging.Error("ModelState.Invalid {ModelStateErrors}", ModelStateErrors);
                        return BadRequest(ModelState);
                    }
                    if (id != delta.Id)
                    {
                        Logging.Error("{id} Id not same", id);
                        return BadRequest($"{id} Id not same");
                    }
                    if (!IsEntityOk(delta, false))
                    {
                        Logging.Error("{delta.Name} invalid", delta);
                        return BadRequest($"{delta.Name} invalid");
                    }
                    var item = await GetQuery().Where(i => i.Id == id).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Logging.Error("{id} not found", id);
                        return NotFound();
                    }
                    try
                    {
                        //Logging.Verbose("Loaded {@item}", item);
                        Mapper.Map(delta, item);
                        //Logging.Verbose("Mapped delta {@item}", item);
                        SetEntity(ref item, false);
                        Logging.Verbose("Set {@item}", item);
                        await db.SaveChangesAsync();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        var validationErrors = GetValidationErrors(ex);
                        Logging.Exception(ex, "Unable to update {id} {EntityValidationErrors}", item.Id, validationErrors);
                        return BadRequest($"Unable to update {item.Id} EntityValidationErrors: {string.Join("; ", validationErrors)}");
                    }
                    catch (DbUpdateException ex)
                    {
                        Logging.Exception(ex, "Unable to update {id}", id);
                        return BadRequest(ex.Message);
                    }
                    return StatusCode(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to update {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Update a TEntity by Name
        /// </summary>
        /// <param name="name">The Name of the TEnity</param>
        /// <param name="delta">The new TEntity</param>
        [HttpPut("{name}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        //[ResponseType(typeof(void))]
        //[Authorize]
        //[ClientAuthorization("SAEON.Observations.QuerySite")]
        public virtual async Task<IActionResult> PutByName(string name, [FromBody]TEntity delta)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "name", name }, { "delta", delta } }))
            {
                try
                {
                    Logging.Verbose("Updating {id} {@delta}", name, delta);
                    if (!ModelState.IsValid)
                    {
                        Logging.Error("ModelState.Invalid {ModelStateErrors}", ModelStateErrors);
                        return BadRequest(ModelState);
                    }
                    if (name != delta.Name)
                    {
                        Logging.Error("{name} Name not same", name);
                        return BadRequest($"{name} Name not same");
                    }
                    if (!IsEntityOk(delta, false))
                    {
                        Logging.Error("{delta.Name} invalid", delta);
                        return BadRequest($"{delta.Name} invalid");
                    }
                    var item = await GetQuery().Where(i => i.Name == name).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Logging.Error("{name} not found", name);
                        return NotFound();
                    }
                    try
                    {
                        //Logging.Verbose("Loaded {@item}", item);
                        Mapper.Map(delta, item);
                        //Logging.Verbose("Mapped delta {@item}", item);
                        SetEntity(ref item, false);
                        Logging.Verbose("Set {@item}", item);
                        await db.SaveChangesAsync();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        var validationErrors = GetValidationErrors(ex);
                        Logging.Exception(ex, "Unable to update {Name} {EntityValidationErrors}", item.Name, validationErrors);
                        return BadRequest($"Unable to update {item.Name} EntityValidationErrors: {string.Join("; ", validationErrors)}");
                    }
                    catch (DbUpdateException ex)
                    {
                        Logging.Exception(ex, "Unable to update {name}", name);
                        return BadRequest(ex.Message);
                    }
                    return StatusCode(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to update {name}", name);
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        [HttpDelete("{id:guid}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize]
        //[ClientAuthorization("SAEON.Observations.QuerySite")]
        public virtual async Task<IActionResult> DeleteById(Guid id)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Id", id } }))
            {
                try
                {
                    Logging.Verbose("Deleting {id}", id);
                    var item = await GetQuery().Where(i => i.Id == id).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Logging.Error("{id} not found", id);
                        return NotFound();
                    }
                    try
                    {
                        db.Set<TEntity>().Remove(item);
                        Logging.Verbose("Delete {@item}", item);
                        await db.SaveChangesAsync();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        var validationErrors = GetValidationErrors(ex);
                        Logging.Exception(ex, "Unable to delete {id} {EntityValidationErrors}", item.Id, validationErrors);
                        return BadRequest($"Unable to delete {item.Id} EntityValidationErrors: {string.Join("; ", validationErrors)}");
                    }
                    catch (DbUpdateException ex)
                    {
                        Logging.Exception(ex, "Unable to delete {id}", id);
                        return BadRequest(ex.Message);
                    }
                    return StatusCode(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to delete {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete a TEntity by Name
        /// </summary>
        /// <param name="name">The Name of the TEntity</param>
        [HttpDelete("{name}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize]
        //[ClientAuthorization("SAEON.Observations.QuerySite")]
        public virtual async Task<IActionResult> DeleteByName(string name)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Name", name } }))
            {
                try
                {
                    Logging.Verbose("Deleting {name}", name);
                    var item = await GetQuery().Where(i => i.Name == name).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Logging.Error("{name} not found", name);
                        return NotFound();
                    }
                    try
                    {
                        db.Set<TEntity>().Remove(item);
                        Logging.Verbose("Delete {@item}", item);
                        await db.SaveChangesAsync();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        var validationErrors = GetValidationErrors(ex);
                        Logging.Exception(ex, "Unable to delete {Name} {EntityValidationErrors}", item.Name, validationErrors);
                        return BadRequest($"Unable to delete {item.Name} EntityValidationErrors: {string.Join("; ", validationErrors)}");
                    }
                    catch (DbUpdateException ex)
                    {
                        Logging.Exception(ex, "Unable to delete {name}", name);
                        return BadRequest(ex.Message);
                    }
                    return StatusCode(HttpStatusCode.NoContent);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to delete {name}", name);
                    throw;
                }
            }
        }
    }
        */

}
