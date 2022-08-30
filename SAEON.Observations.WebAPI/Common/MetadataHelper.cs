using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static async Task<string> CreateMetadata(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub, HttpContext httpContext)
        {
            using (SAEONLogs.MethodCall(typeof(MetadataHelper)))
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var sb = new StringBuilder();
                    await AddLineAsync("Creating Metadata");
                    await GenerateMetadata();
                    await AddLineAsync($"Done in {stopwatch.Elapsed.TimeStr()}");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateMetadataStatus, line);
                    }

                    async Task GenerateMetadata()
                    {
                        //async Task<Metadata> MetadataForDOIAsync(DigitalObjectIdentifier doi, Metadata parent)
                        Metadata MetadataForDOI(DigitalObjectIdentifier doi, Metadata parent)
                        {
                            //await AddLineAsync($"{doi.DOIType} {doi.Code}, {doi.Name}");
                            var metadata = new Metadata
                            {
                                DOI = doi,
                                Parent = parent
                            };
                            return metadata;
                        }

                        byte[] oldSha256;
                        foreach (var doiDataset in await dbContext.DigitalObjectIdentifiers.Include(i => i.Dataset).Where(i => i.DOIType == DOIType.Dataset).ToListAsync())
                        {
                            //if (doiDataset.Code.StartsWith("SACTN")) continue;
                            SAEONLogs.Verbose($"{doiDataset.DOIType} {doiDataset.Code}, {doiDataset.Name}");
                            doiDataset.SetUrls();
                            var datasetExpansion = await dbContext.VDatasetsExpansion.FirstOrDefaultAsync(i => i.Id == doiDataset.DatasetId);
                            if (datasetExpansion is null)
                            {
                                SAEONLogs.Warning($"Ignoring (No dataset) {doiDataset.DOIType} {doiDataset.Code}, {doiDataset.Name}");
                                await AddLineAsync($"Ignoring (No dataset) {doiDataset.DOIType} {doiDataset.Code}, {doiDataset.Name}");
                                continue;
                            }
                            // Box with point Lat or Lon
                            if ((datasetExpansion.LatitudeNorth != datasetExpansion.LatitudeSouth) && (datasetExpansion.LongitudeEast == datasetExpansion.LongitudeWest) ||
                                (datasetExpansion.LatitudeNorth == datasetExpansion.LatitudeSouth) && (datasetExpansion.LongitudeEast != datasetExpansion.LongitudeWest))
                            {
                                SAEONLogs.Warning($"Ignoring (Bad box) {doiDataset.DOIType} {doiDataset.Code}, {doiDataset.Name}");
                                await AddLineAsync($"Ignoring (Bad box) {doiDataset.DOIType} {doiDataset.Code}, {doiDataset.Name}");
                                continue;
                            }
                            var metaDataset = MetadataForDOI(doiDataset, null);
                            metaDataset.StartDate = datasetExpansion.StartDate;
                            metaDataset.EndDate = datasetExpansion.EndDate;
                            metaDataset.LatitudeNorth = datasetExpansion.LatitudeNorth;
                            metaDataset.LatitudeSouth = datasetExpansion.LatitudeSouth;
                            metaDataset.LongitudeWest = datasetExpansion.LongitudeWest;
                            metaDataset.LongitudeEast = datasetExpansion.LongitudeEast;
                            metaDataset.ElevationMinimum = datasetExpansion.ElevationMinimum;
                            metaDataset.ElevationMaximum = datasetExpansion.ElevationMaximum;
                            metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(datasetExpansion.OrganisationName) });
                            metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(datasetExpansion.ProgrammeName) });
                            metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(datasetExpansion.ProjectName) });
                            metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(datasetExpansion.SiteName) });
                            metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(datasetExpansion.StationName) });
                            metaDataset.Subjects.Add(new MetadataSubject { Name = CleanPrefixes(datasetExpansion.PhenomenonName) });
                            metaDataset.Subjects.Add(new MetadataSubject { Name = $"{CleanPrefixes(datasetExpansion.PhenomenonName)}, {datasetExpansion.OfferingName}, {datasetExpansion.UnitName}" });
                            var variable = $"{CleanPrefixes(datasetExpansion.PhenomenonName)}, {datasetExpansion.OfferingName}, {datasetExpansion.UnitName}";
                            var siteName = CleanPrefixes(datasetExpansion.SiteName);
                            var stationName = CleanPrefixes(datasetExpansion.StationName);
                            string station;
                            if (datasetExpansion.StationName.StartsWith("ELW, "))
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
                            doiDataset.MetadataJson = metaDataset.ToJson();
                            oldSha256 = doiDataset.MetadataJsonSha256;
                            doiDataset.MetadataJsonSha256 = doiDataset.MetadataJson.Sha256();
                            doiDataset.ODPMetadataNeedsUpdate = (!ShaEqual(oldSha256, doiDataset.MetadataJsonSha256)) || (!doiDataset.ODPMetadataIsValid ?? true);
                            if (doiDataset.ODPMetadataNeedsUpdate ?? false)
                            {
                                doiDataset.ODPMetadataIsPublished = false;
                            }
                            doiDataset.MetadataHtml = metaDataset.ToHtml();
                            doiDataset.Dataset.Title = doiDataset.Title;
                            doiDataset.Dataset.Description = doiDataset.Description;
                            if (dbContext.Entry(doiDataset).State != EntityState.Unchanged)
                            {
                                doiDataset.UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString();
                                await AddLineAsync($"{doiDataset.DOIType} {doiDataset.Code}, {doiDataset.Name}");
                            }
                            if (dbContext.Entry(doiDataset.Dataset).State != EntityState.Unchanged)
                            {
                                doiDataset.Dataset.UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString();
                                doiDataset.Dataset.UserId = new Guid(httpContext?.User?.UserId() ?? Guid.Empty.ToString());
                            }
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

            bool ShaEqual(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
            {
                return a1.SequenceEqual(a2);
            }
        }
    }
}