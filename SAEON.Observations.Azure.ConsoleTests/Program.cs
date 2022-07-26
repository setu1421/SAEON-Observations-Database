using SAEON.Azure.CosmosDB;
using SAEON.Logs;
using System;
using System.Linq;

namespace SAEON.Observations.Azure.ConsoleTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SAEONLogs
                .CreateConfiguration(@"Logs\SAEON.Observations.Azure.ConsoleTests {Date}.txt")
                .Initialize();
            try
            {
                using (SAEONLogs.MethodCall(typeof(Program)))
                {
                    try
                    {
                        SAEONLogs.Information("Initializing");
                        var azure = new ObservationsAzure();
                        azure.DeleteImportBatch(new Guid("aea07399-300f-442a-8a46-7cf8a878417f"));
                        var importBatchId = new Guid("426170df-73fa-4c25-81c0-ea00264d60c8");
                        SAEONLogs.Information("Adding items");
                        var item = new ObservationItem
                        {
                            Id = "123",
                            ImportBatch = new ObservationImportBatch { Id = importBatchId, Code = 123 },
                            Sensor = new ObservationSensor { Id = Guid.NewGuid(), Code = "Algoa Bay" },
                            Phenomenon = new ObservationPhenomenon { Id = Guid.NewGuid(), Code = "Air Temperature" },
                            Offering = new ObservationOffering { Id = Guid.NewGuid(), Code = "Actual" },
                            Unit = new ObservationUnit { Id = Guid.NewGuid(), Code = "C" },
                            ValueDate = new EpochDate(DateTime.Now),
                            RawValue = 1.2,
                            DataValue = 1.2
                        };
                        SAEONLogs.Information("Add: {Cost}", azure.AddObservation(item));
                        item.Comment = "12345";
                        SAEONLogs.Information("Upsert: {Cost}", azure.UpsertObservation(item));
                        item.Comment = "abc";
                        SAEONLogs.Information("Replace: {Cost}", azure.ReplaceObservation(item));
                        var getResponse = azure.GetObservationWithCost(item);
                        SAEONLogs.Information("Item: {@Item} Cost: {cost}", getResponse.item, getResponse.cost);
                        var (items, cost) = azure.GetObservationsWithCost(i => i.ImportBatchId == importBatchId);
                        SAEONLogs.Information("Count: {Count} Cost: {cost}", items.Count(), cost);
                        SAEONLogs.Information("Delete: {Cost}", azure.DeleteObservation(item));
                        SAEONLogs.Information("DeleteBatch: {Cost}", azure.DeleteImportBatch(importBatchId));
                        SAEONLogs.Information("Done");
                    }
                    catch (Exception ex)
                    {
                        SAEONLogs.Exception(ex);
                        throw;
                    }
                }
            }
            finally
            {
                SAEONLogs.ShutDown();
            }
        }
    }
}
