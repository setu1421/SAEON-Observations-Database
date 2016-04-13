using Ext.Net;
using da = SAEON.ObservationsDB.Data;
using System;
using System.Linq;
using SubSonic;
using Serilog;

public partial class Admin_Site : System.Web.UI.Page
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
    protected void SiteGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.SiteGrid.GetStore().DataSource = SiteRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
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
            site.Code = tfCode.Text.Trim();
            if (string.IsNullOrEmpty(site.Code)) site.Code = null;
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

            SiteGrid.DataBind();

            this.DetailWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Save");
        }
    }

    protected void SiteStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("Site", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    protected void DoDelete(object sender, DirectEventArgs e)
    {
        string ActionType = e.ExtraParams["type"];
        string recordID = e.ExtraParams["id"];
        try
        {
            if (ActionType == "RemoveStation")
            {
                da.Station station = new da.Station(recordID);
                if (station != null)
                {
                    station.SiteID = null;
                    station.UserId = AuthHelper.GetLoggedInUserId;
                    station.Save();
                    StationGrid.DataBind();
                }
            }
            else if (ActionType == "RemoveOrganisation")
            {
                new da.SiteOrganisationController().Delete(recordID);
                OrganisationGrid.DataBind();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DoDelete({ActionType},{RecordID})", ActionType, recordID);
        }
    }
    #endregion

    #region Stations

    protected void StationGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["SiteID"] != null && e.Parameters["SiteID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["SiteID"].ToString());
            da.StationCollection stationCol = new da.StationCollection()
                .Where(da.Station.Columns.SiteID, Id)
                .OrderByAsc(da.Station.Columns.Code)
                .Load();
            this.StationGrid.GetStore().DataSource = stationCol;
            this.StationGrid.GetStore().DataBind();
        }
    }

    protected void AvailableStationsStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["SiteID"] != null && e.Parameters["SiteID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["SiteID"].ToString());
            da.StationCollection stationCol = new Select()
                .From(da.Station.Schema)
                .Where(da.Station.IdColumn)
                .NotIn(new Select(new string[] { da.Station.Columns.Id }).From(da.Station.Schema).Where(da.Station.IdColumn).IsEqualTo(Id))
                .And(da.Station.SiteIDColumn)
                .IsNull()
                .OrderAsc(da.Station.Columns.Code)
                .ExecuteAsCollection<da.StationCollection>();
            this.AvailableStationsGrid.GetStore().DataSource = stationCol;
            this.AvailableStationsGrid.GetStore().DataBind();
        }
    }

    protected void AcceptStations_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel sm = this.AvailableStationsGrid.SelectionModel.Primary as RowSelectionModel;
            RowSelectionModel siteRow = this.SiteGrid.SelectionModel.Primary as RowSelectionModel;

            var siteID = siteRow.SelectedRecordID;
            if (sm.SelectedRows.Count > 0)
            {
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    da.Station station = new da.Station(row.RecordID);
                    if (station != null)
                    {
                        station.SiteID = new Guid(siteID);
                        station.UserId = AuthHelper.GetLoggedInUserId;
                        station.Save();
                    }
                }
                StationGrid.DataBind();
                AvailableStationsWindow.Hide();
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Invalid Selection",
                    Message = "Select at least one Station",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO
                });
            }

        }
        catch (Exception ex)
        {
            Log.Error(ex, "AcceptStations_Click");
        }
    }
    #endregion

    #region Organisations

    protected void OrganisationGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["SiteID"] != null && e.Parameters["SiteID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["SiteID"].ToString());
            da.VSiteOrganisationCollection SiteOrganisationCol = new da.VSiteOrganisationCollection()
                .Where(da.VSiteOrganisation.Columns.SiteID, Id)
                .OrderByAsc(da.VSiteOrganisation.Columns.StartDate)
                .OrderByAsc(da.VSiteOrganisation.Columns.EndDate)
                .OrderByAsc(da.VSiteOrganisation.Columns.OrganisationName)
                .OrderByAsc(da.VSiteOrganisation.Columns.OrganisationRoleName)
                .Load();
            this.OrganisationGrid.GetStore().DataSource = SiteOrganisationCol;
            this.OrganisationGrid.GetStore().DataBind();
        }
    }

    protected void AcceptOrganisation_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel siteRow = this.SiteGrid.SelectionModel.Primary as RowSelectionModel;
            var siteID = siteRow.SelectedRecordID;
            da.SiteOrganisation siteOrganisation = new da.SiteOrganisation();
            siteOrganisation.SiteID = new Guid(siteID);
            siteOrganisation.OrganisationID = new Guid(cbOrganisation.SelectedItem.Value.Trim());
            siteOrganisation.OrganisationRoleID = new Guid(cbOrganisationRole.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
                siteOrganisation.StartDate = dfOrganisationStartDate.SelectedDate;
            if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
                siteOrganisation.EndDate = dfOrganisationEndDate.SelectedDate;
            siteOrganisation.UserId = AuthHelper.GetLoggedInUserId;
            siteOrganisation.Save();
            OrganisationGrid.DataBind();
            OrganisationWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "AcceptOrganisation_Click");
        }
    }
    #endregion
}
