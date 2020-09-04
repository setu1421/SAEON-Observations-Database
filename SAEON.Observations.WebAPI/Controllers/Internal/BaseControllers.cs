using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [Route("Internal/[controller]")]
    [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AccessTokenPolicy)]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class BaseController : ControllerBase
    {
        //protected IConfiguration Config => _config ?? (_config = HttpContext.RequestServices.GetService<IConfiguration>());
        private ObservationsDbContext _dbContext;
        protected ObservationsDbContext DbContext => _dbContext ??= HttpContext.RequestServices.GetRequiredService<ObservationsDbContext>();
    }

    public abstract class BaseController<TEntity> : BaseController where TEntity : BaseEntity
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
        public virtual List<TEntity> GetAll()
        {
            using (SAEONLogs.MethodCall<TEntity>(GetType()))
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
}
