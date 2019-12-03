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
        [JsonProperty("importBatchId")]
        public Guid ImportBatchId => ImportBatch?.Id ?? new Guid();
        [JsonProperty("importBatch")]
        public ObservationImportBatch ImportBatch { get; set; }
        [JsonProperty("site")]
        public ObservationSite Site { get; set; }
        [JsonProperty("station")]
        public ObservationStation Station { get; set; }
        [JsonProperty("instrument")]
        public ObservationInstrument Instrument { get; set; }
        [JsonProperty("sensor")]
        public ObservationSensor Sensor { get; set; }
        [JsonProperty("phenomenon")]
        public ObservationPhenomenon Phenomenon { get; set; }
        [JsonProperty("offering")]
        public ObservationOffering Offering { get; set; }
        [JsonProperty("unit")]
        public ObservationUnit Unit { get; set; }
        [JsonProperty("status")]
        public ObservationStatus Status { get; set; }
        [JsonProperty("statusReason")]
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
