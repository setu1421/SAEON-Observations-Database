﻿using Ext.Net;
using da = SAEON.ObservationsDB.Data;
using System;
using System.Linq;
using SubSonic;
using Serilog;
using System.Collections.Generic;

public partial class Admin_Sites : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            OrganisationStore.DataSource = new da.OrganisationCollection().OrderByAsc(da.Organisation.Columns.Name).Load();
            OrganisationStore.DataBind();
            OrganisationRoleStore.DataSource = new da.OrganisationRoleCollection().OrderByAsc(da.OrganisationRole.Columns.Name).Load();
            OrganisationRoleStore.DataBind();
            ProjectStore.DataSource = new da.ProjectCollection().OrderByAsc(da.Project.Columns.Name).Load();
            ProjectStore.DataBind();
            StationStore.DataSource = new da.StationCollection().OrderByAsc(da.Station.Columns.Name).Load();
            StationStore.DataBind();
        }
    }

    #region Sites
    protected void SitesGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        SitesGrid.GetStore().DataSource = SiteRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }


    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        da.SiteCollection col = new da.SiteCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = da.Site.Columns.Code;
            errorMessage = "The specified Site Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = da.Site.Columns.Name;
            errorMessage = "The specified Site Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new da.SiteCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new da.SiteCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(da.Site.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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
            da.Site site = new da.Site();
            if (String.IsNullOrEmpty(tfID.Text))
                site.Id = Guid.NewGuid();
            else
                site = new da.Site(tfID.Text.Trim());
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
            Auditing.Log("Sites.Save", new Dictionary<string, object> { { "ID", site.Id }, { "Code", site.Code }, { "Name", site.Name } });

            SitesGrid.DataBind();

            DetailWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sites.Save");
            MessageBoxes.Error(ex, "Error", "Unable to save site");
        }
    }

    protected void SitesGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("Site", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    #endregion

    #region Organisations

    protected void OrganisationLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["SiteID"] != null && e.Parameters["SiteID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["SiteID"].ToString());
            da.VSiteOrganisationCollection col = new da.VSiteOrganisationCollection()
                .Where(da.VSiteOrganisation.Columns.SiteID, Id)
                .OrderByAsc(da.VSiteOrganisation.Columns.StartDate)
                .OrderByAsc(da.VSiteOrganisation.Columns.EndDate)
                .OrderByAsc(da.VSiteOrganisation.Columns.OrganisationName)
                .OrderByAsc(da.VSiteOrganisation.Columns.OrganisationRoleName)
                .Load();
            OrganisationLinksGrid.GetStore().DataSource = col;
            OrganisationLinksGrid.GetStore().DataBind();
        }
    }

    protected void LinkOrganisation_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = SitesGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            da.SiteOrganisation siteOrganisation = new da.SiteOrganisation(Utilities.MakeGuid(OrganisationLinkID.Value));
            siteOrganisation.SiteID = masterID;
            siteOrganisation.OrganisationID = new Guid(cbOrganisation.SelectedItem.Value.Trim());
            siteOrganisation.OrganisationRoleID = new Guid(cbOrganisationRole.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
                siteOrganisation.StartDate = dfOrganisationStartDate.SelectedDate;
            else
                siteOrganisation.StartDate = null;
            if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
                siteOrganisation.EndDate = dfOrganisationEndDate.SelectedDate;
            else
                siteOrganisation.EndDate = null;
            siteOrganisation.UserId = AuthHelper.GetLoggedInUserId;
            siteOrganisation.Save();
            Auditing.Log("Sites.AddOrganisationLink", new Dictionary<string, object> {
                { "SiteID", siteOrganisation.SiteID },
                { "SiteCode", siteOrganisation.Site.Code },
                { "OrganisationID", siteOrganisation.OrganisationID},
                { "OrganisationCode", siteOrganisation.Organisation.Code},
                { "RoleID", siteOrganisation.OrganisationRoleID },
                { "RoleCode", siteOrganisation.OrganisationRole.Code},
                { "StartDate", siteOrganisation.StartDate },
                { "EndDate", siteOrganisation.EndDate}
            });
            OrganisationLinksGrid.DataBind();
            OrganisationLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sites.LinkOrganisation_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link organisation");
        }
    }

    [DirectMethod]
    public void ConfirmDeleteOrganisationLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteOrganisationLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this organisation link?");
    }

    [DirectMethod]
    public void DeleteOrganisationLink(Guid aID)
    {
        try
        {
            new da.SiteOrganisationController().Delete(aID);
            Auditing.Log("Sites.DeleteOrganisationLink", new Dictionary<string, object> { { "ID", aID } });
            OrganisationLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sites.DeleteOrganisationLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete organisation link");
        }
    }

    [DirectMethod]
    public void AddOrganisationClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion

    #region Projects

    protected void ProjectLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["SiteID"] != null && e.Parameters["SiteID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["SiteID"].ToString());
            da.VSiteProjectCollection col = new da.VSiteProjectCollection()
                .Where(da.VSiteProject.Columns.SiteID, Id)
                .OrderByAsc(da.VSiteProject.Columns.StartDate)
                .OrderByAsc(da.VSiteProject.Columns.EndDate)
                .OrderByAsc(da.VSiteProject.Columns.ProjectName)
                .Load();
            ProjectLinksGrid.GetStore().DataSource = col;
            ProjectLinksGrid.GetStore().DataBind();
        }
    }

    protected void LinkProject_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = SitesGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            da.SiteProject siteProject = new da.SiteProject(Utilities.MakeGuid(ProjectLinkID.Value));
            siteProject.SiteID = masterID;
            siteProject.ProjectID = new Guid(cbProject.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfProjectStartDate.Text) && (dfProjectStartDate.SelectedDate.Year >= 1900))
                siteProject.StartDate = dfProjectStartDate.SelectedDate;
            else
                siteProject.StartDate = null;
            if (!String.IsNullOrEmpty(dfProjectEndDate.Text) && (dfProjectEndDate.SelectedDate.Year >= 1900))
                siteProject.EndDate = dfProjectEndDate.SelectedDate;
            else
                siteProject.EndDate = null;
            siteProject.UserId = AuthHelper.GetLoggedInUserId;
            siteProject.Save();
            Auditing.Log("Sites.AddProjectLink", new Dictionary<string, object> {
                { "SiteID", siteProject.SiteID },
                { "SiteCode", siteProject.Site.Code },
                { "ProjectID", siteProject.ProjectID},
                { "ProjectCode", siteProject.Project.Code},
                { "StartDate", siteProject.StartDate },
                { "EndDate", siteProject.EndDate}
            });
            ProjectLinksGrid.DataBind();
            ProjectLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sites.LinkProject_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link Project");
        }
    }

    [DirectMethod]
    public void ConfirmDeleteProjectLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteProjectLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this Project link?");
    }

    [DirectMethod]
    public void DeleteProjectLink(Guid aID)
    {
        try
        {
            new da.SiteProjectController().Delete(aID);
            Auditing.Log("Sites.DeleteProjectLink", new Dictionary<string, object> { { "ID", aID } });
            ProjectLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sites.DeleteProjectLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete Project link");
        }
    }

    [DirectMethod]
    public void AddProjectClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion

    #region Stations

    protected void StationLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["SiteID"] != null && e.Parameters["SiteID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["SiteID"].ToString());
            da.VSiteStationCollection col = new da.VSiteStationCollection()
                .Where(da.VSiteStation.Columns.SiteID, Id)
                .OrderByAsc(da.VSiteStation.Columns.StartDate)
                .OrderByAsc(da.VSiteStation.Columns.EndDate)
                .OrderByAsc(da.VSiteStation.Columns.SiteName)
                .Load();
            StationLinksGrid.GetStore().DataSource = col;
            StationLinksGrid.GetStore().DataBind();
        }
    }

    protected void LinkStation_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = SitesGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            da.SiteStation siteStation = new da.SiteStation(Utilities.MakeGuid(StationLinkID.Value));
            siteStation.SiteID = masterID;
            siteStation.StationID = new Guid(cbStation.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfStationStartDate.Text) && (dfStationStartDate.SelectedDate.Year >= 1900))
                siteStation.StartDate = dfStationStartDate.SelectedDate;
            else
                siteStation.StartDate = null;
            if (!String.IsNullOrEmpty(dfStationEndDate.Text) && (dfStationEndDate.SelectedDate.Year >= 1900))
                siteStation.EndDate = dfStationEndDate.SelectedDate;
            else
                siteStation.EndDate = null;
            siteStation.UserId = AuthHelper.GetLoggedInUserId;
            siteStation.Save();
            Auditing.Log("Sites.AddStationLink", new Dictionary<string, object> {
                { "SiteID", siteStation.SiteID},
                { "SiteCode", siteStation.Site.Code},
                { "StationID", siteStation.StationID },
                { "StationCode", siteStation.Station.Name },
                { "StartDate", siteStation.StartDate },
                { "EndDate", siteStation.EndDate}
            });
            StationLinksGrid.DataBind();
            StationLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sites.LinkStation_Click");
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
            new da.SiteStationController().Delete(aID);
            Auditing.Log("Sites.DeleteStationLink", new Dictionary<string, object> { { "ID", aID } });
            StationLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sites.DeleteStationLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete station link");
        }
    }

    [DirectMethod]
    public void AddStationClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }

    #endregion

}
