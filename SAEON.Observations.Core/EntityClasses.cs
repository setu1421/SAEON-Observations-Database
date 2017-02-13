using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SAEON.Observations.Core
{
    /// <summary>
    /// Base for entities
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        //[Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false), HiddenInput]
        public Guid Id { get; set; }
        /// Name of the Entity
        /// </summary>
        [Required, StringLength(150)]
        public string Name { get; set; }
    }

    /// <summary>
    /// Instrument entity
    /// </summary>
    public class Instrument : BaseEntity
    {
        /// <summary>
        /// Code of the Instrument
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// <summary>
        /// Description of the Instrument
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Instrument
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        /// <summary>
        /// StartDate of the Instrument, null means always
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the Instrument, null means always
        /// </summary>
        public DateTime? EndDate { get; set; }

        // Navigation
        /// <summary>
        /// Stations linked to this Instrument
        /// </summary>
        public ICollection<Station> Stations { get; set; }
        /// <summary>
        /// Sensors linked to this Instrument
        /// </summary>
        public ICollection<Sensor> Sensors { get; set; }
    }

    /// <summary>
    /// Offering entity
    /// </summary>
    public class Offering : BaseEntity
    {
        /// <summary>
        /// Code of the Offering
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// <summary>
        /// Description of the Offering
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
    }

    /// <summary>
    /// Phenomenon entity
    /// </summary>
    public class Phenomenon : BaseEntity
    {
        /// <summary>
        /// Code of the Phenomenon
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// <summary>
        /// Description of the Phenomenon
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Phenomenon
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }

        // Navigation
        /// <summary>
        /// Sensors linked to this Phenomenon
        /// </summary>
        public ICollection<Sensor> Sensors { get; set; }

    }

    /// <summary>
    /// Sensor entity
    /// </summary>
    public class Sensor : BaseEntity
    {
        /// <summary>
        /// Code of the Sensor
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// <summary>
        /// Description of the Sensor
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Sensor
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }

        // Navigation
        /// <summary>
        /// Instruments linked to this Sensor
        /// </summary>
        public ICollection<Instrument> Instruments { get; set; }
        /// <summary>
        /// Phenomenon of the Sensor
        /// </summary>
        public Phenomenon Phenomenon { get; set; }
    }

    /// <summary>
    /// Site entity
    /// </summary>
    public class Site : BaseEntity
    {
        /// <summary>
        /// Code of the Site
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// <summary>
        /// Description of the Site
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// Url of the Site
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        /// <summary>
        /// StartDate of the Site, null means always
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the Site, null means always
        /// </summary>
        public DateTime? EndDate { get; set; }

        // Navigation

        /// <summary>
        /// The Stations linked to this Site
        /// </summary>
        public ICollection<Station> Stations { get; set; }
    }

    /// <summary>
    /// Station entity
    /// </summary>
    public class Station : BaseEntity
    {
        /// <summary>
        /// Code of the Station
        /// </summary>
        [Required, StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// <summary>
        /// Description of the Station
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// The SiteId of the Station
        /// </summary>
        [Required]
        public Guid SiteId { get; set; }
        /// <summary>
        /// Url of the Station
        /// </summary>
        [Url, StringLength(250)]
        public string Url { get; set; }
        /// <summary>
        /// StartDate of the site, null means always
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// EndDate of the Station, null means always
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Latitude of the Station
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// Logitude of the Station
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// Elevation of the Station, positive above sea level, negative below sea level
        /// </summary>
        public int? Elevation { get; set; }

        // Navigation

        /// <summary>
        /// Site of the Station
        /// </summary>
        public Site Site { get; set; }
        /// <summary>
        /// Instruments linked to this Station
        /// </summary>
        public ICollection<Instrument> Instruments { get; set; }
    }

    /// <summary>
    /// UnitOfMeasure Entity
    /// </summary>
    public class UnitOfMeasure : BaseEntity
    {
        /// <summary>
        /// Symbol of the Entity
        /// </summary>
        [Required, StringLength(20)]
        public string Symbol { get; set; }
    }

    /// <summary>
    /// UserDownload entity
    /// </summary>
    public class UserDownload : BaseEntity
    {
        /// <summary>
        /// <summary>
        /// Description of the UserDownload
        /// </summary>
        [StringLength(5000)]
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
        public string DownloadURI { get; set; }
        /// <summary>
        /// UserId of UserDownload
        /// </summary>
        [StringLength(128), ScaffoldColumn(false), HiddenInput]
        public string UserId { get; set; }
        /// <summary>
        /// UserId of user who added the UserDownload
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string AddedBy { get; set; }
        /// <summary>
        /// UserId of user who last updated the UserDownload
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
    }

    /// <summary>
    /// UserQuery entity
    /// </summary>
    public class UserQuery : BaseEntity
    {
        /// <summary>
        /// <summary>
        /// Description of the UserQuery
        /// </summary>
        [StringLength(5000)]
        public string Description { get; set; }
        /// <summary>
        /// URI of the user query
        /// </summary>
        [Required, StringLength(500)]
        public string QueryURI { get; set; }
        /// <summary>
        /// UserId of UserQuery
        /// </summary>
        [StringLength(128), ScaffoldColumn(false), HiddenInput]
        public string UserId { get; set; }
        /// <summary>
        /// UserId of user who added the UserQuery
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string AddedBy { get; set; }
        /// <summary>
        /// UserId of user who last updated the UserQuery
        /// </summary>
        [StringLength(128), ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
    }

    public class ObservationsDbContext : DbContext
    {
        public ObservationsDbContext() : base("Observations")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<Offering> Offerings { get; set; }
        public DbSet<Phenomenon> Phenomenons { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<UserDownload> UserDownloads { get; set; }
        public DbSet<UserQuery> UserQueries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Station>()
                .HasMany<Instrument>(l => l.Instruments)
                .WithMany(r => r.Stations)
                .Map(cs =>
                {
                    cs.MapLeftKey("StationID");
                    cs.MapRightKey("InstrumentID");
                    cs.ToTable("Station_Instrument");
                });
            modelBuilder.Entity<Instrument>()
                .HasMany<Sensor>(l => l.Sensors)
                .WithMany(r => r.Instruments)
                .Map(cs =>
                {
                    cs.MapLeftKey("InstrumentID");
                    cs.MapRightKey("SensorID");
                    cs.ToTable("Instrument_Sensor");
                });
            modelBuilder.Entity<UnitOfMeasure>().ToTable("UnitOfMeasure");
            modelBuilder.Entity<UnitOfMeasure>().Property(p => p.Name).HasColumnName("Unit");
            modelBuilder.Entity<UnitOfMeasure>().Property(p => p.Symbol).HasColumnName("UnitSymbol");
            modelBuilder.Entity<UserDownload>().ToTable("UserDownloads");
            modelBuilder.Entity<UserQuery>().ToTable("UserQueries");
            modelBuilder.Ignore<ApplicationUser>();
            modelBuilder.Ignore<IdentityRole>();
            modelBuilder.Ignore<IdentityUserClaim>();
            modelBuilder.Ignore<IdentityUserLogin>();
            modelBuilder.Ignore<IdentityUserRole>();
        }
    }

}
