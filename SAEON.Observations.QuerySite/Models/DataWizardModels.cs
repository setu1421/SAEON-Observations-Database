using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAEON.Observations.QuerySite.Models
{
    public class DataWizardModel : BaseModel
    {
        public List<UserQuery> UserQueries { get; } = new List<UserQuery>();
    }

    public class LoadQueryModel
    {
        [Required, StringLength(150)]
        public string Name { get; set; }
    }

    public class SaveQueryModel
    {
        [Required, StringLength(150)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
    }
}