
using Ext.Net;
using System;
using System.Xml;

public partial class Admin_Inventory : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
        }
    }

    protected void InventoryGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        InventoryGridStore.DataSource = InventoryRepository.GetPagedList(e, e.Parameters[InventoryGridFilters.ParamPrefix]);
    }

    protected void InventoryGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
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
        BaseRepository.Export("vInventorySensors", gridData, visCols, sortCol, sortDir, type, "Inventory", Response);
    }
}
