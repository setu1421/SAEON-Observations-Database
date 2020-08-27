using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SAEON.Core;
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
        public LinkAttribute ToolTip { get; set; }
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

    public enum MaxtixDataType { mdtString, mdtInt, mdtDouble, mdtDate, mdtBoolean };

    public class DataMatrixColumn
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public MaxtixDataType DataType { get; set; }

        public Type AsType()
        {
            return DataType switch
            {
                MaxtixDataType.mdtBoolean => typeof(bool),
                MaxtixDataType.mdtDate => typeof(DateTime),
                MaxtixDataType.mdtDouble => typeof(double),
                MaxtixDataType.mdtInt => typeof(int),
                MaxtixDataType.mdtString => typeof(string),
                _ => typeof(object),
            };
        }

        public string AsString(object value)
        {
            if (value == null) return string.Empty;
            return DataType switch
            {
                MaxtixDataType.mdtBoolean => ((bool)value).ToString(),
                MaxtixDataType.mdtDate => ((DateTime)value).ToString("o"),
                MaxtixDataType.mdtDouble => ((double)value).ToString(),
                MaxtixDataType.mdtInt => ((int)value).ToString(),
                MaxtixDataType.mdtString => ((string)value).DoubleQuoted(),
                _ => value.ToString(),
            };
        }
    }

    public class DataMatrixRow
    {
        public List<Object> Columns { get; } = new List<object>();
        internal DataMatrix Matrix { get; set; }

        public DataMatrixRow() { }

        public DataMatrixRow(DataMatrix dataMatrix)
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
        public List<DataMatrixRow> Rows { get; } = new List<DataMatrixRow>();

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

        public DataMatrixRow AddRow(params object[] values)

        {
            var result = new DataMatrixRow(this);
            Rows.Add(result);
            for (int c = 0; c < Columns.Count; c++)
            {
                //var col = Columns[c];
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
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(',');
                }
                sb.Append(dmCol.Name);
            }
            sb.AppendLine();
            foreach (var dmRow in Rows)
            {
                isFirst = true;
                foreach (var dmCol in Columns)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        sb.Append(',');
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
        //public List<Guid> Instruments { get; } = new List<Guid>();
        public List<Guid> Phenomena { get; } = new List<Guid>();
        public List<Guid> Offerings { get; } = new List<Guid>();
        public List<Guid> Units { get; } = new List<Guid>();
        public DateTime StartDate { get; set; } = DateTime.Now.AddYears(-100).Date;
        public DateTime EndDate { get; set; } = DateTime.Now.Date;
        public float ElevationMinimum { get; set; } = -100; // m
        public float ElevationMaximum { get; set; } = 3000; // m
    }

    public class DataWizardDataOutput
    {
        public DataMatrix DataMatrix { get; } = new DataMatrix();
        public List<ChartSeries> ChartSeries { get; } = new List<ChartSeries>();
        public DateTime Date { get; } = DateTime.Now;
        public string Title { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Semi-colon separated Name;Scheme;Uri
        /// </summary>
        public List<string> Keywords { get; } = new List<string>();
        /// <summary>
        /// Lookup on GeoNames in format Name:Country:Lat:Lon
        /// </summary>
        public List<string> Places { get; } = new List<string>();
        public double? LatitudeNorth { get; set; } // + N to -S
        public double? LatitudeSouth { get; set; } // + N to -S
        public double? LongitudeWest { get; set; } // -W to +E
        public double? LongitudeEast { get; set; } // -W to +E
        public double? ElevationMinimum { get; set; } // m
        public double? ElevationMaximum { get; set; } // m
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class DataWizardApproximation
    {
        public long RowCount { get; set; }
        public List<string> Errors { get; } = new List<string>();
    }

    public enum DownloadFormat { CSV, Excel, NetCDF }

    public class DataWizardDownloadInput : DataWizardDataInput
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DownloadFormat DownloadFormat { get; set; } = DownloadFormat.CSV;
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

}
