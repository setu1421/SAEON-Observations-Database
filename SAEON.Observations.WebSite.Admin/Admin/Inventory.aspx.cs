using Ext.Net;
using SAEON.Observations.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

public partial class Admin_Inventory : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!X.IsAjaxRequest)
        //{
        SetupForm();
        //}
    }

    void SetupForm()
    {
        Store store = ViewGrid.GetStore();

        JsonReader reader = store.Reader[0] as JsonReader;

        DataTable tb = ViewRepository.GetSchema("vInventory");

        if (tb.Columns.IndexOf("CNT") > 0) tb.Columns.Remove("CNT");
        if (tb.Columns.IndexOf("RowNo") > 0) tb.Columns.Remove("RowNo");


        int index = 0;
        foreach (DataColumn dfld in tb.Columns)
        {


            Type fldtype = dfld.DataType;
            RecordFieldType tp = GetExtTypeFromSystemType(fldtype);
            RecordField fld = new RecordField(dfld.ColumnName, tp);
            if (tp == RecordFieldType.Date)
            {
                //fld.DateFormat = "yyyy-MM-ddThh:mm:ss";
            }
            fld.UseNull = true;
            store.AddField(fld);

            ColumnBase col = null;
            GridFilter filter = null;


            switch (tp)
            {
                case RecordFieldType.Auto:
                    break;
                case RecordFieldType.Boolean:
                    col = new BooleanColumn();
                    filter = new BooleanFilter();
                    break;
                case RecordFieldType.Date:
                    col = new DateColumn();
                    filter = new DateFilter();

                    break;
                case RecordFieldType.Float:
                case RecordFieldType.Int:
                    col = new NumberColumn();

                    filter = new NumericFilter();
                    break;
                default:
                    col = new Column();
                    filter = new StringFilter();
                    break;
            }

            filter.DataIndex = dfld.ColumnName;

            col.Header = dfld.ColumnName;
            col.DataIndex = dfld.ColumnName;
            col.Width = Unit.Pixel(200);
            col.Groupable = true;
            col.GroupName = dfld.ColumnName;


            ViewGridFilters.Filters.Add(filter);
            ViewGrid.ColumnModel.Columns.Add(col);

            if (index == 0)
            {

                ViewGrid.GetStore().SortInfo.Field = dfld.ColumnName;
                ViewGrid.GetStore().SortInfo.Direction = Ext.Net.SortDirection.ASC;
            }

            index++;
        }


    }

    RecordFieldType GetExtTypeFromSystemType(System.Type type)
    {
        if (type == typeof(Int16) || type == typeof(Int32))
            return RecordFieldType.Int;
        else if (type == typeof(DateTime))
            return RecordFieldType.Date;
        else if (type == typeof(Int64))
            return RecordFieldType.Float;
        else if (type == typeof(Double))
            return RecordFieldType.Float;

        else
            return RecordFieldType.String;
    }

    protected void ViewGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {

        DataTable tb = ViewRepository.GetPagedSet(ref e, e.Parameters[this.ViewGridFilters.ParamPrefix], "vInventory");

        Store store = ViewGrid.GetStore();

        JsonReader reader = store.Reader[0] as JsonReader;

        foreach (DataColumn col in tb.Columns)
        {
            Type fldtype = col.DataType;
            RecordFieldType tp = GetExtTypeFromSystemType(fldtype);
            RecordField fld = new RecordField(col.ColumnName, tp);
            if (tp == RecordFieldType.Date)
            {
                fld.DateFormat = "yyyy-MM-dd";
            }

            reader.Fields.Add(fld);
        }

        this.ViewGrid.GetStore().DataSource = tb;
    }

    protected void ViewGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {


        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();

        string json = GridData.Value.ToString();
        StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(json, null);
        XmlNode xml = eSubmit.Xml;

        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        //string js = ViewRepository.Export(gridData, visCols, sortCol, sortDir, "vInventory");
        //BaseRepository.doExport(type, js);
        BaseRepository.Export("vInventory", gridData, visCols, sortCol, sortDir, type, "Inventory");
    }
}
