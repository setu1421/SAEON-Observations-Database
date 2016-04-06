using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FieldDefinition
/// </summary>
/// 
[Serializable]
public class FieldDefinition
{
    public FieldDefinition()
    {
    }
    public string id { get; set; }
    public string Name { get; set; }
    public Boolean Datefield { get; set; }
    public string Dateformat { get; set; }
    public Boolean Timefield { get; set; }
    public string Timeformat { get; set; }
    public Boolean Ignorefield { get; set; }
    public Guid? PhenomenonID { get; set; }
    public  Guid?  OfferingID { get; set; }
    public Guid? UnitofMeasureID { get; set; }
    public string EmptyValue { get; set; }
    public int? FieldLength { get; set; }
    public Boolean Offeringfield { get; set; }
    public Boolean FixedTimeField { get; set; }
    public string FixedTime { get; set; }
    public Boolean Commentfield { get; set; }
}