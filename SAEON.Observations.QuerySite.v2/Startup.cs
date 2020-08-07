using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAEON.Core;
using SAEON.Logs;
using System;

namespace SAEON.Observations.QuerySite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            try
            {
                SAEONLogs.Information("Starting {Application} LogLevel: {LogLevel}", ApplicationHelper.ApplicationName, SAEONLogs.Level);
                //SAEONLogs.Debug("AuthenticationServerUrl: {AuthenticationServerUrl} AuthenticationServerIntrospectionUrl: {AuthenticationServerIntrospectionUrl}",
                //    Configuration[Constants.AuthenticationServerUrl], Configuration[Constants.AuthenticationServerIntrospectionUrl]);
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex, "Unable to configure application");
                throw;
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    services.Configure<CookiePolicyOptions>(options =>
                    {
                        options.Secure = CookieSecurePolicy.Always;
                        options.HttpOnly = HttpOnlyPolicy.Always;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                        //options.OnAppendCookie = cookieContext =>
                        //    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                        //options.OnDeleteCookie = cookieContext =>
                        //    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                    });

                    services.AddControllersWithViews();
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to configure services");
                    throw;
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (SAEONLogs.MethodCall(GetType()))
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
                    }
                    app.UseStaticFiles();

                    app.UseRouting();

                    app.UseCookiePolicy();
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
                    SAEONLogs.Exception(ex, "Unable to configure application");
                    throw;
                }
            }
        }
    }
}

