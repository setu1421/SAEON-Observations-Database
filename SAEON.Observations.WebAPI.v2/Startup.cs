using AutoMapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;

namespace SAEON.Observations.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            try
            {
                Logging.Information("Starting {Application} LogLevel: {LogLevel}", ApplicationHelper.ApplicationName, Logging.Level);
                Logging.Debug("AuthServerUrl: {AuthServerUrl} AuthIntrospectionUrl: {AuthIntrospectionUrl}", Configuration[Constants.AuthServerUrl], Configuration[Constants.AuthIntrospectionUrl]);
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Unable to configure application");
                throw;
            }
        }

        protected IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    services.AddCors();
                    services.AddAutoMapper(typeof(Startup));
                    services.AddMvc().AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.IgnoreNullValues = true;
                    });
                    services.AddControllersWithViews();
                    services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
                    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie();
                    services.AddAuthorization(options =>
                    {
                        options.AddPolicy(Constants.TenantAuthorizationPolicy, policy => policy.AddRequirements(new TenantAuthorizationRequirement(Configuration[Constants.TenantTenants], Configuration[Constants.TenantDefault])));
                        options.AddPolicy(Constants.ODPAuthorizationPolicy, policy => policy.AddRequirements(new ODPAuthorizationRequirement()));
                    });
                    services.AddScoped<IAuthorizationHandler, TenantAuthorizationHandler>();
                    //services.AddScoped<IAuthorizationHandler, ODPAuthorizationHandler>();
                    services.AddHttpContextAccessor();
                    services.AddScoped<HttpContext>(p => p.GetService<IHttpContextAccessor>()?.HttpContext);
                    services.AddHealthChecks()
                        .AddUrlGroup(new Uri(Configuration[Constants.AuthServerHealthCheckUrl]), Constants.AuthServerUrl)
                        .AddUrlGroup(new Uri(Configuration[Constants.AuthIntrospectionHealthCheckUrl]), Constants.AuthIntrospectionUrl)
                        .AddDbContextCheck<ObservationsDbContext>("ObservationsDatabase");
                    services.AddDbContext<ObservationsDbContext>();
                    services.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new OpenApiInfo
                        {
                            Title = "SAEON.Observations.WebAPI",
                            Version = "v1",
                            Description = "SAEON Observations Database WebAPI",
                            Contact = new OpenApiContact { Name = "Tim Parker-Nance", Email = "timpn@saeon.ac.za", Url = new Uri("http://www.saeon.ac.za") },
                            License = new OpenApiLicense { Name = "Creative Commons Attribution-ShareAlike 4.0 International License", Url = new Uri("https://creativecommons.org/licenses/by-sa/4.0/") }

                        });
                        c.IncludeXmlComments("SAEON.Observations.WebAPI.v2.xml");
                        c.SchemaFilter<SwaggerIgnoreFilter>();
                    });
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to configure services");
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

                    app.UseRouting();
                    app.UseCors();

                    app.UseAuthentication();
                    app.UseAuthorization();

                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SAEON.Observations.WebAPI");
                    });

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                        {
                            Predicate = _ => true,
                            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                        });
                        endpoints.MapControllerRoute(
                            name: "default",
                            pattern: "{controller=Home}/{action=Index}/{id?}");
                    });
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to configure application");
                    throw;
                }
            }
        }
    }
}

