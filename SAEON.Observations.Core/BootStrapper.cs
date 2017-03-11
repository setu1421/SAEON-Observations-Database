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
            using (Logging.MethodCall(typeof(BootStrapper)))
            {
                try
                {
                    Database.SetInitializer<ObservationsDbContext>(null);
                    Mapper.Initialize(cfg =>
                    {
                        //cfg.CreateMap<Instrument, Instrument>().ForMember(i => i.Id, y => y.Ignore());
                        //cfg.CreateMap<Site, Site>().ForMember(i => i.Id, y => y.Ignore());
                        //cfg.CreateMap<Station, Station>().ForMember(i => i.Id, y => y.Ignore());
                        cfg.CreateMap<UserDownload, UserDownload>().ForMember(i => i.Id, y => y.Ignore());
                        cfg.CreateMap<UserQuery, UserQuery>().ForMember(i => i.Id, y => y.Ignore());
                    });
                }
                catch (Exception ex)
                {
                    Logging.ErrorInCall(ex, "Unable to initialise bootstrapper");
                    throw;
                }
            }
        }
    }
}
