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
using System.Web.Http;
using System.Web.Http.Description;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    public abstract class BaseController : ApiController
    {
        protected ObservationsDbContext db = null;

        public BaseController() : base()
        {
            using (Logging.MethodCall(GetType()))
            {
                db = new ObservationsDbContext();
            }
        }

        ~BaseController()
        {
            db = null;
        }
    }

    public abstract class BaseEntityController<TEntity> : BaseController where TEntity : BaseEntity
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
        protected virtual List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            return new List<Expression<Func<TEntity, object>>>();
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
            var query = db.Set<TEntity>().AsQueryable();
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

    public abstract class IDEntityController<TEntity> : BaseEntityController<TEntity> where TEntity : IDEntity
    {
        /// <summary>
        /// Get a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        /// <returns>TEntity</returns>
        [HttpGet]
        [Route("{id:guid}")]
        //[ResponseType(typeof(TEntity))] required in derived classes
        public virtual async Task<IHttpActionResult> GetById([FromUri] Guid id)
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

        /// <summary>
        /// Related Entity TEntity.TRelated
        /// </summary>
        /// <typeparam name="TRelated"></typeparam>
        /// <param name="id">Id of TEntity</param>
        /// <param name="select">Lambda to select TRelated</param>
        /// <param name="include">Lamda to include TRelated.ListOf(TEntrity)</param>
        /// <returns>TaskOf(IHttpActionResult)</returns>
        [HttpGet]
        //[ResponseType(typeof(TRelated))] Required in derived classes
        //[Route("{id:guid}/TRelated")] Required in derived classes
        protected async Task<IHttpActionResult> GetSingle<TRelated>(Guid id, Expression<Func<TEntity, TRelated>> select, Expression<Func<TRelated, IEnumerable<TEntity>>> include) where TRelated : IDEntity
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
                    return Ok(await GetQuery(i => (i.Id == id)).Select(select).Include(include).FirstOrDefaultAsync());
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
        [HttpGet]
        //[Route("{id:guid}/TRelated")] Required in derived classes
        protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select, Expression<Func<TRelated, TEntity>> include) where TRelated : NamedEntity
        {
            using (Logging.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    return GetQuery(i => i.Id == id).SelectMany(select).Include(include);
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
        [HttpGet]
        //[Route("{id:guid}/TRelated")] Required in derived classes
        protected IQueryable<TRelated> GetMany<TRelated>(Guid id, Expression<Func<TEntity, IEnumerable<TRelated>>> select, Expression<Func<TRelated, IEnumerable<TEntity>>> include) where TRelated : NamedEntity
        {
            using (Logging.MethodCall<TEntity, TRelated>(GetType()))
            {
                try
                {
                    return GetQuery(i => i.Id == id).SelectMany(select).Include(include);
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
        public virtual async Task<IHttpActionResult> GetByName([FromUri] string name)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Name", name } }))
            {
                try
                {
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
        public virtual async Task<IHttpActionResult> GetByCode([FromUri] string code)
        {
            using (Logging.MethodCall<TEntity>(GetType(), new ParameterList { { "Code", code } }))
            {
                try
                {
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