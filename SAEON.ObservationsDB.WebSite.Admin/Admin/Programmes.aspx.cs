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
            else
                site.StartDate = null;
            if (!String.IsNullOrEmpty(dfEndDate.Text) && (dfEndDate.SelectedDate.Year >= 1900))
                site.EndDate = dfEndDate.SelectedDate;
            else
                site.EndDate = null;
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

    protected void ProjectLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["ProgrammeID"] != null && e.Parameters["ProgrammeID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["ProgrammeID"].ToString());
            VProgrammeProjectCollection col = new VProgrammeProjectCollection()
                .Where(VProgrammeProject.Columns.ProgrammeID, Id)
                .OrderByAsc(VProgrammeProject.Columns.StartDate)
                .OrderByAsc(VProgrammeProject.Columns.EndDate)
                .OrderByAsc(VProgrammeProject.Columns.ProjectName)
                .Load();
            ProjectLinksGrid.GetStore().DataSource = col;
            ProjectLinksGrid.GetStore().DataBind();
        }
    }

    protected void LinkProject_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = ProgrammesGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            ProgrammeProject programmeProject = new ProgrammeProject(Utilities.MakeGuid(ProjectLinkID.Value));
            programmeProject.ProgrammeID = masterID;
            programmeProject.ProjectID = new Guid(cbProject.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfProjectStartDate.Text) && (dfProjectStartDate.SelectedDate.Year >= 1900))
                programmeProject.StartDate = dfProjectStartDate.SelectedDate;
            if (!String.IsNullOrEmpty(dfProjectEndDate.Text) && (dfProjectEndDate.SelectedDate.Year >= 1900))
                programmeProject.EndDate = dfProjectEndDate.SelectedDate;
            programmeProject.UserId = AuthHelper.GetLoggedInUserId;
            programmeProject.Save();
            Auditing.Log("Programmes.AddProjectLink", new Dictionary<string, object> {
                { "ProgrammeID", programmeProject.ProgrammeID },
                { "ProgrammeCode", programmeProject.Programme.Code },
                { "ProjectID", programmeProject.ProjectID},
                { "ProjectCode", programmeProject.Project.Code},
                { "StartDate", programmeProject.StartDate },
                { "EndDate", programmeProject.EndDate}
            });
            ProjectLinksGrid.DataBind();
            ProjectLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Programmes.LinkProject_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link project");
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
            new ProgrammeProjectController().Delete(aID);
            Auditing.Log("Programmes.DeleteProjectLink", new Dictionary<string, object> { { "ID", aID } });
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