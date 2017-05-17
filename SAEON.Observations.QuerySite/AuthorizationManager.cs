using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace SAEON.Observations.QuerySite
{
    public class AuthorizationManager : ResourceAuthorizationManager
    {
        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
        {
            using (Logging.MethodCall(GetType()))
            {
                Logging.Verbose("Resource: {resource}", context.Resource.First().Value);
                Logging.Verbose("Claims: {claims}", string.Join("; ", context.Principal.Claims.Select(i => i.Value)));
                switch (context.Resource.First().Value)
                {
                    case "DataQueries":
                    case "DataDownloads":
                        return AuthorizeQuerySite(context);
                    case "DataGaps":
                        return AuthorizeAdmin(context);
                    default:
                        return Nok();
                }
            }
        }

        private Task<bool> AuthorizeQuerySite(ResourceAuthorizationContext context)
        {
            using (Logging.MethodCall(GetType()))
            {
                Logging.Verbose("Action: {action}", context.Action.First().Value);
                Logging.Verbose("Claims: {claims}", string.Join("; ", context.Principal.Claims.Select(i => i.Value)));
                switch (context.Action.First().Value)
                {
                    case "Observations.QuerySite":
                        return Eval(context.Principal.HasClaim("role", "Observations.QuerySite"));
                    default:
                        return Nok();
                }
            }
        }

        private Task<bool> AuthorizeAdmin(ResourceAuthorizationContext context)
        {
            using (Logging.MethodCall(GetType()))
            {
                Logging.Verbose("Action: {action}", context.Action.First().Value);
                Logging.Verbose("Claims: {claims}", string.Join("; ", context.Principal.Claims.Select(i => i.Value)));
                switch (context.Action.First().Value)
                {
                    case "Observations.DataGaps":
                        return Eval(context.Principal.HasClaim("role", "Observations.Admin"));
                    default:
                        return Nok();
                }
            }
        }
    }
}