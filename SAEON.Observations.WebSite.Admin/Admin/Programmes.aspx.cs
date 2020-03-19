using Ext.Net;
using SAEON.Logs;
using SAEON.Observations.Data;
using SubSonic;
using System;

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
        string checkColumn = String.Empty;
        string errorMessage = String.Empty;
        e.Success = true;
        tfCode.HasValue();
        tfName.HasValue();
        tfDescription.HasValue();

        if (e.ID == "tfCode" || e.ID == "tfName")
        {
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

            if (tfID.IsEmpty)
                col = new ProgrammeCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            else
                col = new ProgrammeCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Programme.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

            if (col.Count > 0)
            {
                e.Success = false;
                e.ErrorMessage = errorMessage;
            }
        }
    }

    protected void Save(object sender, DirectEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            try
            {
                Programme programme = new Programme();
                if (!tfID.HasValue())
                    programme.Id = Guid.NewGuid();
                else
                    programme = new Programme(tfID.Text);
                if (tfCode.HasValue())
                    programme.Code = tfCode.Text;
                if (tfName.HasValue())
                    programme.Name = tfName.Text;
                if (tfDescription.HasValue())
                    programme.Description = tfDescription.Text;
                if (tfUrl.HasValue())
                    programme.Url = tfUrl.Text;
                else
                    programme.Url = null;
                if (dfStartDate.HasValue())
                    programme.StartDate = dfStartDate.SelectedDate;
                else
                    programme.StartDate = null;
                if (dfEndDate.HasValue())
                    programme.EndDate = dfEndDate.SelectedDate;
                else
                    programme.EndDate = null;
                programme.UserId = AuthHelper.GetLoggedInUserId;

                programme.Save();
                Auditing.Log(GetType(), new MethodCallParameters { { "ID", programme.Id }, { "Code", programme.Code }, { "Name", programme.Name } });

                ProgrammesGrid.DataBind();

                DetailWindow.Hide();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to save programme");
            }
        }
    }

    protected void ProgrammesGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        //string js = BaseRepository.BuildExportQ("Programme", gridData, visCols, sortCol, sortDir);
        //BaseRepository.doExport(type, js);
        BaseRepository.Export("Programme", gridData, visCols, sortCol, sortDir, type, "Programmes", Response);
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

    protected void ProjectLinksSave(object sender, DirectEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
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
                            Auditing.Log(GetType(), new MethodCallParameters {
                                { "ProgrammeID", project.ProgrammeID},
                                { "ProgrammeCode", project.Programme.Code},
                                { "ProjectID", project.Id },
                                { "ProjectCode", project.Code }
                            });
                        }
                        catch (Exception ex)
                        {
                            Logging.Exception(ex);
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
    //            { "StartDate", programmeProject?.StartDate },
    //            { "EndDate", programmeProject?.EndDate}
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

    #endregion

}