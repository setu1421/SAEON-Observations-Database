using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;

namespace SAEON.Observations.QuerySite.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IConfiguration config;

        public BaseController(IHttpContextAccessor httpContextAccessor, IConfiguration config) : base()
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    this.config = config;
                    var httpContext = httpContextAccessor.HttpContext;
                    var tenant = httpContext.Session.GetString(TenantPolicyDefaults.HeaderKeyTenant);
                    if (string.IsNullOrWhiteSpace(tenant))
                    {
                        httpContext.Session.SetString(TenantPolicyDefaults.HeaderKeyTenant, config[TenantPolicyDefaults.ConfigKeyDefaultTenant]);
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
    }
}
