using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAEON.Observations.WebAPI.Models
{
    /// <summary>
    /// UserDownload model
    /// </summary>
    public class UserDownloadModel
    {
        /// <summary>
        /// Id of the user dowbload
        /// </summary>
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// Name of the user download
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Description of the user download
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The URI of the original query that generated the download
        /// </summary>
        public string QueryURI { get; set; }
        /// <summary>
        /// URI of the user download
        /// </summary>
        [Required]
        public string URI { get; set; }
    }
}