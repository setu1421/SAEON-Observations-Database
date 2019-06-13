using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using System;
using System.Data.Entity;

namespace SAEON.Observations.Core
{
    public static class BootStrapper
    {
        public static void Initialize()
        {
            using (Logging.MethodCall(typeof(BootStrapper)))
            {
                try
                {
                    Database.SetInitializer<ObservationsDbContext>(null);
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
