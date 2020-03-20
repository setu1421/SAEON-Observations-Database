using SAEON.Azure.CosmosDB;
using SAEON.Logs;
using System;

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
                        var azure = new Azure();
                        azure.Initialize();
                        //Logging.Information("Adding documents");
                        //var doc = new ObservationDocument
                        //{
                        //    ImportBatch = new ObservationImportBatch { Id = Guid.NewGuid(), Code = 123 },
                        //    Sensor = new ObservationSensor { Id = Guid.NewGuid(), Code = "Algoa Bay" },
                        //    Phenomenon = new ObservationPhenomenon { Id = Guid.NewGuid(), Code = "Air Temperature" },
                        //    Offering = new ObservationOffering { Id = Guid.NewGuid(), Code = "Actual" },
                        //    Unit = new ObservationUnit { Id = Guid.NewGuid(), Code = "C" },
                        //    ValueDate = new EpochDate(DateTime.Now),
                        //    RawValue = 1.2,
                        //    DataValue = 1.2
                        //};
                        //azure.AddObservation(doc);
                        azure.DeleteImportBatch(new Guid("426170df-73fa-4c25-81c0-ea00264d60c8"));
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
