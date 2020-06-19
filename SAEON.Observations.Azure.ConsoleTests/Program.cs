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
            Logging
                .CreateConfiguration(@"Logs\SAEON.Observations.Azure.ConsoleTests {Date}.txt")
                .Create();
            try
            {
                using (Logging.MethodCall(typeof(Program)))
                {
                    try
                    {
                        Logging.Information("Initializing");
                        var azure = new ObservationsAzure();
                        var importBatchId = new Guid("426170df-73fa-4c25-81c0-ea00264d60c8");
                        Logging.Information("Adding items");
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
                        Logging.Information("Add: {Cost}", azure.AddObservation(item));
                        item.Comment = "12345";
                        Logging.Information("Upsert: {Cost}", azure.UpsertObservation(item));
                        item.Comment = "abc";
                        Logging.Information("Replace: {Cost}", azure.ReplaceObservation(item));
                        var getResponse = azure.GetObservationWithCost(item);
                        Logging.Information("Item: {@Item} Cost: {cost}", getResponse.item, getResponse.cost);
                        var (items, cost) = azure.GetObservationsWithCost(i => i.ImportBatchId == importBatchId);
                        Logging.Information("Count: {Count} Cost: {cost}", items.Count(), cost);
                        Logging.Information("Delete: {Cost}", azure.DeleteObservation(item));
                        Logging.Information("DeleteBatch: {Cost}", azure.DeleteImportBatch(importBatchId));
                        Logging.Information("Done");
                    }
                    catch (Exception ex)
                    {
                        Logging.Exception(ex);
                        throw;
                    }
                }
            }
            finally
            {
                Logging.ShutDown();
            }
        }
    }
}
