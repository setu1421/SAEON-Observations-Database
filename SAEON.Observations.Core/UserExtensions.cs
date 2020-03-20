﻿using SAEON.AspNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SAEON.Observations.Core
{
    public static class UserExtensions
    {
        public static string GetUserId(this IPrincipal principal)
        {
            return (principal as ClaimsPrincipal).Claims.FirstOrDefault(c => c.Type.Equals(AspNetConstants.ClaimSubject, StringComparison.CurrentCultureIgnoreCase))?.Value;
        }

        public static string GetUserName(this IPrincipal principal)
        {
            return (principal as ClaimsPrincipal).Claims.FirstOrDefault(c => c.Type.Equals(AspNetConstants.ClaimUserName, StringComparison.CurrentCultureIgnoreCase))?.Value;
        }

        public static List<string> GetClaims(this IPrincipal principal)
        {
            return (principal as ClaimsPrincipal).Claims.Select(c => c.Type + ": " + c.Value).ToList();
        }
    }
}
