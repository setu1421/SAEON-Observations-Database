using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public class ObservationsDbContext : DbContext
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContectAccessor;

        public ObservationsDbContext(DbContextOptions<ObservationsDbContext> options, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _config = config;
            _httpContectAccessor = httpContextAccessor;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<DataSchema> DataSchemas { get; set; }
        public DbSet<DataSource> DataSources { get; set; }
        public DbSet<DataSourceType> DataSourceTypes { get; set; }
        public DbSet<Dataset> Datasets { get; set; }
        public DbSet<VDatasetExpansion> DatasetExpansions { get; set; }
        public DbSet<DigitalObjectIdentifier> DigitalObjectIdentifiers { get; set; }
        public DbSet<ImportBatch> ImportBatches { get; set; }
        public DbSet<ImportBatchSummary> ImportBatchSummaries { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<InventorySnapshot> InventorySnapshots { get; set; }
        public DbSet<Offering> Offerings { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<OrganisationRole> OrganisationRoles { get; set; }
        public DbSet<Phenomenon> Phenomena { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Observation> Observations { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UserDownload> UserDownloads { get; set; }
        public DbSet<UserQuery> UserQueries { get; set; }

        // Views
        public DbSet<VLocation> VLocations { get; set; }
        public DbSet<VVariable> VVariables { get; set; }
        public DbSet<VImportBatchSummary> VImportBatchSummaries { get; set; }
        public DbSet<InventoryDataset> InventoryDatasets { get; set; }
        public DbSet<InventorySensor> InventorySensors { get; set; }
        public DbSet<VObservationExpansion> VObservationExpansions { get; set; }

        // SensorThings
        public DbSet<SensorThingsDatastream> SensorThingsDatastreams { get; set; }
        public DbSet<SensorThingsFeatureOfInterest> SensorThingsFeaturesOfInterest { get; set; }
        public DbSet<SensorThingsHistoricalLocation> SensorThingsHistoricalLocations { get; set; }
        public DbSet<SensorThingsLocation> SensorThingsLocations { get; set; }
        public DbSet<SensorThingsObservation> SensorThingsObservations { get; set; }
        public DbSet<SensorThingsObservedProperty> SensorThingsObservedProperies { get; set; }
        public DbSet<SensorThingsSensor> SensorThingsSensors { get; set; }
        public DbSet<SensorThingsThing> SensorThingsThings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            using (SAEONLogs.MethodCall(GetType()))
            {
                var tenant = TenantAuthenticationHandler.GetTenantFromHeaders(_httpContectAccessor.HttpContext.Request, _config);
                var connectionString = _config.GetConnectionString(tenant);
                optionsBuilder.UseSqlServer(connectionString, options =>
                {
                    options.EnableRetryOnFailure();
                });
                //SAEONLogs.Debug("Tenant: {Tenant} ConnectionString: {ConnectionString}", tenant, connectionString);
                base.OnConfiguring(optionsBuilder);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Dataset>().ToView("vStationDatasets");
            modelBuilder.Entity<VDatasetExpansion>().ToView("vDatasetsExpansion");
            modelBuilder.Entity<InventoryDataset>().ToView("vInventoryDatasets");
            modelBuilder.Entity<InventorySensor>().ToView("vInventorySensors");
            modelBuilder.Entity<VObservationExpansion>().ToView("vObservationExpansion");
            modelBuilder.Entity<DigitalObjectIdentifier>().Property("DOIType").HasConversion<byte>();
            //modelBuilder.Entity<DigitalObjectIdentifier>().HasOne(i => i.Parent).WithMany(i => i.Children).HasForeignKey(i => i.ParentId);
            // One to One
            modelBuilder.Entity<Dataset>().HasOne(i => i.DigitalObjectIdentifier).WithOne(i => i.Dataset).HasForeignKey<DigitalObjectIdentifier>(i => i.DatasetId);
            // Many to Many
            modelBuilder.Entity<Organisation>()
                .HasMany(i => i.Instruments)
                .WithMany(i => i.Organisations)
                .UsingEntity<OrganisationInstrument>(
                    oi => oi.HasOne(i => i.Instrument).WithMany().HasForeignKey(i => i.InstrumentId),
                    oi => oi.HasOne(i => i.Organisation).WithMany().HasForeignKey(i => i.OrganisationId))
                .ToTable("Organisation_Instrument")
                .HasKey(i => new { i.OrganisationId, i.InstrumentId });
            modelBuilder.Entity<Organisation>()
                .HasMany(i => i.Sites)
                .WithMany(i => i.Organisations)
                .UsingEntity<OrganisationSite>(
                    os => os.HasOne(i => i.Site).WithMany().HasForeignKey(i => i.SiteId),
                    os => os.HasOne(i => i.Organisation).WithMany().HasForeignKey(i => i.OrganisationId))
                .ToTable("Organisation_Site")
                .HasKey(i => new { i.OrganisationId, i.SiteId });
            modelBuilder.Entity<Organisation>()
                .HasMany(i => i.Stations)
                .WithMany(i => i.Organisations)
                .UsingEntity<OrganisationStation>(
                    os => os.HasOne(i => i.Station).WithMany().HasForeignKey(i => i.StationId),
                    os => os.HasOne(i => i.Organisation).WithMany().HasForeignKey(i => i.OrganisationId))
                .ToTable("Organisation_Station")
                .HasKey(i => new { i.OrganisationId, i.StationId });
            modelBuilder.Entity<Project>()
                .HasMany(i => i.Stations)
                .WithMany(i => i.Projects)
                .UsingEntity<ProjectStation>(
                    ps => ps.HasOne(i => i.Station).WithMany().HasForeignKey(i => i.StationId),
                    ps => ps.HasOne(i => i.Project).WithMany().HasForeignKey(i => i.ProjectId))
                .ToTable("Project_Station")
                .HasKey(i => new { i.ProjectId, i.StationId });
            modelBuilder.Entity<Station>()
                .HasMany(i => i.Instruments)
                .WithMany(i => i.Stations)
                .UsingEntity<StationInstrument>(
                    si => si.HasOne(i => i.Instrument).WithMany().HasForeignKey(i => i.InstrumentId),
                    si => si.HasOne(i => i.Station).WithMany().HasForeignKey(i => i.StationId))
                .ToTable("Station_Instrument")
                .HasKey(i => new { i.StationId, i.InstrumentId });
            modelBuilder.Entity<Instrument>()
                .HasMany(i => i.Sensors)
                .WithMany(i => i.Instruments)
                .UsingEntity<InstrumentSensor>(
                    si => si.HasOne(i => i.Sensor).WithMany().HasForeignKey(i => i.SensorId),
                    si => si.HasOne(i => i.Instrument).WithMany().HasForeignKey(i => i.InstrumentId))
                .ToTable("Instrument_Sensor")
                .HasKey(i => new { i.InstrumentId, i.SensorId });
            modelBuilder.Entity<Phenomenon>()
                .HasMany(i => i.Offerings)
                .WithMany(i => i.Phenomena)
                .UsingEntity<PhenomenonOffering>(
                    i => i.HasOne(i => i.Offering).WithMany().HasForeignKey(i => i.OfferingId),
                    i => i.HasOne(i => i.Phenomenon).WithMany().HasForeignKey(i => i.PhenomenonId))
                .ToTable("PhenomenonOffering")
                .HasKey(i => new { i.PhenomenonId, i.OfferingId });
            modelBuilder.Entity<Phenomenon>()
                .HasMany(i => i.Units)
                .WithMany(i => i.Phenomena)
                .UsingEntity<PhenomenonUnit>(
                    i => i.HasOne(i => i.Unit).WithMany().HasForeignKey(i => i.UnitId),
                    i => i.HasOne(i => i.Phenomenon).WithMany().HasForeignKey(i => i.PhenomenonId))
                .ToTable("PhenomenonUOM")
                .HasKey(i => new { i.PhenomenonId, i.UnitId });
            //SAEONLogs.Verbose("EFModel: {EFModel}", modelBuilder.Model.ToDebugString(MetadataDebugStringOptions.ShortDefault));
        }

        private void ValidateChanges()
        {
            var entities = from e in ChangeTracker.Entries()
                           where e.State == EntityState.Added
                               || e.State == EntityState.Modified
                           select e.Entity;
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(
                    entity,
                    validationContext,
                    validateAllProperties: true);
            }
        }

        public override int SaveChanges()
        {
            ValidateChanges();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ValidateChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ValidateChanges();
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
