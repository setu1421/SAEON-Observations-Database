using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SAEON.Core;
using SAEON.Observations.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.Serialization;
using System.Text;

namespace SAEON.Observations.Core
{
    public class LinkAttribute
    {
        public string Title { get; set; }

        public LinkAttribute() { }
        public LinkAttribute(string title)
        {
            Title = title;
        }
    }

    public abstract class TreeNode : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        [Key]
        public string Key { get; set; }
        public string ParentKey { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public bool HasChildren { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
        public bool IsChecked { get; set; }
        public LinkAttribute ToolTip { get; set; } = null;
    }

    public class LocationNode : TreeNode
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Elevation { get; set; }
        public string Url { get; set; }
    }

    public class FeatureNode : TreeNode { }

    #region Maps
    public class MapPoint
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Elevation { get; set; }
        public bool IsSelected { get; set; }
    }
    #endregion

    #region DataWizard

    public enum MaxtixDataType { String, Int, Double, Date, Boolean };

    public class DataMatrixColumn
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public MaxtixDataType DataType { get; set; }

        public Type AsType()
        {
            switch (DataType)
            {
                case MaxtixDataType.Boolean:
                    return typeof(bool);
                case MaxtixDataType.Date:
                    return typeof(DateTime);
                case MaxtixDataType.Double:
                    return typeof(double);
                case MaxtixDataType.Int:
                    return typeof(int);
                case MaxtixDataType.String:
                    return typeof(string);
                default:
                    return typeof(object);
            }
        }

        public string AsString(object value)
        {
            switch (DataType)
            {
                case MaxtixDataType.Boolean:
                    return ((bool)value).ToString();
                case MaxtixDataType.Date:
                    return ((DateTime)value).ToString("o");
                case MaxtixDataType.Double:
                    return ((double)value).ToString();
                case MaxtixDataType.Int:
                    return ((int)value).ToString();
                case MaxtixDataType.String:
                    return ((string)value).DoubleQuoted();
                default:
                    return value.ToString();
            }
        }
    }

    public class DataMatixRow
    {
        public List<Object> Columns { get; set; } = new List<object>();
        internal DataMatrix Matrix { get; set; } = null;

        public DataMatixRow() { }

        public DataMatixRow(DataMatrix dataMatrix)
        {
            Matrix = dataMatrix;
        }

        public object this[string name]
        {
            get
            {
                var index = Matrix.Columns.FindIndex(i => i.Name == name);
                return Columns[index];
            }
            set
            {
                var index = Matrix.Columns.FindIndex(i => i.Name == name);
                Columns[index] = value;
            }
        }

        public object this[int index]
        {
            get { return Columns[index]; }
            set { Columns[index] = value; }
        }

        public bool IsNull(string name)
        {
            var val = this[name];
            return (val == null);
        }

    }

    public class DataMatrix
    {
        public List<DataMatrixColumn> Columns { get; } = new List<DataMatrixColumn>();
        public List<DataMatixRow> Rows { get; } = new List<DataMatixRow>();

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            foreach (var row in Rows)
            {
                row.Matrix = this;
            }
        }

        public DataMatrixColumn AddColumn(string name, string caption, MaxtixDataType dataType)
        {
            var result = new DataMatrixColumn { Name = name, Caption = caption, DataType = dataType };
            Columns.Add(result);
            foreach (var row in Rows)
            {
                row.Columns.Add(null);
            }
            return result;
        }

        public DataMatixRow AddRow(params object[] values)

        {
            var result = new DataMatixRow(this);
            Rows.Add(result);
            for (int c = 0; c < Columns.Count; c++)
            {
                var col = Columns[c];
                if (c < values.Length)
                {
                    result.Columns.Add(values[c]);
                }
                else
                {
                    result.Columns.Add(null);
                }
            }
            return result;
        }

        public DataTable AsDataTable()
        {
            var result = new DataTable();
            foreach (var dmCol in Columns)
            {
                result.Columns.Add(dmCol.Name, dmCol.AsType()).Caption = dmCol.Caption;
            }
            foreach (var dmRow in Rows)
            {
                var dataRow = result.NewRow();
                foreach (var dmCol in Columns)
                {
                    var value = dmRow[dmCol.Name];
                    if (value != null)
                    { 
                        dataRow[dmCol.Name] = value;
                    }
                }
                result.Rows.Add(dataRow);
            }
            return result;
        }

        public String AsCSV()
        {
            var sb = new StringBuilder();
            var isFirst = true;
            foreach (var dmCol in Columns)
            {
                if (!isFirst)
                {
                    sb.Append(",");
                    isFirst = false;
                }
                sb.Append(dmCol.Name);
            }
            sb.AppendLine();
            foreach (var dmRow in Rows)
            {
                isFirst = true;
                foreach (var dmCol in Columns)
                {
                    if (!isFirst)
                    {
                        sb.Append(",");
                        isFirst = false;
                    }
                    sb.Append(dmCol.AsString(dmRow[dmCol.Name]));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

    }

    public class ChartData
    {
        public DateTime Date { get; set; }
        public double? Value { get; set; }
    }

    public class ChartSeries
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        public List<ChartData> Data { get; } = new List<ChartData>();

        public void Add(DateTime date, double? value)
        {
            Data.Add(new ChartData { Date = date, Value = value });
        }
    }

    public class DataWizardDataInput
    {
        public List<Guid> Organisations { get; } = new List<Guid>();
        public List<Guid> Sites { get; } = new List<Guid>();
        public List<Guid> Stations { get; } = new List<Guid>();
        public List<Guid> Instruments { get; } = new List<Guid>();
        public List<Guid> Phenomena { get; } = new List<Guid>();
        public List<Guid> Offerings { get; } = new List<Guid>();
        public List<Guid> Units { get; } = new List<Guid>();
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddYears(-100);
    }

    public class DataWizardDataOutput
    {
        public DataMatrix DataMatrix { get; } = new DataMatrix();
        public List<ChartSeries> ChartSeries { get; } = new List<ChartSeries>();
    }

    public class DataWizardApproximation
    {
        public long RowCount { get; set; } = 0;
        public List<string> Errors { get; set; } = new List<string>();
    }

    public enum DownloadFormats { CSV, Excel, NetCDF }

    public class DataWizardDownloadInput : DataWizardDataInput
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DownloadFormats DownloadFormat { get; set; } = DownloadFormats.CSV;
    }

    #endregion

    #region SpacialCoverage
    /*
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
                hash = (hash * HashingMultiplier) ^ (!(Name is null) ? Name.GetHashCode() : 0);
                return hash;
            }
        }
    }

    public class SpacialCoverageOutput
    {
        public List<SpacialStation> Stations { get; private set; } = new List<SpacialStation>();
    }
    */
    #endregion

    #region TemporalCoverage
    /*
    public class TemporalCoverageInput : DataQueryInput { }

    public class TemporalCoverageOutput
    {
        public List<DataSeries> Series { get; private set; } = new List<DataSeries>();
        public List<ExpandoObject> Data { get; private set; } = new List<ExpandoObject>();
    }
    */
    #endregion

    #region Inventory
    /*
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
    */
    #endregion
}
