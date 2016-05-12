using Ext.Net;
using SAEON.ObservationsDB.Data;
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
            Programme site = new Programme();
            if (String.IsNullOrEmpty(tfID.Text))
                site.Id = Guid.NewGuid();
            else
                site = new Programme(tfID.Text.Trim());
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
            Auditing.Log("Programmes.Save", new Dictionary<string, object> { { "ID", site.Id }, { "Code", site.Code }, { "Name", site.Name } });

            ProgrammesGrid.DataBind();

            DetailWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Programmes.Save");
            MessageBoxes.Error(ex, "Error", "Unable to save site");
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

    protected void ProjectsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["ProgrammeID"] != null && e.Parameters["ProgrammeID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["ProgrammeID"].ToString());
            VProjectCollection col = new ProjectCollection()
                .Where(Project.Columns.ProgrammeID, Id)
                .OrderByAsc(Project.Columns.Code)
                .Load();
            ProjectsGrid.GetStore().DataSource = col;
            ProjectsGrid.GetStore().DataBind();
        }
    }

    protected void AvailableProjectsStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["ProgrammeID"] != null && e.Parameters["ProgrammeID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["ProgrammeID"].ToString());
            ProjectCollection col = new Select()
                .From(Project.Schema)
                .Where(Project.IdColumn)
                .NotIn(new Select(new string[] { Project.Columns.Id }).From(Project.Schema).Where(Project.IdColumn).IsEqualTo(Id))
                .And(Project.ProgrammeIDColumn)
                .IsNull()
                .OrderAsc(Project.Columns.Code)
                .ExecuteAsCollection<ProjectCollection>();
            AvailableProjectsGrid.GetStore().DataSource = col;
            AvailableProjectsGrid.GetStore().DataBind();
        }
    }

    protected void LinkProjects_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel sm = AvailableProjectsGrid.SelectionModel.Primary as RowSelectionModel;
            RowSelectionModel masteRow = ProgrammesGrid.SelectionModel.Primary as RowSelectionModel;

            var masterID = new Guid(masteRow.SelectedRecordID);
            if (sm.SelectedRows.Count > 0)
            {
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    Project project = new Project(row.RecordID);
                    if (project != null)
                    {
                        project.ProgrammeID = masterID;
                        project.UserId = AuthHelper.GetLoggedInUserId;
                        project.Save();
                        Auditing.Log("Programmes.AddProjectLink", new Dictionary<string, object> {
                            { "ProgrammeID", masterID }, { "ID", project.Id }, { "Code", project.Code }, { "Name", project.Name } });
                    }
                }
                ProjectsGrid.DataBind();
                AvailableProjectsWindow.Hide();
            }
            else
            {
                MessageBoxes.Info("Invalid Selection", "Select at least one project");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Programmes.LinkProjects_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link projects");
        }
    }

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
            if (project != null)
            {
                project.ProgrammeID = null;
                project.UserId = AuthHelper.GetLoggedInUserId;
                project.Save();
                Auditing.Log("Programmes.DeleteProjectLink", new Dictionary<string, object> {
                        { "ID", project.Id }, { "Code", project.Code }, { "Name", project.Name } });
                ProjectsGrid.DataBind();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Programmes.DeleteProjectLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete project link");
        }
    }

    #endregion

}