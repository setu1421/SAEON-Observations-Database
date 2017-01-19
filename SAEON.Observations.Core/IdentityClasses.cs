using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace SAEON.Observations.Core
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    [ApiExplorerSettings(IgnoreApi = true)]
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

        private void AddUser(string name, string email, string password, string[] roles, UserManager<ApplicationUser> userManager)
        {
            var user = new ApplicationUser { UserName = email, Email = email, Name = name, EmailConfirmed = true };
            var result = userManager.Create(user, password);
            if (result.Succeeded)
                foreach (var role in roles)
                    userManager.AddToRole(user.Id, role);
        }

        protected override void Seed(ApplicationDbContext context)
        {
            using (LogContext.PushProperty("Method", "Seed"))
            {
                Log.Verbose("Seeding database");
                try
                {
                    base.Seed(context);
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                    string[] roles = new string[] { "Administrator", "DataReader", "DataWriter", "QuerySite" };
                    foreach (var role in roles)
                        if (!roleManager.RoleExists(role)) roleManager.Create(new IdentityRole(role));
                    AddUser("Administrator", "observations@saeon.ac.za.za", "0d3DHCClCsAh", new string[] { "Administrator" }, userManager);
                    AddUser("Tim Parker-Nance", "tim@nimbusservices.co.za", "TimPN1#", new string[] { "Administrator" }, userManager);
                    AddUser("Query Site", "observations@saeon.ac.za", "0583dUSVyuFs", new string[] { "QuerySite" }, userManager);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to seed database");
                    throw;
                }
            }
        }

    }
}
