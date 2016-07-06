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
            ProgrammeStore.DataSource = new ProgrammeCollection().OrderByAsc(Programme.Columns.Name).Load();
            ProgrammeStore.DataBind();
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

        if (!string.IsNullOrEmpty(checkColumn))
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
            Project project = new Project();
            if (String.IsNullOrEmpty(tfID.Text))
                project.Id = Guid.NewGuid();
            else
                project = new Project(tfID.Text.Trim());
            if (!string.IsNullOrEmpty(tfCode.Text.Trim()))
                project.Code = tfCode.Text.Trim();
            if (!string.IsNullOrEmpty(tfName.Text.Trim()))
                project.Name = tfName.Text.Trim();
            if (cbProgramme.SelectedItem.Value == null)
                project.ProgrammeID = null;
            else
                project.ProgrammeID = Utilities.MakeGuid(cbProgramme.SelectedItem.Value.Trim());
            project.Description = tfDescription.Text.Trim();
            project.Url = tfUrl.Text.Trim();
            if (!String.IsNullOrEmpty(dfStartDate.Text) && (dfStartDate.SelectedDate.Year >= 1900))
                project.StartDate = dfStartDate.SelectedDate;
            else
                project.StartDate = null;
            if (!String.IsNullOrEmpty(dfEndDate.Text) && (dfEndDate.SelectedDate.Year >= 1900))
                project.EndDate = dfEndDate.SelectedDate;
            else
                project.EndDate = null;
            project.UserId = AuthHelper.GetLoggedInUserId;

            project.Save();
            Auditing.Log("Projects.Save", new Dictionary<string, object> { { "ID", project.Id }, { "Code", project.Code }, { "Name", project.Name } });

            ProjectsGrid.DataBind();

            DetailWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Projects.Save");
            MessageBoxes.Error(ex, "Error", "Unable to save project");
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

    protected void StationLinkSave(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = ProjectsGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            ProjectStation projectStation = new ProjectStation(Utilities.MakeGuid(ProjectStationLinkID.Value));
            projectStation.ProjectID = masterID;
            projectStation.StationID = new Guid(cbStation.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfStationStartDate.Text) && (dfStationStartDate.SelectedDate.Year >= 1900))
                projectStation.StartDate = dfStationStartDate.SelectedDate;
            if (!String.IsNullOrEmpty(dfStationEndDate.Text) && (dfStationEndDate.SelectedDate.Year >= 1900))
                projectStation.EndDate = dfStationEndDate.SelectedDate;
            projectStation.UserId = AuthHelper.GetLoggedInUserId;
            projectStation.Save();
            Auditing.Log("Projects.AddStationLink", new Dictionary<string, object> {
                { "ProjectID", projectStation.ProjectID },
                { "ProjectCode", projectStation.Project.Name },
                { "StationID", projectStation.StationID},
                { "StationCode", projectStation.Station.Code},
                { "StartDate", projectStation.StartDate },
                { "EndDate", projectStation.EndDate}
            });
            StationLinksGrid.DataBind();
            StationLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Projects.LinkStation_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link station");
        }
    }

    [DirectMethod]
    public void ConfirmDeleteStationLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteStationLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this station link?");
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
            MessageBoxes.Error(ex, "Error", "Unable to delete station link");
        }
    }

    [DirectMethod]
    public void AddStationClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Stations"));
    }
    #endregion
}