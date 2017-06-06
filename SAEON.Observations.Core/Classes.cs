using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SAEON.Observations.Core
{
    public abstract class TreeNode
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        [Key]
        public string Key { get; set; }
        public string ParentKey { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public string SpriteImage { get; set; }
        public string ImageURL { get; set; }
        public bool HasChildren { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
        public bool IsChecked { get; set; }
        public object NodeProperty { get; set; }
        public object LinkProperty { get; set; }
        public object ImageProperty { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Elevation { get; set; }
        public string Url { get; set; }
    }

    public class Feature : TreeNode { }
    public class Location : TreeNode { }

    #region Maps
    public class MapPoint
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Elevation { get; set; }
    }
    #endregion

    #region DataQuery
    public class QueryMapPoint : MapPoint
    {
        public bool IsSelected { get; set; }
    }

    public class DataQueryInput
    {
        public List<Guid> Stations { get; set; }
        public List<Guid> PhenomenaOfferings { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class DataSeries
    {
        public string Caption { get; set; }
        public string Name { get; set; }
        public bool IsFeature { get; set; }
        public string Status { get; set; }
    }

    public class DataFeature
    {
        public string Caption { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is DataFeature feature)) return false;
            return Equals(feature);
        }

        public bool Equals(DataFeature feature)
        {
            if (feature == null) return false;
            return
                (Name == feature.Name) &&
                (Status == feature.Status);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;
                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Name) ? Name.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Status) ? Status.GetHashCode() : 0);
                return hash;
            }
        }
    }

    public class CardInstrument
    {
        public string Name { get; set; }
    }

    public class CardPhenomenon
    {
        public string Name { get; set; }
    }

    public class Card
    {
        public string Site { get; set; }
        public string Station { get; set; }
        public List<CardInstrument> Instruments { get; private set; } = new List<CardInstrument>();
        public List<CardPhenomenon> Phenomena { get; private set; } = new List<CardPhenomenon>();
    }

    public class DataQueryOutput
    {
        public List<Card> Cards { get; private set; } = new List<Card>();
        public List<DataSeries> Series { get; private set; } = new List<DataSeries>();
        public DataTable DataTable { get; private set; } = new DataTable("Data");
        public List<ExpandoObject> Data { get; private set; } = new List<ExpandoObject>();
    }
    #endregion

    #region DataDowload
    public class DataDownloadInput : DataQueryInput { }

    public class DataDownloadOutput
    {
    }
    #endregion DataDownload

    #region SpacialCoverage
    public class SpacialCoverageInput : DataQueryInput { }

    public enum SpacialStatus
    {
        NoStatus,
        Unverified,
        BeingVerified,
        Verified
    }

    public class SpacialMapPoint : MapPoint
    {
        public SpacialStatus Status { get; set; }
    }

    public class SpacialStation
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Elevation { get; set; }
        public string Url { get; set; }
        public SpacialStatus Status { get; set; }
        public int NoStatus { get; set; }
        public int Unverified { get; set; }
        public int BeingVerified { get; set; }
        public int Verified { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is SpacialStation spacialStation)) return false;
            return Equals(spacialStation);
        }

        public bool Equals(SpacialStation spacialStation)
        {
            if (spacialStation == null) return false;
            return
                (Name == spacialStation.Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;
                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Name) ? Name.GetHashCode() : 0);
                return hash;
            }
        }
    }

    public class SpacialCoverageOutput
    {
        public List<SpacialStation> Stations { get; private set; } = new List<SpacialStation>();
    }
    #endregion

    #region TemporalCoverage
    public class TemporalCoverageInput : DataQueryInput { }

    public class TemporalCoverageOutput
    {
        public List<DataSeries> Series { get; private set; } = new List<DataSeries>();
        public List<ExpandoObject> Data { get; private set; } = new List<ExpandoObject>();
    }
    #endregion

    #region Inventory
    public class InventoryInput
    {
        public List<Guid> Stations { get; set; }
        public List<Guid> Offerings { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? UserQuery { get; set; }
        public bool? Full { get; set; }
        public bool? Statistics { get; set; }
    }

    public class InventoryTotalItem
    {
        public string Status { get; set; }
        public int? Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
    }

    public class InventoryStationItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Status { get; set; }
        public int? Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
    }

    public class InventoryPhenomenonOfferingItem
    {
        public string Phenomenon { get; set; }
        public string Offering { get; set; }
        public string Status { get; set; }
        public int? Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
    }

    public class InventoryInstrumentItem
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public int? Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
    }

    public class InventoryYearItem
    {
        public int Year { get; set; }
        public string Status { get; set; }
        public int? Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
    }

    public class InventoryOrganisationItem
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public int? Count { get; set; }
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double? Average { get; set; }
        public double? StandardDeviation { get; set; }
        public double? Variance { get; set; }
    }

    public class InventoryOutput
    {
        public bool Success { get; set; }
        public List<string> ErrorMessage { get; private set; } = new List<string>();
        public List<InventoryTotalItem> Totals { get; private set; } = new List<InventoryTotalItem>();
        public List<InventoryStationItem> Stations { get; private set; } = new List<InventoryStationItem>();
        public List<InventoryInstrumentItem> Instruments { get; private set; } = new List<InventoryInstrumentItem>();
        public List<InventoryPhenomenonOfferingItem> PhenomenaOfferings { get; private set; } = new List<InventoryPhenomenonOfferingItem>();
        public List<InventoryYearItem> Years { get; private set; } = new List<InventoryYearItem>();
        public List<InventoryOrganisationItem> Organisations { get; private set; } = new List<InventoryOrganisationItem>();
    }
    #endregion
}
