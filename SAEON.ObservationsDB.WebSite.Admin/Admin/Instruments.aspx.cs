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

public partial class Admin_Instruments : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            StationStore.DataSource = new StationCollection().OrderByAsc(Station.Columns.Name).Load();
            StationStore.DataBind();

            OrganisationStore.DataSource = new OrganisationCollection().OrderByAsc(Organisation.Columns.Name).Load();
            OrganisationStore.DataBind();
            OrganisationRoleStore.DataSource = new OrganisationRoleCollection().OrderByAsc(OrganisationRole.Columns.Name).Load();
            OrganisationRoleStore.DataBind();
        }
    }

    #region Instrument
    protected void InstrumentsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        InstrumentsGrid.GetStore().DataSource = InstrumentRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        InstrumentCollection col = new InstrumentCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = Instrument.Columns.Code;
            errorMessage = "The specified Instrument Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = Instrument.Columns.Name;
            errorMessage = "The specified Instrument Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new InstrumentCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new InstrumentCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Instrument.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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
            Instrument instrument = new Instrument();

            if (String.IsNullOrEmpty(tfID.Text))
                instrument.Id = Guid.NewGuid();
            else
                instrument = new Instrument(tfID.Text.Trim());

            if (!string.IsNullOrEmpty(tfCode.Text.Trim()))
                instrument.Code = tfCode.Text.Trim();
            if (!string.IsNullOrEmpty(tfName.Text.Trim()))
                instrument.Name = tfName.Text.Trim();
            instrument.Description = tfDescription.Text.Trim();
            instrument.StationID = new Guid(cbStation.SelectedItem.Value.Trim());

            if (!string.IsNullOrEmpty(tfUrl.Text))
                instrument.Url = tfUrl.Text;

            if (!String.IsNullOrEmpty(dfStartDate.Text) && (dfStartDate.SelectedDate.Year >= 1900))
                instrument.StartDate = dfStartDate.SelectedDate;
            else
                instrument.StartDate = null;
            if (!String.IsNullOrEmpty(dfEndDate.Text) && (dfEndDate.SelectedDate.Year >= 1900))
                instrument.EndDate = dfEndDate.SelectedDate;
            else
                instrument.EndDate = null;

            instrument.UserId = AuthHelper.GetLoggedInUserId;

            instrument.Save();
            Auditing.Log("Instruments.Save", new Dictionary<string, object> {
                { "ID", instrument.Id }, { "Code", instrument.Code }, { "Name", instrument.Name } });

            InstrumentsGrid.DataBind();

            DetailWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Instruments.Save");
            MessageBoxes.Error(ex, "Error", "Unable to save station");
        }
    }

    protected void InstrumentsGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("VInstrument", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    #endregion

    #region Organisations

    protected void OrganisationLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["InstrumentID"] != null && e.Parameters["InstrumentID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["InstrumentID"].ToString());
            VInstrumentOrganisationCollection col = new VInstrumentOrganisationCollection()
                .Where(VInstrumentOrganisation.Columns.InstrumentID, Id)
                .OrderByAsc(VInstrumentOrganisation.Columns.StartDate)
                .OrderByAsc(VInstrumentOrganisation.Columns.EndDate)
                .OrderByAsc(VInstrumentOrganisation.Columns.OrganisationName)
                .OrderByAsc(VInstrumentOrganisation.Columns.OrganisationRoleName)
                .Load();
            OrganisationLinksGrid.GetStore().DataSource = col;
            OrganisationLinksGrid.GetStore().DataBind();
        }
    }

    protected void LinkOrganisation_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = InstrumentsGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            InstrumentOrganisation instrumentOrganisation = new InstrumentOrganisation(Utilities.MakeGuid(OrganisationLinkID.Value));
            instrumentOrganisation.InstrumentID = masterID;
            instrumentOrganisation.OrganisationID = new Guid(cbOrganisation.SelectedItem.Value.Trim());
            instrumentOrganisation.OrganisationRoleID = new Guid(cbOrganisationRole.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
                instrumentOrganisation.StartDate = dfOrganisationStartDate.SelectedDate;
            else
                instrumentOrganisation.StartDate = null;
            if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
                instrumentOrganisation.EndDate = dfOrganisationEndDate.SelectedDate;
            else
                instrumentOrganisation.EndDate = null;
            instrumentOrganisation.UserId = AuthHelper.GetLoggedInUserId;
            instrumentOrganisation.Save();
            Auditing.Log("Instruments.AddOrganisationLink", new Dictionary<string, object> {
                { "InstrumentID", instrumentOrganisation.InstrumentID },
                { "InstrumentCode", instrumentOrganisation.Instrument.Code },
                { "OrganisationID", instrumentOrganisation.OrganisationID},
                { "OrganisationCode", instrumentOrganisation.Organisation.Code},
                { "RoleID", instrumentOrganisation.OrganisationRoleID },
                { "RoleCode", instrumentOrganisation.OrganisationRole.Code},
                { "StartDate", instrumentOrganisation.StartDate },
                { "EndDate", instrumentOrganisation.EndDate}
            });
            OrganisationLinksGrid.DataBind();
            OrganisationLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Instruments.LinkOrganisation_Click");
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
            new InstrumentOrganisationController().Delete(aID);
            Auditing.Log("Instruments.DeleteOrganisationLink", new Dictionary<string, object> { { "ID", aID } });
            OrganisationLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Instruments.DeleteOrganisationLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete organisation link");
        }
    }

    [DirectMethod]
    public void AddOrganisationClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion

    #region Sensors
    protected void SensorsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["InstrumentID"] != null && e.Parameters["InstrumentID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["InstrumentID"].ToString());
            try
            {
                SensorCollection col = new SensorCollection()
                    .Where(Sensor.Columns.InstrumentID, Id)
                    .OrderByAsc(Sensor.Columns.Name)
                    .Load();
                SensorsGrid.GetStore().DataSource = col;
                SensorsGrid.GetStore().DataBind();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Instruments.SensorsGridStore_RefreshData");
                MessageBoxes.Error(ex, "Error", "Unable to refresh sensors grid");
            }
        }
    }

    protected void AvailableSensorsStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["InstrumentID"] != null && e.Parameters["InstrumentID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["InstrumentID"].ToString());
            SensorCollection col = new Select()
                .From(Sensor.Schema)
                .Where(Sensor.IdColumn)
                .NotIn(new Select(new string[] { Sensor.Columns.Id }).From(Sensor.Schema).Where(Sensor.IdColumn).IsEqualTo(Id))
                .And(Sensor.InstrumentIDColumn)
                .IsNull()
                .OrderAsc(Sensor.Columns.Code)
                .ExecuteAsCollection<SensorCollection>();
            AvailableSensorsGrid.GetStore().DataSource = col;
            AvailableSensorsGrid.GetStore().DataBind();
        }
    }

    protected void LinkSensors_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel sm = AvailableSensorsGrid.SelectionModel.Primary as RowSelectionModel;
            RowSelectionModel masterRow = InstrumentsGrid.SelectionModel.Primary as RowSelectionModel;

            var masterID = new Guid(masterRow.SelectedRecordID);
            if (sm.SelectedRows.Count > 0)
            {
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    Sensor sensor = new Sensor(row.RecordID);
                    if (sensor != null)
                    {
                        sensor.InstrumentID = masterID;
                        sensor.UserId = AuthHelper.GetLoggedInUserId;
                        sensor.Save();
                        Auditing.Log("Instruments.AddSensorLink", new Dictionary<string, object> {
                            { "InstrumentID", masterID }, { "ID", sensor.Id }, { "Code", sensor.Code }, { "Name", sensor.Name } });
                    }
                }
                SensorsGrid.DataBind();
                AvailableSensorsWindow.Hide();
            }
            else
            {
                MessageBoxes.Info("Invalid Selection", "Select at least one data source");
            }

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Instruments.LinkSensors_Click");
            MessageBoxes.Error(ex, "Error", "Unable to link sensors");
        }
    }

    [DirectMethod]
    public void ConfirmDeleteSensorLink(Guid aID)
    {
        MessageBoxes.Confirm(
            "Confirm Delete",
            String.Format("DirectCall.DeleteSensorLink(\"{0}\",{{ eventMask: {{ showMask: true}}}});", aID.ToString()),
            "Are you sure you want to delete this sensor link?");
    }

    [DirectMethod]
    public void DeleteSensorLink(Guid aID)
    {
        try
        {
            Sensor sensor = new Sensor(aID);
            if (sensor != null)
            {
                sensor.InstrumentID = null;
                sensor.UserId = AuthHelper.GetLoggedInUserId;
                sensor.Save();
                Auditing.Log("Instruments.DeleteSensorLink", new Dictionary<string, object> {
                        { "ID", sensor.Id }, { "Code", sensor.Code }, { "Name", sensor.Name } });
                SensorsGrid.DataBind();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Instruments.DeleteSensorLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete data source link");
        }
    }

    [DirectMethod]
    public void AddSensorClick(object sender, DirectEventArgs e)
    {
        //X.Redirect(X.ResourceManager.ResolveUrl("Admin/Sites"));
    }
    #endregion
}