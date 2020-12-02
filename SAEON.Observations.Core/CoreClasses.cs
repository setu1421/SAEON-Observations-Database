#if NET472
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#endif
using SAEON.Core;
using System;
using System.Collections.Generic;
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

    public abstract class TreeNode
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
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

    public class VariableNode : TreeNode { }

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
#if NET472
        [JsonConverter(typeof(StringEnumConverter))]
#endif
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

    public class Location
    {
        public Guid StationId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Location location &&
                   StationId.Equals(location.StationId);
        }

        public override int GetHashCode()
        {
            return -637447666 + StationId.GetHashCode();
        }
    }

    public class Variable
    {
        public Guid PhenomenonId { get; set; }
        public Guid OfferingId { get; set; }
        public Guid UnitId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Variable variable &&
                   PhenomenonId.Equals(variable.PhenomenonId) &&
                   OfferingId.Equals(variable.OfferingId) &&
                   UnitId.Equals(variable.UnitId);
        }

#if NET472
        public override int GetHashCode()
        {
            var hashCode = 1825368645;
            hashCode = hashCode * -1521134295 + PhenomenonId.GetHashCode();
            hashCode = hashCode * -1521134295 + OfferingId.GetHashCode();
            hashCode = hashCode * -1521134295 + UnitId.GetHashCode();
            return hashCode;
        }
#else
        public override int GetHashCode()
        {
            return HashCode.Combine(PhenomenonId, OfferingId, UnitId);
        }
#endif
    }

    public class DataWizardDataInput
    {
        public List<Location> Locations { get; set; } = new List<Location>();
        public List<Variable> Variables { get; set; } = new List<Variable>();
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
#if NET472
        [JsonConverter(typeof(StringEnumConverter))]
#endif
        public DownloadFormat DownloadFormat { get; set; } = DownloadFormat.CSV;
    }
    #endregion

}
