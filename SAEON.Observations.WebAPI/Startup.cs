using HealthChecks.UI.Client;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Hubs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;


namespace SAEON.Observations.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            try
            {
                SAEONLogs.Information("Starting {Application} LogLevel: {LogLevel}", ApplicationHelper.ApplicationName, SAEONLogs.Level);
                SAEONLogs.Debug("AuthenticationServerUrl: {AuthenticationServerUrl} AuthenticationServerIntrospectionUrl: {AuthenticationServerIntrospectionUrl}",
                    Configuration["AuthenticationServerUrl"], Configuration["AuthenticationServerIntrospectionUrl"]);
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex, "Unable to start application");
                throw;
            }
        }

        protected IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                try
                {
                    services.AddCors(o => o.AddSAEONCorsPolicies());
                    services.AddResponseCompression(options => options.EnableForHttps = true);
#if ResponseCaching
                    services.AddResponseCaching();
#endif
                    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie();
                    services
                        .AddAuthentication()
                        .AddODPAuthentication(options =>
                        {
                            options.IntrospectionUrl = Configuration[ObsDBAuthenticationDefaults.ConfigKeyAuthenticationServerIntrospectionUrl];
                        })
                        .AddTenantAuthentication(options =>
                        {
                            options.Tenants = Configuration[TenantAuthenticationDefaults.ConfigKeyTenants];
                            options.DefaultTenant = Configuration[TenantAuthenticationDefaults.ConfigKeyDefaultTenant];
                        });
                    services
                         .AddAuthorization(options =>
                         {
                             options.AddODPPolicies();
                             options.AddTenantPolicy();
                             options.AddSAEONPolicies();
                         });

                    services.AddHttpContextAccessor();

                    services.AddHealthChecks()
                        .AddUrlGroup(new Uri(Configuration["AuthenticationServerHealthCheckUrl"]), "Authentication Server")
                        .AddUrlGroup(new Uri(Configuration["AuthenticationServerIntrospectionHealthCheckUrl"]), "Authentication Server Introspection")
                        .AddDbContextCheck<ObservationsDbContext>("ObservationsDatabase");

                    services.AddDbContext<ObservationsDbContext>();

                    services.AddSwaggerGen(options =>
                    {
                        options.SwaggerDoc("v1", new OpenApiInfo
                        {
                            Title = "SAEON.Observations.WebAPI",
                            Version = "v1",
                            Description = "SAEON Observations Database WebAPI",
                            Contact = new OpenApiContact { Name = "Tim Parker-Nance", Email = "timpn@saeon.ac.za", Url = new Uri("http://www.saeon.ac.za") },
                            License = new OpenApiLicense { Name = "Creative Commons Attribution-ShareAlike 4.0 International License", Url = new Uri("https://creativecommons.org/licenses/by-sa/4.0/") }

                        });
                        options.IncludeXmlComments("SAEON.Observations.WebAPI.xml");
                        options.SchemaFilter<SwaggerIgnoreFilter>();

                        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Flows = new OpenApiOAuthFlows
                            {
                                AuthorizationCode = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = new Uri(Configuration["AuthenticationServerUrl"].AddTrailingForwardSlash() + "oauth2/auth"),
                                    TokenUrl = new Uri(Configuration["AuthenticationServerUrl"].AddTrailingForwardSlash() + "oauth2/token"),
                                    Scopes = new Dictionary<string, string>
                                    {
                                        {"SAEON.Observations.WebAPI","" }
                                    }
                                },
                                ClientCredentials = new OpenApiOAuthFlow
                                {
                                    TokenUrl = new Uri(Configuration["AuthenticationServerUrl"].AddTrailingForwardSlash() + "oauth2/token"),
                                    Scopes = new Dictionary<string, string>
                                    {
                                        {"SAEON.Observations.WebAPI","" }
                                    }
                                },
                            }
                        });
                        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" },
                                },
                                new[] { "SAEON.Observations.WebAPI"}
                            }
                        });
                    });
                    services.AddSignalR();
                    services.Configure<ApiBehaviorOptions>(options =>
                    {
                        options.SuppressModelStateInvalidFilter = true;
                    });
                    services.AddControllers();
                    services.AddControllersWithViews();
                    services.AddOData();
                    SetODataFormatters(services);
                    services.AddApplicationInsightsTelemetry();
                    IFileProvider physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                    services.AddSingleton<IFileProvider>(physicalProvider);
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to configure services");
                    throw;
                }
            }


            void SetODataFormatters(IServiceCollection services)
            {
                services.AddMvcCore(options =>
                {
                    foreach (var formatter in options.OutputFormatters
                        .OfType<ODataOutputFormatter>()
                        .Where(it => !it.SupportedMediaTypes.Any()))
                    {
                        formatter.SupportedMediaTypes.Add(
                            new MediaTypeHeaderValue("application/odata"));
                    }
                    foreach (var formatter in options.InputFormatters
                        .OfType<ODataInputFormatter>()
                        .Where(it => !it.SupportedMediaTypes.Any()))
                    {
                        formatter.SupportedMediaTypes.Add(
                            new MediaTypeHeaderValue("application/odata"));
                    }
                });
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
                    app.UseResponseCaching();
                    app.UseResponseCompression();
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        OnPrepareResponse = ctx =>
                        {
                            ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = $"public,max-age={Defaults.CacheDuration}";
                        }
                    });
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "node_modules")),
                        RequestPath = "/node_modules",
                    });

                    var supportedCultures = new[]
                    {
                        new CultureInfo("en-GB")
                    };

                    app.UseRequestLocalization(new RequestLocalizationOptions
                    {
                        DefaultRequestCulture = new RequestCulture("en-GB"),
                        // Formatting numbers, dates, etc.
                        SupportedCultures = supportedCultures,
                        // UI strings that we have localized.
                        SupportedUICultures = supportedCultures
                    });

                    app.UseRouting();
                    //app.UseCors(SAEONAuthenticationDefaults.CorsAllowAllPolicy);
                    app.UseCors();
                    app.UseAuthentication();
                    app.UseAuthorization();
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SAEON.Observations.WebAPI");
                        options.OAuthClientId(Configuration["SwaggerClientId"]);
                        options.OAuthClientSecret(Configuration["SwaggerClientSecret"]);
                        options.OAuthScopes("SAEON.Observations.WebAPI");
                        options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
                    });

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                        {
                            Predicate = _ => true,
                            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                        });
                        endpoints.MapDefaultControllerRoute();
                        endpoints.MapControllers();
                        endpoints.MapHub<AdminHub>("/AdminHub").RequireCors(ObsDBAuthenticationDefaults.CorsAllowSignalRPolicy);
                        //endpoints.MapODataRoute("Internal", "Internal", GetInternalEdmModel()).Select().Filter().OrderBy().Count().Expand().MaxTop(ODataDefaults.MaxTop);
                        endpoints.MapODataRoute("OData", "OData", GetODataEdmModel()).Select().Filter().OrderBy().Count().Expand().MaxTop(ODataDefaults.MaxTop); ;
                    });
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex, "Unable to configure application");
                    throw;
                }
            }

            //IEdmModel GetInternalEdmModel()
            //{
            //    var builder = new ODataConventionModelBuilder() { ContainerName = "Internal" };
            //    builder.EntitySet<InventoryDataset>("InventoryDatasets");
            //    builder.EntitySet<InventorySensor>("InventorySensors");
            //    builder.EntitySet<Organisation>("Organisations");
            //    builder.EntitySet<Programme>("Programmes");
            //    builder.EntitySet<Project>("Projects");
            //    builder.EntitySet<Site>("Sites");
            //    builder.EntitySet<Station>("Stations");
            //    builder.EntitySet<Instrument>("Instruments");
            //    builder.EntitySet<Sensor>("Sensors");
            //    builder.EntitySet<Phenomenon>("Phenomena");
            //    builder.EntitySet<Offering>("Offerings");
            //    builder.EntitySet<Unit>("Units");
            //    builder.EntitySet<UserDownload>("UserDownloads");
            //    builder.EntitySet<UserQuery>("UserQueries");
            //    return builder.GetEdmModel();
            //}

            IEdmModel GetODataEdmModel()
            {
                var builder = new ODataConventionModelBuilder() { ContainerName = "OData" };
                builder.EntitySet<InventoryDataset>("InventoryDatasets");
                builder.EntitySet<InventorySensor>("InventorySensors");
                builder.EntitySet<Organisation>("Organisations");
                builder.EntitySet<Programme>("Programmes");
                builder.EntitySet<Project>("Projects");
                builder.EntitySet<Site>("Sites");
                builder.EntitySet<Station>("Stations");
                builder.EntitySet<Instrument>("Instruments");
                builder.EntitySet<Sensor>("Sensors");
                builder.EntitySet<Phenomenon>("Phenomena");
                builder.EntitySet<Offering>("Offerings");
                builder.EntitySet<Unit>("Units");
                return builder.GetEdmModel();
            }
        }
    }
}

