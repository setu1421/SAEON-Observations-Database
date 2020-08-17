using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SAEON.Core;
using SAEON.Logs;
using Syncfusion.Licensing;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace SAEON.Observations.QuerySite
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SyncfusionLicenseProvider.RegisterLicense("MTcxMDU4QDMxMzcyZTMzMmUzME1WOGhZUHRaaVVwUzAwaGpyMGhVbXdQazJrd2lnMmh1bi9FOTFSTHUwOFk9;MTcxMDU5QDMxMzcyZTMzMmUzMEtJN1pmeW9kMUFqaE5NQVFmZkxQN3VkdCt1UmZ2UFFEWERySzdrRnhsMlk9");

            //Logging.CreateConfiguration(configuration).Create();
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    Logging.Information("Starting {Application} LogLevel: {LogLevel}", ApplicationHelper.ApplicationName, Logging.Level);
                    Logging.Debug("IdentityServer: {name}", Configuration["IdentityServerUrl"]);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
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
                    services.AddControllersWithViews();
                    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
                    services.AddApplicationInsightsTelemetry();
                    services.AddDistributedMemoryCache();

                    services.AddSession(options =>
                    {
                        options.IdleTimeout = TimeSpan.FromSeconds(10 * 60);
                        options.Cookie.HttpOnly = true;
                        options.Cookie.IsEssential = true;
                    });

                    services.AddAuthentication(options =>
                        {
                            options.DefaultScheme = "Cookies";
                            options.DefaultChallengeScheme = "oidc";
                        })
                        .AddCookie("Cookies")
                        .AddOpenIdConnect("oidc", options =>
                        {
                            options.SignInScheme = "Cookies";
                            options.Authority = Configuration["IdentityServerUrl"];
                            options.RequireHttpsMetadata = false;
                            options.ClientId = "SAEON.Observations.QuerySite";
                            options.ClientSecret = "It6fWPU5J708";
                            options.ResponseType = "code id_token";
                            options.SaveTokens = true;
                            options.GetClaimsFromUserInfoEndpoint = true;
                            options.Scope.Add("SAEON.Observations.WebAPI");
                            options.Scope.Add("offline_access");
                            //options.ClaimActions.MapJsonKey("website", "website");
                            //options.SignedOutCallbackPath = "/Home/Index";
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
                        app.UseHttpsRedirection();
                    }
                    app.UseStaticFiles();
                    app.UseSession();
                    app.UseRouting();

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
