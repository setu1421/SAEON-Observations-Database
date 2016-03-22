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
/// Summary description for ProjectSite
/// </summary>
public partial class _ProjectSite : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            this.Store1.DataSource = new OrganisationCollection().OrderByAsc(Organisation.Columns.Name).Load();
            this.Store1.DataBind();
        }
    }

    protected void ProjectSiteStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.ProjectSiteGrid.GetStore().DataSource = ProjectSiteRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        ProjectSiteCollection col = new ProjectSiteCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = ProjectSite.Columns.Code;
            errorMessage = "The specified Project Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = ProjectSite.Columns.Name;
            errorMessage = "The specified Project Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new ProjectSiteCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new ProjectSiteCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(ProjectSite.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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

        ProjectSite prosite = new ProjectSite();

        if (String.IsNullOrEmpty(tfID.Text))
            prosite.Id = Guid.NewGuid();
        else
            prosite = new ProjectSite(tfID.Text.Trim());

        prosite.Code = tfCode.Text.Trim();
        prosite.Name = tfName.Text.Trim();
        prosite.Description = tfDescription.Text.Trim();
        prosite.OrganisationID = new Guid(cbOrg.SelectedItem.Value.Trim());

        prosite.UserId = AuthHelper.GetLoggedInUserId;

        prosite.Save();

        ProjectSiteGrid.DataBind();

        this.DetailWindow.Hide();
    }

    protected void ProjectSiteStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("VProjectSite", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }
}