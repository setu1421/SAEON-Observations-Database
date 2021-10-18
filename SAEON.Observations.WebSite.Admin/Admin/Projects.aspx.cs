﻿using Ext.Net;
using SAEON.Logs;
using SAEON.Observations.Data;
using System;
using System.Configuration;
using System.Linq;

public partial class Admin_Projects : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var showValidate = ConfigurationManager.AppSettings["ShowValidateButton"] == "true" && Request.IsLocal;
        btnValidate.Hidden = !showValidate;
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
        string checkColumn = String.Empty;
        string errorMessage = String.Empty;
        e.Success = true;
        tfCode.HasValue();
        tfName.HasValue();

        if (e.ID == "tfCode" || e.ID == "tfName")
        {
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

            if (tfID.IsEmpty)
                col = new ProjectCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            else
                col = new ProjectCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Project.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

            if (col.Count > 0)
            {
                e.Success = false;
                e.ErrorMessage = errorMessage;
            }
        }
        if (e.ID == "dfStartDate" && dfStartDate.HasValue() && dfEndDate.HasValue())
        {
            if (dfStartDate.SelectedDate > dfEndDate.SelectedDate)
            {
                e.Success = false;
                e.ErrorMessage = "Start Date must be before End Date";
            }
        }
        if (e.ID == "dfEndDate" && dfStartDate.HasValue() && dfEndDate.HasValue())
        {
            if (dfStartDate.SelectedDate > dfEndDate.SelectedDate)
            {
                e.Success = false;
                e.ErrorMessage = "End Date must be after Start Date";
            }
        }
        if (e.ID == "dfStationStartDate" && dfStationStartDate.HasValue() && dfStationEndDate.HasValue())
        {
            if (dfStationStartDate.SelectedDate > dfStationEndDate.SelectedDate)
            {
                e.Success = false;
                e.ErrorMessage = "Start Date must be before End Date";
            }
        }
        if (e.ID == "dfStationEndDate" && dfStationStartDate.HasValue() && dfStationEndDate.HasValue())
        {
            if (dfStationStartDate.SelectedDate > dfStationEndDate.SelectedDate)
            {
                e.Success = false;
                e.ErrorMessage = "End Date must be after Start Date";
            }
        }
    }

    protected void Save(object sender, DirectEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                Project project = new Project();
                if (!tfID.HasValue())
                    project.Id = Guid.NewGuid();
                else
                    project = new Project(tfID.Text);
                if (tfCode.HasValue())
                    project.Code = tfCode.Text.Trim();
                if (tfName.HasValue())
                    project.Name = tfName.Text.Trim();
                project.ProgrammeID = Utilities.MakeGuid(cbProgramme.SelectedItem.Value.Trim());
                if (tfDescription.HasValue())
                    project.Description = tfDescription.Text;
                if (tfUrl.HasValue())
                    project.Url = tfUrl.Text;
                else
                    project.Url = null;
                if (dfStartDate.HasValue())
                    project.StartDate = dfStartDate.SelectedDate;
                else
                    project.StartDate = null;
                if (dfEndDate.HasValue())
                    project.EndDate = dfEndDate.SelectedDate;
                else
                    project.EndDate = null;
                project.UserId = AuthHelper.GetLoggedInUserId;

                project.Save();
                Auditing.Log(GetType(), new MethodCallParameters { { "ID", project.Id }, { "Code", project.Code }, { "Name", project.Name } });

                ProjectsGrid.DataBind();

                DetailWindow.Hide();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to save project");
            }
        }
    }

    protected void ProjectsGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        //string js = BaseRepository.BuildExportQ("vProject", gridData, visCols, sortCol, sortDir);
        //BaseRepository.doExport(type, js);
        BaseRepository.Export("vProject", gridData, visCols, sortCol, sortDir, type, "Projects", Response);
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

    private bool StationLinkOk()
    {
        RowSelectionModel masterRow = ProjectsGrid.SelectionModel.Primary as RowSelectionModel;
        var masterID = new Guid(masterRow.SelectedRecordID);
        ProjectStationCollection col = new ProjectStationCollection()
            .Where(ProjectStation.Columns.ProjectID, masterID)
            .Where(ProjectStation.Columns.StationID, cbStation.SelectedItem.Value);
        if (!String.IsNullOrEmpty(dfStationStartDate.Text) && (dfStationStartDate.SelectedDate.Year >= 1900))
            col.Where(ProjectStation.Columns.StartDate, dfStationStartDate.SelectedDate);
        if (!String.IsNullOrEmpty(dfStationEndDate.Text) && (dfStationEndDate.SelectedDate.Year >= 1900))
            col.Where(ProjectStation.Columns.EndDate, dfStationEndDate.SelectedDate);
        col.Load();
        var id = Utilities.MakeGuid(ProjectStationLinkID.Value);
        return !col.Any(i => i.Id != id);
    }

    protected void StationLinkSave(object sender, DirectEventArgs e)
    {
        using (SAEONLogs.MethodCall(GetType()))
        {
            try
            {
                if (!StationLinkOk())
                {
                    MessageBoxes.Error("Error", "Station is already linked");
                    return;
                }
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
                Auditing.Log(GetType(), new MethodCallParameters {
                { "ProjectID", projectStation.ProjectID },
                { "ProjectCode", projectStation.Project.Name },
                { "StationID", projectStation.StationID},
                { "StationCode", projectStation.Station.Code},
                { "StartDate", projectStation?.StartDate },
                { "EndDate", projectStation?.EndDate}
            });
                StationLinksGrid.DataBind();
                StationLinkWindow.Hide();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to link station");
            }
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
        using (SAEONLogs.MethodCall(GetType(), new MethodCallParameters { { "ID", aID } }))
        {
            try
            {
                ProjectStation.Delete(aID);
                Auditing.Log(GetType(), new MethodCallParameters { { "ID", aID } });
                StationLinksGrid.DataBind();
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
                MessageBoxes.Error(ex, "Error", "Unable to delete station link");
            }
        }
    }

    [DirectMethod]
    public void AddStationClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Stations"));
    }
    #endregion
}