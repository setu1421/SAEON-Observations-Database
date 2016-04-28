using Ext.Net;
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
            if (!String.IsNullOrEmpty(dfEndDate.Text) && (dfEndDate.SelectedDate.Year >= 1900))
                site.EndDate = dfEndDate.SelectedDate;
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

    #region Stations

    protected void StationsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["SiteID"] != null && e.Parameters["SiteID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["SiteID"].ToString());
            da.StationCollection col = new da.StationCollection()
                .Where(da.Station.Columns.SiteID, Id)
                .OrderByAsc(da.Station.Columns.Code)
                .Load();
            StationsGrid.GetStore().DataSource = col;
            StationsGrid.GetStore().DataBind();
        }
    }

    protected void AvailableStationsStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["SiteID"] != null && e.Parameters["SiteID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["SiteID"].ToString());
            da.StationCollection col = new Select()
                .From(da.Station.Schema)
                .Where(da.Station.IdColumn)
                .NotIn(new Select(new string[] { da.Station.Columns.Id }).From(da.Station.Schema).Where(da.Station.IdColumn).IsEqualTo(Id))
                .And(da.Station.SiteIDColumn)
                .IsNull()
                .OrderAsc(da.Station.Columns.Code)
                .ExecuteAsCollection<da.StationCollection>();
            AvailableStationsGrid.GetStore().DataSource = col;
            AvailableStationsGrid.GetStore().DataBind();
        }
    }

    protected void LinkStations_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel sm = AvailableStationsGrid.SelectionModel.Primary as RowSelectionModel;
            RowSelectionModel masteRow = SitesGrid.SelectionModel.Primary as RowSelectionModel;

            var masterID = new Guid(masteRow.SelectedRecordID);
            if (sm.SelectedRows.Count > 0)
            {
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    da.Station station = new da.Station(row.RecordID);
                    if (station != null)
                    {
                        station.SiteID = masterID;
                        station.UserId = AuthHelper.GetLoggedInUserId;
                        station.Save();
                        Auditing.Log("Sites.AddStationLink", new Dictionary<string, object> {
                            { "SiteID", masterID }, { "ID", station.Id }, { "Code", station.Code }, { "Name", station.Name } });
                    }
                }
                StationsGrid.DataBind();
                AvailableStationsWindow.Hide();
            }
            else
            {
                MessageBoxes.Info("Invalid Selection", "Select at least one station");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sites.LinkStations_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link stations");
        }
    }

    protected void StationLink(object sender, DirectEventArgs e)
    {
        string actionType = e.ExtraParams["type"];
        string recordID = e.ExtraParams["id"];
        try
        {
            if (actionType == "Edit")
            {
            }
            else if (actionType == "Delete")
            {
                da.Station station = new da.Station(recordID);
                if (station != null)
                {
                    station.SiteID = null;
                    station.UserId = AuthHelper.GetLoggedInUserId;
                    station.Save();
                    Auditing.Log("Sites.DeleteStationLink", new Dictionary<string, object> {
                        { "ID", station.Id }, { "Code", station.Code }, { "Name", station.Name } });
                    StationsGrid.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sites.StationLink({ActionType},{RecordID})", actionType, recordID);
            MessageBoxes.Error(ex, "Unable to {0} station link", actionType);
        }
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
            da.SiteOrganisation siteOrganisation = new da.SiteOrganisation(hID.Value);
            siteOrganisation.SiteID = masterID;
            siteOrganisation.OrganisationID = new Guid(cbOrganisation.SelectedItem.Value.Trim());
            siteOrganisation.OrganisationRoleID = new Guid(cbOrganisationRole.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
                siteOrganisation.StartDate = dfOrganisationStartDate.SelectedDate;
            if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
                siteOrganisation.EndDate = dfOrganisationEndDate.SelectedDate;
            siteOrganisation.UserId = AuthHelper.GetLoggedInUserId;
            siteOrganisation.Save();
            Auditing.Log("Sites.AddOrganisationLink", new Dictionary<string, object> {
                { "SiteID", masterID },
                { "OrganisationID", siteOrganisation.OrganisationID},
                { "OrganisationCode", siteOrganisation.Organisation.Code},
                { "RoleID", siteOrganisation.OrganisationRoleID },
                { "RoleCode", siteOrganisation.OrganisationRole.Code},
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

    protected void OrganisationLink(object sender, DirectEventArgs e)
    {
        string actionType = e.ExtraParams["type"];
        string recordID = e.ExtraParams["id"];
        try
        {
            if (actionType == "Edit")
            {
                OrganisationLinkFormPanel.SetValues(new da.SiteOrganisation(recordID));
                OrganisationLinkWindow.Show();
            }
            else if (actionType == "Delete")
            {
                new da.SiteOrganisationController().Delete(recordID);
                Auditing.Log("Sites.DeleteOrganisationLink", new Dictionary<string, object> { { "ID", recordID } });
                OrganisationLinksGrid.DataBind();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Sites.OrganisationLink({ActionType},{RecordID})", actionType, recordID);
            MessageBoxes.Error(ex, "Error", "Unable to {0} organisation link", actionType);
        }
    }

    #endregion
}
