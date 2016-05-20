using Ext.Net;
using da = SAEON.ObservationsDB.Data;
using Serilog;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_Projects : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            SiteStore.DataSource = new da.SiteCollection().OrderByAsc(da.Site.Columns.Name).Load();
            SiteStore.DataBind();
        }
    }

    #region Projects
    protected void ProjectsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        ProjectsGrid.GetStore().DataSource = ProjectRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }


    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        da.ProjectCollection col = new da.ProjectCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = da.Project.Columns.Code;
            errorMessage = "The specified Project Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = da.Project.Columns.Name;
            errorMessage = "The specified Project Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new da.ProjectCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new da.ProjectCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(da.Project.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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
        try
        {
            da.Project site = new da.Project();
            if (String.IsNullOrEmpty(tfID.Text))
                site.Id = Guid.NewGuid();
            else
                site = new da.Project(tfID.Text.Trim());
            if (!string.IsNullOrEmpty(tfCode.Text.Trim()))
                site.Code = tfCode.Text.Trim();
            if (!string.IsNullOrEmpty(tfName.Text.Trim()))
                site.Name = tfName.Text.Trim();
            if (string.IsNullOrEmpty(site.Name)) site.Name = null;
            site.Description = tfDescription.Text.Trim();
            site.Url = tfUrl.Text.Trim();
            if (!String.IsNullOrEmpty(dfStartDate.Text) && (dfStartDate.SelectedDate.Year >= 1900))
                site.StartDate = dfStartDate.SelectedDate;
            else
                site.StartDate = null;
            if (!String.IsNullOrEmpty(dfEndDate.Text) && (dfEndDate.SelectedDate.Year >= 1900))
                site.EndDate = dfEndDate.SelectedDate;
            else
                site.EndDate = null;
            site.UserId = AuthHelper.GetLoggedInUserId;

            site.Save();
            Auditing.Log("Projects.Save", new Dictionary<string, object> { { "ID", site.Id }, { "Code", site.Code }, { "Name", site.Name } });

            ProjectsGrid.DataBind();

            DetailWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Projects.Save");
            MessageBoxes.Error(ex, "Error", "Unable to save site");
        }
    }

    protected void ProjectsGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("Project", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    #endregion


    #region Sites

    protected void SiteLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["ProjectID"] != null && e.Parameters["ProjectID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["ProjectID"].ToString());
            da.VSiteProjectCollection col = new da.VSiteProjectCollection()
                .Where(da.VSiteProject.Columns.ProjectID, Id)
                .OrderByAsc(da.VSiteProject.Columns.StartDate)
                .OrderByAsc(da.VSiteProject.Columns.EndDate)
                .OrderByAsc(da.VSiteProject.Columns.SiteName)
                .Load();
            SiteLinksGrid.GetStore().DataSource = col;
            SiteLinksGrid.GetStore().DataBind();
        }
    }

    protected void LinkSite_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = ProjectsGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            da.SiteProject siteProject = new da.SiteProject(Utilities.MakeGuid(SiteLinkID.Value));
            siteProject.ProjectID = masterID;
            siteProject.SiteID = new Guid(cbSite.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfSiteStartDate.Text) && (dfSiteStartDate.SelectedDate.Year >= 1900))
                siteProject.StartDate = dfSiteStartDate.SelectedDate;
            if (!String.IsNullOrEmpty(dfSiteEndDate.Text) && (dfSiteEndDate.SelectedDate.Year >= 1900))
                siteProject.EndDate = dfSiteEndDate.SelectedDate;
            siteProject.UserId = AuthHelper.GetLoggedInUserId;
            siteProject.Save();
            Auditing.Log("Projects.AddSiteLink", new Dictionary<string, object> {
                { "ProjectID", siteProject.ProjectID },
                { "ProjectCode", siteProject.Project.Name },
                { "SiteID", siteProject.SiteID},
                { "SiteCode", siteProject.Site.Code},
                { "StartDate", siteProject.StartDate },
                { "EndDate", siteProject.EndDate}
            });
            SiteLinksGrid.DataBind();
            SiteLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Projects.LinkSite_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link site");
        }
    }

    [DirectMethod]
    public void ConfirmDeleteSiteLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteSiteLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this site link?");
    }

    [DirectMethod]
    public void DeleteSiteLink(Guid aID)
    {
        try
        {
            new da.SiteProjectController().Delete(aID);
            Auditing.Log("Projects.DeleteSiteLink", new Dictionary<string, object> { { "ID", aID } });
            SiteLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Projects.DeleteSiteLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete site link");
        }
    }

    [DirectMethod]
    public void AddSiteClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion
}