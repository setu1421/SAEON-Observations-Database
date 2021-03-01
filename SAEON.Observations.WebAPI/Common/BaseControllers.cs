//#define ODPAuth
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;

namespace SAEON.Observations.WebAPI
{
    #region ApiControllers
    [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
#if ODPAuth
    [Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
#endif
    [ApiController]
    [ResponseCache(Duration = Defaults.CacheDuration)]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

    public abstract class BaseApiController : ControllerBase
    {
        private IConfiguration config;
        protected IConfiguration Config => config ??= HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        private ObservationsDbContext dbContext;
        protected ObservationsDbContext DbContext => dbContext ??= HttpContext.RequestServices.GetRequiredService<ObservationsDbContext>();
    }

    public abstract class BaseListController<TObject> : BaseApiController where TObject : class
    {
        protected virtual List<TObject> GetList()
        {
            var result = new List<TObject>();
            return result;
        }

        [HttpGet]
        public virtual List<TObject> GetAll()
        {
            using (SAEONLogs.MethodCall<TObject>(GetType()))
            {
                try
                {
                    return GetList();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }
    }

    public abstract class BaseReadController<TEntity> : BaseApiController where TEntity : BaseEntity
    {
        [NotMapped]
        public string EntitySetName { get; protected set; }
        [NotMapped]
        public List<string> NavigationLinks { get; } = new List<string>();

        //protected void UpdateRequest()
        //{
        //    EntityConfig.BaseUrl = Request.GetUri().GetLeftPart(UriPartial.Authority) + "/Api";
        //}
        protected abstract void UpdateRequest();

        protected virtual List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            return new List<Expression<Func<TEntity, object>>>();
        }

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

        [HttpGet]
        public virtual IQueryable<TEntity> GetAll()
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("Uri: {Uri}", Request.GetUri());
                    return GetQuery();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
    }

    /*
    public abstract class BaseApiEntityWriteController<TEntity> : BaseApiEntityController<TEntity> where TEntity : NamedEntity
    {
        protected IMapper Mapper { get; private set; }

        public BaseApiEntityWriteController() : base()
        {
            var config = new MapperConfiguration(cfg =>
            {
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

        //public int SaveChanges()
        //{
        //    try
        //    {
        //        return DbContext.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        SAEONLogs.Exception(ex, "Validation error");
        //        throw;
        //    }
        //}

        //public async Task<int> SaveChangesAsync()
        //{
        //    try
        //    {
        //        return await DbContext.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        SAEONLogs.Exception(ex, "Validation error");
        //        throw;
        //    }
        //}

        /// <summary>
        /// Create a TEntity
        /// </summary>
        /// <param name="item">The new TEntity </param>
        [HttpPost]
        //[Route] Required in derived classes
        public virtual async Task<ActionResult> Post([FromBody] TEntity item)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "item", item } }))
            {
                try
                {
                    if (item == null)
                    {
                        SAEONLogs.Error("item cannot be null");
                        return BadRequest("item cannot be null");
                    }
                    SAEONLogs.Verbose("Adding {Name} {@item}", item.Name, item);
                    if (!ModelState.IsValid)
                    {
                        SAEONLogs.Error("ModelState.Invalid {ModelStateErrors}", ModelStateErrors);
                        return BadRequest(ModelState);
                    }
                    if (!IsEntityOk(item, true))
                    {
                        SAEONLogs.Error("{Name} invalid", item.Name);
                        return BadRequest($"{item.Name} invalid");
                    }
                    try
                    {
                        SetEntity(ref item, true);
                        SAEONLogs.Verbose("Add {@item}", item);
                        DbContext.Set<TEntity>().Add(item);
                        await DbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateException ex)
                    {
                        if (await GetQuery().Where(i => i.Name == item.Name).AnyAsync())
                        {
                            SAEONLogs.Error("{Name} conflict", item.Name);
                            return Conflict();
                        }
                        else
                        {
                            SAEONLogs.Exception(ex, "Unable to add {Name}", item.Name);
                            return BadRequest(ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex, "Unable to add {Name}", item.Name);
                        return BadRequest($"Unable to add {item.Name}");
                    }
                    //var attr = (RoutePrefixAttribute)GetType().GetCustomAttributes(typeof(RoutePrefixAttribute), true)?[0];
                    //var location = $"{attr?.Prefix ?? typeof(TEntity).Name}/{item.Id}";
                    var location = $"{typeof(TEntity).Name}/{item.Id}";
                    SAEONLogs.Verbose("Location: {location} Id: {Id} Item: {@item}", location, item.Id, item);
                    return Created(location, item);
                    //return CreatedAtRoute(name, new { id = item.Id }, item);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to add {Name}", item.Name);
                    throw;
                }
            }
        }

        /// <summary>
        /// Update a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEnity</param>
        /// <param name="delta">The new TEntity</param>
        [HttpPut("id:guid")]
        public virtual async Task<ActionResult> PutById(Guid id, [FromBody] TEntity delta)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "id", id }, { "delta", delta } }))
            {
                try
                {
                    if (delta == null)
                    {
                        SAEONLogs.Error("delta cannot be null");
                        return BadRequest("delta cannot be null");
                    }
                    SAEONLogs.Verbose("Updating {id} {@delta}", id, delta);
                    if (!ModelState.IsValid)
                    {
                        SAEONLogs.Error("ModelState.Invalid {ModelStateErrors}", ModelStateErrors);
                        return BadRequest(ModelState);
                    }
                    if (id != delta.Id)
                    {
                        SAEONLogs.Error("{id} Id not same", id);
                        return BadRequest($"{id} Id not same");
                    }
                    if (!IsEntityOk(delta, false))
                    {
                        SAEONLogs.Error("{delta.Name} invalid", delta);
                        return BadRequest($"{delta.Name} invalid");
                    }
                    var item = await GetQuery().Where(i => i.Id == id).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        SAEONLogs.Error("{id} not found", id);
                        return NotFound();
                    }
                    try
                    {
                        //SAEONLogs.Verbose("Loaded {@item}", item);
                        Mapper.Map(delta, item);
                        //SAEONLogs.Verbose("Mapped delta {@item}", item);
                        SetEntity(ref item, false);
                        SAEONLogs.Verbose("Set {@item}", item);
                        await DbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex, "Unable to update {id}", id);
                        return BadRequest(ex.Message);
                    }
                    return NoContent();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to update {id}", id);
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete a TEntity by Id
        /// </summary>
        /// <param name="id">The Id of the TEntity</param>
        [HttpDelete("id:guid")]
        public virtual async Task<ActionResult> DeleteById(Guid id)
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType(), new MethodCallParameters { { "Id", id } }))
            {
                try
                {
                    SAEONLogs.Verbose("Deleting {id}", id);
                    var item = await GetQuery().Where(i => i.Id == id).FirstOrDefaultAsync();
                    if (item == null)
                    {
                        SAEONLogs.Error("{id} not found", id);
                        return NotFound();
                    }
                    try
                    {
                        DbContext.Set<TEntity>().Remove(item);
                        SAEONLogs.Verbose("Delete {@item}", item);
                        await DbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex, "Unable to delete {id}", id);
                        return BadRequest(ex.Message);
                    }
                    return NoContent();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to delete {id}", id);
                    throw;
                }
            }
        }
    }
    */
    #endregion

    #region ODataControllers
    [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
#if ODPAuth
    [Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
#endif
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = Defaults.CacheDuration)]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class BaseODataController<TEntity> : ODataController where TEntity : BaseEntity
    {
        public static string BaseUrl { get; set; }

        private ObservationsDbContext _dbContext;
        protected ObservationsDbContext DbContext => _dbContext ??= HttpContext.RequestServices.GetRequiredService<ObservationsDbContext>();

        protected virtual List<Expression<Func<TEntity, object>>> GetOrderBys()
        {
            return new List<Expression<Func<TEntity, object>>>();
        }

        /// <summary>
        /// query for items
        /// </summary>
        /// <returns></returns>
        protected IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> extraWhere = null)
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

        /*
        protected void UpdateRequest()
        {
            BaseUrl = Request.GetUri().GetLeftPart(UriPartial.Authority) + "/OData";
            //if (isMany)
            //{
            //    var uri = Request.RequestUri.ToString();
            //    var query = Request.RequestUri.Query.ToLowerInvariant();
            //    if (!query.Contains("$count=true"))
            //    {
            //        if (string.IsNullOrWhiteSpace(query))
            //            uri += "?$count = true";
            //        else
            //            uri += "&$count=true";
            //        Request.RequestUri = new Uri(uri);
            //    }
            //}
            Request.Headers.TryAdd("Prefer", "odata.include-annotations=*");
        }
        */

        protected abstract void UpdateRequest();

        [HttpGet]
        [EnableQuery(PageSize = ODataDefaults.PageSize, MaxTop = ODataDefaults.MaxTop)]
        public virtual ActionResult<IQueryable<TEntity>> Get()
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    UpdateRequest();
                    SAEONLogs.Verbose("uri: {uri}", Request.GetUri());
                    return Ok(GetQuery().Take(ODataDefaults.MaxAll));
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to get all");
                    throw;
                }
            }
        }
    }
    #endregion
}

