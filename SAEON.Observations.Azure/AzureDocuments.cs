using Newtonsoft.Json;
using SAEON.Azure.CosmosDB;
using System;

namespace SAEON.Observations.Azure
{
    public class ObservationImportBatch : AzureSubDocument
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("date")]
        public EpochDate Date { get; set; }
    }

    public class ObservationSubDocument : AzureSubDocument
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class ObservationSite : ObservationSubDocument { }

    public class ObservationStation : ObservationSubDocument { }

    public class ObservationInstrument : ObservationSubDocument { }

    public class ObservationSensor : ObservationSubDocument { }

    public class ObservationPhenomenon : ObservationSubDocument { }

    public class ObservationOffering : ObservationSubDocument { }

    public class ObservationUnit : ObservationSubDocument { }

    public class ObservationStatus : ObservationSubDocument { }

    public class ObservationStatusReason : ObservationSubDocument { }

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
        [JsonProperty("valueDate")]
        public EpochDate ValueDate { get; set; }
        [JsonProperty("valueDay")]
        public EpochDate ValueDay { get; set; }
        [JsonProperty("valueYear")]
        public int ValueYear { get; set; }
        [JsonProperty("valueDecade")]
        public int ValueDecade { get; set; }
        [JsonProperty("textValue")]
        public string TextValue { get; set; }
        [JsonProperty("rawValue")]
        public double? RawValue { get; set; }
        [JsonProperty("dataValue")]
        public double? DataValue { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("correlationId")]
        public Guid? CorrelationId { get; set; }
        [JsonProperty("latitude")]
        public double? Latitude { get; set; }
        [JsonProperty("longitude")]
        public double? Longitude { get; set; }
        [JsonProperty("elevation")]
        public double? Elevation { get; set; }
        [JsonProperty("userId")]
        public Guid UserId { get; set; }
        [JsonProperty("addedDate")]
        public EpochDate AddedDate { get; set; }
        [JsonProperty("addedAt")]
        public EpochDate AddedAt { get; set; }
        [JsonProperty("updatedAt")]
        public EpochDate UpdatedAt { get; set; }
    }
}
