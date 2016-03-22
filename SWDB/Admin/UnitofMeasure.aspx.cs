using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAEON.ObservationsDB.Data;
using Ext.Net;
using FileHelpers.Dynamic;
using FileHelpers;
using SubSonic;
using System.Xml;
using System.Xml.Xsl;

public partial class _UnitofMeasure : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void UnitOfMeasureStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.UnitOfMeasureGrid.GetStore().DataSource = UnitOfMeasureRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        UnitOfMeasureCollection col = new UnitOfMeasureCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = UnitOfMeasure.Columns.Code;
            errorMessage = "The specified Unit Code already exists";
        }
        else if (e.ID == "tfUnit")
        {
            checkColumn = UnitOfMeasure.Columns.Unit;
            errorMessage = "The specified Unit already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new UnitOfMeasureCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new UnitOfMeasureCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(UnitOfMeasure.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

        if (col.Count > 0)
        {
            e.Success = false;
            e.ErrorMessage = errorMessage;
        }
        else
            e.Success = true;
    }
    protected void Save(object sender, DirectEventArgs e)
    {

        UnitOfMeasure prosite = new UnitOfMeasure();

        if (String.IsNullOrEmpty(tfID.Text))
            prosite.Id = Guid.NewGuid();
        else
            prosite = new UnitOfMeasure(tfID.Text.Trim());

        prosite.Code = tfCode.Text.Trim();
        prosite.Unit = tfUnit.Text.Trim();
        prosite.UnitSymbol = tfSymbol.Text.Trim();

        prosite.UserId = AuthHelper.GetLoggedInUserId;

        prosite.Save();

        UnitOfMeasureGrid.DataBind();

        this.DetailWindow.Hide();
    }

	protected void UnitOfMeasureStore_Submit(object sender, StoreSubmitDataEventArgs e)
	{
		string type = FormatType.Text;
		string visCols = VisCols.Value.ToString();
		string gridData = GridData.Text;
		string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
		string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

		string js = BaseRepository.BuildExportQ("UnitOfMeasure", gridData, visCols, sortCol, sortDir);

		BaseRepository.doExport(type, js);
	}
}