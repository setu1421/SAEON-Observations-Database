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

public partial class Admin_Programmes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            ProjectStore.DataSource = new ProjectCollection().OrderByAsc(Project.Columns.Name).Load();
            ProjectStore.DataBind();
        }
    }

    #region Programmes
    protected void ProgrammesGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        ProgrammesGrid.GetStore().DataSource = ProgrammeRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }


    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        ProgrammeCollection col = new ProgrammeCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = Programme.Columns.Code;
            errorMessage = "The specified Programme Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = Programme.Columns.Name;
            errorMessage = "The specified Programme Name already exists";

        }

        if (!string.IsNullOrEmpty(checkColumn))
            if (String.IsNullOrEmpty(tfID.Text.ToString()))
                col = new ProgrammeCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            else
                col = new ProgrammeCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Programme.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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
            Programme programme = new Programme();
            if (String.IsNullOrEmpty(tfID.Text))
                programme.Id = Guid.NewGuid();
            else
                programme = new Programme(tfID.Text.Trim());
            if (!string.IsNullOrEmpty(tfCode.Text.Trim()))
                programme.Code = tfCode.Text.Trim();
            if (!string.IsNullOrEmpty(tfName.Text.Trim()))
                programme.Name = tfName.Text.Trim();
            programme.Description = tfDescription.Text.Trim();
            programme.Url = tfUrl.Text.Trim();
            if (!String.IsNullOrEmpty(dfStartDate.Text) && (dfStartDate.SelectedDate.Year >= 1900))
                programme.StartDate = dfStartDate.SelectedDate;
            else
                programme.StartDate = null;
            if (!String.IsNullOrEmpty(dfEndDate.Text) && (dfEndDate.SelectedDate.Year >= 1900))
                programme.EndDate = dfEndDate.SelectedDate;
            else
                programme.EndDate = null;
            programme.UserId = AuthHelper.GetLoggedInUserId;

            programme.Save();
            Auditing.Log("Programmes.Save", new Dictionary<string, object> { { "ID", programme.Id }, { "Code", programme.Code }, { "Name", programme.Name } });

            ProgrammesGrid.DataBind();

            DetailWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Programmes.Save");
            MessageBoxes.Error(ex, "Error", "Unable to save programme");
        }
    }

    protected void ProgrammesGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("Programme", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    #endregion


    #region Projects

    protected void ProjectLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["ProgrammeID"] != null && e.Parameters["ProgrammeID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["ProgrammeID"].ToString());
            ProjectCollection col = new ProjectCollection()
                .Where(Project.Columns.ProgrammeID, Id)
                .OrderByAsc(Project.Columns.StartDate)
                .OrderByAsc(Project.Columns.EndDate)
                .OrderByAsc(Project.Columns.Name)
                .Load();
            ProjectLinksGrid.GetStore().DataSource = col;
            ProjectLinksGrid.GetStore().DataBind();
        }
    }

    protected void AvailableProjectsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["ProgrammeID"] != null && e.Parameters["ProgrammeID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["ProgrammeID"].ToString());
            ProjectCollection col = new Select()
                .From(Project.Schema)
                .Where(Project.IdColumn)
                .NotIn(new Select(new string[] { Project.Columns.Id }).From(Project.Schema).Where(Project.ProgrammeIDColumn).IsEqualTo(Id))
                .And(Project.ProgrammeIDColumn)
                .IsNull()
                .OrderAsc(Project.Columns.StartDate)
                .OrderAsc(Project.Columns.EndDate)
                .OrderAsc(Project.Columns.Name)
                .ExecuteAsCollection<ProjectCollection>();
            AvailableProjectsGrid.GetStore().DataSource = col;
            AvailableProjectsGrid.GetStore().DataBind();
        }
    }

    protected void AcceptProjectsButton_Click(object sender, DirectEventArgs e)
    {
        RowSelectionModel sm = AvailableProjectsGrid.SelectionModel.Primary as RowSelectionModel;
        RowSelectionModel programmeRow = ProgrammesGrid.SelectionModel.Primary as RowSelectionModel;

        string programmeID = programmeRow.SelectedRecordID;
        if (sm.SelectedRows.Count > 0)
        {
            foreach (SelectedRow row in sm.SelectedRows)
            {
                Project project = new Project(row.RecordID);
                if (project != null)
                    try
                    {
                        project.ProgrammeID = new Guid(programmeID);
                        project.UserId = AuthHelper.GetLoggedInUserId;
                        project.Save();
                        Auditing.Log("Programmes.AddProjectLink", new Dictionary<string, object> {
                                { "ProgrammeID", project.ProgrammeID},
                                { "ProgrammeCode", project.Programme.Code},
                                { "ProjectID", project.Id },
                                { "ProjectCode", project.Code }
                            });
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Programme.LinkProject_Click");
                        MessageBoxes.Error(ex, "Error", "Unable to link programme");
                    }
            }
            ProjectLinksGridStore.DataBind();
            AvailableProjectsWindow.Hide();
        }
        else
        {
            MessageBoxes.Error("Invalid Selection", "Select at least one project");
        }
    }

    //protected void LinkProject_Click(object sender, DirectEventArgs e)
    //{
    //    try
    //    {
    //        RowSelectionModel masterRow = ProgrammesGrid.SelectionModel.Primary as RowSelectionModel;
    //        var masterID = new Guid(masterRow.SelectedRecordID);
    //        ProgrammeProject programmeProject = new ProgrammeProject(Utilities.MakeGuid(ProjectLinkID.Value));
    //        programmeProject.ProgrammeID = masterID;
    //        programmeProject.ProjectID = new Guid(cbProject.SelectedItem.Value.Trim());
    //        if (!String.IsNullOrEmpty(dfProjectStartDate.Text) && (dfProjectStartDate.SelectedDate.Year >= 1900))
    //            programmeProject.StartDate = dfProjectStartDate.SelectedDate;
    //        if (!String.IsNullOrEmpty(dfProjectEndDate.Text) && (dfProjectEndDate.SelectedDate.Year >= 1900))
    //            programmeProject.EndDate = dfProjectEndDate.SelectedDate;
    //        programmeProject.UserId = AuthHelper.GetLoggedInUserId;
    //        programmeProject.Save();
    //        Auditing.Log("Programmes.AddProjectLink", new Dictionary<string, object> {
    //            { "ProgrammeID", programmeProject.ProgrammeID },
    //            { "ProgrammeCode", programmeProject.Programme.Code },
    //            { "ProjectID", programmeProject.ProjectID},
    //            { "ProjectCode", programmeProject.Project.Code},
    //            { "StartDate", programmeProject.StartDate },
    //            { "EndDate", programmeProject.EndDate}
    //        });
    //        ProjectLinksGrid.DataBind();
    //        ProjectLinkWindow.Hide();
    //    }
    //    catch (Exception ex)
    //    {
    //        Log.Error(ex, "Programmes.LinkProject_Click");
    //        MessageBoxes.Error(ex, "Error", "Unable to link project");
    //    }
    //}

    [DirectMethod]
    public void ConfirmDeleteProjectLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteProjectLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this project link?");
    }

    [DirectMethod]
    public void DeleteProjectLink(Guid aID)
    {
        try
        {
            Project project = new Project(aID);
            project.ProgrammeID = null;
            project.Save();
            ProjectLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Programmes.DeleteProjectLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete project link");
        }
    }

    [DirectMethod]
    public void AddProjectClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Projects"));
    }
    #endregion

}