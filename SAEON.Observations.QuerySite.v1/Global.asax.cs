using SAEON.AspNet.Common;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SAEON.Observations.QuerySite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            SAEONLogs
                 .CreateConfiguration(HostingEnvironment.MapPath(@"~/App_Data/Logs/SAEON.Observations.QuerySite {Date}.txt"))
                 .Initialize();
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Verbose("Starting application");
                    SAEONLogs.Information("LogLevel: {LogLevel}", SAEONLogs.Level);
                    BootStrapper.Initialize();
                    AreaRegistration.RegisterAllAreas();
                    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                    RouteConfig.RegisterRoutes(RouteTable.Routes);
                    BundleConfig.RegisterBundles(BundleTable.Bundles);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            Session[AspNetConstants.TenantSession] = ConfigurationManager.AppSettings[AspNetConstants.TenantDefault] ?? "Fynbos";
        }

    }
}
