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
                    Database.SetInitializer<ApplicationDbContext>(null);
                    //Database.SetInitializer<ApplicationDbContext>(new MigrateDatabaseToLatestVersion<ApplicationDbContext, ApplicationDbMigration>());
                    using (var context = new ApplicationDbContext())
                    {
                        context.Database.Initialize(false);
                        context.Seed();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to initialize database");
                }
            }
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Instrument, Instrument>().ForMember(i => i.Id, y => y.Ignore());
                cfg.CreateMap<Site, Site>().ForMember(i => i.Id, y => y.Ignore());
                cfg.CreateMap<Station, Station>().ForMember(i => i.Id, y => y.Ignore());
                cfg.CreateMap<UserDownload, UserDownload>().ForMember(i => i.Id, y => y.Ignore());
                cfg.CreateMap<UserQuery, UserQuery>().ForMember(i => i.Id, y => y.Ignore());
            });
        }
    }
}
