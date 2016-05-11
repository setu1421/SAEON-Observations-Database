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
            if (!String.IsNullOrEmpty(dfEndDate.Text) && (dfEndDate.SelectedDate.Year >= 1900))
                site.EndDate = dfEndDate.SelectedDate;
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

    protected void SitesGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["ProjectID"] != null && e.Parameters["ProjectID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["ProjectID"].ToString());
            da.SiteCollection col = new da.SiteCollection()
                .Where(da.Site.Columns.ProjectID, Id)
                .OrderByAsc(da.Site.Columns.Code)
                .Load();
            SitesGrid.GetStore().DataSource = col;
            SitesGrid.GetStore().DataBind();
        }
    }

    protected void AvailableSitesStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["ProjectID"] != null && e.Parameters["ProjectID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["ProjectID"].ToString());
            da.SiteCollection col = new Select()
                .From(da.Site.Schema)
                .Where(da.Site.IdColumn)
                .NotIn(new Select(new string[] { da.Site.Columns.Id }).From(da.Site.Schema).Where(da.Site.IdColumn).IsEqualTo(Id))
                .And(da.Site.ProjectIDColumn)
                .IsNull()
                .OrderAsc(da.Site.Columns.Code)
                .ExecuteAsCollection<da.SiteCollection>();
            AvailableSitesGrid.GetStore().DataSource = col;
            AvailableSitesGrid.GetStore().DataBind();
        }
    }

    protected void LinkSites_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel sm = AvailableSitesGrid.SelectionModel.Primary as RowSelectionModel;
            RowSelectionModel masteRow = ProjectsGrid.SelectionModel.Primary as RowSelectionModel;

            var masterID = new Guid(masteRow.SelectedRecordID);
            if (sm.SelectedRows.Count > 0)
            {
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    da.Site site = new da.Site(row.RecordID);
                    if (site != null)
                    {
                        site.ProjectID = masterID;
                        site.UserId = AuthHelper.GetLoggedInUserId;
                        site.Save();
                        Auditing.Log("Projects.AddSiteLink", new Dictionary<string, object> {
                            { "ProjectID", masterID }, { "ID", site.Id }, { "Code", site.Code }, { "Name", site.Name } });
                    }
                }
                SitesGrid.DataBind();
                AvailableSitesWindow.Hide();
            }
            else
            {
                MessageBoxes.Info("Invalid Selection", "Select at least one site");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Projects.LinkSites_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link sites");
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
            da.Site site = new da.Site(aID);
            if (site != null)
            {
                site.ProjectID = null;
                site.UserId = AuthHelper.GetLoggedInUserId;
                site.Save();
                Auditing.Log("Projects.DeleteSiteLink", new Dictionary<string, object> {
                        { "ID", site.Id }, { "Code", site.Code }, { "Name", site.Name } });
                SitesGrid.DataBind();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Projects.DeleteSiteLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete site link");
        }
    }

    #endregion
}