using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Hubs;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class DOIHelper
    {
        private static readonly string blankJson = "{}";
        private static readonly string blankHtml = "<>";
        private static readonly string blankTitle = "Title";

        public static readonly string ObservationsDbCode = "ObservationsDB";
        public static readonly string DynamicDOIsCode = "DynamicDOIs";
        public static readonly string PeriodicDOIsCode = "PeriodicDOIs";
        public static readonly string AdHocDOIsCode = "AddHocDOIs";

        public static async Task<DigitalObjectIdentifier> CreateAdHocDOI(ObservationsDbContext dbContext, HttpContext httpContext, string code, string name)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper)))
            {
                try
                {
                    var doiAdHocs = await dbContext.DigitalObjectIdentifiers.SingleAsync(i => i.DOIType == DOIType.Collection && i.Code == AdHocDOIsCode);
                    var doi = await AddDOI(DOIType.AdHoc, code, name, doiAdHocs);
                    return doi;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }

            async Task<DigitalObjectIdentifier> AddDOI(DOIType doiType, string code, string name, DigitalObjectIdentifier parent)
            {
                SAEONLogs.Information("Adding {doiType} {code}, {name}", DOIType.AdHoc, code, name);
                var doi = new DigitalObjectIdentifier
                {
                    Parent = parent,
                    DOIType = doiType,
                    Code = code,
                    Name = name ?? code,
                    Title = blankTitle,
                    MetadataJson = blankJson,
                    MetadataJsonSha256 = blankJson.Sha256(),
                    MetadataUrl = "https://catalogue.saeon.ac.za/records/",
                    MetadataHtml = blankHtml,
                    QueryUrl = "https://observations.saeon.ac.za/",
                    AddedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString(),
                    UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString()
                };
                await dbContext.DigitalObjectIdentifiers.AddAsync(doi);
                await dbContext.SaveChangesAsync();
                doi.SetUrls();
                SAEONLogs.Verbose("DOI: {@DOI}", doi);
                await dbContext.SaveChangesAsync();
                return doi;
            }
        }

        public static async Task<string> CreateDOIs(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub, HttpContext httpContext)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper)))
            {
                try
                {
                    var sb = new StringBuilder();
                    await GenerateDOIs();
                    await AddLineAsync("Done");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateDOIsStatusUpdate, line);
                    }

                    async Task GenerateDOIs()
                    {
                        async Task<DigitalObjectIdentifier> AddDOI(DOIType doiType, string code, string name, DigitalObjectIdentifier parent)
                        {
                            await AddLineAsync($"Adding {doiType} {code}, {name}");
                            var doi = new DigitalObjectIdentifier
                            {
                                Parent = parent,
                                DOIType = doiType,
                                Code = code,
                                Name = name ?? code,
                                Title = blankTitle,
                                MetadataJson = blankJson,
                                MetadataJsonSha256 = blankJson.Sha256(),
                                MetadataUrl = "https://catalogue.saeon.ac.za/records/",
                                MetadataHtml = blankHtml,
                                QueryUrl = "https://observations.saeon.ac.za/",
                                AddedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString(),
                                UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString()
                            };
                            await dbContext.DigitalObjectIdentifiers.AddAsync(doi);
                            await dbContext.SaveChangesAsync();
                            doi.SetUrls();
                            await dbContext.SaveChangesAsync();
                            return doi;
                        }

                        async Task<(DigitalObjectIdentifier doiDynamics, DigitalObjectIdentifier doiPeriodics)> EnsureCollectionsDOIs()
                        {
                            var doiObservations = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.ObservationsDb);
                            if (doiObservations == null)
                            {
                                doiObservations = await AddDOI(DOIType.ObservationsDb, "ObservationsDB", "Observations Database", null);
                                await dbContext.SaveChangesAsync();
                            }
                            var doiDynamics = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => (i.DOIType == DOIType.Collection) && (i.Code == DynamicDOIsCode));
                            if (doiDynamics == null)
                            {
                                doiDynamics = await AddDOI(DOIType.Collection, DynamicDOIsCode, "Observations Database Dynamic DOIs", doiObservations);
                                await dbContext.SaveChangesAsync();
                            }
                            var doiPeriodics = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => (i.DOIType == DOIType.Collection) && (i.Code == PeriodicDOIsCode));
                            if (doiPeriodics == null)
                            {
                                doiPeriodics = await AddDOI(DOIType.Collection, PeriodicDOIsCode, "Observations Database Periodic DOIs", doiObservations);
                                await dbContext.SaveChangesAsync();
                            }
                            var doiAdHocs = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => (i.DOIType == DOIType.Collection) && (i.Code == AdHocDOIsCode));
                            if (doiAdHocs == null)
                            {
                                doiAdHocs = await AddDOI(DOIType.Collection, AdHocDOIsCode, "Observations Database AdHoc DOIs", doiObservations);
                                await dbContext.SaveChangesAsync();
                            }
                            return (doiDynamics, doiPeriodics);
                        }

                        async Task<DigitalObjectIdentifier> EnsureOrganisationDOI(DigitalObjectIdentifier doiCollectionDynamics, Organisation organisation)
                        {
                            var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Organisation && i.Code == organisation.Code);
                            if (doi == null)
                            {
                                doi = await AddDOI(DOIType.Organisation, organisation.Code, organisation.Name, doiCollectionDynamics);
                                organisation.DigitalObjectIdentifier = doi;
                                await dbContext.SaveChangesAsync();
                            }
                            return doi;
                        }

                        IQueryable<VImportBatchSummaries> GetImportBatches()
                        {
                            return dbContext.VImportBatchSummary
                                .Where(i =>
                                    i.LatitudeNorth.HasValue && i.LatitudeSouth.HasValue &&
                                    i.LongitudeWest.HasValue && i.LongitudeEast.HasValue &&
                                    i.Count > 0)
                                .OrderBy(i => i.OrganisationName)
                                .ThenBy(i => i.ProgrammeName)
                                .ThenBy(i => i.ProjectName)
                                .ThenBy(i => i.SiteName)
                                .ThenBy(i => i.StationName)
                                .ThenBy(i => i.PhenomenonName)
                                .ThenBy(i => i.OfferingName)
                                .ThenBy(i => i.UnitName)
                                .ThenBy(i => i.StartDate);
                        }

                        async Task<DigitalObjectIdentifier> EnsureProgrammeDOI(DigitalObjectIdentifier doiOrganisation, Programme programme)
                        {
                            var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Programme && i.Code == programme.Code);
                            if (doi == null)
                            {
                                doi = await AddDOI(DOIType.Programme, programme.Code, programme.Name, doiOrganisation);
                                programme.DigitalObjectIdentifier = doi;
                                await dbContext.SaveChangesAsync();
                            }
                            return doi;
                        }

                        async Task<DigitalObjectIdentifier> EnsureProjectDOI(DigitalObjectIdentifier doiProgramme, Project project)
                        {
                            var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Project && i.Code == project.Code);
                            if (doi == null)
                            {
                                doi = await AddDOI(DOIType.Project, project.Code, project.Name, doiProgramme);
                                project.DigitalObjectIdentifier = doi;
                                await dbContext.SaveChangesAsync();
                            }
                            return doi;
                        }

                        async Task<DigitalObjectIdentifier> EnsureSiteDOI(DigitalObjectIdentifier doiProject, Site site)
                        {
                            var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Site && i.Code == site.Code);
                            if (doi == null)
                            {
                                doi = await AddDOI(DOIType.Site, site.Code, site.Name, doiProject);
                                site.DigitalObjectIdentifier = doi;
                                await dbContext.SaveChangesAsync();
                            }
                            return doi;
                        }

                        async Task<DigitalObjectIdentifier> EnsureStationDOI(DigitalObjectIdentifier doiSite, Station station)
                        {
                            var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Station && i.Code == station.Code);
                            if (doi == null)
                            {
                                doi = await AddDOI(DOIType.Station, station.Code, station.Name, doiSite);
                                station.DigitalObjectIdentifier = doi;
                                await dbContext.SaveChangesAsync();
                            }
                            return doi;
                        }

                        async Task<DigitalObjectIdentifier> EnsureDatasetDOI(DigitalObjectIdentifier doiStation, Station station, Dataset dataset)
                        {
                            var code = $"{station.Code}~{dataset.PhenomenonCode}~{dataset.OfferingCode}~{dataset.UnitCode}";
                            var name = $"{station.Name}, {dataset.PhenomenonName}, {dataset.OfferingName}, {dataset.UnitName}";
                            var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Dataset && i.Code == code);
                            if (doi == null)
                            {
                                doi = await AddDOI(DOIType.Dataset, code, name, doiStation);
                                //dataset.DigitalObjectIdentifier = doi;
                                await dbContext.SaveChangesAsync();
                            }
                            return doi;
                        }

                        async Task<DigitalObjectIdentifier> EnsureImportBatchSummaryDOI(DigitalObjectIdentifier doiDataset, ImportBatchSummary importBatchSummary)
                        {
                            var instrument = await dbContext.Instruments.SingleAsync(i => i.Id == importBatchSummary.InstrumentId);
                            var sensor = await dbContext.Sensors.SingleAsync(i => i.Id == importBatchSummary.SensorId);
                            var code = $"{doiDataset.Code}~{importBatchSummary.Id}";
                            var name = $"{doiDataset.Name}, {instrument.Name}, {sensor.Name}, {importBatchSummary.ImportBatch.Code}, {importBatchSummary.StartDate.ToJsonDate()} to {importBatchSummary.EndDate.ToJsonDate()}";
                            var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Periodic && i.Code == code);
                            if (doi == null)
                            {
                                doi = await AddDOI(DOIType.Periodic, code, name, doiDataset);
                                importBatchSummary.DigitalObjectIdentifier = doi;
                                await dbContext.SaveChangesAsync();
                            }
                            return doi;
                        }

                        //async Task<DigitalObjectIdentifier> EnsureImportBatchDOI(DigitalObjectIdentifier doiPeriodics, ImportBatch importBatch)
                        //{
                        //    var code = $"{importBatch.Code}~{importBatch.DataSource.Code}";
                        //    var name = $"{importBatch.Code}, {importBatch.DataSource.Name}";
                        //    var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Periodic && i.Code == code);
                        //    if (doi == null)
                        //    {
                        //        doi = await AddDOI(DOIType.Periodic, code, name, doiPeriodics);
                        //        await dbContext.SaveChangesAsync();
                        //    }
                        //    return doi;
                        //}

                        await AddLineAsync("Generating DOIs");
                        // Ensure Collection DOIs exists
                        var (doiDynamicDOIs, doiPeriodics) = await EnsureCollectionsDOIs();
                        // Ensure SAEON DOI exists
                        var orgSAEON = await dbContext.Organisations.Where(i => i.Code == "SAEON").FirstOrDefaultAsync();
                        if (orgSAEON != null)
                        {
                            await EnsureOrganisationDOI(doiDynamicDOIs, orgSAEON);
                        }
                        // We only create Dynamic DOIs for SAEON, SMCRI and EFTEON
                        var orgCodes = new string[] { "SAEON", "SMCRI", "EFTEON" };
                        foreach (var organisation in await dbContext.Organisations.Where(i => orgCodes.Contains(i.Code)).OrderBy(i => i.Name).ToListAsync())
                        {
                            var doiOrganisation = await EnsureOrganisationDOI(doiDynamicDOIs, organisation);
                            var programmeCodes = GetImportBatches()
                                .Where(i => i.OrganisationCode == organisation.Code)
                                .Select(i => i.ProgrammeCode)
                                .Distinct();
                            foreach (var programme in await dbContext.Programmes
                                .Where(i => programmeCodes.Contains(i.Code))
                                .ToListAsync())
                            {
                                var doiProgramme = await EnsureProgrammeDOI(doiOrganisation, programme);
                                var projectCodes = GetImportBatches()
                                    .Where(i =>
                                        i.OrganisationCode == organisation.Code &&
                                        i.ProgrammeCode == programme.Code)
                                    .Select(i => i.ProjectCode)
                                    .Distinct();
                                foreach (var project in await dbContext.Projects
                                    .Where(i => projectCodes.Contains(i.Code))
                                    .ToListAsync())
                                {
                                    var doiProject = await EnsureProjectDOI(doiProgramme, project);
                                    var siteCodes = GetImportBatches()
                                        .Where(i =>
                                            i.OrganisationCode == organisation.Code &&
                                            i.ProgrammeCode == programme.Code &&
                                            i.ProjectCode == project.Code)
                                        .Select(i => i.SiteCode)
                                        .Distinct();
                                    foreach (var site in await dbContext.Sites
                                        .Where(i => siteCodes.Contains(i.Code))
                                       .ToListAsync())
                                    {
                                        var doiSite = await EnsureSiteDOI(doiProject, site);
                                        var stationCodes = GetImportBatches()
                                            .Where(i =>
                                                i.OrganisationCode == organisation.Code &&
                                                i.ProgrammeCode == programme.Code &&
                                                i.ProjectCode == project.Code &&
                                                i.SiteCode == site.Code)
                                            .Select(i => i.StationCode)
                                            .Distinct();
                                        foreach (var station in await dbContext.Stations.Where(i => stationCodes.Contains(i.Code))
                                            .ToListAsync())
                                        {
                                            var doiStation = await EnsureStationDOI(doiSite, station);
                                            foreach (var dataset in await dbContext.Datasets.Where(i => i.StationCode == station.Code).ToListAsync())
                                            {
                                                var doiDataset = await EnsureDatasetDOI(doiStation, station, dataset);
                                                var importBatchSummaryIds = GetImportBatches()
                                                    .Where(i =>
                                                        i.OrganisationCode == organisation.Code &&
                                                        i.ProgrammeCode == programme.Code &&
                                                        i.ProjectCode == project.Code &&
                                                        i.SiteCode == site.Code &&
                                                        i.StationCode == station.Code &&
                                                        i.PhenomenonCode == dataset.PhenomenonCode &&
                                                        i.OfferingCode == dataset.OfferingCode &&
                                                        i.UnitCode == dataset.UnitCode)
                                                    .Select(i => i.Id);
                                                foreach (var importBatchSummary in await dbContext.ImportBatchSummaries
                                                    .Include(i => i.ImportBatch)
                                                    .Where(i => importBatchSummaryIds.Contains(i.Id))
                                                    .ToListAsync())
                                                {
                                                    var doiImportBatchSummary = await EnsureImportBatchSummaryDOI(doiDataset, importBatchSummary);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        await dbContext.SaveChangesAsync();
                        // Periodics
                        //foreach (var importBatch in dbContext.ImportBatches.OrderBy(i => i.Code))
                        //{
                        //    var doiImportBatch = EnsureImportBatchDOI(doiPeriodics, importBatch);
                        //}
                        await AddLineAsync("Setting Urls");
                        foreach (var doi in await dbContext.DigitalObjectIdentifiers.ToListAsync())
                        {
                            //await AddLineAsync($"{doi.DOIType} {doi.Name}");
                            doi.SetUrls();
                        }
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }

        public static async Task<string> CreateDOIsV2(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub, HttpContext httpContext)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper)))
            {
                try
                {
                    var sb = new StringBuilder();
                    await GenerateDOIs();
                    await AddLineAsync("Done");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateDOIsStatusUpdate, line);
                    }

                    async Task GenerateDOIs()
                    {
                        async Task<DigitalObjectIdentifier> AddDOI(DOIType doiType, string code, string name, DigitalObjectIdentifier parent)
                        {
                            await AddLineAsync($"Adding {doiType} {code}, {name}");
                            var doi = new DigitalObjectIdentifier
                            {
                                Parent = parent,
                                DOIType = doiType,
                                Code = code,
                                Name = name ?? code,
                                Title = blankTitle,
                                MetadataJson = blankJson,
                                MetadataJsonSha256 = blankJson.Sha256(),
                                MetadataUrl = "https://catalogue.saeon.ac.za/records/",
                                MetadataHtml = blankHtml,
                                QueryUrl = "https://observations.saeon.ac.za/",
                                AddedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString(),
                                UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString()
                            };
                            await dbContext.DigitalObjectIdentifiers.AddAsync(doi);
                            await dbContext.SaveChangesAsync();
                            doi.SetUrls();
                            await dbContext.SaveChangesAsync();
                            return doi;
                        }

                        IQueryable<VImportBatchSummaries> GetImportBatchSummaries()
                        {
                            return dbContext.VImportBatchSummary
                                .Where(i =>
                                    i.LatitudeNorth.HasValue && i.LatitudeSouth.HasValue &&
                                    i.LongitudeWest.HasValue && i.LongitudeEast.HasValue &&
                                    i.VerifiedCount > 0)
                                .OrderBy(i => i.OrganisationName)
                                .ThenBy(i => i.ProgrammeName)
                                .ThenBy(i => i.ProjectName)
                                .ThenBy(i => i.SiteName)
                                .ThenBy(i => i.StationName)
                                .ThenBy(i => i.PhenomenonName)
                                .ThenBy(i => i.OfferingName)
                                .ThenBy(i => i.UnitName)
                                .ThenBy(i => i.StartDate);
                        }

                        async Task<DigitalObjectIdentifier> EnsureDatasetDOI(Station station, Dataset dataset)
                        {
                            var code = $"{station.Code}~{dataset.PhenomenonCode}~{dataset.OfferingCode}~{dataset.UnitCode}";
                            var name = $"{station.Name}, {dataset.PhenomenonName}, {dataset.OfferingName}, {dataset.UnitName}";
                            var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Dataset && i.Code == code);
                            if (doi == null)
                            {
                                doi = await AddDOI(DOIType.Dataset, code, name, null);
                                await dbContext.SaveChangesAsync();
                            }
                            return doi;
                        }

                        await AddLineAsync("Generating DOIs");
                        // Preload ImportBatchSummarries
                        var importBatchSummaries = GetImportBatchSummaries();
                        // We only create Dynamic DOIs for SAEON, SMCRI and EFTEON
                        var orgCodes = new string[] { "SAEON", "SMCRI", "EFTEON" };
                        foreach (var organisation in await dbContext.Organisations.Where(i => orgCodes.Contains(i.Code)).OrderBy(i => i.Name).ToListAsync())
                        {
                            var programmeCodes = importBatchSummaries
                                .Where(i => i.OrganisationCode == organisation.Code)
                                .Select(i => i.ProgrammeCode)
                                .Distinct();
                            foreach (var programme in await dbContext.Programmes
                                .Where(i => programmeCodes.Contains(i.Code))
                                .ToListAsync())
                            {
                                var projectCodes = importBatchSummaries
                                    .Where(i =>
                                        i.OrganisationCode == organisation.Code &&
                                        i.ProgrammeCode == programme.Code)
                                    .Select(i => i.ProjectCode)
                                    .Distinct();
                                foreach (var project in await dbContext.Projects
                                    .Where(i => projectCodes.Contains(i.Code))
                                    .ToListAsync())
                                {
                                    var siteCodes = importBatchSummaries
                                        .Where(i =>
                                            i.OrganisationCode == organisation.Code &&
                                            i.ProgrammeCode == programme.Code &&
                                            i.ProjectCode == project.Code)
                                        .Select(i => i.SiteCode)
                                        .Distinct();
                                    foreach (var site in await dbContext.Sites
                                        .Where(i => siteCodes.Contains(i.Code))
                                       .ToListAsync())
                                    {
                                        var stationCodes = importBatchSummaries
                                            .Where(i =>
                                                i.OrganisationCode == organisation.Code &&
                                                i.ProgrammeCode == programme.Code &&
                                                i.ProjectCode == project.Code &&
                                                i.SiteCode == site.Code)
                                           .Select(i => i.StationCode)
                                           .Distinct();
                                        foreach (var station in await dbContext.Stations.Where(i => stationCodes.Contains(i.Code))
                                            .ToListAsync())
                                        {
                                            foreach (var dataset in await dbContext.Datasets.Where(i => i.StationCode == station.Code).ToListAsync())
                                            {
                                                var doiDataset = await EnsureDatasetDOI(station, dataset);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }
        }
    }
}