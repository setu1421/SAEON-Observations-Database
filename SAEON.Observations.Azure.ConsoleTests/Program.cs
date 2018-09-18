using SAEON.Logs;
using Serilog;
using System;

namespace SAEON.Observations.Azure.ConsoleTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logging
                .CreateConfiguration(@"Logs\SAEON.Observations.Azure.ConsoleTests {Date}.txt")
                .WriteTo.Console()
                .Create();
            using (Logging.MethodCall(typeof(Program)))
            {
                try
                {
                    Logging.Information("This is a test");
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
    }
}
