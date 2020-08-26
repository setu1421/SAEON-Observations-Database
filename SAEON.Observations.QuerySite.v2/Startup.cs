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
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core.Authentication;
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
                SAEONLogs.Exception(ex, "Unable to start application");
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
                    IdentityModelEventSource.ShowPII = true;

                    //services.AddCors();
                    services.Configure<CookiePolicyOptions>(options =>
                    {
                        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });
                    services.AddDistributedMemoryCache();
                    services.AddSession(options =>
                    {
                        options.Cookie.HttpOnly = false;
                        options.Cookie.IsEssential = true;
                    });
                    services
                        .AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                        })
                        .AddCookie()
                        .AddOpenIdConnect(options =>
                        {
                            options.Authority = Configuration["AuthenticationServerUrl"];
                            options.ClientId = ODPAuthenticationDefaults.QuerySiteClientId;
                            options.ClientSecret = Configuration["QuerySiteClientSecret"];
                            options.Scope.Clear();
                            options.Scope.Add(OpenIdConnectScope.OpenId);
                            //options.Scope.Add(OpenIdConnectScope.OfflineAccess);
                            options.Scope.Add(ODPAuthenticationDefaults.WebAPIClientId);
                            options.ResponseType = OpenIdConnectResponseType.Code;
                            options.SaveTokens = true;
                            options.GetClaimsFromUserInfoEndpoint = true;
                            SAEONLogs.Information("Options: {@Options}", options);
                            //options.Validate();
                        });
                    services.AddHttpContextAccessor();
                    services.AddHealthChecks()
                       .AddUrlGroup(new Uri(Configuration["AuthenticationServerHealthCheckUrl"]), "Authentication Server")
                       .AddUrlGroup(new Uri(Configuration["WebAPIHealthCheckUrl"]), "WebAPI");
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
                        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                        app.UseHsts();
                    }
                    app.UseHttpsRedirection();
                    app.UseStaticFiles();

                    app.UseCookiePolicy();
                    app.UseRouting();
                    //app.UseCors();
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

