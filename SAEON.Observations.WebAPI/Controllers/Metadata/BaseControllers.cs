using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SAEON.Observations.Core;

namespace SAEON.Observations.WebAPI.Controllers.Metadata
{
    [Route("[controller]/[action]")]
    [Authorize(Policy = TenantAuthenticationDefaults.TenantPolicy)]
    //[Authorize(Policy = ODPAuthenticationDefaults.AdminTokenPolicy)]
    public abstract class BaseController : Controller
    {
        private ObservationsDbContext _dbContext;
        protected ObservationsDbContext DbContext => _dbContext ??= HttpContext.RequestServices.GetRequiredService<ObservationsDbContext>();
    }
}
