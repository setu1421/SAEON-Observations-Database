using AutoMapper;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.Core
{
    public static class BootStrapper
    {
        public static void Initialize()
        {
            using (LogContext.PushProperty("Method", "BootStrapper.Initialize"))
            {
                try
                {
                    Database.SetInitializer<ObservationsDbContext>(null);
                    Database.SetInitializer<ApplicationDbContext>(new MigrateDatabaseToLatestVersion<ApplicationDbContext, ApplicationDbMigration>());
                    using (var context = new ApplicationDbContext())
                    {
                        context.Database.Initialize(false);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to initialize database");
                }
            }
            Mapper.Initialize(cfg => {
                cfg.CreateMap<UserDownload, UserDownloadDTO>();
                cfg.CreateMap<UserQuery, UserQueryDTO>();
            });
        }
    }
}
