using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.Core
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("Observations", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        private void AddUser(string name, string email, string password, string[] roles, UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByName(email) == null)
            {
                userManager.Create(new ApplicationUser { UserName = email, Email = email, Name = name, EmailConfirmed = true }, password);
            }
            var user = userManager.FindByName(email);
            if (user != null)
                foreach (var role in roles)
                {
                    if (!userManager.IsInRole(user.Id, role))
                    {
                        userManager.AddToRole(user.Id, role);
                    }
                }
        }

        public void Seed()
        {
            using (LogContext.PushProperty("Method", "Seed"))
            {
                Log.Verbose("Seeding database");
                try
                {
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this));
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this));
                    string[] roles = new string[] { "Administrator", "DataReader", "DataWriter", "QuerySite" };
                    foreach (var role in roles)
                    {
                        if (!roleManager.RoleExists(role))
                        {
                            roleManager.Create(new IdentityRole(role));
                        }
                    }
                    AddUser("Administrator", "observations@nimbusservices.co.za", "0d3DHCClCsAh", new string[] { "Administrator" }, userManager);
                    AddUser("Tim Parker-Nance", "tim@nimbusservices.co.za", "25m2Ue*9&E0i", new string[] { "Administrator" }, userManager);
                    AddUser("Query Site", "querysite@nimbusservices.co.za", "0583dUSVyuFs", new string[] { "QuerySite" }, userManager);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to seed database");
                    throw;
                }
            }
        }

        public System.Data.Entity.DbSet<SAEON.Observations.Core.UserDownload> UserDownloads { get; set; }

        public System.Data.Entity.DbSet<SAEON.Observations.Core.UserQuery> UserQueries { get; set; }
    }
    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext> { }

    public class ApplicationDbMigration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public ApplicationDbMigration()
            : base()
        {
            AutomaticMigrationsEnabled = true;
            //AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);
            context.Seed();
        }

    }
}
