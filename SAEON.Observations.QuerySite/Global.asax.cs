using SAEON.Core;
using SAEON.Logs;
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
            ApplicationHelper.ApplicationName = "SAEON.Observations.QuerySite";
            SAEONLogs
                 .CreateConfiguration(HostingEnvironment.MapPath(@"~/App_Data/Logs/SAEON.Observations.QuerySite-.log"))
                 .Initialize();
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    SAEONLogs.Information("Starting {Application} LogLevel: {LogLevel}", ApplicationHelper.ApplicationName, SAEONLogs.Level);
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

        protected void Session_Start(object sender, EventArgs e)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    var tenant = ConfigurationManager.AppSettings[Constants.ConfigKeyDefaultTenant] ?? "SAEON";
                    SAEONLogs.Verbose("Starting Session, Tenant: {Tenant}", tenant);
                    Session[Constants.SessionKeyTenant] = tenant;
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
