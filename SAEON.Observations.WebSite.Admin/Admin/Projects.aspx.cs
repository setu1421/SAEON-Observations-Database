using Ext.Net;
using SAEON.Observations.Data;
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
            StationStore.DataSource = new StationCollection().OrderByAsc(Station.Columns.Name).Load();
            StationStore.DataBind();
        }
    }

    #region Projects
    protected void ProjectsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        ProjectsGrid.GetStore().DataSource = ProjectRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }


    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        ProjectCollection col = new ProjectCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = Project.Columns.Code;
            errorMessage = "The specified Project Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = Project.Columns.Name;
            errorMessage = "The specified Project Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new ProjectCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new ProjectCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Project.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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
            Project site = new Project();
            if (String.IsNullOrEmpty(tfID.Text))
                site.Id = Guid.NewGuid();
            else
                site = new Project(tfID.Text.Trim());
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


    #region Stations

    protected void StationLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["ProjectID"] != null && e.Parameters["ProjectID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["ProjectID"].ToString());
            VProjectStationCollection col = new VProjectStationCollection()
                .Where(VProjectStation.Columns.ProjectID, Id)
                .OrderByAsc(VProjectStation.Columns.StartDate)
                .OrderByAsc(VProjectStation.Columns.EndDate)
                .OrderByAsc(VProjectStation.Columns.StationName)
                .Load();
            StationLinksGrid.GetStore().DataSource = col;
            StationLinksGrid.GetStore().DataBind();
        }
    }

    protected void LinkStation_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = ProjectsGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            ProjectStation siteProject = new ProjectStation(Utilities.MakeGuid(StationLinkID.Value));
            siteProject.ProjectID = masterID;
            siteProject.StationID = new Guid(cbStation.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfStationStartDate.Text) && (dfStationStartDate.SelectedDate.Year >= 1900))
                siteProject.StartDate = dfStationStartDate.SelectedDate;
            if (!String.IsNullOrEmpty(dfStationEndDate.Text) && (dfStationEndDate.SelectedDate.Year >= 1900))
                siteProject.EndDate = dfStationEndDate.SelectedDate;
            siteProject.UserId = AuthHelper.GetLoggedInUserId;
            siteProject.Save();
            Auditing.Log("Projects.AddStationLink", new Dictionary<string, object> {
                { "ProjectID", siteProject.ProjectID },
                { "ProjectCode", siteProject.Project.Name },
                { "StationID", siteProject.StationID},
                { "StationCode", siteProject.Station.Code},
                { "StartDate", siteProject.StartDate },
                { "EndDate", siteProject.EndDate}
            });
            StationLinksGrid.DataBind();
            StationLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Projects.LinkStation_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link site");
        }
    }

    [DirectMethod]
    public void ConfirmDeleteStationLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteStationLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this site link?");
    }

    [DirectMethod]
    public void DeleteStationLink(Guid aID)
    {
        try
        {
            new ProjectStationController().Delete(aID);
            Auditing.Log("Projects.DeleteStationLink", new Dictionary<string, object> { { "ID", aID } });
            StationLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Projects.DeleteStationLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete site link");
        }
    }

    [DirectMethod]
    public void AddStationClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Stations"));
    }
    #endregion
}