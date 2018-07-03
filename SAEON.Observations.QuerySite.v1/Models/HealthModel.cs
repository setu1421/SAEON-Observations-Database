using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAEON.Observations.QuerySite.Models
{
    public class HealthModel
    {
        public string IdentityService { get; set; } = "Ok";
        public string WebApi { get; set; } = "Ok";
        public bool Healthy { get; set; } = true;
    }
}