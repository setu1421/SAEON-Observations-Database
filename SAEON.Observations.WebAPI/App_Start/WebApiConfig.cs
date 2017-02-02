using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using SAEON.Observations.Core;
using System.Reflection;
using System.Linq.Expressions;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace SAEON.Observations.WebAPI
{
    public static class WebApiConfig
    {

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.Routes.MapHttpRoute(
                name: "ApiById",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Web API routes
            config.MapHttpAttributeRoutes();

            // OData
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Instrument>("Instruments");
            builder.EntitySet<Site>("Sites");
            builder.EntitySet<Station>("Stations");
            builder.EntitySet<UserDownload>("UserDownloads");
            builder.EntitySet<UserQuery>("UserQueries");
            builder.Ignore<ApplicationUser>();
            //builder.EntitySet<ApplicationUser>("Users");
            //builder.EntityType<ApplicationUser>().Ignore(c => c.AccessFailedCount);
            //builder.EntityType<ApplicationUser>().Ignore(c => c.Claims);
            //builder.EntityType<ApplicationUser>().Ignore(c => c.Email);
            //builder.EntityType<ApplicationUser>().Ignore(c => c.EmailConfirmed);
            //builder.EntityType<ApplicationUser>().Ignore(c => c.LockoutEnabled);
            //builder.EntityType<ApplicationUser>().Ignore(c => c.LockoutEndDateUtc);
            //builder.EntityType<ApplicationUser>().Ignore(c => c.Logins);
            //builder.EntityType<ApplicationUser>().Ignore(c => c.PasswordHash);
            //builder.EntityType<ApplicationUser>().Ignore(c => c.PhoneNumber);
            //builder.EntityType<ApplicationUser>().Ignore(c => c.PhoneNumberConfirmed);
            //builder.EntityType<ApplicationUser>().Ignore(c => c.Roles);
            //builder.EntityType<ApplicationUser>().Ignore(c => c.SecurityStamp);
            //builder.EntityType<ApplicationUser>().Ignore(c => c.TwoFactorEnabled);
            builder.Ignore<IdentityRole>();
            builder.Ignore<IdentityUserClaim>();
            builder.Ignore<IdentityUserLogin>();
            builder.Ignore<IdentityUserRole>();
            config.MapODataServiceRoute("OData", "OData", builder.GetEdmModel());
        }
    }
}
