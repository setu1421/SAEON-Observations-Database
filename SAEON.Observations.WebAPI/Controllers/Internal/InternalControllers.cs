using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    [Route("Internal/[controller]")]
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class InternalApiController : BaseApiController
    {
    }

    [Route("Internal/[controller]")]
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class InternalListController<TEntity> : BaseListController<TEntity> where TEntity : class
    {
    }

    [Route("Internal/[controller]")]
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class InternalReadController<TEntity> : BaseReadController<TEntity> where TEntity : BaseEntity
    {
        protected override void UpdateRequest()
        {
            EntityConfig.BaseUrl = Request.GetUri().GetLeftPart(UriPartial.Authority) + "/Internal";
        }
    }

    [Route("Internal/[controller]")]
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class InternalODataController<TEntity> : BaseODataController<TEntity> where TEntity : BaseEntity
    {
        protected override void UpdateRequest()
        {
            BaseUrl = Request.GetUri().GetLeftPart(UriPartial.Authority) + "/Internal";
        }
    }
}
