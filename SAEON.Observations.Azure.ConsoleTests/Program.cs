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
                        Logging.Information("Adding documents");
                        var doc = new ObservationDocument
                        {
                            Sensor = new ObservationSensor { Code = "Algoa Bay" },
                            Phenomenon = new ObservationPhenomenon { Code = "Air Temperature"},
                            Offering = new ObservationOffering { Code = "Actual"},
                            Unit = new ObservationUnit { Code = "C"},
                            ValueDate = new EpochDate(DateTime.Now),
                            RawValue = 1.2,
                            DataValue = 1.2
                        };
                        azure.AddObservation(doc);
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
