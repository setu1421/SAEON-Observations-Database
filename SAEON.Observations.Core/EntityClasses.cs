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
    public class UserDownload : UserDownloadDTO
    {
        public ApplicationUser User { get; set; }
    }

    public class UserQuery: UserQueryDTO
    {
        public ApplicationUser User { get; set; }
    }

    public class ObservationsDbContext : DbContext
    {
        public ObservationsDbContext() : base("Observations")
        { }

        public DbSet<UserDownload> UserDownloads { get; set; }
        public DbSet<UserQuery> UserQueries { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().HasKey(u => u.Id);
        }
    }

}
