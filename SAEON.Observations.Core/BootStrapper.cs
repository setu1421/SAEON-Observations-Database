using AutoMapper;
using SAEON.Logs;
using SAEON.Observations.Core.Entities;
using System;

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
                    //Database.SetInitializer<ObservationsDbContext>(null);
                    Mapper.Initialize(cfg =>
                    {
                        //cfg.CreateMap<UserDownload, UserDownload>()
                        //    .ForMember(dest => dest.Id, opt => opt.Ignore())
                        //    .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                        //    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
                        //cfg.CreateMap<UserQuery, UserQuery>()
                        //    .ForMember(dest => dest.Id, opt => opt.Ignore())
                        //    .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                        //    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
                        //cfg.CreateMap<InventoryTotal, InventoryTotalItem>();
                        //cfg.CreateMap<InventoryStation, InventoryStationItem>();
                        //cfg.CreateMap<InventoryPhenomenonOffering, InventoryPhenomenonOfferingItem>();
                        //cfg.CreateMap<InventoryInstrument, InventoryInstrumentItem>();
                        //cfg.CreateMap<InventoryYear, InventoryYearItem>();
                        //cfg.CreateMap<InventoryOrganisation, InventoryOrganisationItem>();
                    });
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex, "Unable to initialize bootstrapper");
                    throw;
                }
            }
        }
    }
}
