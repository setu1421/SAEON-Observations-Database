using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAEON.Logs;
using System;

namespace SAEON.Observations.WebAPI
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Logging.CreateConfiguration("Logs/SAEON.Identity.Service.txt", configuration).Create();

            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    Logging.Verbose("IdentityServer: {name}", Configuration["IdentityServerUrl"]);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to configure application");
                    throw;
                }
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    services.AddCors();
                    services.AddControllersWithViews();
                    services.AddApplicationInsightsTelemetry();
                    services.AddAuthentication("Bearer")
                         .AddJwtBearer("Bearer", options =>
                         {
                             options.Authority = Configuration["IdentityServerUrl"];
                             options.RequireHttpsMetadata = false;
                             options.Audience = "SAEON.Observations.WebAPI";
                         });
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    if (env.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                    else
                    {
                        app.UseExceptionHandler("/Home/Error");
                        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                        app.UseHsts();
                    }
                    app.UseHttpsRedirection();
                    app.UseStaticFiles();

                    app.UseRouting();
                    app.UseCors();

                    app.UseAuthentication();
                    app.UseAuthorization();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllerRoute(
                            name: "default",
                            pattern: "{controller=Home}/{action=Index}/{id?}");
                    });
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
    }
}
