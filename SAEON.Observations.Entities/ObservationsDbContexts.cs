using SAEON.Observations.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.Entities
{
    public class ObservationsDbContext : DbContext
    {
        public ObservationsDbContext() : base("Observations")
        { }

        public DbSet<UserDownload> UserDownloads { get; set; }
        public DbSet<UserQuery> UserQueries { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
    }
}
