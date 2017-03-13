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
                        cfg.CreateMap<UserDownload, UserDownload>()
                            .ForMember(i => i.Id, opt => opt.Ignore())
                            .ForMember(i => i.AddedBy, opt => opt.Ignore())
                            .ForMember(i => i.UpdatedBy, opt => opt.Ignore());
                        cfg.CreateMap<UserQuery, UserQuery>()
                            .ForMember(i => i.Id, opt => opt.Ignore())
                            .ForMember(i => i.AddedBy, opt => opt.Ignore())
                            .ForMember(i => i.UpdatedBy, opt => opt.Ignore());
                    });
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to initialise bootstrapper");
                    throw;
                }
            }
        }
    }
}
