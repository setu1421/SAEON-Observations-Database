using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class DOIHelper
    {
        public static async Task<string> CreateDOIs(ObservationsDbContext dbContext, HttpContext httpContext)
        {
            var sb = new StringBuilder();
            await GenerateDOIs();
            AddLine("Done");
            return sb.ToString();

            void AddLine(string line)
            {
                sb.AppendLine(line);
                SAEONLogs.Information(line);
            }

            async Task GenerateDOIs()
            {
                async Task<DigitalObjectIdentifier> AddDOI(DOIType doiType, string code, string name, DigitalObjectIdentifier parent)
                {
                    AddLine($"Adding {doiType} {code}, {name}");
                    var blankJson = "{}";
                    var blankHtml = "";
                    var doi = new DigitalObjectIdentifier
                    {
                        DOIType = doiType,
                        Code = code,
                        Name = name ?? code,
                        MetadataJson = blankJson,
                        MetadataJsonSha256 = blankJson.Sha256(),
                        MetadataUrl = "https://metadata.saeon.ac.za/whoknows/",
                        MetadataHtml = blankHtml,
                        QueryUrl = "https://observations.saeon.ac.za",
                        AddedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString(),
                        UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString()
                    };
                    if (parent != null)
                    {
                        doi.Parent = parent;
                    }
                    await dbContext.DigitalObjectIdentifiers.AddAsync(doi);
                    return doi;
                }

                async Task<DigitalObjectIdentifier> EnsureObservationsDbDOI()
                {
                    var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.ObservationsDb);
                    if (doi == null)
                    {
                        doi = await AddDOI(DOIType.ObservationsDb, "ObservationsDB", "Observations Database", null);
                        await dbContext.SaveChangesAsync();
                    }
                    return doi;
                }

                async Task<DigitalObjectIdentifier> EnsureOrganisationDOI(DigitalObjectIdentifier doiObservations, Organisation organisation)
                {
                    var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Organisation && i.Code == organisation.Code);
                    if (doi == null)
                    {
                        doi = await AddDOI(DOIType.Organisation, organisation.Code, organisation.Name, doiObservations);
                        organisation.DigitalObjectIdentifier = doi;
                        await dbContext.SaveChangesAsync();
                    }
                    return doi;
                }

                IQueryable<VImportBatchSummary> GetImportBatches()
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

                AddLine("Generating DOIs");
                var doiObservations = await EnsureObservationsDbDOI();
                foreach (var organisation in await dbContext.Organisations.Where(i => i.Code == "SAEON").ToListAsync())
                {
                    var doiOrganisation = await EnsureOrganisationDOI(doiObservations, organisation);
                    var programmeCodes = GetImportBatches()
                        .Where(i => i.OrganisationCode == organisation.Code)
                        .Select(i => i.ProgrammeCode)
                        .Distinct();
                    foreach (var programme in await dbContext.Programmes
                        .Where(i => i.Code == "SAEON" || i.Code == "SACTN") // Remove once live
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
                            .Where(i => i.Code == "SAEON" || i.Code == "SACTN") // Remove once live
                            .Where(i => projectCodes.Contains(i.Code)).ToListAsync())
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
                                .Where(i => i.Code == "HSB" || i.Code == "CNSP" || i.Code == "JNHK" || i.Code == "CDBG" || i.Code.StartsWith("SACTN")) // Remove once live
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
                                foreach (var station in await dbContext.Stations.Where(i => stationCodes.Contains(i.Code)).ToListAsync())
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
                                        foreach (var importBatchSummary in await dbContext.ImportBatchSummary
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
                AddLine("Setting Urls");
                foreach (var doi in await dbContext.DigitalObjectIdentifiers.ToListAsync())
                {
                    AddLine($"{doi.DOIType} {doi.Name}");
                    doi.MetadataUrl = $"https://metadata.saeon.ac.za/whoknows/{doi.DOI}";
                    doi.QueryUrl = $"https://observations.saeon.ac.za/DOI/{doi.DOI}";
                }
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
