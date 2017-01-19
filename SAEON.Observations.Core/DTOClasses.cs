using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.Core
{
    /// <summary>
    /// UserDownload model
    /// </summary>
    public class UserDownloadDTO
    {
        /// <summary>
        /// Id of the user dowbload
        /// </summary>
        [Required]
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// UserId of user query
        /// </summary>
        [Required, StringLength(128)]
        public string UserId { get; set; }
        /// <summary>
        /// Name of the user download
        /// </summary>
        [Required, StringLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// Description of the user download
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
        /// <summary>
        /// The URI of the original query that generated the download
        /// </summary>
        [StringLength(500)]
        public string QueryURI { get; set; }
        /// <summary>
        /// URI of the user download
        /// </summary>
        [Required, StringLength(500)]
        public string URI { get; set; }

    }

    public class UserQueryDTO
    {
        /// <summary>
        /// Id of the user query
        /// </summary>
        [Required]
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// UserId of user query
        /// </summary>
        [Required, StringLength(128)]
        public string UserId { get; set; }
        /// <summary>
        /// Name of the user query
        /// </summary>
        [Required, StringLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// Description of the user query
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
        /// <summary>
        /// URI of the user query
        /// </summary>
        [Required, StringLength(500)]
        public string URI { get; set; }
    }

}
