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
    }

    public class Feature : TreeNode { }
    public class Location : TreeNode { }

    public class DataQueryInput
    {
        public List<Guid> Locations { get; set; }
        public List<Guid> Features { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class SeriesPoint
    {
        public DateTime Date { get; set; }
        public double? Value { get; set; }

    }

    public class Series
    {
        public string Caption { get; set; }
        public string Name { get; set; }
        public bool IsFeature { get; set; }
        public List<SeriesPoint> Points { get; private set; } = new List<SeriesPoint>();
    }

    public class DataQueryOutput
    {
        public DataTable Data { get; private set; } = new DataTable();
        public List<Series> Series { get; private set; } = new List<Core.Series>();
    }
}
