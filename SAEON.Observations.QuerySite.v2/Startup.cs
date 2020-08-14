using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
                SAEONLogs.Debug("AuthenticationServerUrl: {AuthenticationServerUrl}", Configuration["AuthenticationServerUrl"]);
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
                    services.AddCors();
                    services.Configure<CookiePolicyOptions>(options =>
                    {
                        options.CheckConsentNeeded = context => true; // consent required
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });
                    //services.Configure<CookiePolicyOptions>(options =>
                    //{
                    //    options.Secure = CookieSecurePolicy.Always;
                    //    options.HttpOnly = HttpOnlyPolicy.Always;
                    //    options.MinimumSameSitePolicy = SameSiteMode.None;
                    //    //options.OnAppendCookie = cookieContext =>
                    //    //    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                    //    //options.OnDeleteCookie = cookieContext =>
                    //    //    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                    //});
                    services.AddDistributedMemoryCache();
                    services.AddSession(options =>
                    {
                        //options.IdleTimeout = TimeSpan.FromSeconds(10);
                        options.Cookie.HttpOnly = true;
                        options.Cookie.IsEssential = true;
                        options.Cookie.Name = ".SAEON.Observations.QuerySite";
                    });
                    services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    })
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = Configuration["AuthenticationServerUrl"];
                        options.ClientId = Configuration["QuerySiteClientID"];
                        options.ClientSecret = Configuration["QuerySiteClientSecret"];
                        options.Scope.Clear();
                        options.Scope.Add(OpenIdConnectScope.OpenId);
                        //options.Scope.Add(OpenIdConnectScope.OfflineAccess);
                        options.Scope.Add("SAEON.Observations.WebAPI");
                        options.ResponseType = OpenIdConnectResponseType.Code;
                        options.SaveTokens = true;
                        options.GetClaimsFromUserInfoEndpoint = true;
                        SAEONLogs.Information("Options: {@Options}", options);
                        //options.Validate();
                    });
                    services.AddHttpContextAccessor();
                    services.AddHealthChecks()
                       .AddUrlGroup(new Uri(Configuration["AuthenticationServerHealthCheckUrl"]), "AuthenticationServerUrl")
                       .AddUrlGroup(new Uri(Configuration["WebAPIHealthCheckUrl"]), "WebAPIUrl");
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
                    app.UseCors();
                    app.UseCookiePolicy();
                    app.UseAuthentication();
                    app.UseAuthorization();
                    app.UseSession();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                        {
                            Predicate = _ => true,
                            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                        });
                        endpoints.MapDefaultControllerRoute();
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

