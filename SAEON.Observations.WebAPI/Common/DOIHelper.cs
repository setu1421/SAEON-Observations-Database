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
            //await GenerateMetadata();
            AddLine("Done");
            return sb.ToString();

            void AddLine(string line)
            {
                sb.AppendLine(line);
                SAEONLogs.Information(line);
            }

            async Task GenerateDOIs()
            {
                DigitalObjectIdentifier BlankDOI(DOIType doiType, string code, string name = null)
                {
                    var blankJson = "{}";
                    var blankHtml = "";
                    return new DigitalObjectIdentifier
                    {
                        DOIType = doiType,
                        Code = code,
                        Name = name ?? code,
                        MetadataJson = blankJson,
                        MetadataJsonSha256 = blankJson.Sha256(),
                        MetadataUrl = "https://metadata.saeon.ac.za/whoknows/",
                        MetadataHtml = blankHtml,
                        QueryUrl = "https://observations.saeon.ac.za",
                        ODPMetadataId = Guid.Empty,
                        ODPMetadataNeedsUpdate = true,
                        AddedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString(),
                        UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString()
                    };
                }

                async Task<DigitalObjectIdentifier> EnsureObservationsDbDOI()
                {
                    var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.ObservationsDb);
                    if (doi == null)
                    {
                        AddLine("Adding ObservationsDB");
                        doi = BlankDOI(DOIType.ObservationsDb, "ObservationsDB", "Observations Database");
                        await dbContext.DigitalObjectIdentifiers.AddAsync(doi);
                        await dbContext.SaveChangesAsync();
                    }
                    return doi;
                }

                async Task<DigitalObjectIdentifier> EnsureOrganisationDOI(DigitalObjectIdentifier doiObservations, Organisation organisation)
                {
                    var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Organisation && i.Code == organisation.Code);
                    if (doi == null)
                    {
                        AddLine($"Adding organisation {organisation.Code}, {organisation.Name}");
                        doi = BlankDOI(DOIType.Organisation, organisation.Code);
                        doi.Parent = doiObservations;
                        await dbContext.DigitalObjectIdentifiers.AddAsync(doi);
                        organisation.DigitalObjectIdentifier = doi;
                        await dbContext.SaveChangesAsync();
                    }
                    return doi;
                }

                IQueryable<VImportBatchSummary> GetImportBatches()
                {
                    return dbContext.VImportBatchSummary.Where(i =>
                        i.LatitudeNorth.HasValue && i.LatitudeSouth.HasValue &&
                        i.LongitudeWest.HasValue && i.LongitudeEast.HasValue &&
                        i.Count > 0);
                }

                async Task<DigitalObjectIdentifier> EnsureProgrammeDOI(DigitalObjectIdentifier doiOrganisation, Programme programme)
                {
                    var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Programme && i.Code == programme.Code);
                    if (doi == null)
                    {
                        AddLine($"Adding programme {programme.Code}, {programme.Name}");
                        doi = BlankDOI(DOIType.Programme, programme.Code);
                        doi.Parent = doiOrganisation;
                        dbContext.DigitalObjectIdentifiers.Add(doi);
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
                        AddLine($"Adding project {project.Code}, {project.Name}");
                        doi = BlankDOI(DOIType.Project, project.Code);
                        doi.Parent = doiProgramme;
                        dbContext.DigitalObjectIdentifiers.Add(doi);
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
                        AddLine($"Adding site {site.Code}, {site.Name}");
                        doi = BlankDOI(DOIType.Site, site.Code);
                        doi.Parent = doiProject;
                        dbContext.DigitalObjectIdentifiers.Add(doi);
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
                        AddLine($"Adding station {station.Code}, {station.Name}");
                        doi = BlankDOI(DOIType.Station, station.Code);
                        doi.Parent = doiSite;
                        dbContext.DigitalObjectIdentifiers.Add(doi);
                        station.DigitalObjectIdentifier = doi;
                        await dbContext.SaveChangesAsync();
                    }
                    return doi;
                }

                async Task<DigitalObjectIdentifier> EnsureDatasetDOI(DigitalObjectIdentifier doiStation, Station station, Dataset dataset)
                {
                    var code = $"{station.Code}~{dataset.PhenomenonCode}~{dataset.OfferingCode}~{dataset.UnitCode}";
                    var name = $"{station.Name} {dataset.PhenomenonName} {dataset.OfferingName} {dataset.UnitName}";
                    var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Dataset && i.Code == code);
                    if (doi == null)
                    {
                        AddLine($"Adding dataset {name}");
                        doi = BlankDOI(DOIType.Dataset, code, name);
                        doi.Parent = doiStation;
                        dbContext.DigitalObjectIdentifiers.Add(doi);
                        //dataset.DigitalObjectIdentifier = doi;
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
                        .Where(i => i.Code == "SAEON" || i.Code == "SACTN")
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
                        foreach (var project in await dbContext.Projects.Where(i => projectCodes.Contains(i.Code)).ToListAsync())
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
                                .Where(i => i.Code == "HSB" || i.Code == "CNSP" || i.Code == "JNHK" || i.Code == "CDBG" || i.Code.StartsWith("SACTN"))
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
                                    }
                                }
                            }
                        }
                    }
                }
                AddLine("Setting Urls");
                foreach (var doi in await dbContext.DigitalObjectIdentifiers.ToListAsync())
                {
                    AddLine($"{doi.DOIType} {doi.Name}");
                    doi.MetadataUrl = $"https://metadata.saeon.ac.za/whoknows/{doi.DOI}";
                    doi.QueryUrl = $"https://observations.saeon.ac.za/DOI/{doi.DOI}";
                }
                await dbContext.SaveChangesAsync();
            }

            async Task GenerateMetadata()
            {
                Metadata MetadataForDOI(DigitalObjectIdentifier doi, Metadata parent, string itemDescription = null, string itemUrl = null)
                {
                    AddLine($"{doi.DOIType} {doi.Code}, {doi.Name}");
                    var metadata = new Metadata
                    {
                        DOI = doi,
                        Identifier = doi.DOI,
                        Parent = parent,
                        ItemDescription = itemDescription,
                        ItemUrl = itemUrl
                    };
                    metadata.AlternateIdentifiers.Add(new MetadataAlternateIdentifier { Name = doi.AlternateId.ToString(), Type = "Internal unique identifier" });
                    return metadata;
                }

                AddLine("Generating metadata");
                var doiObservations = await dbContext.DigitalObjectIdentifiers.Include(i => i.Children).SingleAsync(i => i.DOIType == DOIType.ObservationsDb);
                var metaObservations = MetadataForDOI(doiObservations, null);
                foreach (var doiOrganisation in await dbContext
                    .DigitalObjectIdentifiers
                    .Include(i => i.Parent)
                    .Include(i => i.Children)
                    .Where(i => i.DOIType == DOIType.Organisation && i.Code == "SAEON")
                    .ToListAsync())
                {
                    var organisation = await dbContext.Organisations.SingleAsync(i => i.Code == doiOrganisation.Code);
                    var metaOrganisation = MetadataForDOI(doiOrganisation, metaObservations, organisation.Description, organisation.Url);
                    foreach (var doiProgramme in await dbContext
                        .DigitalObjectIdentifiers
                        .Include(i => i.Parent)
                        .Include(i => i.Children)
                        .Where(i => i.ParentId == doiOrganisation.Id)
                        .ToListAsync())
                    {
                        var programme = await dbContext.Programmes.SingleAsync(i => i.Code == doiProgramme.Code);
                        var metaProgramme = MetadataForDOI(doiProgramme, metaOrganisation, programme.Description, programme.Url);
                        foreach (var doiProject in await dbContext
                            .DigitalObjectIdentifiers
                            .Include(i => i.Parent)
                            .Include(i => i.Children)
                            .Where(i => i.ParentId == doiProgramme.Id)
                            .ToListAsync())
                        {
                            var project = await dbContext.Projects.SingleAsync(i => i.Code == doiProject.Code);
                            var metaProject = MetadataForDOI(doiProject, metaProgramme, project.Description, project.Url);
                            foreach (var doiSite in await dbContext
                                .DigitalObjectIdentifiers
                                .Include(i => i.Parent)
                                .Include(i => i.Children)
                                .Where(i => i.ParentId == doiProject.Id)
                                .ToListAsync())
                            {
                                var site = await dbContext.Sites.SingleAsync(i => i.Code == doiSite.Code);
                                var metaSite = MetadataForDOI(doiSite, metaProject, site.Description, site.Url);
                                foreach (var doiStation in await dbContext
                                    .DigitalObjectIdentifiers
                                    .Include(i => i.Parent)
                                    .Include(i => i.Children)
                                    .Where(i => i.ParentId == doiSite.Id)
                                    .ToListAsync())
                                {
                                    var station = await dbContext.Stations.SingleAsync(i => i.Code == doiStation.Code);
                                    var metaStation = MetadataForDOI(doiStation, metaSite, station.Description, station.Url);
                                    foreach (var doiDataset in await dbContext
                                        .DigitalObjectIdentifiers
                                        .Include(i => i.Parent)
                                        .Include(i => i.Children)
                                        .Where(i => i.ParentId == doiStation.Id)
                                        .ToListAsync())
                                    {
                                        var splits = doiDataset.Code.Split('~', StringSplitOptions.RemoveEmptyEntries);
                                        var phenomenon = await dbContext.Phenomena.SingleAsync(i => i.Code == splits[1]);
                                        var dataset = await dbContext.Datasets.Where(i =>
                                            i.StationCode == splits[0] &&
                                            i.PhenomenonCode == splits[1] &&
                                            i.OfferingCode == splits[2] &&
                                            i.UnitCode == splits[3])
                                            .SingleAsync();
                                        var metaDataset = MetadataForDOI(doiDataset, metaStation, phenomenon.Description, phenomenon.Url);
                                        metaDataset.StartDate = dataset.StartDate;
                                        metaDataset.EndDate = dataset.EndDate;
                                        metaDataset.LatitudeNorth = dataset.LatitudeNorth;
                                        metaDataset.LatitudeSouth = dataset.LatitudeSouth;
                                        metaDataset.LongitudeWest = dataset.LongitudeWest;
                                        metaDataset.LongitudeEast = dataset.LongitudeEast;
                                        metaDataset.ElevationMinimum = dataset.ElevationMinimum;
                                        metaDataset.ElevationMaximum = dataset.ElevationMaximum;
                                        metaDataset.Subjects.Add(new MetadataSubject { Name = dataset.StationName });
                                        metaDataset.Subjects.Add(new MetadataSubject { Name = $"{dataset.PhenomenonName}" });
                                        metaDataset.Subjects.Add(new MetadataSubject { Name = $"{dataset.PhenomenonName} {dataset.OfferingName} {dataset.UnitName}" });
                                        metaDataset.Generate($"Station {dataset.StationName} observations of {dataset.PhenomenonName}, {dataset.OfferingName} in {dataset.UnitName}",
                                            $"The observations at station {dataset.StationName} of {dataset.PhenomenonName}, {dataset.OfferingName} in {dataset.UnitName}");
                                        doiDataset.MetadataJson = metaDataset.ToJson();
                                        doiDataset.MetadataJsonSha256 = doiDataset.MetadataJson.Sha256();
                                        doiDataset.MetadataHtml = metaDataset.ToHtml();
                                    }
                                    metaStation.Generate();
                                    doiStation.MetadataJson = metaStation.ToJson();
                                    doiStation.MetadataJsonSha256 = doiStation.MetadataJson.Sha256();
                                    doiStation.MetadataHtml = metaStation.ToHtml();
                                    await dbContext.SaveChangesAsync();
                                }
                                metaSite.Generate();
                                doiSite.MetadataJson = metaSite.ToJson();
                                doiSite.MetadataJsonSha256 = doiSite.MetadataJson.Sha256();
                                doiSite.MetadataHtml = metaSite.ToHtml();
                                await dbContext.SaveChangesAsync();
                            }
                            metaProject.Generate();
                            doiProject.MetadataJson = metaProject.ToJson();
                            doiProject.MetadataJsonSha256 = doiProject.MetadataJson.Sha256();
                            doiProject.MetadataHtml = metaProject.ToHtml();
                            await dbContext.SaveChangesAsync();
                        }
                        metaProgramme.Generate();
                        doiProgramme.MetadataJson = metaProgramme.ToJson();
                        doiProgramme.MetadataJsonSha256 = doiProgramme.MetadataJson.Sha256();
                        doiProgramme.MetadataHtml = metaProgramme.ToHtml();
                        await dbContext.SaveChangesAsync();
                    }
                    metaOrganisation.Generate();
                    doiOrganisation.MetadataJson = metaOrganisation.ToJson();
                    doiOrganisation.MetadataJsonSha256 = doiOrganisation.MetadataJson.Sha256();
                    doiOrganisation.MetadataHtml = metaOrganisation.ToHtml();
                    await dbContext.SaveChangesAsync();
                }
                metaObservations.Generate("Complete collection of observations in the SAEON Observations Database",
                    "The complete collection of observations in the SAEON Observations Database");
                doiObservations.MetadataJson = metaObservations.ToJson();
                doiObservations.MetadataJsonSha256 = doiObservations.MetadataJson.Sha256();
                doiObservations.MetadataHtml = metaObservations.ToHtml();
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
