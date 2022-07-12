using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SAEON.AspNet.Auth;
using SAEON.Core;
using SAEON.Logs;
using SAEON.Observations.Core;
using SAEON.Observations.WebAPI.Hubs;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class DOIHelper
    {
        public static readonly string BlankJson = "{}";
        public static readonly string BlankHtml = "<>";
        public static readonly string BlankText = "()";

        public static readonly string ObservationsDbCode = "ObservationsDB";
        public static readonly string DynamicDOIsCode = "DynamicDOIs";
        public static readonly string PeriodicDOIsCode = "PeriodicDOIs";
        public static readonly string AdHocDOIsCode = "AddHocDOIs";

        public static async Task<string> CreateDOIs(ObservationsDbContext dbContext, IHubContext<AdminHub> adminHub, HttpContext httpContext)
        {
            using (SAEONLogs.MethodCall(typeof(DOIHelper)))
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var sb = new StringBuilder();
                    await AddLineAsync("Creating DOIs");
                    await GenerateDOIs();
                    await AddLineAsync($"Done in {stopwatch.Elapsed.TimeStr()}");
                    return sb.ToString();

                    async Task AddLineAsync(string line)
                    {
                        sb.AppendLine(line);
                        SAEONLogs.Information(line);
                        await adminHub.Clients.All.SendAsync(SignalRDefaults.CreateDOIsStatus, line);
                    }

                    async Task GenerateDOIs()
                    {
                        async Task<DigitalObjectIdentifier> AddDOI(DOIType doiType, string code, string name)
                        {
                            await AddLineAsync($"Adding {doiType} {code}, {name}");
                            var doi = new DigitalObjectIdentifier
                            {
                                DOIType = doiType,
                                Code = code,
                                Name = name ?? code,
                                Title = BlankText,
                                Description = BlankText,
                                DescriptionHtml = BlankHtml,
                                Citation = BlankText,
                                CitationHtml = BlankHtml,
                                MetadataJson = BlankJson,
                                MetadataJsonSha256 = BlankJson.Sha256(),
                                MetadataUrl = "https://catalogue.saeon.ac.za/records/",
                                MetadataHtml = BlankHtml,
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

                        async Task<Dataset> EnsureDataset(VInventoryDataset inventoryDataset)
                        {
                            var dataset = await dbContext.Datasets.FirstOrDefaultAsync(i =>
                                i.StationId == inventoryDataset.StationId &&
                                i.PhenomenonOfferingId == inventoryDataset.PhenomenonOfferingId &&
                                i.PhenomenonUnitId == inventoryDataset.PhenomenonUnitId);
                            if (dataset is null)
                            {
                                await AddLineAsync($"Adding dataset {inventoryDataset.Code}, {inventoryDataset.Name}");
                                dataset = new Dataset
                                {
                                    Code = inventoryDataset.Code,
                                    Name = inventoryDataset.Name,
                                    StationId = inventoryDataset.StationId,
                                    PhenomenonOfferingId = inventoryDataset.PhenomenonOfferingId,
                                    PhenomenonUnitId = inventoryDataset.PhenomenonUnitId,
                                    NeedsUpdate = true,
                                    AddedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString(),
                                    UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString(),
                                    UserId = new Guid(httpContext?.User?.UserId() ?? Guid.Empty.ToString()),
                                };
                                dbContext.Datasets.Add(dataset);
                                await dbContext.SaveChangesAsync();
                            }
                            dataset.Count = inventoryDataset.Count;
                            dataset.ValueCount = inventoryDataset.ValueCount;
                            dataset.NullCount = inventoryDataset.NullCount;
                            dataset.VerifiedCount = inventoryDataset.VerifiedCount;
                            dataset.UnverifiedCount = inventoryDataset.UnverifiedCount;
                            dataset.StartDate = inventoryDataset.StartDate;
                            dataset.EndDate = inventoryDataset.EndDate;
                            dataset.LatitudeNorth = inventoryDataset.LatitudeNorth;
                            dataset.LatitudeSouth = inventoryDataset.LatitudeSouth;
                            dataset.LongitudeWest = inventoryDataset.LongitudeWest;
                            dataset.LongitudeEast = inventoryDataset.LongitudeEast;
                            dataset.ElevationMinimum = inventoryDataset.ElevationMinimum;
                            dataset.ElevationMaximum = inventoryDataset.ElevationMaximum;
                            var oldHashCode = dataset.HashCode;
                            var newHashCode = dataset.CreateHashCode();
                            SAEONLogs.Verbose("OldHashCode: {OldHashCode} NewHashCode: {NewHashCode}", oldHashCode, newHashCode);
                            if (oldHashCode != newHashCode)
                            {
                                dataset.HashCode = newHashCode;
                                dataset.NeedsUpdate = true;
                            }
                            return dataset;
                        }

                        async Task<DigitalObjectIdentifier> EnsureDatasetDOI(VInventoryDataset inventoryDataset)
                        {
                            var doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Dataset && i.Code == inventoryDataset.Code);
                            if (doi is null)
                            {
                                // @@ Remove once all SACTN recycled
                                // Recycle SACTN hourly
                                doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Dataset && i.Code.StartsWith("SACTN ") && i.Code.EndsWith("SAEON~WTMP~AVE_H~DEGC"));
                                UpdateDOI();
                                // Recycle SACTN daily
                                if (doi is null)
                                {
                                    doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Dataset && i.Code.StartsWith("SACTN ") && i.Code.EndsWith("SAEON~WTMP~AVE_D~DEGC"));
                                    UpdateDOI();
                                }
                                // Recycle SACTN monthly
                                if (doi is null)
                                {
                                    doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Dataset && i.Code.StartsWith("SACTN ") && i.Code.EndsWith("SAEON~WTMP~AVE_M~DEGC"));
                                    UpdateDOI();
                                }
                                // Recycle SACTN yearly
                                if (doi is null)
                                {
                                    doi = await dbContext.DigitalObjectIdentifiers.SingleOrDefaultAsync(i => i.DOIType == DOIType.Dataset && i.Code.StartsWith("SACTN ") && i.Code.EndsWith("SAEON~WTMP~AVE_Y~DEGC"));
                                    UpdateDOI();
                                }
                                // None to recycle
                                if (doi is null)
                                    doi = await AddDOI(DOIType.Dataset, inventoryDataset.Code, inventoryDataset.Name);
                                await dbContext.SaveChangesAsync();
                            }
                            return doi;

                            void UpdateDOI()
                            {
                                if (doi is not null)
                                {
                                    doi.Code = inventoryDataset.Code;
                                    doi.Name = inventoryDataset.Name;
                                    doi.ODPMetadataNeedsUpdate = true;
                                    doi.ODPMetadataIsValid = false;
                                    doi.ODPMetadataIsPublished = false;
                                }
                            }
                        }

                        foreach (var inventoryDataset in await dbContext.VInventoryDatasets
                            .Where(i => i.IsValid ?? false)
                            .OrderBy(i => i.OrganisationName)
                            .ThenBy(i => i.ProgrammeName)
                            .ThenBy(i => i.ProgrammeName)
                            .ThenBy(i => i.SiteName)
                            .ThenBy(i => i.StationName)
                            .ToListAsync())
                        {
                            var doiDataset = await EnsureDatasetDOI(inventoryDataset);
                            var dataset = await EnsureDataset(inventoryDataset);
                            dataset.DigitalObjectIdentifierId = doiDataset.Id;
                            doiDataset.DatasetId = dataset.Id;
                            if (dbContext.Entry(doiDataset).State != EntityState.Unchanged)
                            {
                                doiDataset.UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString();
                            }
                            if (dbContext.Entry(dataset).State != EntityState.Unchanged)
                            {
                                dataset.UpdatedBy = httpContext?.User?.UserId() ?? Guid.Empty.ToString();
                                dataset.UserId = new Guid(httpContext?.User?.UserId() ?? Guid.Empty.ToString());
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
        }

    }
}