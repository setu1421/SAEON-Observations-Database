using AutoMapper;
using Serilog;
using Serilog.Context;
using System;
using System.Data.Entity;

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
