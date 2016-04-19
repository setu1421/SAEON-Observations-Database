using Ext.Net;
using da = SAEON.ObservationsDB.Data;
using System;
using System.Linq;
using SubSonic;
using Serilog;

public partial class Admin_StationV2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            SiteStore.DataSource = new da.SiteCollection().OrderByAsc(da.Site.Columns.Name).Load();
            SiteStore.DataBind();
            OrganisationStore.DataSource = new da.OrganisationCollection().OrderByAsc(da.Organisation.Columns.Name).Load();
            OrganisationStore.DataBind();
            OrganisationRoleStore.DataSource = new da.OrganisationRoleCollection().OrderByAsc(da.OrganisationRole.Columns.Name).Load();
            OrganisationRoleStore.DataBind();
        }
    }

    #region Stations
    protected void StationGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        StationGrid.GetStore().DataSource = StationRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }


    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        //da.StationCollection col = new da.StationCollection();

        //string checkColumn = String.Empty,
        //       errorMessage = String.Empty;

        //if (e.ID == "tfCode")
        //{
        //    checkColumn = da.Station.Columns.Code;
        //    errorMessage = "The specified Station Code already exists";
        //}
        //else if (e.ID == "tfName")
        //{
        //    checkColumn = da.Station.Columns.Name;
        //    errorMessage = "The specified Station Name already exists";

        //}

        //if (String.IsNullOrEmpty(tfID.Text.ToString()))
        //    col = new da.StationCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        //else
        //    col = new da.StationCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(da.Station.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

        //if (col.Count > 0)
        //{
        //    e.Success = false;
        //    e.ErrorMessage = errorMessage;
        //}
        //else
        //    e.Success = true;

    }

    protected void Save(object sender, DirectEventArgs e)
    {
        //try
        //{
        //    da.Station station = new da.Station();
        //    if (String.IsNullOrEmpty(tfID.Text))
        //        station.Id = Guid.NewGuid();
        //    else
        //        station = new da.Station(tfID.Text.Trim());
        //    station.Code = tfCode.Text.Trim();
        //    if (string.IsNullOrEmpty(station.Code)) station.Code = null;
        //    station.Name = tfName.Text.Trim();
        //    if (string.IsNullOrEmpty(station.Name)) station.Name = null;
        //    station.Description = tfDescription.Text.Trim();
        //    station.Url = tfUrl.Text.Trim();
        //    if (!String.IsNullOrEmpty(dfStartDate.Text) && (dfStartDate.SelectedDate.Year >= 1900))
        //        station.StartDate = dfStartDate.SelectedDate;
        //    if (!String.IsNullOrEmpty(dfEndDate.Text) && (dfEndDate.SelectedDate.Year >= 1900))
        //        station.EndDate = dfEndDate.SelectedDate;
        //    station.UserId = AuthHelper.GetLoggedInUserId;

        //    station.Save();

        //    StationGrid.DataBind();

        //    DetailWindow.Hide();
        //}
        //catch (Exception ex)
        //{
        //    Log.Error(ex, "Save");
        //}
    }

    protected void StationStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        //string type = FormatType.Text;
        //string visCols = VisCols.Value.ToString();
        //string gridData = GridData.Text;
        //string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        //string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        //string js = BaseRepository.BuildExportQ("Station", gridData, visCols, sortCol, sortDir);

        //BaseRepository.doExport(type, js);
    }

    protected void DoDelete(object sender, DirectEventArgs e)
    {
        //string ActionType = e.ExtraParams["type"];
        //string recordID = e.ExtraParams["id"];
        //try
        //{
        //    if (ActionType == "RemoveStation")
        //    {
        //        da.Station station = new da.Station(recordID);
        //        if (station != null)
        //        {
        //            station.SiteID = null;
        //            station.UserId = AuthHelper.GetLoggedInUserId;
        //            station.Save();
        //            StationGrid.DataBind();
        //        }
        //    }
        //    else if (ActionType == "RemoveOrganisation")
        //    {
        //        new da.SiteOrganisationController().Delete(recordID);
        //        OrganisationGrid.DataBind();
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Log.Error(ex, "DoDelete({ActionType},{RecordID})", ActionType, recordID);
        //}
    }
    #endregion

    #region Instruments

    protected void InstrumentGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        //if (e.Parameters["StationID"] != null && e.Parameters["StationID"].ToString() != "-1")
        //{
        //    Guid Id = Guid.Parse(e.Parameters["StationID"].ToString());
        //    da.DataSourceCollection col = new da.DataSourceCollection()
        //        .Where(da.DataSource.Columns.StationID, Id)
        //        .OrderByAsc(da.DataSource.Columns.Code)
        //        .Load();
        //    InstrumentGrid.GetStore().DataSource = col;
        //    InstrumentGrid.GetStore().DataBind();
        //}
    }

    protected void AvailableInstrumentsStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        //if (e.Parameters["StationID"] != null && e.Parameters["StationID"].ToString() != "-1")
        //{
        //    Guid Id = Guid.Parse(e.Parameters["StationID"].ToString());
        //    da.DataSourceCollection col = new Select()
        //        .From(da.DataSource.Schema)
        //        .Where(da.DataSource.IdColumn)
        //        .NotIn(new Select(new string[] { da.DataSource.Columns.Id }).From(da.DataSource.Schema).Where(da.DataSource.IdColumn).IsEqualTo(Id))
        //        .And(da.DataSource.StationIDColumn)
        //        .IsNull()
        //        .OrderAsc(da.DataSource.Columns.Code)
        //        .ExecuteAsCollection<da.DataSourceCollection>();
        //    AvailableInstrumentsGrid.GetStore().DataSource = col;
        //    AvailableInstrumentsGrid.GetStore().DataBind();
        //}
    }

    protected void AcceptInstruments_Click(object sender, DirectEventArgs e)
    {
        //try
        //{
        //    RowSelectionModel sm = AvailableInstrumentsGrid.SelectionModel.Primary as RowSelectionModel;
        //    RowSelectionModel sr = InstrumentGrid.SelectionModel.Primary as RowSelectionModel;

        //    var id = sr.SelectedRecordID;
        //    if (sm.SelectedRows.Count > 0)
        //    {
        //        foreach (SelectedRow row in sm.SelectedRows)
        //        {
        //            da.DataSource dataSource = new da.DataSource(row.RecordID);
        //            if (dataSource != null)
        //            {
        //                dataSource.StationID = new Guid(id);
        //                dataSource.UserId = AuthHelper.GetLoggedInUserId;
        //                dataSource.Save();
        //            }
        //        }
        //        InstrumentGrid.DataBind();
        //        AvailableInstrumentsWindow.Hide();
        //    }
        //    else
        //    {
        //        X.Msg.Show(new MessageBoxConfig
        //        {
        //            Title = "Invalid Selection",
        //            Message = "Select at least one Instrument",
        //            Buttons = MessageBox.Button.OK,
        //            Icon = MessageBox.Icon.INFO
        //        });
        //    }

        //}
        //catch (Exception ex)
        //{
        //    Log.Error(ex, "AcceptInstruments_Click");
        //}
    }
    #endregion

    #region Organisations

    protected void OrganisationGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        //if (e.Parameters["StationID"] != null && e.Parameters["StationID"].ToString() != "-1")
        //{
        //    Guid Id = Guid.Parse(e.Parameters["StationID"].ToString());
        //    da.VStationOrganisationCollection StationOrganisationCol = new da.VStationOrganisationCollection()
        //        .Where(da.VStationOrganisation.Columns.StationID, Id)
        //        .OrderByAsc(da.VStationOrganisation.Columns.StartDate)
        //        .OrderByAsc(da.VStationOrganisation.Columns.EndDate)
        //        .OrderByAsc(da.VStationOrganisation.Columns.OrganisationName)
        //        .OrderByAsc(da.VStationOrganisation.Columns.OrganisationRoleName)
        //        .Load();
        //    OrganisationGrid.GetStore().DataSource = StationOrganisationCol;
        //    OrganisationGrid.GetStore().DataBind();
        //}
    }

    protected void AcceptOrganisation_Click(object sender, DirectEventArgs e)
    {
        //try
        //{
        //    RowSelectionModel stationRow = StationGrid.SelectionModel.Primary as RowSelectionModel;
        //    var stationID = stationRow.SelectedRecordID;
        //    da.StationOrganisation stationOrganisation = new da.StationOrganisation();
        //    stationOrganisation.StationID = new Guid(stationID);
        //    stationOrganisation.OrganisationID = new Guid(cbOrganisation.SelectedItem.Value.Trim());
        //    stationOrganisation.OrganisationRoleID = new Guid(cbOrganisationRole.SelectedItem.Value.Trim());
        //    if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
        //        stationOrganisation.StartDate = dfOrganisationStartDate.SelectedDate;
        //    if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
        //        stationOrganisation.EndDate = dfOrganisationEndDate.SelectedDate;
        //    stationOrganisation.UserId = AuthHelper.GetLoggedInUserId;
        //    stationOrganisation.Save();
        //    OrganisationGrid.DataBind();
        //    OrganisationWindow.Hide();
        //}
        //catch (Exception ex)
        //{
        //    Log.Error(ex, "AcceptOrganisation_Click");
        //}
    }
    #endregion
}