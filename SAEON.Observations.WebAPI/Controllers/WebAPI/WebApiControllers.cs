using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [Route("Api/[controller]")]
    [EnableCors(ObsDBAuthenticationDefaults.CorsAllowAllPolicy)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public abstract class ApiReadController<TEntity> : BaseReadController<TEntity> where TEntity : BaseEntity
    {
        protected override void UpdateRequest()
        {
            EntityConfig.BaseUrl = Request.GetUri().GetLeftPart(UriPartial.Authority) + "/Api";
        }
    }

    public abstract class IDEntityApiController<TEntity> : ApiReadController<TEntity> where TEntity : GuidIdEntity
    {
        /// <summary>
        /// Get a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet("{id:guid}")]
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "id" })]
#endif
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public virtual async Task<ActionResult<TEntity>> GetByIdAsync(Guid id)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    UpdateRequest();
                    TEntity item = await GetQuery(i => (i.Id == id)).FirstOrDefaultAsync();
                    if (item is null)
                    {
                        SAEONLogs.Error("{id} not found", id);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
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
//[HttpGet("{id:guid}/TRelated")] Required in calling classes
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "id" })]
#endif
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        protected async Task<ActionResult<TRelated>> GetSingleAsync<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select) where TRelated : GuidIdEntity
        {
            using (SAEONLogs.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    if (!await GetQuery(i => (i.Id == id)).AnyAsync())
                    {
                        SAEONLogs.Error("{id} not found", id);
                        return NotFound();
                    }
                    return Ok(await GetQuery(i => (i.Id == id)).Select(select).FirstOrDefaultAsync());
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
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
//[HttpGet("{id:guid}/TRelated")] Required in calling classes
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "id" })]
#endif
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        protected TRelated GetSingle<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select) where TRelated : GuidIdEntity
        {
            using (SAEONLogs.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    if (!GetQuery(i => (i.Id == id)).Any())
                    {
                        SAEONLogs.Error("{id} not found", id);
                        throw new ArgumentException($"{id} not found");
                    }
                    return GetQuery(i => (i.Id == id)).Select(select).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
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
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "id" })]
#endif
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        protected IQueryable<TRelated> GetManyWithGuidId<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select) where TRelated : GuidIdEntity
        {
            using (SAEONLogs.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    return GetQuery(i => i.Id == id).SelectMany(select);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
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
//[HttpGet("{id:guid}/TRelated")] Required in calling classes
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "id" })]
#endif
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        protected IQueryable<TRelated> GetManyWithIntId<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select) where TRelated : IntIdEntity
        {
            using (SAEONLogs.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    return GetQuery(i => i.Id == id).SelectMany(select);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
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
//[HttpGet("{id:guid}/TRelated")] Required in calling classes
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "id" })]
#endif
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        protected IQueryable<TRelated> GetManyWithLongId<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select) where TRelated : LongIdEntity
        {
            using (SAEONLogs.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    return GetQuery(i => i.Id == id).SelectMany(select);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {id}", id);
                    throw;
                }
            }
        }

    }

    public abstract class CodedApiController<TEntity> : IDEntityApiController<TEntity> where TEntity : CodedEntity
    {
        protected override List<OrderBy<TEntity>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Insert(0, new OrderBy<TEntity>(i => i.Code));
            return result;
        }

        /// <summary>
        /// Get a TEntity by Code
        /// </summary>
        /// <param name="code">The Code of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet("ByCode/{code:required}")]
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "code" })]
#endif
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public virtual async Task<ActionResult<TEntity>> GetByCodeAsync(string code)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Code", code } }))
            {
                try
                {
                    UpdateRequest();
                    TEntity item = await GetQuery(i => (i.Code == code)).FirstOrDefaultAsync();
                    if (item is null)
                    {
                        SAEONLogs.Error("{code} not found", code);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {code}", code);
                    throw;
                }
            }
        }
    }

    public abstract class NamedApiController<TEntity> : IDEntityApiController<TEntity> where TEntity : NamedEntity
    {
        protected override List<OrderBy<TEntity>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Insert(0, new OrderBy<TEntity>(i => i.Name));
            return result;
        }

        /// <summary>
        /// Get a TEntity by Name
        /// </summary>
        /// <param name="name">The Name of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet("ByName/{name:required}")]
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "name" })]
#endif
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public virtual async Task<ActionResult<TEntity>> GetByNameAsync(string name)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Name", name } }))
            {
                try
                {
                    UpdateRequest();
                    TEntity item = await GetQuery(i => (i.Name == name)).FirstOrDefaultAsync();
                    if (item is null)
                    {
                        SAEONLogs.Error("{name} not found", name);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {name}", name);
                    throw;
                }
            }
        }
    }

    public abstract class CodedNamedApiController<TEntity> : IDEntityApiController<TEntity> where TEntity : CodedNamedEntity
    {
        protected override List<OrderBy<TEntity>> GetOrderBys()
        {
            var result = base.GetOrderBys();
            result.Insert(0, new OrderBy<TEntity>(i => i.Code));
            result.Insert(0, new OrderBy<TEntity>(i => i.Name));
            return result;
        }

        /// <summary>
        /// Get a TEntity by Code
        /// </summary>
        /// <param name="code">The Code of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet("ByCode/{code:required}")]
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "code" })]
#endif
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public virtual async Task<ActionResult<TEntity>> GetByCodeAsync(string code)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Code", code } }))
            {
                try
                {
                    UpdateRequest();
                    TEntity item = await GetQuery(i => (i.Code == code)).FirstOrDefaultAsync();
                    if (item is null)
                    {
                        SAEONLogs.Error("{code} not found", code);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {code}", code);
                    throw;
                }
            }
        }

        /// <summary>
        /// Get a TEntity by Name
        /// </summary>
        /// <param name="name">The Name of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet("ByName/{name:required}")]
#if ResponseCaching
        [ResponseCache(Duration = Defaults.CacheDuration, VaryByQueryKeys = new[] { "name" })]
#endif
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public virtual async Task<ActionResult<TEntity>> GetByNameAsync(string name)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Name", name } }))
            {
                try
                {
                    UpdateRequest();
                    TEntity item = await GetQuery(i => (i.Name == name)).FirstOrDefaultAsync();
                    if (item is null)
                    {
                        SAEONLogs.Error("{name} not found", name);
                        return NotFound();
                    }
                    return Ok(item);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get {name}", name);
                    throw;
                }
            }
        }
    }

}
