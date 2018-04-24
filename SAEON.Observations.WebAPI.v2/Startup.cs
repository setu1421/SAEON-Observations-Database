using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SAEON.AspNet.Core;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;

namespace SAEON.Observations.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Logging
                .CreateConfiguration("Logs/SAEON.Observations.WebAPI {Date}.txt", configuration)
                .Create();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            using (Logging.MethodCall(GetType()))
            {
                var connectionString = Configuration.GetConnectionString("Observations");
                services.AddDbContext<ObservationsDbContext>(options => options.UseSqlServer(connectionString, b => b.EnableRetryOnFailure()));
                //services.AddMvc();
                services.AddMvc(options =>
                    {
                        options.Filters.Add(typeof(SecurityHeadersAttribute));
                    })
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    });
                //services.AddLogging();
                services.AddCors();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (Logging.MethodCall(GetType()))
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseDatabaseErrorPage();
                    app.UseBrowserLink();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                }
                Logging.Information("Environment: {environment}", env.EnvironmentName);
                Logging.Information("ContentSecurityPolicy: {csp}", Configuration["ContentSecurityPolicy:Policy"]);

                app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
                app.UseStaticFiles();
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), "node_modules")),
                    RequestPath = "/node_modules"
                });

                //app.UseAuthentication();

                app.UseMvcWithDefaultRoute();
                //app.UseMvc(routes =>
                //{
                //    routes.MapRoute(
                //        name: "default",
                //        template: "{controller=Home}/{action=Index}/{id?}");
                //});

            }
        }
    }
}
