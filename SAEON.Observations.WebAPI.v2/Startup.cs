using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SAEON.Logs;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace SAEON.Observations.WebAPI.v2
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
                    Configuration = configuration;
                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
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
                    services.AddControllersWithViews();
                    services.AddApplicationInsightsTelemetry();
                    services.AddHttpClient();

                    services.AddSingleton<IDiscoveryCache>(r =>
                    {
                        var factory = r.GetRequiredService<IHttpClientFactory>();
                        return new DiscoveryCache(Configuration["IdentityServerUrl"], () => factory.CreateClient());
                    });
                    services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = "oidc";
                    })
                        .AddCookie(options =>
                        {
                            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                            options.Cookie.Name = "SAEON.Observations.WebAPI";
                        })
                        .AddOpenIdConnect("oidc", options =>
                        {
                            options.Authority = Configuration["IdentityServerUrl"];
                            options.RequireHttpsMetadata = false;

                            options.ClientSecret = "secret";
                            options.ClientId = "SAEON.Observations.WebAPI";

                            options.ResponseType = "code id_token";

                            options.Scope.Clear();
                            options.Scope.Add("openid");
                            options.Scope.Add("profile");
                            options.Scope.Add("email");
                            options.Scope.Add("SAEON.Observations.WebAPI");
                            options.Scope.Add("offline_access");

                            options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");

                            options.GetClaimsFromUserInfoEndpoint = true;
                            options.SaveTokens = true;

                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                NameClaimType = JwtClaimTypes.Name,
                                RoleClaimType = JwtClaimTypes.Role,
                            };
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
