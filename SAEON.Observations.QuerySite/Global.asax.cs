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
            Logging
                 .CreateConfiguration(HostingEnvironment.MapPath(@"~/App_Data/Logs/SAEON.Observations.QuerySite {Date}.txt"))
                 .Create();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    Logging.Verbose("Starting application");
                    Logging.Information("LogLevel: {LogLevel}", Logging.LogLevel);
                    BootStrapper.Initialize();
                    AreaRegistration.RegisterAllAreas();
                    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                    RouteConfig.RegisterRoutes(RouteTable.Routes);
                    BundleConfig.RegisterBundles(BundleTable.Bundles);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            Session[Constants.TenantSession] = ConfigurationManager.AppSettings[Constants.TenantDefault] ?? "Fynbos";
        }

    }
}
