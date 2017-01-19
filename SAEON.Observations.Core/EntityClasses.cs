using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace SAEON.Observations.Core
{
    /// <summary>
    /// UserDownload model
    /// </summary>
    public class UserDownload
    {
        /// <summary>
        /// Id of the user dowbload
        /// </summary>
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// UserId of user query
        /// </summary>
        public string UserId { get; set; }
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

        // Navigation

        [ApiExplorerSettings(IgnoreApi = true)]
        public ApplicationUser User { get; set; }
    }

    public class UserQuery
    {
        /// <summary>
        /// Id of the user query
        /// </summary>
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// UserId of user query
        /// </summary>
        public string UserId { get; set; }
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

        // Navigation
        [ApiExplorerSettings(IgnoreApi = true)]
        public ApplicationUser User { get; set; }
    }

    public class ObservationsDbContext : DbContext
    {
        public ObservationsDbContext() : base("Observations")
        { }

        public DbSet<UserDownload> UserDownloads { get; set; }
        public DbSet<UserQuery> UserQueries { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
    }

}
