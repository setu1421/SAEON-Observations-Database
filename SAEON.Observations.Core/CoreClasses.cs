﻿#if NET472
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
#if NET5_0_OR_GREATER
using System.Text.Json.Serialization;
#endif

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

    public class LocationTreeNode : TreeNode
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Elevation { get; set; }
        public string Url { get; set; }
    }

    public class VariableTreeNode : TreeNode { }

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
    /*
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
            if (value is null) return string.Empty;
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
            return (val is null);
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

        public DataMatrixColumn AddColumn(string name, MaxtixDataType dataType)
        {
            return AddColumn(name, name, dataType);
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

        public DataTable ToDataTable()
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
                    if (value is not null)
                    {
                        dataRow[dmCol.Name] = value;
                    }
                }
                result.Rows.Add(dataRow);
            }
            return result;
        }

        public byte[] ToExcel()
        {
            using (var ms = new MemoryStream())
            {
                using (var doc = ExcelSaxHelper.CreateSpreadsheet(ms, ToDataTable()))
                {
                    doc.Save();
                }
                return ms.ToArray();
            }

        }

        public String ToCSV()
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
    */

    public class LocationFilter
    {
        public Guid StationId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is LocationFilter filter &&
                   StationId.Equals(filter.StationId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StationId);
        }
    }

    public class VariableFilter
    {
        public Guid PhenomenonId { get; set; }
        public Guid OfferingId { get; set; }
        public Guid UnitId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is VariableFilter variable &&
                   PhenomenonId.Equals(variable.PhenomenonId) &&
                   OfferingId.Equals(variable.OfferingId) &&
                   UnitId.Equals(variable.UnitId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PhenomenonId, OfferingId, UnitId);
        }
    }

    public class DataWizardDataInput : IValidatableObject
    {
        public List<LocationFilter> Locations { get; set; } = new List<LocationFilter>();
        public List<VariableFilter> Variables { get; set; } = new List<VariableFilter>();
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [IsBeforeDate(nameof(EndDate))]
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddYears(-100).Date;
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [IsAfterDate(nameof(StartDate))]
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; } = DateTime.Now.Date;
        public double ElevationMinimum { get; set; } = -100; // m
        public double ElevationMaximum { get; set; } = 3000; // m

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > EndDate)
            {
                yield return new ValidationResult("Start Date can't be after End Date");
            }
            if (EndDate < StartDate)
            {
                yield return new ValidationResult("End Date can't be before Start Date");
            }
        }
    }

    public class ChartData
    {
        public DateTime Date { get; set; }
        public double? Value { get; set; }
    }

    public class ChartSeries
    {
        public string Station { get; set; }
        public string Phenomenon { get; set; }
        public string Offering { get; set; }
        public string Unit { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }
        public List<ChartData> Data { get; } = new List<ChartData>();
    }


    public class ObservationDTO
    {
        public string Station { get; set; }
        public string Variable => $"{Phenomenon.Replace(", ", "_")}, {Offering.Replace(", ", "_")}, {Unit.Replace(", ", "_")}";
        public double? Elevation { get; set; }
        public DateTime Date { get; set; }
        public double? Value { get; set; }
        public string Comment { get; set; }
        public string Site { get; set; }
        public string Phenomenon { get; set; }
        public string Offering { get; set; }
        public string Unit { get; set; }
        public string Instrument { get; set; }
        public string Sensor { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ObservationDTO dTO &&
                   Station == dTO.Station &&
                   Variable == dTO.Variable &&
                   Elevation == dTO.Elevation &&
                   Date == dTO.Date &&
                   Value == dTO.Value &&
                   Comment == dTO.Comment &&
                   Site == dTO.Site &&
                   Phenomenon == dTO.Phenomenon &&
                   Offering == dTO.Offering &&
                   Unit == dTO.Unit &&
                   Instrument == dTO.Instrument &&
                   Sensor == dTO.Sensor &&
                   Latitude == dTO.Latitude &&
                   Longitude == dTO.Longitude;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Station);
            hash.Add(Variable);
            hash.Add(Elevation);
            hash.Add(Date);
            hash.Add(Value);
            hash.Add(Comment);
            hash.Add(Site);
            hash.Add(Phenomenon);
            hash.Add(Offering);
            hash.Add(Unit);
            hash.Add(Instrument);
            hash.Add(Sensor);
            hash.Add(Latitude);
            hash.Add(Longitude);
            return hash.ToHashCode();
        }
    }

    public class DataWizardDataOutput
    {
        public Guid Id { get; } = Guid.NewGuid();
        public List<ObservationDTO> Data { get; set; } = new List<ObservationDTO>();
        public DateTime Date { get; } = DateTime.Now;
        public MetadataCore Metadata { get; private set; } = new MetadataCore();
        public List<ChartSeries> ChartSeries { get; } = new List<ChartSeries>();
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
#elif NET5_0_OR_GREATER
        [JsonConverter(typeof(JsonStringEnumConverter))]
#endif
        public DownloadFormat DownloadFormat { get; set; } = DownloadFormat.CSV;
    }
    #endregion

}
