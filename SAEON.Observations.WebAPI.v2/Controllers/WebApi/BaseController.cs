using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.v2.Controllers.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Constants.TenantPolicy)]
    public abstract class BaseController<TController> : ControllerBase where TController : BaseController<TController>
    {
        private ILogger<TController> _logger;
        protected ILogger<TController> Logger => _logger ?? (_logger = HttpContext.RequestServices.GetService<ILogger<TController>>());
        //private IConfiguration _config;
        //protected IConfiguration Config => _config ?? (_config = HttpContext.RequestServices.GetService<IConfiguration>());
        private ObservationsDbContext _dbContext;
        protected ObservationsDbContext DbContext => _dbContext ?? (_dbContext = HttpContext.RequestServices.GetService<ObservationsDbContext>());
    }

    public abstract class BaseEntityController<TController, TEntity> : BaseController<TController> where TController : BaseController<TController> where TEntity : BaseEntity
    {
        /// <summary>
        /// Overwrite to filter entities
        /// </summary>
        /// <returns>List&lt;Predicate&lt;TEntity&gt;&gt;</returns>
        protected virtual List<Expression<Func<TEntity, bool>>> GetWheres()
        {
            return new List<Expression<Func<TEntity, bool>>>();
        }

        /// <summary>
        /// Overwrite to order of entities
        /// </summary>
        /// <returns>List&lt;Predicate&lt;TEntity&gt;&gt;</returns>
        protected virtual List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            return new List<Expression<Func<TEntity, object>>>();
        }

        protected virtual IQueryable<TEntity> AddIncludes(IQueryable<TEntity> query)
        {
            return query;
        }

        /// <summary>
        /// Query for items
        /// </summary>
        /// <returns></returns>
        protected IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
        {
            var query = DbContext.Set<TEntity>().AsQueryable();
            query = AddIncludes(query);
            foreach (var where in GetWheres())
            {
                query = query.Where(where);
            }
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
        /// <returns>List&lt;TEntity&gt;</returns>
        [HttpGet]
        public virtual async Task<List<TEntity>> GetAll()
        {
            using (Logger.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    return await GetQuery().ToListAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, "Unable to get all");
                    throw;
                }
            }
        }
    }

    public abstract class IDEntityController<TController, TEntity> : BaseEntityController<TController, TEntity> where TController : BaseController<TController> where TEntity : IDEntity
    {
        /// <summary>
        /// Get a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        /// <returns>ActionResult&lt;TEntity&gt;</returns>
        [HttpGet("{id:guid}")]
        public virtual async Task<ActionResult<TEntity>> GetByIdAsync(Guid id)
        {
            using (Logger.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    TEntity item = await GetQuery(i => (i.Id == id)).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Logger.LogError("{id} not found", id);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Get TEntity.TRelated
        /// </summary>
        /// <typeparam name="TRelated"></typeparam>
        /// <param name="id">Id of TEntity</param>
        /// <param name="select">Lambda to select TRelated</param>
        /// <returns>ActionResult&lt;TRelated&gt;</returns>
        //[HttpGet("{id:guid}/TRelated")] Required in derived classes
        protected async Task<ActionResult<TRelated>> GetSingleAsync<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select) where TRelated : IDEntity
        {
            using (Logger.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    if (!await GetQuery(i => (i.Id == id)).AnyAsync())
                    {
                        Logging.Error("{id} not found", id);
                        return NotFound();
                    }
                    return Ok(await GetQuery(i => (i.Id == id)).Select(select).FirstOrDefaultAsync());
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Get List&lt;TRelated&gt;
        /// </summary>
        /// <typeparam name="TRelated"></typeparam>
        /// <param name="id">Id of TEntity</param>
        /// <param name="select">Lambda to select ListOf(TRelated)</param>
        /// <returns>List&lt;TRelated&gt;</returns>
        //[HttpGet("{id:guid}/TRelated")] Required in derived classes
        protected async Task<List<TRelated>> GetManyAsync<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select) where TRelated : NamedEntity
        {
            using (Logger.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    return await GetQuery(i => i.Id == id).SelectMany(select).ToListAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }
    }

    public abstract class NamedEntityController<TController, TEntity> : IDEntityController<TController, TEntity> where TController : BaseController<TController> where TEntity : NamedEntity
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
        /// <returns>ActionResult&lt;TEntity&gt;</returns>
        [HttpGet("name/{name}")]
        public virtual async Task<ActionResult<TEntity>> GetByNameAsync(string name)
        {
            using (Logger.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Name", name } }))
            {
                try
                {
                    TEntity item = await GetQuery(i => (i.Name == name)).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Logger.LogError("{name} not found", name);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, "Unable to get {name}", name);
                    throw;
                }
            }
        }
    }

    public abstract class CodedEntityController<TController, TEntity> : NamedEntityController<TController, TEntity> where TController : BaseController<TController> where TEntity : CodedEntity
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
        /// <param name="code">The Name of the TEntity</param>
        /// <returns>ActionResult&lt;TEntity&gt;</returns>
        [HttpGet("code/{code}")]
        public virtual async Task<ActionResult<TEntity>> GetByCodeAsync(string code)
        {
            using (Logger.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Code", code } }))
            {
                try
                {
                    TEntity item = await GetQuery(i => (i.Code == code)).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        Logger.LogError("{code} not found", code);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, "Unable to get {code}", code);
                    throw;
                }
            }
        }
    }
}