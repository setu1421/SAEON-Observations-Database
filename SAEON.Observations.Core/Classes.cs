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

    public class DataQueryInput
    {
        public List<Guid> Stations { get; set; }
        public List<Guid> Offerings { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class Series
    {
        public string Caption { get; set; }
        public string ColumnName { get; set; }
        public bool IsFeature { get; set; }
    }

    public class DataQueryOutput
    {
        public List<Series> Series { get; private set; } = new List<Series>();
        public List<ExpandoObject> Data { get; private set; } = new List<ExpandoObject>();
    }

    public class DataDownloadInput : DataQueryInput
    {
    }

    public class DataDownloadOutput
    {
    }

    public class InventoryInput
    {
        public Guid? UserQuery { get; set; }
        public bool? Full { get; set; }
        public bool? Statistics { get; set; }
    }

    public class TotalItem
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class InventoryOutput
    {
        public bool Success { get; set; }
        public List<string> ErrorMessage { get; private set; } = new List<string>();
        public List<TotalItem> TotalRecords { get; private set; } = new List<TotalItem>();
    }
}
