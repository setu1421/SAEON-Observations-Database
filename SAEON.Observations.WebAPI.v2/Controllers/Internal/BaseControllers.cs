using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SAEON.Logs;
using SAEON.Observations.Core.Authentication;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [Route("internal/[controller]")]
    [Authorize(AuthenticationSchemes = ODPAccessTokenAuthenticationDefaults.AuthenticationScheme + "," + TenantAuthenticationDefaults.AuthenticationScheme)]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class BaseController<TController> : ControllerBase where TController : BaseController<TController>
    {
        //protected IConfiguration Config => _config ?? (_config = HttpContext.RequestServices.GetService<IConfiguration>());
        private ObservationsDbContext _dbContext;
        protected ObservationsDbContext DbContext => _dbContext ??= HttpContext.RequestServices.GetService<ObservationsDbContext>();

        protected object GetUserInfo()
        {
            var result = new
            {
                User,
                User.Identity.IsAuthenticated,
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            };
            return result;
        }
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
