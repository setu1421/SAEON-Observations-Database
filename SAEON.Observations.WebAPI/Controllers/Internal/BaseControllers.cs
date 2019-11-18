using AutoMapper;
using SAEON.AspNet.WebApi;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
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

namespace SAEON.Observations.WebAPI.Controllers.Internal
{

    [ApiExplorerSettings(IgnoreApi = true)]
    //[ClientAuthorization("SAEON.Observations.QuerySite")] //Uncomment when going live
    [TenantAuthorization]
    public abstract class BaseController : ApiController
    {
        private ObservationsDbContext dbContext = null;
        protected ObservationsDbContext DbContext
        {
            get
            {
                if (dbContext == null) dbContext = new ObservationsDbContext(TenantAuthorizationAttribute.GetTenantFromHeaders(Request));
                return dbContext;
            }
            private set => dbContext = value;
        }

        protected IMapper Mapper { get; private set; }

        public BaseController()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<UserDownload, UserDownload>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
                cfg.CreateMap<UserQuery, UserQuery>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
            });

            Mapper = config.CreateMapper();
        }

        ~BaseController()
        {
            DbContext = null;
        }
    }

    public abstract class BaseListController<TEntity> : BaseController where TEntity : BaseEntity
    {
        /// <summary>
        /// query for items
        /// </summary>
        /// <returns></returns>
        protected virtual List<TEntity> GetList()
        {
            var result = new List<TEntity>();
            return result;
        }

        /// <summary>
        /// Get all TEntity
        /// </summary>
        /// <returns>ListOf(TEntity)</returns>
        [HttpGet]
        [Route]
        public IQueryable<TEntity> GetAll()
        {
            using (Logging.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    return GetList().AsQueryable();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
    }

    public class OrderBy<TEntity>
    {
        public Expression<Func<TEntity, object>> Expression { get; set; }
        public bool Ascending { get; set; } = true;

        public OrderBy() { }
        public OrderBy(Expression<Func<TEntity, object>> expression)
        {
            Expression = expression;
        }
        public OrderBy(Expression<Func<TEntity, object>> expression, bool ascending)
        {
            Expression = expression;
            Ascending = ascending;
        }
    }

    public abstract class BaseContoller<TEntity> : BaseController where TEntity : IDEntity
    {
        /// <summary>
        /// Overwrite to filter entities
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TEntity, bool>>> GetWheres()
        {
            return new List<Expression<Func<TEntity, bool>>>();
        }

        /// <summary>
        /// Overwrite to order of entities
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<OrderBy<TEntity>> GetOrderBys()
        {
            return new List<OrderBy<TEntity>>();
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
        /// query for items
        /// </summary>
        /// <returns></returns>
        protected IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
        {
            var query = DbContext.Set<TEntity>().AsQueryable();
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
            var orderBys = GetOrderBys();
            //Logging.Information("OrderBys: {orderBys}", orderBys?.Count);
            var orderBy = orderBys.FirstOrDefault();
            if (orderBy != null)
            {
                query = query.OrderByMember(orderBy.Expression, orderBy.Ascending);
                foreach (var thenBy in orderBys.Skip(1))
                {
                    query = query.ThenByMember(orderBy.Expression, orderBy.Ascending);
                }
            }
            return query;
        }

        /// <summary>
        /// Get all TEntity
        /// </summary>
        /// <returns>ListOf(TEntity)</returns>
        [HttpGet]
        [Route]
        public virtual IQueryable<TEntity> GetAll()
        {
            using (Logging.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    return GetQuery();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }

        /// <summary>
        /// Get a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet]
        [Route("{id:guid}")]
        //[ResponseType(typeof(TEntity))] required in derived classes
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public virtual async Task<IHttpActionResult> GetById([FromUri] Guid id)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Id", id } }))
            {
                try
                {
                    TEntity item = await GetQuery(i => (i.Id == id)).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Logging.Error("{id} not found", id);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }
    }

    [Authorize]
    public abstract class BaseWriteController<TEntity> : BaseContoller<TEntity> where TEntity : NamedEntity
    {
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

        public List<string> ModelStateErrors
        {
            get
            {
                return ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception).ToList();
            }
        }

        public List<string> GetValidationErrors(DbEntityValidationException ex)
        {
            return ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(m => m.PropertyName + ": " + m.ErrorMessage)).ToList();
        }

        public void SaveChanges()
        {
            try
            {
                DbContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Logging.Exception(ex, string.Join("; ", GetValidationErrors(ex)));
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                Logging.Exception(ex, string.Join("; ", GetValidationErrors(ex)));
                throw;
            }
        }

        /// <summary>
        /// Create a TEntity
        /// </summary>
        /// <param name="item">The new TEntity </param>
        [HttpPost]
        //[Route] Required in derived classes
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public virtual async Task<IHttpActionResult> Post([FromBody]TEntity item)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "item", item } }))
            {
                try
                {
                    Logging.Verbose("Adding {Name} {@item}", item.Name, item);
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
                        DbContext.Set<TEntity>().Add(item);
                        await DbContext.SaveChangesAsync();
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
        [HttpPut]
        [ResponseType(typeof(void))]
        //[Route("{id:guid}")] Required in derived classes
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public virtual async Task<IHttpActionResult> PutById(Guid id, [FromBody]TEntity delta)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
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
                        await DbContext.SaveChangesAsync();
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

        /*
        /// <summary>
        /// Update a TEntity by Name
        /// </summary>
        /// <param name="name">The Name of the TEnity</param>
        /// <param name="delta">The new TEntity</param>
        [HttpPut]
        [ResponseType(typeof(void))]
        //[Route] Required in derived classes
        public virtual async Task<IHttpActionResult> PutByName(string name, [FromBody]TEntity delta)
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
        */

        /// <summary>
        /// Delete a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        [HttpDelete]
        [ResponseType(typeof(void))]
        //[Route("{id:guid}")] Required in derived classes
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public virtual async Task<IHttpActionResult> DeleteById(Guid id)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
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
                        DbContext.Set<TEntity>().Remove(item);
                        Logging.Verbose("Delete {@item}", item);
                        await DbContext.SaveChangesAsync();
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

        /*
        /// <summary>
        /// Delete a TEntity by Name
        /// </summary>
        /// <param name="name">The Name of the TEntity</param>
        [HttpDelete]
        [ResponseType(typeof(void))]
        //[Route] Required in derived classes
        public virtual async Task<IHttpActionResult> DeleteByName(string name)
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
        */
    }

    public static class QueryableExtensions
    {
        /// <summary>
        ///     Supports sorting of a given member as an expression when type is not known. It solves problem with LINQ to Entities unable to
        ///     cast different types as 'System.DateTime', 'System.DateTime?' to type 'System.Object'.
        ///     LINQ to Entities only supports casting Entity Data Model primitive types.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="query">query to apply sorting on.</param>
        /// <param name="expression">the member expression to apply</param>
        /// <param name="ascending">the sort order to apply</param>
        /// <returns>Query with sorting applied as IOrderedQueryable of type T</returns>
        public static IOrderedQueryable<T> OrderByMember<T>(
            this IQueryable<T> query,
            Expression<Func<T, object>> expression,
            bool ascending)
        {
            var body = expression.Body as UnaryExpression;

            if (body != null)
            {
                var memberExpression = body.Operand as MemberExpression;

                if (memberExpression != null)
                {
                    return
                        (IOrderedQueryable<T>)
                        query.Provider.CreateQuery(
                            Expression.Call(
                                typeof(Queryable),
                                ascending ? "OrderBy" : "OrderByDescending",
                                new[] { typeof(T), memberExpression.Type },
                                query.Expression,
                                Expression.Lambda(memberExpression, expression.Parameters)));
                }
            }

            return ascending ? query.OrderBy(expression) : query.OrderByDescending(expression);
        }

        /// <summary>
        ///     Supports sorting of a given member as an expression when type is not known. It solves problem with LINQ to Entities unable to
        ///     cast different types as 'System.DateTime', 'System.DateTime?' to type 'System.Object'.
        ///     LINQ to Entities only supports casting Entity Data Model primitive types.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="query">query to apply sorting on.</param>
        /// <param name="expression">the member expression to apply</param>
        /// <param name="ascending">the sort order to apply</param>
        /// <returns>Query with sorting applied as IOrderedQueryable of type T</returns>
        public static IOrderedQueryable<T> ThenByMember<T>(
            this IQueryable<T> query,
            Expression<Func<T, object>> expression,
            bool ascending)
        {
            return ((IOrderedQueryable<T>)query).ThenByMember(expression, ascending);
        }

        /// <summary>
        ///     Supports sorting of a given member as an expression when type is not known. It solves problem with LINQ to Entities unable to
        ///     cast different types as 'System.DateTime', 'System.DateTime?' to type 'System.Object'.
        ///     LINQ to Entities only supports casting Entity Data Model primitive types.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="query">query to apply sorting on.</param>
        /// <param name="expression">the member expression to apply</param>
        /// <param name="ascending">the sort order to apply</param>
        /// <returns>Query with sorting applied as IOrderedQueryable of type T</returns>
        public static IOrderedQueryable<T> ThenByMember<T>(
            this IOrderedQueryable<T> query,
            Expression<Func<T, object>> expression,
            bool ascending)
        {
            var body = expression.Body as UnaryExpression;

            if (body != null)
            {
                var memberExpression = body.Operand as MemberExpression;

                if (memberExpression != null)
                {
                    return
                        (IOrderedQueryable<T>)
                        query.Provider.CreateQuery(
                            Expression.Call(
                                typeof(Queryable),
                                ascending ? "ThenBy" : "ThenByDescending",
                                new[] { typeof(T), memberExpression.Type },
                                query.Expression,
                                Expression.Lambda(memberExpression, expression.Parameters)));
                }
            }

            return ascending ? query.ThenBy(expression) : query.ThenByDescending(expression);
        }
    }
}

