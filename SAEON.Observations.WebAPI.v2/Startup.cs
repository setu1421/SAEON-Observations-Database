using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Mime;

namespace SAEON.Observations.WebAPI
{
    public class Startup
    {
        private IConfiguration _config { get; }

        public Startup(IConfiguration config)
        {
            _config = config;
            try
            {
                Logging.Information("Starting {Application} LogLevel: {LogLevel}", ApplicationHelper.ApplicationName, Logging.Level);
                Logging.Debug("IdentityServer: {name}", _config["IdentityServerUrl"]);
            }
            catch (Exception ex)
            {
                Logging.Exception(ex, "Unable to configure application");
                throw;
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
                    services.AddMvc().AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.IgnoreNullValues = true;
                    });
                    services.AddControllersWithViews();
                    services.AddApplicationInsightsTelemetry();
                    services.AddAuthentication("Bearer")
                         .AddJwtBearer("Bearer", options =>
                         {
                             options.Authority = _config["IdentityServerUrl"];
                             options.RequireHttpsMetadata = false;
                             options.Audience = "SAEON.Observations.WebAPI";
                         });
                    services.AddAuthorization(options =>
                        options.AddPolicy(Constants.TenantPolicy, policy => policy.AddRequirements(new TenantAuthorizationRequirement(_config[Constants.TenantTenants], _config[Constants.TenantDefault])))
                    );
                    services.AddHttpContextAccessor();
                    services.AddScoped<HttpContext>(p => p.GetService<IHttpContextAccessor>()?.HttpContext);
                    services.AddScoped<IAuthorizationHandler, TenantAuthorizationHandler>();
                    services.AddDbContext<ObservationsDbContext>();
                    services.AddHttpClient("IdentityServer", c =>
                    {
                        c.BaseAddress = new Uri(_config["IdentityServerUrl"]);
                        c.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
                    });
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
                    });
                    //services.AddOData();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to configure services");
                    throw;
                }
            }
        }

        private static IEdmModel GetODataEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder { ContainerName = "Observations" };
            builder.EntitySet<Instrument>("Instruments");
            return builder.GetEdmModel();
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
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SAEON.Observations.WebAPI");
                    });

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
                    Logging.Exception(ex, "Unable to configure application");
                    throw;
                }
            }
        }
    }
}
