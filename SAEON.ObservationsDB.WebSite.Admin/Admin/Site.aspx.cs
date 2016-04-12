using Ext.Net;
using da = SAEON.ObservationsDB.Data;
using System;
using System.Linq;
using SubSonic;

public partial class Admin_Site : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            //this.StationStore.DataSource = new da.StationCollection().OrderByAsc(da.Station.Columns.Name).Load();
            //this.StationStore.DataBind();
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

        da.Site site = new da.Site();

        if (String.IsNullOrEmpty(tfID.Text))
            site.Id = Guid.NewGuid();
        else
            site = new da.Site(tfID.Text.Trim());

        site.Code = tfCode.Text.Trim();
        site.Name = tfName.Text.Trim();
        site.Description = tfDescription.Text.Trim();
        site.Url = tfUrl.Text.Trim();
        if (!String.IsNullOrEmpty(tfStartDate.Text) && (tfStartDate.SelectedDate.Year >= 1900))
            site.StartDate = tfStartDate.SelectedDate;
        if (!String.IsNullOrEmpty(tfEndDate.Text) && (tfEndDate.SelectedDate.Year >= 1900))
            site.EndDate = tfEndDate.SelectedDate;
        site.UserId = AuthHelper.GetLoggedInUserId;

        site.Save();

        SiteGrid.DataBind();

        this.DetailWindow.Hide();
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

        if (ActionType == "RemoveStation")
        {
            da.Station station = new da.StationCollection().Where(da.Station.Columns.Id, recordID).Load().FirstOrDefault();
            if (station != null)
            {
                station.SiteID = null;
                station.Save();
                StationGrid.DataBind();
            }
        }
        else if (ActionType == "RemoveOrganisation")
        {
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
                .NotIn(new Select(new string[] {da.Station.Columns.Id}).From(da.Station.Schema).Where(da.Station.IdColumn).IsEqualTo(Id))
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
    #endregion

    #region Organisations
    protected void OrganisationGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        //if (e.Parameters["SiteID"] != null && e.Parameters["SiteID"].ToString() != "-1")
        //{
        //    Guid Id = Guid.Parse(e.Parameters["SiteID"].ToString());
        //    da.OrganisationCollection OrganisationCol = new da.OrganisationCollection()
        //        .Where(da.Organisation.Columns.SiteID, Id)
        //        .OrderByAsc(da.Organisation.Columns.Code)
        //        .Load();
        //    this.OrganisationGrid.GetStore().DataSource = OrganisationCol;
        //    this.OrganisationGrid.GetStore().DataBind();
        //}
    }

    #endregion
}
