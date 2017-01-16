using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAEON.Observations.WebAPI.Models
{
    /// <summary>
    /// UserQuery model
    /// </summary>
    public class UserQueryModel
    {
        /// <summary>
        /// Id of the user query
        /// </summary>
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// Name of the user query
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Description of the user query
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// URI of the user query
        /// </summary>
        [Required]
        public string URI { get; set; }
    }
}