using SAEON.AspNet.WebApi;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [TenantAuthorization]
    public abstract class BaseEntityController<TEntity> : ApiController where TEntity : BaseEntity
    {
        //protected const int PageSize = 25;
        //protected const int MaxTop = 500;
        //protected const int MaxAll = 10000;

        private ObservationsDbContext dbContext = null;
        protected ObservationsDbContext DbContext => dbContext ?? (dbContext = new ObservationsDbContext(TenantAuthorizationAttribute.GetTenantFromHeaders(Request)));

        [NotMapped]
        public string EntitySetName { get; protected set; }
        [NotMapped]
        public List<string> NavigationLinks { get; } = new List<string>();

        protected void SetBaseUrl()
        {
            EntityConfig.BaseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/Api";
        }

        /// <summary>
        /// Overwrite to order of entities
        /// </summary>
        /// <returns>ListOf(PredicateOf(TEntity))</returns>
        protected virtual List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            return new List<Expression<Func<TEntity, object>>>();
        }

        /// <summary>
        /// query for items
        /// </summary>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
        {
            var query = DbContext.Set<TEntity>().AsNoTracking().AsQueryable();
            if (extraWhere != null)
            {
                query = query.Where(extraWhere);
            }
            foreach (var orderBy in GetOrderBys())
            {
                query = query.OrderBy(orderBy);
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
                    SetBaseUrl();
                    return GetQuery();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }
    }

    public abstract class IDEntityController<TEntity> : BaseEntityController<TEntity> where TEntity : GuidIdEntity
    {
        /// <summary>
        /// Get a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet]
        [Route("{id:guid}")]
        //[ResponseType(typeof(TEntity))] required in derived classes
        public virtual async Task<IHttpActionResult> GetByIdAsync([FromUri] Guid id)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    SetBaseUrl();
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

        /// <summary>
        /// Related Entity TEntity.TRelated
        /// </summary>
        /// <typeparam name="TRelated"></typeparam>
        /// <param name="id">Id of TEntity</param>
        /// <param name="select">Lambda to select TRelated</param>
        /// <returns>TaskOf(IHttpActionResult)</returns>
        //[HttpGet] Required in calling classes
        //[ResponseType(typeof(TRelated))] Required in calling classes
        //[Route("{id:guid}/TRelated")] Required in calling classes
        protected async Task<IHttpActionResult> GetSingleAsync<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select) where TRelated : GuidIdEntity
        {
            using (Logging.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    SetBaseUrl();
                    if (!await GetQuery(i => (i.Id == id)).AnyAsync())
                    {
                        Logging.Error("{id} not found", id);
                        return NotFound();
                    }
                    return Ok(await GetQuery(i => (i.Id == id)).Select(select).FirstOrDefaultAsync());
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
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
        /// <returns>TaskOf(IHttpActionResult)</returns>
        //[HttpGet] Required in calling classes
        //[ResponseType(typeof(TRelated))] Required in calling classes
        //[Route("{id:guid}/TRelated")] Required in calling classes
        protected TRelated GetSingle<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select) where TRelated : GuidIdEntity
        {
            using (Logging.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    SetBaseUrl();
                    if (!GetQuery(i => (i.Id == id)).Any())
                    {
                        Logging.Error("{id} not found", id);
                        throw new ArgumentException($"{id} not found");
                    }
                    return GetQuery(i => (i.Id == id)).Select(select).FirstOrDefault();
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
        /// <returns>IQueryableOf(TRelated)</returns>
        //[HttpGet]
        //[Route("{id:guid}/TRelated")] Required in derived classes
        protected IQueryable<TRelated> GetManyIdEntity<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select) where TRelated : IdEntity
        {
            using (Logging.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    SetBaseUrl();
                    return GetQuery(i => i.Id == id).SelectMany(select);
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
        /// <returns>IQueryableOf(TRelated)</returns>
        //[HttpGet]
        //[Route("{id:guid}/TRelated")] Required in derived classes
        protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select) where TRelated : NamedEntity
        {
            using (Logging.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    SetBaseUrl();
                    return GetQuery(i => i.Id == id).SelectMany(select);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }
    }

    public abstract class NamedApiController<TEntity> : IDEntityController<TEntity> where TEntity : NamedEntity
    {
        protected override List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Add(i => i.Name);
            return result;
        }

        /// <summary>
        /// Get a TEntity by Name
        /// </summary>
        /// <param name="name">The Name of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet]
        [Route]
        //[ResponseType(typeof(TEntity))] required in derived classes
        public virtual async Task<IHttpActionResult> GetByNameAsync([FromUri] string name)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Name", name } }))
            {
                try
                {
                    SetBaseUrl();
                    TEntity item = await GetQuery(i => (i.Name == name)).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Logging.Error("{name} not found", name);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {name}", name);
                    throw;
                }
            }
        }
    }

    public abstract class CodedApiController<TEntity> : NamedApiController<TEntity> where TEntity : CodedEntity
    {
        protected override List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Insert(0, i => i.Code);
            return result;
        }

        /// <summary>
        /// Get a TEntity by Code
        /// </summary>
        /// <param name="code">The Code of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet]
        [Route]
        //[ResponseType(typeof(TEntity))] required in derived classes
        public virtual async Task<IHttpActionResult> GetByCodeAsync([FromUri] string code)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Code", code } }))
            {
                try
                {
                    SetBaseUrl();
                    TEntity item = await GetQuery(i => (i.Code == code)).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Logging.Error("{code} not found", code);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to get {code}", code);
                    throw;
                }
            }
        }
    }

}