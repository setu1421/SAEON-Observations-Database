using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;

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
    public abstract class InternalApiListController<TEntity> : BaseApiListController<TEntity> where TEntity : class
    {
    }

    [Route("Internal/[controller]")]
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class InternalApiEntityController<TEntity> : BaseApiEntityController<TEntity> where TEntity : BaseEntity
    {
    }

    [Route("Internal/[controller]")]
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class InternalMvcController : BaseMvcController
    {
    }

    [Route("Internal/[controller]")]
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowQuerySitePolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AllowedClientsPolicy)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class InternalODataController<TEntity> : BaseODataController<TEntity> where TEntity : BaseEntity
    {
    }
}
