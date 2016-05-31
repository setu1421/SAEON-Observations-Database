using Ext.Net;
using SAEON.Observations.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_Organisations : System.Web.UI.Page
{
    #region Organisations
    protected void OrganisationStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        OrganisationsGrid.GetStore().DataSource = OrganisationRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        OrganisationCollection col = new OrganisationCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = Organisation.Columns.Code;
            errorMessage = "The specified Organisation Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = Organisation.Columns.Name;
            errorMessage = "The specified Organisation Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new OrganisationCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new OrganisationCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Organisation.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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

        Organisation org = new Organisation();

        if (String.IsNullOrEmpty(tfID.Text))
            org.Id = Guid.NewGuid();
        else
            org = new Organisation(tfID.Text.Trim());

        org.Code = tfCode.Text.Trim();
        org.Name = tfName.Text.Trim();
        org.Description = tfDescription.Text.Trim();

        org.UserId = AuthHelper.GetLoggedInUserId;

        org.Save();

        OrganisationsGrid.DataBind();

        this.DetailWindow.Hide();
    }

    protected void OrganisationStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("Organisation", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }
    #endregion

    #region Sites
    protected void SiteLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["OrganisationID"] != null && e.Parameters["OrganisationID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["OrganisationID"].ToString());
            VOrganisationSiteCollection col = new VOrganisationSiteCollection()
                .Where(VOrganisationSite.Columns.OrganisationID, Id)
                .OrderByAsc(VOrganisationSite.Columns.StartDate)
                .OrderByAsc(VOrganisationSite.Columns.EndDate)
                .OrderByAsc(VOrganisationSite.Columns.SiteName)
                .OrderByAsc(VOrganisationSite.Columns.OrganisationRoleName)
                .Load();
            SiteLinksGrid.GetStore().DataSource = col;
            SiteLinksGrid.GetStore().DataBind();
        }
    }
    #endregion

    #region Stations
    protected void StationLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["OrganisationID"] != null && e.Parameters["OrganisationID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["OrganisationID"].ToString());
            VOrganisationStationCollection col = new VOrganisationStationCollection()
                .Where(VOrganisationStation.Columns.OrganisationID, Id)
                .OrderByAsc(VOrganisationStation.Columns.StartDate)
                .OrderByAsc(VOrganisationStation.Columns.EndDate)
                .OrderByAsc(VOrganisationStation.Columns.StationName)
                .OrderByAsc(VOrganisationStation.Columns.OrganisationRoleName)
                .Load();
            StationLinksGrid.GetStore().DataSource = col;
            StationLinksGrid.GetStore().DataBind();
        }
    }
    #endregion

    #region Instruments
    protected void InstrumentLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["OrganisationID"] != null && e.Parameters["OrganisationID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["OrganisationID"].ToString());
            VOrganisationInstrumentCollection col = new VOrganisationInstrumentCollection()
                .Where(VOrganisationInstrument.Columns.OrganisationID, Id)
                .OrderByAsc(VOrganisationInstrument.Columns.StartDate)
                .OrderByAsc(VOrganisationInstrument.Columns.EndDate)
                .OrderByAsc(VOrganisationInstrument.Columns.InstrumentName)
                .OrderByAsc(VOrganisationInstrument.Columns.OrganisationRoleName)
                .Load();
            InstrumentLinksGrid.GetStore().DataSource = col;
            InstrumentLinksGrid.GetStore().DataBind();
        }
    }
    #endregion
}