using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //modelBuilder.Entity<ApplicationUser>().Map(k => k.MapInheritedProperties());
            modelBuilder.Entity<ApplicationUser>().HasKey(k => k.Id);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.AccessFailedCount);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.Claims);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.Email);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.EmailConfirmed);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.LockoutEnabled);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.LockoutEndDateUtc);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.Logins);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.PasswordHash);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.PhoneNumber);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.PhoneNumberConfirmed);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.Roles);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.SecurityStamp);
            modelBuilder.Entity<ApplicationUser>().Ignore(c => c.TwoFactorEnabled);
            //modelBuilder.Entity<IdentityUser>().HasKey(k => k.Id);
            modelBuilder.Ignore<IdentityRole>();
            //modelBuilder.Entity<IdentityRole>().HasKey(k => k.Id);
            modelBuilder.Ignore<IdentityUserClaim>();
            //modelBuilder.Entity<IdentityUserClaim>().HasKey(k => k.Id);
            modelBuilder.Ignore<IdentityUserLogin>();
            //modelBuilder.Entity<IdentityUserLogin>().HasKey(k => new { k.LoginProvider, k.ProviderKey, k.UserId});
            modelBuilder.Ignore<IdentityUserRole>();
            //modelBuilder.Entity<IdentityUserRole>().HasKey(k => new { k.UserId, k.RoleId });
        }
    }

}
