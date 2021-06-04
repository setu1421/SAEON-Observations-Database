using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using SAEON.Observations.Auth;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class MetadataHelper
    {
        public static DOIType[] DynamicDOITypes => new DOIType[] { /*DOIType.ObservationsDb, DOIType.Collection, DOIType.Organisation, DOIType.Programme, DOIType.Project, DOIType.Site, DOIType.Station,*/ DOIType.Dataset };


        public static string CleanPrefixes(string name)
        {
            var prefixes = new List<string> { "ELW, ", "EOV - " };
            var result = name;
            foreach (var prefix in prefixes)
            {
                result = result.Replace(prefix, "");
            }
            return result;
        }

        public static async Task<string> CreateMetadataV2(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub)
        {
            using (SAEONLogs.MethodCall(typeof(MetadataHelper)))
            {
                try
                {
                    var sb = new StringBuilder();
                    await GenerateMetadata();
                    await AddLineAsync("Done");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateMetadataStatusUpdate, line);
                    }

                    async Task GenerateMetadata()
                    {
                        async Task<Metadata> MetadataForDOIAsync(DigitalObjectIdentifier doi, Metadata parent)
                        {
                            await AddLineAsync($"{doi.DOIType} {doi.Code}, {doi.Name}");
                            var metadata = new Metadata
                            {
                                DOI = doi,
                                Parent = parent
                            };
                            return metadata;
                        }

                        await AddLineAsync("Generating metadata");
                        byte[] oldSha256;
                        foreach (var doiDataset in await dbContext.DigitalObjectIdentifiers.Where(i => i.DOIType == DOIType.Dataset).ToListAsync())
                        {
                            var splits = doiDataset.Code.Split('~', StringSplitOptions.RemoveEmptyEntries);
                            var dataset = await dbContext.Datasets.Where(i =>
                                i.StationCode == splits[0] &&
                                i.PhenomenonCode == splits[1] &&
                                i.OfferingCode == splits[2] &&
                                i.UnitCode == splits[3])
                                .SingleAsync();
                            var metaDataset = await MetadataForDOIAsync(doiDataset, null);
                            metaDataset.StartDate = dataset.StartDate;
                            metaDataset.EndDate = dataset.EndDate;
                            metaDataset.LatitudeNorth = dataset.LatitudeNorth;
                            metaDataset.LatitudeSouth = dataset.LatitudeSouth;
                            metaDataset.LongitudeWest = dataset.LongitudeWest;
                            metaDataset.LongitudeEast = dataset.LongitudeEast;
                            metaDataset.ElevationMinimum = dataset.ElevationMinimum;
                            metaDataset.ElevationMaximum = dataset.ElevationMaximum;
                            metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(dataset.OrganisationName) });
                            metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(dataset.ProgrammeName) });
                            metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(dataset.ProjectName) });
                            metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(dataset.SiteName) });
                            metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(dataset.StationName) });
                            metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(dataset.PhenomenonName) });
                            metaDataset.Subjects.Add(new MetadataSubject { Name = $"{CleanPrefixes(dataset.PhenomenonName)}, {dataset.OfferingName}, {dataset.UnitName}" });
                            var variable = $"{CleanPrefixes(dataset.PhenomenonName)}, {dataset.OfferingName}, {dataset.UnitName}";
                            var siteName = CleanPrefixes(dataset.SiteName);
                            var stationName = CleanPrefixes(dataset.StationName);
                            string station;
                            if (dataset.StationName.StartsWith("ELW, "))
                            {
                                if (stationName.EndsWith(siteName))
                                {
                                    stationName = stationName.Substring(0, stationName.Length - siteName.Length - 2);
                                }
                                station = stationName;
                            }
                            else
                            {
                                station = $"{siteName}, {stationName}";
                            }
                            metaDataset.Generate(variable, station);
                            doiDataset.Title = metaDataset.Title;
                            doiDataset.Description = metaDataset.Description;
                            doiDataset.DescriptionHtml = metaDataset.DescriptionHtml;
                            doiDataset.Citation = metaDataset.Citation;
                            doiDataset.CitationHtml = metaDataset.CitationHtml;
                            doiDataset.SetUrls();
                            doiDataset.MetadataJson = metaDataset.ToJson();
                            oldSha256 = doiDataset.MetadataJsonSha256;
                            doiDataset.MetadataJsonSha256 = doiDataset.MetadataJson.Sha256();
                            doiDataset.ODPMetadataNeedsUpdate = (oldSha256 != doiDataset.MetadataJsonSha256) || (!doiDataset.ODPMetadataIsValid ?? true);
                            doiDataset.MetadataHtml = metaDataset.ToHtml();
                            await dbContext.SaveChangesAsync();
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

        /*
        public static async Task<Metadata> CreateAddHocMetadata(ObservationsDbContext dbContext, DigitalObjectIdentifier doi, MetadataCore metadataCore)
        {
            using (SAEONLogs.MethodCall(typeof(MetadataHelper)))
            {
                try
                {
                    var doiAdHocs = await dbContext.DigitalObjectIdentifiers.Include(i => i.Children).SingleAsync(i => i.DOIType == DOIType.Collection && i.Code == DOIHelper.AdHocDOIsCode);
                    var metaObservations = MetadataForDOIAsync(doiAdHocs, null);
                    var metadata = MetadataForDOIAsync(doi, metaObservations);
                    return metadata;
                }
                catch (Exception ex)
                {
                    SAEONLogs.Exception(ex);
                    throw;
                }
            }

            Metadata MetadataForDOIAsync(DigitalObjectIdentifier doi, Metadata parent)
            {
                SAEONLogs.Information("{DOIType} {Code}, {Name}", doi.DOIType, doi.Code, doi.Name);
                var metadata = new Metadata(metadataCore)
                {
                    DOI = doi,
                    Parent = parent
                };
                return metadata;
            }
        }
        */

        /*
         public static async Task<string> CreateMetadata(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub)
         {
             using (SAEONLogs.MethodCall(typeof(MetadataHelper)))
             {
                 try
                 {
                     var sb = new StringBuilder();
                     await GenerateMetadata();
                     await AddLineAsync("Done");
                     return sb.ToString();

                     async Task AddLineAsync(string line)
                     {
                         sb.AppendLine(line);
                         SAEONLogs.Information(line);
                         await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateMetadataStatusUpdate, line);
                     }

                     async Task GenerateMetadata()
                     {
                         async Task<Metadata> MetadataForDOIAsync(DigitalObjectIdentifier doi, Metadata parent)
                         {
                             await AddLineAsync($"{doi.DOIType} {doi.Code}, {doi.Name}");
                             var metadata = new Metadata
                             {
                                 DOI = doi,
                                 Parent = parent
                             };
                             return metadata;
                         }

                         await AddLineAsync("Generating metadata");
                         byte[] oldSha256;
                         var doiObservations = await dbContext.DigitalObjectIdentifiers.Include(i => i.Children)
                             .SingleAsync(i => i.DOIType == DOIType.ObservationsDb);
                         var metaObservations = await MetadataForDOIAsync(doiObservations, null);
                         var doiDynamics = await dbContext.DigitalObjectIdentifiers.Include(i => i.Parent).Include(i => i.Children)
                             .SingleAsync(i => i.DOIType == DOIType.Collection && i.Code == DOIHelper.DynamicDOIsCode);
                         var metaDynamics = await MetadataForDOIAsync(doiDynamics, metaObservations);
                         var doiPeriodics = await dbContext.DigitalObjectIdentifiers.Include(i => i.Parent).Include(i => i.Children)
                             .SingleAsync(i => i.DOIType == DOIType.Collection && i.Code == DOIHelper.PeriodicDOIsCode);
                         var metaPeriodics = await MetadataForDOIAsync(doiPeriodics, metaObservations);
                         var doiAdHocs = await dbContext.DigitalObjectIdentifiers.Include(i => i.Parent).Include(i => i.Children)
                             .SingleAsync(i => i.DOIType == DOIType.Collection && i.Code == DOIHelper.AdHocDOIsCode);
                         var metaAdHocs = await MetadataForDOIAsync(doiAdHocs, metaObservations);
                         var orgCodes = new string[] { "SAEON", "SMCRI", "EFTEON" };
                         foreach (var doiOrganisation in await dbContext
                             .DigitalObjectIdentifiers
                             .Include(i => i.Parent)
                             .Include(i => i.Children)
                             .Where(i => i.DOIType == DOIType.Organisation && orgCodes.Contains(i.Code))
                             .ToListAsync())
                         {
                             var organisation = await dbContext.Organisations.SingleAsync(i => i.Code == doiOrganisation.Code);
                             var metaOrganisation = await MetadataForDOIAsync(doiOrganisation, metaDynamics);
                             foreach (var doiProgramme in await dbContext
                                 .DigitalObjectIdentifiers
                                 .Include(i => i.Parent)
                                 .Include(i => i.Children)
                                 .Where(i => i.ParentId == doiOrganisation.Id)
                                 .ToListAsync())
                             {
                                 var programme = await dbContext.Programmes.SingleAsync(i => i.Code == doiProgramme.Code);
                                 var metaProgramme = await MetadataForDOIAsync(doiProgramme, metaOrganisation);
                                 foreach (var doiProject in await dbContext
                                     .DigitalObjectIdentifiers
                                     .Include(i => i.Parent)
                                     .Include(i => i.Children)
                                     .Where(i => i.ParentId == doiProgramme.Id)
                                     .ToListAsync())
                                 {
                                     var project = await dbContext.Projects.SingleAsync(i => i.Code == doiProject.Code);
                                     var metaProject = await MetadataForDOIAsync(doiProject, metaProgramme);
                                     foreach (var doiSite in await dbContext
                                         .DigitalObjectIdentifiers
                                         .Include(i => i.Parent)
                                         .Include(i => i.Children)
                                         .Where(i => i.ParentId == doiProject.Id)
                                         .ToListAsync())
                                     {
                                         var site = await dbContext.Sites.SingleAsync(i => i.Code == doiSite.Code);
                                         var metaSite = await MetadataForDOIAsync(doiSite, metaProject);
                                         foreach (var doiStation in await dbContext
                                             .DigitalObjectIdentifiers
                                             .Include(i => i.Parent)
                                             .Include(i => i.Children)
                                             .Where(i => i.ParentId == doiSite.Id)
                                             .ToListAsync())
                                         {
                                             var station = await dbContext.Stations.SingleAsync(i => i.Code == doiStation.Code);
                                             var metaStation = await MetadataForDOIAsync(doiStation, metaSite);
                                             foreach (var doiDataset in await dbContext
                                                 .DigitalObjectIdentifiers
                                                 .Include(i => i.Parent)
                                                 .Include(i => i.Children)
                                                 .Where(i => i.ParentId == doiStation.Id)
                                                 .ToListAsync())
                                             {
                                                 var splits = doiDataset.Code.Split('~', StringSplitOptions.RemoveEmptyEntries);
                                                 var dataset = await dbContext.Datasets.Where(i =>
                                                     i.StationCode == splits[0] &&
                                                     i.PhenomenonCode == splits[1] &&
                                                     i.OfferingCode == splits[2] &&
                                                     i.UnitCode == splits[3])
                                                     .SingleAsync();
                                                 var metaDataset = await MetadataForDOIAsync(doiDataset, metaStation);
                                                 metaDataset.StartDate = dataset.StartDate;
                                                 metaDataset.EndDate = dataset.EndDate;
                                                 metaDataset.LatitudeNorth = dataset.LatitudeNorth;
                                                 metaDataset.LatitudeSouth = dataset.LatitudeSouth;
                                                 metaDataset.LongitudeWest = dataset.LongitudeWest;
                                                 metaDataset.LongitudeEast = dataset.LongitudeEast;
                                                 metaDataset.ElevationMinimum = dataset.ElevationMinimum;
                                                 metaDataset.ElevationMaximum = dataset.ElevationMaximum;
                                                 metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(organisation.Name) });
                                                 metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(programme.Name) });
                                                 metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(project.Name) });
                                                 metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(site.Name) });
                                                 metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(station.Name) });
                                                 metaDataset.Subjects.Add(new MetadataSubject { Name = dataset.PhenomenonName });
                                                 metaDataset.Subjects.Add(new MetadataSubject { Name = $"{dataset.PhenomenonName}, {dataset.OfferingName}, {dataset.UnitName}" });
                                                 foreach (var doiPeriodic in await dbContext
                                                      .DigitalObjectIdentifiers
                                                      .Include(i => i.Parent)
                                                      .Include(i => i.Children)
                                                      .Where(i => i.ParentId == doiDataset.Id)
                                                      .ToListAsync())
                                                 {
                                                     var metaPeriodic = await MetadataForDOIAsync(doiPeriodic, metaDataset);
                                                     splits = doiPeriodic.Code.Split('~', StringSplitOptions.RemoveEmptyEntries);
                                                     var importBatchSummary = await dbContext.ImportBatchSummaries.Include(i => i.ImportBatch).Where(i => i.Id == new Guid(splits[4])).SingleAsync();
                                                     var instrument = await dbContext.Instruments.SingleAsync(i => i.Id == importBatchSummary.InstrumentId);
                                                     var sensor = await dbContext.Sensors.SingleAsync(i => i.Id == importBatchSummary.SensorId);
                                                     metaPeriodic.StartDate = importBatchSummary.StartDate;
                                                     metaPeriodic.EndDate = importBatchSummary.EndDate;
                                                     metaPeriodic.LatitudeNorth = importBatchSummary.LatitudeNorth;
                                                     metaPeriodic.LatitudeSouth = importBatchSummary.LatitudeSouth;
                                                     metaPeriodic.LongitudeWest = importBatchSummary.LongitudeWest;
                                                     metaPeriodic.LongitudeEast = importBatchSummary.LongitudeEast;
                                                     metaPeriodic.ElevationMinimum = importBatchSummary.ElevationMinimum;
                                                     metaPeriodic.ElevationMaximum = importBatchSummary.ElevationMaximum;
                                                     metaPeriodic.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(dataset.StationName) });
                                                     metaPeriodic.Subjects.Add(new MetadataSubject { Name = $"{dataset.PhenomenonName}" });
                                                     metaPeriodic.Subjects.Add(new MetadataSubject { Name = $"{dataset.PhenomenonName}, {dataset.OfferingName}, {dataset.UnitName}" });
                                                     var periodicName = metaPeriodic.GenerateTitle($"{dataset.StationName}, instrument {instrument.Name}, sensor {sensor.Name} " +
                                                             $"of {dataset.PhenomenonName}, {dataset.OfferingName}, {dataset.UnitName} for import batch {importBatchSummary.ImportBatch.Code}");
                                                     metaPeriodic.Generate(periodicName, periodicName);
                                                     doiPeriodic.Title = metaPeriodic.Title;
                                                     doiPeriodic.MetadataJson = metaPeriodic.ToJson();
                                                     oldSha256 = doiPeriodic.MetadataJsonSha256;
                                                     doiPeriodic.MetadataJsonSha256 = doiPeriodic.MetadataJson.Sha256();
                                                     doiPeriodic.ODPMetadataNeedsUpdate = oldSha256 != doiPeriodic.MetadataJsonSha256;
                                                     doiPeriodic.MetadataHtml = metaPeriodic.ToHtml();
                                                     doiPeriodic.CitationHtml = metaPeriodic.CitationHtml;
                                                     doiPeriodic.CitationText = metaPeriodic.CitationText;
                                                     doiPeriodic.QueryUrl = $"https://observations.saeon.ac.za/Download/{doiPeriodic.DOI}";
                                                     await dbContext.SaveChangesAsync();
                                                 }
                                                 var datasetName = metaDataset.GenerateTitle($"{dataset.StationName} of {dataset.PhenomenonName}, {dataset.OfferingName}, {dataset.UnitName}");
                                                 metaDataset.Generate(datasetName, datasetName);
                                                 doiDataset.Title = metaDataset.Title;
                                                 doiDataset.MetadataJson = metaDataset.ToJson();
                                                 oldSha256 = doiDataset.MetadataJsonSha256;
                                                 doiDataset.MetadataJsonSha256 = doiDataset.MetadataJson.Sha256();
                                                 doiDataset.ODPMetadataNeedsUpdate = (oldSha256 != doiDataset.MetadataJsonSha256) || (!doiDataset.ODPMetadataIsValid ?? true);
                                                 doiDataset.MetadataHtml = metaDataset.ToHtml();
                                                 doiDataset.CitationHtml = metaDataset.CitationHtml;
                                                 doiDataset.CitationText = metaDataset.CitationText;
                                                 doiDataset.SetUrls();
                                                 await dbContext.SaveChangesAsync();
                                             }
                                             metaStation.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(station.Name) });
                                             metaStation.Generate();
                                             if (!string.IsNullOrWhiteSpace(station.Url))
                                             {
                                                 metaStation.Title += $" {station.Url}";
                                             }
                                             doiStation.Title = metaStation.Title;
                                             doiStation.MetadataJson = metaStation.ToJson();
                                             oldSha256 = doiStation.MetadataJsonSha256;
                                             doiStation.MetadataJsonSha256 = doiStation.MetadataJson.Sha256();
                                             doiStation.ODPMetadataNeedsUpdate = oldSha256 != doiStation.MetadataJsonSha256 || (!doiStation.ODPMetadataIsValid ?? true); ;
                                             doiStation.MetadataHtml = metaStation.ToHtml();
                                             doiStation.CitationHtml = metaStation.CitationHtml;
                                             doiStation.CitationText = metaStation.CitationText;
                                             doiStation.SetUrls();
                                             await dbContext.SaveChangesAsync();
                                         }
                                         metaSite.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(site.Name) });
                                         metaSite.Generate();
                                         if (!string.IsNullOrWhiteSpace(site.Url))
                                         {
                                             metaSite.Title += $" {site.Url}";
                                         }
                                         doiSite.Title = metaSite.Title;
                                         doiSite.MetadataJson = metaSite.ToJson();
                                         oldSha256 = doiSite.MetadataJsonSha256;
                                         doiSite.MetadataJsonSha256 = doiSite.MetadataJson.Sha256();
                                         doiSite.ODPMetadataNeedsUpdate = oldSha256 != doiSite.MetadataJsonSha256 || (!doiSite.ODPMetadataIsValid ?? true); ;
                                         doiSite.MetadataHtml = metaSite.ToHtml();
                                         doiSite.CitationHtml = metaSite.CitationHtml;
                                         doiSite.CitationText = metaSite.CitationText;
                                         doiSite.SetUrls();
                                         await dbContext.SaveChangesAsync();
                                     }
                                     metaProject.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(project.Name) });
                                     metaProject.Generate();
                                     if (!string.IsNullOrWhiteSpace(project.Url))
                                     {
                                         metaProject.Title += $" {project.Url}";
                                     }
                                     doiProject.Title = metaProject.Title;
                                     doiProject.MetadataJson = metaProject.ToJson();
                                     oldSha256 = doiProject.MetadataJsonSha256;
                                     doiProject.MetadataJsonSha256 = doiProject.MetadataJson.Sha256();
                                     doiProject.ODPMetadataNeedsUpdate = oldSha256 != doiProject.MetadataJsonSha256 || (!doiProject.ODPMetadataIsValid ?? true); ;
                                     doiProject.MetadataHtml = metaProject.ToHtml();
                                     doiProject.CitationHtml = metaProject.CitationHtml;
                                     doiProject.CitationText = metaProject.CitationText;
                                     doiProject.SetUrls();
                                     await dbContext.SaveChangesAsync();
                                 }
                                 metaProgramme.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(programme.Name) });
                                 metaProgramme.Generate();
                                 if (!string.IsNullOrWhiteSpace(programme.Url))
                                 {
                                     metaProgramme.Title += $" {programme.Url}";
                                 }
                                 doiProgramme.Title = metaProgramme.Title;
                                 doiProgramme.MetadataJson = metaProgramme.ToJson();
                                 oldSha256 = doiProgramme.MetadataJsonSha256;
                                 doiProgramme.MetadataJsonSha256 = doiProgramme.MetadataJson.Sha256();
                                 doiProgramme.ODPMetadataNeedsUpdate = oldSha256 != doiProgramme.MetadataJsonSha256 || (!doiProgramme.ODPMetadataIsValid ?? true); ;
                                 doiProgramme.MetadataHtml = metaProgramme.ToHtml();
                                 doiProgramme.CitationHtml = metaProgramme.CitationHtml;
                                 doiProgramme.CitationText = metaProgramme.CitationText;
                                 doiProgramme.SetUrls();
                                 await dbContext.SaveChangesAsync();
                             }
                             metaOrganisation.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(organisation.Name) });
                             metaOrganisation.Generate();
                             if (!string.IsNullOrWhiteSpace(organisation.Url))
                             {
                                 metaOrganisation.Title += $" {organisation.Url}";
                             }
                             doiOrganisation.Title = metaOrganisation.Title;
                             doiOrganisation.MetadataJson = metaOrganisation.ToJson();
                             oldSha256 = doiOrganisation.MetadataJsonSha256;
                             doiOrganisation.MetadataJsonSha256 = doiOrganisation.MetadataJson.Sha256();
                             doiOrganisation.ODPMetadataNeedsUpdate = oldSha256 != doiOrganisation.MetadataJsonSha256 || (!doiOrganisation.ODPMetadataIsValid ?? true); ;
                             doiOrganisation.MetadataHtml = metaOrganisation.ToHtml();
                             doiOrganisation.CitationHtml = metaOrganisation.CitationHtml;
                             doiOrganisation.CitationText = metaOrganisation.CitationText;
                             doiOrganisation.SetUrls();
                             await dbContext.SaveChangesAsync();
                         }

                         // AdHocs
                         foreach (var doiAdHoc in await dbContext
                             .DigitalObjectIdentifiers
                             .Include(i => i.Parent)
                             .Include(i => i.Children)
                             .Where(i => i.DOIType == DOIType.AdHoc)
                             .ToListAsync())
                         {
                             var metaAddHoc = await MetadataForDOIAsync(doiAdHoc, metaAdHocs);
                             metaAddHoc.Generate();
                         }

                         // DynamicDOIs collection
                         metaDynamics.Generate("Collection of Dynamic DOIs in the SAEON Observations Database", "The collection of Dynamic DOIs in the SAEON Observations Database");
                         doiDynamics.Title = metaDynamics.Title;
                         doiDynamics.MetadataJson = metaDynamics.ToJson();
                         oldSha256 = doiDynamics.MetadataJsonSha256;
                         doiDynamics.MetadataJsonSha256 = doiDynamics.MetadataJson.Sha256();
                         doiDynamics.ODPMetadataNeedsUpdate = oldSha256 != doiDynamics.MetadataJsonSha256 || (!doiDynamics.ODPMetadataIsValid ?? true); ;
                         doiDynamics.MetadataHtml = metaDynamics.ToHtml();
                         doiDynamics.CitationHtml = metaDynamics.CitationHtml;
                         doiDynamics.CitationText = metaDynamics.CitationText;
                         doiDynamics.SetUrls();

                         // PeriodicDOIs collection
                         metaPeriodics.Generate("Collection of Periodic DOIs in the SAEON Observations Database", "The collection of Periodic DOIs in the SAEON Observations Database");
                         doiPeriodics.Title = metaPeriodics.Title;
                         doiPeriodics.MetadataJson = metaPeriodics.ToJson();
                         oldSha256 = doiPeriodics.MetadataJsonSha256;
                         doiPeriodics.MetadataJsonSha256 = doiPeriodics.MetadataJson.Sha256();
                         doiPeriodics.ODPMetadataNeedsUpdate = oldSha256 != doiPeriodics.MetadataJsonSha256 || (!doiPeriodics.ODPMetadataIsValid ?? true); ;
                         doiPeriodics.MetadataHtml = metaPeriodics.ToHtml();
                         doiPeriodics.CitationHtml = metaPeriodics.CitationHtml;
                         doiPeriodics.CitationText = metaPeriodics.CitationText;
                         doiPeriodics.SetUrls();

                         // AdHocDOIs collection
                         metaAdHocs.Generate("Collection of AdHoc DOIs in the SAEON Observations Database", "The collection of AdHoc DOIs in the SAEON Observations Database");
                         doiAdHocs.Title = metaAdHocs.Title;
                         doiAdHocs.MetadataJson = metaAdHocs.ToJson();
                         oldSha256 = doiAdHocs.MetadataJsonSha256;
                         doiAdHocs.MetadataJsonSha256 = doiAdHocs.MetadataJson.Sha256();
                         doiAdHocs.ODPMetadataNeedsUpdate = oldSha256 != doiAdHocs.MetadataJsonSha256 || (!doiAdHocs.ODPMetadataIsValid ?? true); ;
                         doiAdHocs.MetadataHtml = metaAdHocs.ToHtml();
                         doiAdHocs.CitationHtml = metaAdHocs.CitationHtml;
                         doiAdHocs.CitationText = metaAdHocs.CitationText;
                         doiAdHocs.SetUrls();

                         // Whole collection
                         metaObservations.Generate("Complete collection of observations in the SAEON Observations Database",
                             "The complete collection of observations in the SAEON Observations Database");
                         doiObservations.Title = metaObservations.Title;
                         doiObservations.MetadataJson = metaObservations.ToJson();
                         oldSha256 = doiObservations.MetadataJsonSha256;
                         doiObservations.MetadataJsonSha256 = doiObservations.MetadataJson.Sha256();
                         doiObservations.ODPMetadataNeedsUpdate = oldSha256 != doiObservations.MetadataJsonSha256 || (!doiObservations.ODPMetadataIsValid ?? true); ;
                         doiObservations.MetadataHtml = metaObservations.ToHtml();
                         doiObservations.CitationHtml = metaObservations.CitationHtml;
                         doiObservations.CitationText = metaObservations.CitationText;
                         doiObservations.SetUrls();
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
         */
    }
}