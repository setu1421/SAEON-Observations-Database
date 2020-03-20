using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [Route("internal/[controller]")]
    [ApiController]
    [Authorize(Policy = Constants.TenantPolicy)]
    //[Authorize(Policy = Constants. ClientPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class BaseController<TController> : ControllerBase where TController : BaseController<TController>
    {
        private ILogger<TController> _logger;
        protected ILogger<TController> Logger => _logger ?? (_logger = HttpContext.RequestServices.GetService<ILogger<TController>>());
        //private IConfiguration _config;
        //protected IConfiguration Config => _config ?? (_config = HttpContext.RequestServices.GetService<IConfiguration>());
        private ObservationsDbContext _dbContext;
        protected ObservationsDbContext DbContext => _dbContext ?? (_dbContext = HttpContext.RequestServices.GetService<ObservationsDbContext>());
    }

    public abstract class BaseListController<TController, TEntity> : BaseController<TController> where TController : BaseController<TController> where TEntity : BaseEntity
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
            using (Logger.MethodCall<TEntity>(GetType()))
            {
                try
                {
                    return GetList();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, "Unable to get all");
                    throw;
                }
            }
        }
    }


}
