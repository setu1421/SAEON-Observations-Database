using Newtonsoft.Json;
using SAEON.Core;
using SAEON.CSV;
using SAEON.Logs;
using SAEON.Observations.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class ObservationDTO
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Site { get; set; }
    public string Station { get; set; }
    public string Instrument { get; set; }
    public string Sensor { get; set; }
    public string Phenomenon { get; set; }
    public string Offering { get; set; }
    public string Unit { get; set; }
    public string UnitSymbol { get; set; }
    public string Variable => $"{Phenomenon.Replace(", ", "_")}, {Offering.Replace(", ", "_")}, {Unit.Replace(", ", "_")}";
    public DateTime Date { get; set; }
    public double? Value { get; set; }
    public string Comment { get; set; }
    public string Status { get; set; }
    public string Reason { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Elevation { get; set; }

    public override bool Equals(object obj)
    {
        return obj is ObservationDTO dTO &&
               Site == dTO.Site &&
               Station == dTO.Station &&
               Instrument == dTO.Instrument &&
               Sensor == dTO.Sensor &&
               Phenomenon == dTO.Phenomenon &&
               Offering == dTO.Offering &&
               Unit == dTO.Unit &&
               Variable == dTO.Variable &&
               Date == dTO.Date &&
               Value == dTO.Value &&
               Comment == dTO.Comment &&
               Latitude == dTO.Latitude &&
               Longitude == dTO.Longitude &&
               Elevation == dTO.Elevation;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Site);
        hash.Add(Station);
        hash.Add(Variable);
        hash.Add(Instrument);
        hash.Add(Sensor);
        hash.Add(Phenomenon);
        hash.Add(Offering);
        hash.Add(Unit);
        hash.Add(Date);
        hash.Add(Value);
        hash.Add(Comment);
        hash.Add(Latitude);
        hash.Add(Longitude);
        hash.Add(Elevation);
        return hash.ToHashCode();
    }
}

public static class DatasetHelper
{
    private static List<ObservationDTO> LoadFromDisk(Dataset dataset)
    {
        using (SAEONLogs.MethodCall(typeof(DatasetHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
        {
            try
            {
                //Guard.IsNotNull(dataset, nameof(dataset));
                if (dataset is null) throw new ArgumentNullException(nameof(dataset));
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                using (var reader = new StreamReader(Path.Combine(ConfigurationManager.AppSettings["DatasetsFolder"], dataset.CSVFileName)))
                {
                    using (var csv = CsvReaderHelper.GetCsvReader(reader))
                    {
                        var result = csv.GetRecords<ObservationDTO>()/*.Where(i => (((i.Status == null) || (i.Status == "Verified"))))*/.ToList();
                        SAEONLogs.Verbose("Loaded {Count} from disk in {Elapsed}", result.Count, stopwatch.Elapsed.TimeStr());
                        return result;
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

    private static List<ObservationDTO> LoadFromDatabase(Dataset dataset)
    {
        using (SAEONLogs.MethodCall(typeof(DatasetHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
        {
            try
            {
                //Guard.IsNotNull(dataset, nameof(dataset));
                if (dataset is null) throw new ArgumentNullException(nameof(dataset));
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var observations = new VObservationExpansionCollection()
                    .Where(VObservationExpansion.Columns.StationID, dataset.StationID)
                    .Where(VObservationExpansion.Columns.PhenomenonOfferingID, dataset.PhenomenonOfferingID)
                    .Where(VObservationExpansion.Columns.PhenomenonUOMID, dataset.PhenomenonUOMID)
                    .Load();
                var result = new List<ObservationDTO>();
                foreach (var observation in observations)
                {
                    result.Add(new ObservationDTO
                    {
                        Id = observation.Id,
                        Site = observation.SiteName,
                        Station = observation.StationName,
                        Phenomenon = observation.PhenomenonName,
                        Offering = observation.OfferingName,
                        Unit = observation.UnitOfMeasureUnit,
                        UnitSymbol = observation.UnitOfMeasureSymbol,
                        Date = DateTime.SpecifyKind(observation.ValueDate, DateTimeKind.Local),
                        Value = observation.DataValue,
                        Instrument = observation.InstrumentName,
                        Sensor = observation.SensorName,
                        Comment = observation.Comment,
                        Latitude = observation.Latitude,
                        Longitude = observation.Longitude,
                        Elevation = observation.Elevation,
                        Status = observation.StatusName,
                        Reason = observation.StatusReasonName,
                    });
                }
                SAEONLogs.Verbose("Loaded {Count} from database in {Elapsed}", result.Count, stopwatch.Elapsed.TimeStr());
                return result.OrderBy(i => i.Elevation).ThenBy(i => i.Date).ToList();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }

    private static bool IsOnDisk(Dataset dataset)
    {
        using (SAEONLogs.MethodCall(typeof(DatasetHelper), new MethodCallParameters { { "datasetId", dataset?.Id } }))
        {
            try
            {
                //Guard.IsNotNull(dataset, nameof(dataset));
                if (dataset is null) throw new ArgumentNullException(nameof(dataset));
                string fileName = dataset.CSVFileName;
                var useDisk = ConfigurationManager.AppSettings["DatasetsUseDisk"]?.IsTrue() ?? false;
                //SAEONLogs.Verbose("FileName: {FileName}", Path.Combine(ConfigurationManager.AppSettings["DatasetsFolder"], dataset.CSVFileName));
                var fileExists = (!string.IsNullOrEmpty(fileName) && File.Exists(Path.Combine(ConfigurationManager.AppSettings["DatasetsFolder"], dataset.CSVFileName)));
                var result = useDisk && fileExists;
                SAEONLogs.Verbose("UseDisk: {UseDisk} FileName: {FileName} FileExists: {FileExists} IsOnDisk: {IsOnDisk}", useDisk, fileName, fileExists, result);
                return result;
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }

    public static List<ObservationDTO> Load(Guid datasetId)
    {
        using (SAEONLogs.MethodCall(typeof(DatasetHelper), new MethodCallParameters { { "DatasetId", datasetId } }))
        {
            try
            {
                var dataset = new Dataset(datasetId);
                //Guard.IsNotNull(dataset, nameof(dataset));
                if (dataset is null) throw new ArgumentNullException(nameof(dataset));
                if (IsOnDisk(dataset))
                {
                    return LoadFromDisk(dataset);
                }
                else
                {
                    return LoadFromDatabase(dataset);
                }
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }
    }

    public static void UpdateDatasetsFromDisk()
    {
        using (SAEONLogs.MethodCall(typeof(DatasetHelper)))
        {
            try
            {
                SAEONLogs.Verbose("Updating datasets from disk");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var count = WalkFolder(ConfigurationManager.AppSettings["DatasetsFolder"]);
                SAEONLogs.Information("Updated {Count} in {Elapsed}", count, stopwatch.Elapsed.TimeStr());
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }

        int WalkFolder(string folder)
        {
            var di = new DirectoryInfo(folder);
            var result = 0;
            foreach (var sdi in di.EnumerateDirectories())
            {
                //SAEONLogs.Information(sdi.FullName);
                result += WalkFolder(sdi.FullName);
            }
            foreach (var fi in di.EnumerateFiles())
            {
                //SAEONLogs.Information(fi.FullName);
                var subFolder = fi.Name.Substring(0, 6).Insert(4, "-");
                var code = Path.GetFileNameWithoutExtension(fi.Name).Remove(0, "202207221012 ".Length);
                var dataset = new Dataset(Dataset.Columns.Code, code);
                if (dataset is null)
                {
                    SAEONLogs.Error("Ignoring dataset {Code}, not found", code);
                }
                else if (!dataset.IsNew)
                {
                    if (fi.Name.EndsWith(".csv"))
                    {
                        dataset.CSVFileName = $"{subFolder}\\{fi.Name}";
                    }
                    else if (fi.Name.EndsWith(".xlsx"))
                    {
                        dataset.ExcelFileName = $"{subFolder}\\{fi.Name}";
                    }
                    //else if (fi.Name.EndsWith(".nc"))
                    //{
                    //    dataset.NetCDFFileName = $"{subFolder}\\{fi.Name}";
                    //}
                    if (dataset.IsDirty)
                    {
                        SAEONLogs.Verbose("Dataset: {Dataset}", dataset.Code);
                        dataset.Save();
                        result++;
                    }
                }
            }
            return result;
        }
    }

    public static void CleanFilesOnDisk()
    {
        using (SAEONLogs.MethodCall(typeof(DatasetHelper)))
        {
            try
            {
                SAEONLogs.Verbose("Cleaning dataset files on disk");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var count = WalkFolder(ConfigurationManager.AppSettings["DatasetsFolder"]);
                SAEONLogs.Information("Cleaned {Count} files in {Elapsed}", count, stopwatch.Elapsed.TimeStr());
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                throw;
            }
        }

        int WalkFolder(string folder)
        {
            var result = 0;
            var di = new DirectoryInfo(folder);
            foreach (var sdi in di.EnumerateDirectories())
            {
                //SAEONLogs.Verbose(sdi.FullName);
                result += WalkFolder(sdi.FullName);
            }
            foreach (var fi in di.EnumerateFiles())
            {
                //SAEONLogs.Verbose(fi.FullName);
                var subFolder = fi.Name.Substring(0, 6).Insert(4, "-");
                var code = Path.GetFileNameWithoutExtension(fi.Name).Remove(0, "202207221012 ".Length);
                var dataset = new Dataset(Dataset.Columns.Code, code);
                if (dataset is null)
                {
                    SAEONLogs.Error("Ignoring dataset {Code}, not found", code);
                    SAEONLogs.Verbose("Deleted {FileName}", fi.Name);
                    fi.Delete();
                    result++;
                }
                else if (!dataset.IsNew)
                {
                    var fileName = $"{subFolder}\\{fi.Name}";
                    if (fi.Name.EndsWith(".csv"))
                    {
                        if (dataset.CSVFileName != fileName)
                        {
                            SAEONLogs.Verbose("Deleted {FileName}", fi.Name);
                            fi.Delete();
                            result++;
                        }
                    }
                    else if (fi.Name.EndsWith(".xlsx"))
                    {
                        if (dataset.ExcelFileName != fileName)
                        {
                            SAEONLogs.Verbose("Deleted {FileName}", fi.Name);
                            fi.Delete();
                            result++;
                        }
                    }
                }
            }
            return result;
        }
    }
}