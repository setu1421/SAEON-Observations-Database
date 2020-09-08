using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using SAEON.Observations.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI.Controllers.Internal
{
    public static class DOIHelper
    {
        public static async Task<string> CreateDOIs(ObservationsDbContext dbContext, HttpContext httpContext)
        {
            var sb = new StringBuilder();
            await GenerateDOIs();
            await GenerateMetadata();
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
                        MetadataUrl = "https://metadata.saeon.ac.za",
                        MetadataHtml = blankHtml,
                        OpenDataPlatformId = Guid.Empty,
                        AddedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString(),
                        UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString()
                    };
                }

                async Task<DigitalObjectIdentifier> EnsureObservationsDbDOI()
                {
                    var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.ObservationsDb);
                    if (doi == null)
                    {
                        AddLine("Adding ObservationsDB DOI");
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
                        AddLine($"Adding organisation {organisation.Code} DOI");
                        doi = BlankDOI(DOIType.Organisation, organisation.Code);
                        doi.Parent = doiObservations;
                        await dbContext.DigitalObjectIdentifiers.AddAsync(doi);
                        organisation.DigitalObjectIdentifier = doi;
                        await dbContext.SaveChangesAsync();
                    }
                    return doi;
                }

                IQueryable<ImportBatchSummary> GetImportBatches()
                {
                    return dbContext.ImportBatchSummary.Where(i =>
                        i.LatitudeNorth.HasValue && i.LatitudeSouth.HasValue &&
                        i.LongitudeWest.HasValue && i.LongitudeEast.HasValue &&
                        i.Count > 0);
                }

                async Task<DigitalObjectIdentifier> EnsureProgrammeDOI(DigitalObjectIdentifier doiOrganisation, Programme programme)
                {
                    var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Programme && i.Code == programme.Code);
                    if (doi == null)
                    {
                        AddLine($"Adding programme {programme.Code} DOI");
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
                        AddLine($"Adding project {project.Code} DOI");
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
                        AddLine($"Adding site {site.Code} DOI");
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
                        AddLine($"Adding station {station.Code} DOI");
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
                        AddLine($"Adding dataset {code} DOI");
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
                    foreach (var programme in await dbContext.Programmes.Where(i => programmeCodes.Contains(i.Code)).ToListAsync())
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
                            foreach (var site in await dbContext.Sites.Where(i => siteCodes.Contains(i.Code)).ToListAsync())
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
            }

            async Task GenerateMetadata()
            {
                Metadata MetadataForDOI(DigitalObjectIdentifier doi, Metadata parent)
                {
                    AddLine($"{doi.DOIType} {doi.Name}");
                    var metadata = new Metadata
                    {
                        DOI = doi,
                        Identifier = doi.DOI,
                        Parent = parent
                    };
                    return metadata;
                }

                AddLine("Generating metadata");
                var doiObservations = dbContext.DigitalObjectIdentifiers.Include(i => i.Children).Single(i => i.DOIType == DOIType.ObservationsDb);
                var metaObservations = MetadataForDOI(doiObservations, null);
                foreach (var doiOrganisation in await dbContext
                    .DigitalObjectIdentifiers
                    .Include(i => i.Parent)
                    .Include(i => i.Children)
                    .Where(i => i.DOIType == DOIType.Organisation && i.Code == "SAEON")
                    .ToListAsync())
                {
                    var metaOrganisation = MetadataForDOI(doiOrganisation, metaObservations);
                    foreach (var doiProgramme in await dbContext
                        .DigitalObjectIdentifiers
                        .Include(i => i.Parent)
                        .Include(i => i.Children)
                        .Where(i => i.ParentId == doiOrganisation.Id)
                        .ToListAsync())
                    {
                        var metaProgramme = MetadataForDOI(doiProgramme, metaOrganisation);
                        foreach (var doiProject in await dbContext
                            .DigitalObjectIdentifiers
                            .Include(i => i.Parent)
                            .Include(i => i.Children)
                            .Where(i => i.ParentId == doiProgramme.Id)
                            .ToListAsync())
                        {
                            var metaProject = MetadataForDOI(doiProject, metaProgramme);
                            foreach (var doiSite in await dbContext
                                .DigitalObjectIdentifiers
                                .Include(i => i.Parent)
                                .Include(i => i.Children)
                                .Where(i => i.ParentId == doiProject.Id)
                                .ToListAsync())
                            {
                                var metaSite = MetadataForDOI(doiSite, metaProject);
                                foreach (var doiStation in await dbContext
                                    .DigitalObjectIdentifiers
                                    .Include(i => i.Parent)
                                    .Include(i => i.Children)
                                    .Where(i => i.ParentId == doiSite.Id)
                                    .ToListAsync())
                                {
                                    var metaStation = MetadataForDOI(doiStation, metaSite);
                                    foreach (var doiDataset in await dbContext
                                        .DigitalObjectIdentifiers
                                        .Include(i => i.Parent)
                                        .Include(i => i.Children)
                                        .Where(i => i.ParentId == doiStation.Id)
                                        .ToListAsync())
                                    {
                                        var splits = doiDataset.Code.Split('~', StringSplitOptions.RemoveEmptyEntries);
                                        var dataset = await dbContext.Datasets.Where(i =>
                                            i.StationName == splits[0] &&
                                            i.PhenomenonCode == splits[1] &&
                                            i.OfferingCode == splits[2] &&
                                            i.UnitCode == splits[3]).SingleAsync();
                                        var metaDataset = MetadataForDOI(doiDataset, metaStation);
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

    public static class StringExtensions
    {
        public static byte[] Sha256(this string value)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
        }

        public static string HtmlB(this string value)
        {
            return $"<b>{value}</b>";
        }
    }

    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendHtmlH2(this StringBuilder builder, string text)
        {
            return builder.AppendLine($"<h2>{text}</h2>");
        }

        public static StringBuilder AppendHtmlH3(this StringBuilder builder, string text)
        {
            return builder.AppendLine($"<h3>{text}</h3>");
        }

        public static StringBuilder AppendHtmlP(this StringBuilder builder, string text)
        {
            return builder.AppendLine($"<p>{text}</p>");
        }

        public static StringBuilder AppendHtmlUL(this StringBuilder builder, List<string> items)
        {
            builder.AppendLine("<ul>");
            foreach (var item in items)
                builder.AppendLine($"<li>{item}</li>");
            return builder.AppendLine("</ul>");
        }

        public static StringBuilder AppendHtmlUL(this StringBuilder builder, IEnumerable<string> items)
        {
            builder.AppendLine("<ul>");
            foreach (var item in items)
                builder.AppendLine($"<li>{item}</li>");
            return builder.AppendLine("</ul>");
        }

    }
}
