using SAEON.Observations.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SAEON.Observations.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(Server.MapPath(@"~/App_Data/Logs/SAEON.Observations.WebAPI.Admin-{Date}.txt"))
                .WriteTo.Seq("http://localhost:5341/")
                .CreateLogger();
            try
            {
                Database.SetInitializer<ApplicationDbContext>(new MigrateDatabaseToLatestVersion<ApplicationDbContext, ApplicationDbMigration>());
                using (var context = new ApplicationDbContext())
                {
                    context.Database.Initialize(false);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to initialize database");
            }
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
