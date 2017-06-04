using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
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
                //Logging.Verbose("Claims: {claims}", string.Join("; ", context.Principal.Claims.Select(i => i.Value)));
                if (!context.Principal.Identity.IsAuthenticated) return Nok();
                switch (context.Resource.First().Value)
                {
                    case "Inventory":
                    case "SpacialCoverage":
                    case "TemporalCoverage":
                        return AuthorizeAdmin(context);
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
                //Logging.Verbose("Claims: {claims}", string.Join("; ", context.Principal.Claims.Select(i => $"{i.Type}: {i.Value}")));
                switch (context.Action.First().Value)
                {
                    case "Observations.Admin":
                        return Eval(context.Principal.HasClaim("role", "Observations.Admin"));
                    default:
                        return Nok();
                }
            }
        }
    }
}