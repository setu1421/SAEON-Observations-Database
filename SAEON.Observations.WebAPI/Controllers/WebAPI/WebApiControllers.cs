using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SAEON.Observations.Auth;

namespace SAEON.Observations.WebAPI.Controllers.WebAPI
{
    [Route("Api/[controller]")]
    [EnableCors(SAEONAuthenticationDefaults.CorsAllowAllPolicy)]
    public abstract class WebApiController<TEntity> : BaseApiEntityController<TEntity> where TEntity : BaseEntity
    {
    }
}
