using Ext.Net;
using da = SAEON.ObservationsDB.Data;
using System;
using System.Linq;

public partial class Admin_Site : System.Web.UI.Page
{
    protected void SiteStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.SiteGrid.GetStore().DataSource = SiteRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        da.SiteCollection col = new da.SiteCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = da.Site.Columns.Code;
            errorMessage = "The specified Site Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = da.Site.Columns.Name;
            errorMessage = "The specified Site Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new da.SiteCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new da.SiteCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(da.Site.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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

        da.Site org = new da.Site();

        if (String.IsNullOrEmpty(tfID.Text))
            org.Id = Guid.NewGuid();
        else
            org = new da.Site(tfID.Text.Trim());

        org.Code = tfCode.Text.Trim();
        org.Name = tfName.Text.Trim();
        org.Description = tfDescription.Text.Trim();

        org.UserId = AuthHelper.GetLoggedInUserId;

        org.Save();

        SiteGrid.DataBind();

        this.DetailWindow.Hide();
    }

    protected void SiteStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("Site", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }
}