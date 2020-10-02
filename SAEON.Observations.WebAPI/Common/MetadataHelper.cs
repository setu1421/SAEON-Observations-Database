using Microsoft.EntityFrameworkCore;
using SAEON.Logs;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.WebAPI
{
    public static class MetadataHelper
    {
        public static DOIType[] DynamicDOITypes = { DOIType.ObservationsDb, DOIType.Organisation, DOIType.Programme, DOIType.Project, DOIType.Site, DOIType.Station, DOIType.Dataset };

        public static async Task<string> CreateMetadata(ObservationsDbContext dbContext)
        {
            var sb = new StringBuilder();
            await GenerateMetadata();
            AddLine("Done");
            return sb.ToString();

            void AddLine(string line)
            {
                sb.AppendLine(line);
                SAEONLogs.Information(line);
            }

            async Task GenerateMetadata()
            {
                Metadata MetadataForDOI(DigitalObjectIdentifier doi, Metadata parent, string itemDescription = null, string itemUrl = null)
                {
                    AddLine($"{doi.DOIType} {doi.Code}, {doi.Name}");
                    var metadata = new Metadata
                    {
                        DOI = doi,
                        Parent = parent,
                        ItemDescription = itemDescription,
                        ItemUrl = itemUrl
                    };
                    metadata.AlternateIdentifiers.Add(new MetadataAlternateIdentifier { Name = doi.AlternateId.ToString(), Type = "Internal unique identifier" });
                    return metadata;
                }

                AddLine("Generating metadata");
                byte[] oldSha256;
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
                                        var dataset = await dbContext.Datasets.Where(i =>
                                            i.StationCode == splits[0] &&
                                            i.PhenomenonCode == splits[1] &&
                                            i.OfferingCode == splits[2] &&
                                            i.UnitCode == splits[3])
                                            .SingleAsync();
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
                                        //foreach (var doiPeriodic in await dbContext
                                        //     .DigitalObjectIdentifiers
                                        //     .Include(i => i.Parent)
                                        //     .Include(i => i.Children)
                                        //     .Where(i => i.ParentId == doiDataset.Id)
                                        //     .ToListAsync())
                                        //{
                                        //    var metaPeriodic = MetadataForDOI(doiPeriodic, metaDataset);
                                        //    splits = doiPeriodic.Code.Split('~', StringSplitOptions.RemoveEmptyEntries);
                                        //    var importBatchSummary = await dbContext.ImportBatchSummary.Include(i => i.ImportBatch).Where(i => i.Id == new Guid(splits[4])).SingleAsync();
                                        //    var instrument = await dbContext.Instruments.SingleAsync(i => i.Id == importBatchSummary.InstrumentId);
                                        //    var sensor = await dbContext.Sensors.SingleAsync(i => i.Id == importBatchSummary.SensorId);
                                        //    metaPeriodic.StartDate = importBatchSummary.StartDate;
                                        //    metaPeriodic.EndDate = importBatchSummary.EndDate;
                                        //    metaPeriodic.LatitudeNorth = importBatchSummary.LatitudeNorth;
                                        //    metaPeriodic.LatitudeSouth = importBatchSummary.LatitudeSouth;
                                        //    metaPeriodic.LongitudeWest = importBatchSummary.LongitudeWest;
                                        //    metaPeriodic.LongitudeEast = importBatchSummary.LongitudeEast;
                                        //    metaPeriodic.ElevationMinimum = importBatchSummary.ElevationMinimum;
                                        //    metaPeriodic.ElevationMaximum = importBatchSummary.ElevationMaximum;
                                        //    metaPeriodic.Subjects.Add(new MetadataSubject { Name = dataset.StationName });
                                        //    metaPeriodic.Subjects.Add(new MetadataSubject { Name = $"{dataset.PhenomenonName}" });
                                        //    metaPeriodic.Subjects.Add(new MetadataSubject { Name = $"{dataset.PhenomenonName} {dataset.OfferingName} {dataset.UnitName}" });
                                        //    var periodicName = metaPeriodic.GenerateTitle($"{dataset.StationName}, instrument {instrument.Name}, sensor {sensor.Name} " +
                                        //            $"of {dataset.PhenomenonName}, {dataset.OfferingName}, {dataset.UnitName} for import batch {importBatchSummary.ImportBatch.Code}");
                                        //    metaPeriodic.Generate(periodicName, periodicName);
                                        //    doiPeriodic.MetadataJson = metaPeriodic.ToJson();
                                        //    oldSha256 = doiPeriodic.MetadataJsonSha256;
                                        //    doiPeriodic.MetadataJsonSha256 = doiPeriodic.MetadataJson.Sha256();
                                        //    doiPeriodic.ODPMetadataNeedsUpdate = oldSha256 != doiPeriodic.MetadataJsonSha256;
                                        //    doiPeriodic.MetadataHtml = metaPeriodic.ToHtml();
                                        //    await dbContext.SaveChangesAsync();
                                        //}
                                        var datasetName = metaDataset.GenerateTitle($"{dataset.StationName} of {dataset.PhenomenonName}, {dataset.OfferingName}, {dataset.UnitName}");
                                        metaDataset.Generate(datasetName, datasetName);
                                        doiDataset.MetadataJson = metaDataset.ToJson();
                                        oldSha256 = doiDataset.MetadataJsonSha256;
                                        doiDataset.MetadataJsonSha256 = doiDataset.MetadataJson.Sha256();
                                        doiDataset.ODPMetadataNeedsUpdate = oldSha256 != doiDataset.MetadataJsonSha256;
                                        doiDataset.MetadataHtml = metaDataset.ToHtml();
                                        await dbContext.SaveChangesAsync();
                                    }
                                    metaStation.Generate();
                                    doiStation.MetadataJson = metaStation.ToJson();
                                    oldSha256 = doiStation.MetadataJsonSha256;
                                    doiStation.MetadataJsonSha256 = doiStation.MetadataJson.Sha256();
                                    doiStation.ODPMetadataNeedsUpdate = oldSha256 != doiStation.MetadataJsonSha256;
                                    doiStation.MetadataHtml = metaStation.ToHtml();
                                    await dbContext.SaveChangesAsync();
                                }
                                metaSite.Generate();
                                doiSite.MetadataJson = metaSite.ToJson();
                                oldSha256 = doiSite.MetadataJsonSha256;
                                doiSite.MetadataJsonSha256 = doiSite.MetadataJson.Sha256();
                                doiSite.ODPMetadataNeedsUpdate = oldSha256 != doiSite.MetadataJsonSha256;
                                doiSite.MetadataHtml = metaSite.ToHtml();
                                await dbContext.SaveChangesAsync();
                            }
                            metaProject.Generate();
                            doiProject.MetadataJson = metaProject.ToJson();
                            oldSha256 = doiProject.MetadataJsonSha256;
                            doiProject.MetadataJsonSha256 = doiProject.MetadataJson.Sha256();
                            doiProject.ODPMetadataNeedsUpdate = oldSha256 != doiProject.MetadataJsonSha256;
                            doiProject.MetadataHtml = metaProject.ToHtml();
                            await dbContext.SaveChangesAsync();
                        }
                        metaProgramme.Generate();
                        doiProgramme.MetadataJson = metaProgramme.ToJson();
                        oldSha256 = doiProgramme.MetadataJsonSha256;
                        doiProgramme.MetadataJsonSha256 = doiProgramme.MetadataJson.Sha256();
                        doiProgramme.ODPMetadataNeedsUpdate = oldSha256 != doiProgramme.MetadataJsonSha256;
                        doiProgramme.MetadataHtml = metaProgramme.ToHtml();
                        await dbContext.SaveChangesAsync();
                    }
                    metaOrganisation.Generate();
                    doiOrganisation.MetadataJson = metaOrganisation.ToJson();
                    oldSha256 = doiOrganisation.MetadataJsonSha256;
                    doiOrganisation.MetadataJsonSha256 = doiOrganisation.MetadataJson.Sha256();
                    doiOrganisation.ODPMetadataNeedsUpdate = oldSha256 != doiOrganisation.MetadataJsonSha256;
                    doiOrganisation.MetadataHtml = metaOrganisation.ToHtml();
                    await dbContext.SaveChangesAsync();
                }
                metaObservations.Generate("Complete collection of observations in the SAEON Observations Database",
                    "The complete collection of observations in the SAEON Observations Database");
                doiObservations.MetadataJson = metaObservations.ToJson();
                oldSha256 = doiObservations.MetadataJsonSha256;
                doiObservations.MetadataJsonSha256 = doiObservations.MetadataJson.Sha256();
                doiObservations.ODPMetadataNeedsUpdate = oldSha256 != doiObservations.MetadataJsonSha256;
                doiObservations.MetadataHtml = metaObservations.ToHtml();
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
