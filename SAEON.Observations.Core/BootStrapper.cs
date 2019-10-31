using SAEON.Logs;
using System;

namespace SAEON.Observations.Core
{
    [Obsolete("BootStrapper is obsolete", true)]
    public static class BootStrapper
    {
        public static void Initialize()
        {
            using (Logging.MethodCall(typeof(BootStrapper)))
            {
                try
                {
                    //Database.SetInitializer<ObservationsDbContext>(null);
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
