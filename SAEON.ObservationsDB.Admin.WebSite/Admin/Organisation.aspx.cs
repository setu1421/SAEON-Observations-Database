using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Observations.Data;
using Ext.Net;
using SubSonic;
using System.Xml;
using System.Xml.Xsl;

/// <summary>
/// Summary description for Organisation
/// </summary>
public partial class _Organisation : System.Web.UI.Page
{

    protected void OrganisationStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.OrganisationGrid.GetStore().DataSource = OrginasationRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        OrganisationCollection col = new OrganisationCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = Organisation.Columns.Code;
            errorMessage = "The specified Organisation Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = Organisation.Columns.Name;
            errorMessage = "The specified Organisation Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new OrganisationCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new OrganisationCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Organisation.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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

        Organisation org = new Organisation();

        if (String.IsNullOrEmpty(tfID.Text))
            org.Id = Guid.NewGuid();
        else
            org = new Organisation(tfID.Text.Trim());

        org.Code = tfCode.Text.Trim();
        org.Name = tfName.Text.Trim();
        org.Description = tfDescription.Text.Trim();

        org.UserId = AuthHelper.GetLoggedInUserId;

        org.Save();

        OrganisationGrid.DataBind();

        this.DetailWindow.Hide();
    }

	protected void OrganisationStore_Submit(object sender, StoreSubmitDataEventArgs e)
	{
		string type = FormatType.Text;
		string visCols = VisCols.Value.ToString();
		string gridData = GridData.Text;
		string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
		string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

		string js = BaseRepository.BuildExportQ("Organisation", gridData, visCols, sortCol, sortDir);

		BaseRepository.doExport(type, js);
	}
}