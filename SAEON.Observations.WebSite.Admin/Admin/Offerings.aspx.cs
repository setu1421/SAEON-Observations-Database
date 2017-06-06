using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAEON.Observations.Data;
using Ext.Net;
using SubSonic;
using System.Xml;
using System.Xml.Xsl;

/// <summary>
/// Summary description for Offering
/// </summary>
public partial class _Offerings : System.Web.UI.Page
{
    protected void OfferingStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.OfferingGrid.GetStore().DataSource = OfferingRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        OfferingCollection col = new OfferingCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = Offering.Columns.Code;
            errorMessage = "The specified Offering Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = Offering.Columns.Name;
            errorMessage = "The specified Offering Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new OfferingCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new OfferingCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Offering.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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

        Offering org = new Offering();

        if (String.IsNullOrEmpty(tfID.Text))
            org.Id = Guid.NewGuid();
        else
            org = new Offering(tfID.Text.Trim());

        org.Code = tfCode.Text.Trim();
        org.Name = tfName.Text.Trim();
        org.Description = tfDescription.Text.Trim();

        org.UserId = AuthHelper.GetLoggedInUserId;

        org.Save();

        OfferingGrid.DataBind();

        this.DetailWindow.Hide();
    }

	protected void OfferingStore_Submit(object sender, StoreSubmitDataEventArgs e)
	{
		string type = FormatType.Text;
		string visCols = VisCols.Value.ToString();
		string gridData = GridData.Text;
		string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
		string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

		string js = BaseRepository.BuildExportQ("Offering", gridData, visCols, sortCol, sortDir);

		BaseRepository.doExport(type, js);
	}
}