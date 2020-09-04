using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class MetadataBoundingBox
    {
        public double LongitudeWest { get; set; }
        public double LongitudeEast { get; set; }
        public double LatitudeNorth { get; set; }
        public double LatitudeSouth { get; set; }

        public double Latitude => (LatitudeNorth + LatitudeSouth) / 2;
        public double Longitude => (LongitudeWest + LongitudeEast) / 2;
    }

    public class MetadataRelationship
    {
        public string Name { get; set; }
    }

    public class MetadataResourceType
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class MetadataRights
    {
        public string Name { get; set; }
        public string URI { get; set; }
        public string Identifier { get; set; }
        public string Scheme { get; set; }
        public string SchemeURI { get; set; }
    }

    public class MetadataSubject
    {
        public string Name { get; set; }
        public string Scheme { get; set; }
        public string SchemeUri { get; set; }
    }

    public class Metadata
    {
        public string Identifier { get; set; } //@@
        public string Language { get; set; } = "en-za"; //@@
        public MetadataResourceType ResourceType { get; } = new MetadataResourceType { Name = "Dataset", Type = "Observations" }; //@@
        public string Publisher { get; set; } = "South African Environmental Observation Network (SAEON)"; //@@
        public int PublicationYear => EndDate.Year; //@@
        public DateTimeOffset StartDate { get; set; } //@@
        public DateTimeOffset EndDate { get; set; } //@@
        public string Title { get; set; } //@@
        public string Description { get; set; } //@@
        public List<MetadataRights> Rights { get; } = new List<MetadataRights> { new MetadataRights {
            Name = "Attribution-ShareAlike 4.0 International (CC BY-SA 4.0)",
            URI = "https://creativecommons.org/licenses/by-sa/4.0",
            Identifier = "CC-BY-SA-4.0",
            Scheme = "SPDX",
            SchemeURI = "https://spdx.org/licenses"
        } }; //@@
        public List<MetadataSubject> Subjects { get; } = new List<MetadataSubject> {
            new MetadataSubject { Name="Observations"},
            new MetadataSubject { Name = "South African Environmental Observation Network (SAEON)"},
            new MetadataSubject
            {
                Name = "Observations Database",
                Scheme = "SOFTWARE_APP",
                SchemeUri = "https://observations.saeon.ac.za"
            },
            new MetadataSubject
            {
                Name = "https://observations.saeon.ac.za",
                Scheme = "SOFTWARE_URL",
                SchemeUri = "https://observations.saeon.ac.za"
            }
        }; //@@
        public MetadataBoundingBox BoundingBox;
        public MetadataRelationship IsPartOf;
        public List<MetadataRelationship> HasParts { get; } = new List<MetadataRelationship>();

        public override string ToString()
        {
            var jRights = new JArray();
            foreach (var right in Rights)
            {
                jRights.Add(
                    new JObject(
                        new JProperty("rights", right.Name),
                        new JProperty("rightsURI", right.URI),
                        new JProperty("rightsIdentifier", right.Identifier),
                        new JProperty("rightsIdentifierScheme", right.Scheme),
                        new JProperty("schemeURI", right.SchemeURI)
                    ));
            }
            var jSubjects = new JArray();
            foreach (var subject in Subjects)
            {
                if (string.IsNullOrWhiteSpace(subject.Scheme))
                {
                    jSubjects.Add(new JObject(new JProperty("subject", subject.Name)));
                }
                else
                {
                    jSubjects.Add(
                        new JObject(
                            new JProperty("subject", subject.Name),
                            new JProperty("subjectScheme", subject.Scheme),
                            new JProperty("schemeURI", subject.SchemeUri)
                        ));
                }
            }
            var jObj =
                new JObject(
                    new JProperty("identifier",
                        new JObject(
                            new JProperty("identifier", Identifier),
                            new JProperty("identifierType", "DOI")
                        )
                    ),
                    new JProperty("language", Language),
                    new JProperty("resourceType",
                        new JObject(
                            new JProperty("resourceTypeGeneral", ResourceType.Name),
                            new JProperty("resourceType", ResourceType.Type)
                        )
                    ),
                    new JProperty("publisher", Publisher),
                    new JProperty("publicationYear", PublicationYear),
                    new JProperty("dates",
                        new JArray(
                            new JObject(
                                new JProperty("date", EndDate.ToString("yyyy-MM-dd")),
                                new JProperty("dateType", "Accepted")
                            ),
                            new JObject(
                                new JProperty("date", EndDate.ToString("yyyy-MM-dd")),
                                new JProperty("dateType", "Issued")
                            ),
                            new JObject(
                                new JProperty("date", StartDate.ToString("O")),
                                new JProperty("dateType", "Collected")
                            ),
                            new JObject(
                                new JProperty("date", EndDate.ToString("O")),
                                new JProperty("dateType", "Collected")
                            )
                        )
                    ),
                    new JProperty("titles",
                        new JArray(
                            new JObject(
                                new JProperty("title", Title)
                            )
                        )
                    ),
                    new JProperty("descriptions",
                        new JArray(
                            new JObject(
                                new JProperty("descriptionType", "Abstract"),
                                new JProperty("description", Description)
                            )
                        )
                    ),
                    new JProperty("rightsList", jRights),
                    new JProperty("subjects", jSubjects)
                );
            return jObj.ToString(Formatting.Indented);
        }
    }

    public static class DOIHelper
    {
        public static async Task<string> CreateDOIs(ObservationsDbContext dbContext, HttpContext httpContext)
        {
            var sb = new StringBuilder();
            await GenerateDOIs();
            //await GenerateMetadata();
            return sb.ToString();

            void AddLine(string line)
            {
                sb.AppendLine(line);
                SAEONLogs.Information(line);
            }

            DigitalObjectIdentifier BlankDOI(DOIType doiType, string name)
            {
                var blankJson = "{}";
                return new DigitalObjectIdentifier
                {
                    DOIType = doiType,
                    Name = name,
                    MetadataJson = blankJson,
                    MetadataJsonSha256 = blankJson.Sha256(),
                    MetadataUrl = "https://metadata.saeon.ac.za",
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
                    doi = BlankDOI(DOIType.ObservationsDb, "Observations Database");
                    await dbContext.DigitalObjectIdentifiers.AddAsync(doi);
                    await dbContext.SaveChangesAsync();
                }
                return doi;
            }

            async Task<DigitalObjectIdentifier> EnsureOrganisationDOI(DigitalObjectIdentifier doiObservations, Organisation organisation)
            {
                var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Organisation && i.Name == organisation.Code);
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
                var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Programme && i.Name == programme.Code);
                if (doi == null)
                {
                    AddLine($"Adding programme {programme.Code} DOI");
                    doi = BlankDOI(DOIType.Programme, programme.Code);
                    doi.Parent = doiOrganisation;
                    dbContext.DigitalObjectIdentifiers.Add(doi);
                    programme.DigitalObjectIdentifier = doi;
                    dbContext.SaveChanges();
                }
                return doi;
            }

            async Task<DigitalObjectIdentifier> EnsureProjectDOI(DigitalObjectIdentifier doiProgramme, Project project)
            {
                var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Project && i.Name == project.Code);
                if (doi == null)
                {
                    AddLine($"Adding project {project.Code} DOI");
                    doi = BlankDOI(DOIType.Project, project.Code);
                    doi.Parent = doiProgramme;
                    dbContext.DigitalObjectIdentifiers.Add(doi);
                    project.DigitalObjectIdentifier = doi;
                    dbContext.SaveChanges();
                }
                return doi;
            }

            async Task<DigitalObjectIdentifier> EnsureSiteDOI(DigitalObjectIdentifier doiProject, Site site)
            {
                var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Site && i.Name == site.Code);
                if (doi == null)
                {
                    AddLine($"Adding site {site.Code} DOI");
                    doi = BlankDOI(DOIType.Site, site.Code);
                    doi.Parent = doiProject;
                    dbContext.DigitalObjectIdentifiers.Add(doi);
                    site.DigitalObjectIdentifier = doi;
                    dbContext.SaveChanges();
                }
                return doi;
            }

            async Task<DigitalObjectIdentifier> EnsureStationDOI(DigitalObjectIdentifier doiSite, Station station)
            {
                var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Station && i.Name == station.Code);
                if (doi == null)
                {
                    AddLine($"Adding station {station.Code} DOI");
                    doi = BlankDOI(DOIType.Station, station.Code);
                    doi.Parent = doiSite;
                    dbContext.DigitalObjectIdentifiers.Add(doi);
                    station.DigitalObjectIdentifier = doi;
                    dbContext.SaveChanges();
                }
                return doi;
            }

            async Task<DigitalObjectIdentifier> EnsureDatasetDOI(DigitalObjectIdentifier doiStation, Station station, Dataset dataset)
            {
                var code = $"{station.Code} - {dataset.PhenomenonCode} - {dataset.OfferingCode} - {dataset.UnitCode}";
                var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Dataset && i.Name == code);
                if (doi == null)
                {
                    AddLine($"Adding dataset {code} DOI");
                    doi = BlankDOI(DOIType.Dataset, code);
                    doi.Parent = doiStation;
                    dbContext.DigitalObjectIdentifiers.Add(doi);
                    //dataset.DigitalObjectIdentifier = doi;
                    dbContext.SaveChanges();
                }
                return doi;
            }

            async Task GenerateDOIs()
            {
                var doiObservations = await EnsureObservationsDbDOI();
                foreach (var organisation in dbContext.Organisations.Where(i => i.Code == "SAEON").ToList())
                {
                    var doiOrganisation = await EnsureOrganisationDOI(doiObservations, organisation);
                    var programmeCodes = GetImportBatches()
                        .Where(i => i.OrganisationCode == organisation.Code)
                        .Select(i => i.ProgrammeCode)
                        .Distinct();
                    foreach (var programme in dbContext.Programmes.Where(i => programmeCodes.Contains(i.Code)).ToList())
                    {
                        var doiProgramme = await EnsureProgrammeDOI(doiOrganisation, programme);
                        var projectCodes = GetImportBatches()
                            .Where(i =>
                                i.OrganisationCode == organisation.Code &&
                                i.ProgrammeCode == programme.Code)
                            .Select(i => i.ProjectCode)
                            .Distinct();
                        foreach (var project in dbContext.Projects.Where(i => projectCodes.Contains(i.Code)).ToList())
                        {
                            var doiProject = await EnsureProjectDOI(doiProgramme, project);
                            var siteCodes = GetImportBatches()
                                .Where(i =>
                                    i.OrganisationCode == organisation.Code &&
                                    i.ProgrammeCode == programme.Code &&
                                    i.ProjectCode == project.Code)
                                .Select(i => i.SiteCode)
                                .Distinct();
                            foreach (var site in dbContext.Sites.Where(i => siteCodes.Contains(i.Code)).ToList())
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
                                foreach (var station in dbContext.Stations.Where(i => stationCodes.Contains(i.Code)).ToList())
                                {
                                    var doiStation = await EnsureStationDOI(doiProject, station);
                                    foreach (var dataset in dbContext.Datasets.Where(i => i.StationCode == station.Code).ToList())
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
                var doiObservations = dbContext.DigitalObjectIdentifiers.Single(i => i.DOIType == DOIType.ObservationsDb);
                foreach (var doiOrganisation in dbContext.DigitalObjectIdentifiers.Where(i => i.DOIType == DOIType.Organisation))
                {
                    foreach (var doiProgramme in dbContext.DigitalObjectIdentifiers.Where(i => i.DOIType == DOIType.Programme))
                    {

                    }

                }
                var stationCodes = GetImportBatches().Select(i => i.StationCode).Distinct();
                foreach (var stationCode in stationCodes)
                {
                    var datasets = dbContext.Datasets.Where(i => i.StationCode == stationCode);
                    foreach (var dataset in datasets)
                    {
                        var code = $"{stationCode} - {dataset.PhenomenonCode} - {dataset.OfferingCode} - {dataset.UnitCode}";
                        AddLine($"Generating {code} metadata");
                        var doi = await dbContext.DigitalObjectIdentifiers.SingleAsync(i => i.DOIType == DOIType.Dataset && i.Name == code);
                        var title = $"{dataset.StationName} observations of {dataset.PhenomenonName}, {dataset.OfferingName} in {dataset.UnitName}";
                        var description = $"{dataset.StationName} observations of {dataset.PhenomenonName}, {dataset.OfferingName} in {dataset.UnitName}";
                        if ((dataset.LatitudeNorth == dataset.LatitudeSouth) && (dataset.LongitudeWest == dataset.LongitudeEast))
                        {
                            description += $" at {dataset.LatitudeNorth:f5},{dataset.LongitudeWest:f5}";
                        }
                        else
                        {
                            description += $" in area {dataset.LatitudeNorth:f5},{dataset.LongitudeWest:f5} (N,W) {dataset.LatitudeSouth:f5},{dataset.LongitudeEast:f5} (S,E)";
                        }
                        if (dataset.ElevationMaximum.HasValue && dataset.ElevationMaximum.HasValue)
                        {
                            if (dataset.ElevationMinimum == dataset.ElevationMaximum)
                            {
                                description += $" at {dataset.ElevationMaximum:f2}m above mean sea level";
                            }
                            else
                            {
                                description += $" between {dataset.ElevationMinimum:f2}m and {dataset.ElevationMaximum:f2}m above mean sea level";
                            }
                        }
                        if (dataset.StartDate.Value == dataset.EndDate.Value)
                        {
                            title += $" on {dataset.StartDate.Value:yyyy-MM-dd}";
                            description += $" on {dataset.StartDate.Value:O}";
                        }
                        else
                        {
                            title += $" from {dataset.StartDate.Value:yyyy-MM-dd} to {dataset.EndDate.Value:yyyy-MM-dd}";
                            description += $" from {dataset.StartDate.Value:O} to {dataset.EndDate.Value:O}";
                        }
                        var metadata = new Metadata
                        {
                            Identifier = doi.DOI,
                            Title = title,
                            Description = description,
                            StartDate = dataset.StartDate.Value,
                            EndDate = dataset.EndDate.Value,
                            BoundingBox = new MetadataBoundingBox
                            {
                                LatitudeNorth = dataset.LatitudeNorth.Value,
                                LatitudeSouth = dataset.LatitudeSouth.Value,
                                LongitudeWest = dataset.LongitudeWest.Value,
                                LongitudeEast = dataset.LongitudeEast.Value
                            }
                        };
                        metadata.Subjects.Add(new MetadataSubject { Name = dataset.StationName });
                        metadata.Subjects.Add(new MetadataSubject { Name = $"{dataset.PhenomenonName}" });
                        metadata.Subjects.Add(new MetadataSubject { Name = $"{dataset.PhenomenonName} {dataset.OfferingName} {dataset.UnitName}" });
                        var metadataJson = metadata.ToString();
                        doi.MetadataJson = metadataJson;
                        doi.MetadataJsonSha256 = metadataJson.Sha256();
                    }
                }
                dbContext.SaveChanges();
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

    }
}
