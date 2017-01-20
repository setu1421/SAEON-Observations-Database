using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using SAEON.Observations.Core;
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
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Web API routes
            config.MapHttpAttributeRoutes();

            // API versioning
            config.AddApiVersioning(o => o.AssumeDefaultVersionWhenUnspecified = true);

            // Authentication
            //config.Filters.Add(new AuthorizeAttribute());

            // OData
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<UserDownload>("UserDownloads");
            builder.EntitySet<UserQuery>("UserQueries");
            builder.EntitySet<ApplicationUser>("Users");
            config.MapODataServiceRoute("OData", "OData", builder.GetEdmModel());
        }
    }
}
