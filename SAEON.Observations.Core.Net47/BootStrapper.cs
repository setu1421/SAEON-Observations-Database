using SAEON.Logs;
using System;
using System.Data.Entity;

namespace SAEON.Observations.Core
{
    public static class BootStrapper
    {
        public static void Initialize()
        {
            using (SAEONLogs.MethodCall(typeof(BootStrapper)))
            {
                try
                {
                    Database.SetInitializer<ObservationsDbContext>(null);
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
