using Newtonsoft.Json;
using SAEON.Azure.CosmosDB;
using System;

namespace SAEON.Observations.Azure
{
    public class ObservationImportBatch : AzureSubDocument
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public EpochDate Date { get; set; }
    }

    public class ObservationSite : AzureSubDocument
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class ObservationStation : AzureSubDocument
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class ObservationInstrument : AzureSubDocument
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class ObservationSensor : AzureSubDocument
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class ObservationPhenomenon : AzureSubDocument
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class ObservationOffering : AzureSubDocument
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class ObservationUnit : AzureSubDocument
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
    }

    public class ObservationStatus : AzureSubDocument
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class ObservationStatusReason : AzureSubDocument
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class ObservationDocument : AzureDocument
    {
        public ObservationImportBatch ImportBatch { get; set; }
        public ObservationSite Site { get; set; }
        public ObservationStation Station { get; set; }
        public ObservationInstrument Instrument { get; set; }
        public ObservationSensor Sensor { get; set; }
        public ObservationPhenomenon Phenomenon { get; set; }
        public ObservationOffering Offering { get; set; }
        public ObservationUnit Unit { get; set; }
        public ObservationStatus Status { get; set; }
        public ObservationStatusReason StatusReason { get; set; }
        public EpochDate ValueDate { get; set; }
        public EpochDate ValueDay { get; set; }
        public int ValueYear { get; set; }
        public int ValueDecade { get; set; }
        public string TextValue { get; set; }
        public double? RawValue { get; set; }
        public double? DataValue { get; set; }
        public string Comment { get; set; }
        public Guid? CorrelationID { get; set; }
        public double? Latitude { get; set; } 
        public double? Longitude { get; set; }
        public double? Elevation { get; set; }
        public Guid UserID { get; set; }
        public EpochDate AddedDate { get; set; }
        public EpochDate AddedAt { get; set; }
        public EpochDate UpdatedAt { get; set; }
    }
}
