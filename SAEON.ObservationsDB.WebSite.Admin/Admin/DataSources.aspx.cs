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

public partial class Admin_DataSources : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!X.IsAjaxRequest)
        {
            DataSchemaStore.DataSource = new DataSchemaCollection().OrderByAsc(DataSchema.Columns.Name).Load();
            DataSchemaStore.DataBind();

            Dictionary<int, string> frequencylist = listHelper.GetUpdateFrequencyList();

            foreach (var item in frequencylist)
            {
                cbUpdateFrequency.Items.Insert(cbUpdateFrequency.Items.ToArray().Length, new Ext.Net.ListItem(item.Value, item.Key.ToString()));
            }

            OrganisationStore.DataSource = new OrganisationCollection().OrderByAsc(Organisation.Columns.Name).Load();
            OrganisationStore.DataBind();
            OrganisationRoleStore.DataSource = new OrganisationRoleCollection().OrderByAsc(OrganisationRole.Columns.Name).Load();
            OrganisationRoleStore.DataBind();

        }
    }

    #region DataSource
    protected void DataSourcesGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        DataSourcesGrid.GetStore().DataSource = DataSourceRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        DataSourceCollection col = new DataSourceCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = DataSource.Columns.Code;
            errorMessage = "The specified Data Source Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = DataSource.Columns.Name;
            errorMessage = "The specified Data Source Name already exists";

        }
        //else if (e.ID == "ContentPlaceHolder1_tfUrl")
        //{
        //    checkColumn = DataSource.Columns.Url;
        //    errorMessage = "Url is required when update frequency is not ad-hoc";
        //}
        //else if (e.ID == "ContentPlaceHolder1_cbUpdateFrequency")
        //{
        //    checkColumn = DataSource.Columns.Url;
        //    errorMessage = "Url and start date is required when update frequency is not ad-hoc";
        //}
        //else if (e.ID == "ContentPlaceHolder1_StartDate")
        //{
        //    checkColumn = DataSource.Columns.StartDate;
        //    errorMessage = "Start date is required when update frequency is not ad-hoc";
        //}


        if (e.ID == "tfCode" || e.ID == "tfName")
        {
            if (String.IsNullOrEmpty(tfID.Text.ToString()))
                col = new DataSourceCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            else
                col = new DataSourceCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(DataSource.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

            if (col.Count > 0)
            {
                e.Success = false;
                e.ErrorMessage = errorMessage;
            }
            else
                e.Success = true;
        }
        //else if (e.ID == "ContentPlaceHolder1_tfUrl")
        //{
        //    if (cbUpdateFrequency.SelectedItem.Text == "Ad-Hoc")
        //    {
        //        e.Success = true; 
        //    }
        //    else
        //    {
        //        if (tfUrl.Text.Length != 0)
        //        {
        //            e.Success = true; 
        //        }
        //        else
        //        {
        //            e.Success = false;
        //            e.ErrorMessage = errorMessage; 
        //        }
        //    }
        //}
        //else if (e.ID == "ContentPlaceHolder1_StartDate")
        //{
        //    if (cbUpdateFrequency.SelectedItem.Text == "Ad-Hoc")
        //    {
        //        e.Success = true;
        //    }
        //    else
        //    {
        //        if (StartDate.Text != "0001/01/01 00:00:00")
        //        {
        //            e.Success = true;
        //        }
        //        else
        //        {
        //            e.Success = false;
        //            e.ErrorMessage = errorMessage;
        //        }
        //    }
        //}
        //else if (e.ID == "ContentPlaceHolder1_cbUpdateFrequency")
        //{
        //    if (cbUpdateFrequency.SelectedItem.Text == "Ad-Hoc")
        //    {
        //        e.Success = true;
        //    }
        //    else
        //    {
        //        if (tfUrl.Text.Length != 0 && StartDate.Text != "0001/01/01 00:00:00")
        //        {
        //            e.Success = true;
        //        }
        //        else
        //        {
        //            e.Success = false;
        //            e.ErrorMessage = errorMessage;
        //        }
        //    }
        //}


    }

    protected void Save(object sender, DirectEventArgs e)
    {
        try
        {

            DataSource ds = new DataSource();

            if (String.IsNullOrEmpty(tfID.Text))
                ds.Id = Guid.NewGuid();
            else
                ds = new DataSource(tfID.Text.Trim());

            ds.Code = tfCode.Text.Trim();
            ds.Name = tfName.Text.Trim();
            ds.Description = tfDescription.Text.Trim();
            ds.Url = tfUrl.Text;
            //ds.DataSourceTypeID = new Guid(cbDataSourceType.SelectedItem.Value);
            //ds.DefaultNullValue = Int64.Parse(nfDefaultValue.Value.ToString());

            ds.UpdateFreq = int.Parse(cbUpdateFrequency.SelectedItem.Value);

            if (cbDataSchema.SelectedItem.Value != null)
            {
                //test if dataschema is valid (linked to test in datasource files)
                bool isValid = true;
                string sensorName = "";

                SensorCollection spCol = new SensorCollection()
                    .Where(Sensor.Columns.DataSourceID, ds.Id)
                    .Load();

                foreach (Sensor sp in spCol)
                {
                    if (sp.DataSchemaID != null)
                    {
                        isValid = false;
                        sensorName = sp.Name;
                    }
                }

                //
                if (isValid)
                {
                    ds.DataSchemaID = Guid.Parse(cbDataSchema.SelectedItem.Value);
                }
                else
                {
                    MessageBoxes.Error("This data source cant have a data schema because one of the sensors linked to it ({0}) is already linked to a data schema", sensorName);
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Invalid Data Source",
                        //Message = "The selected data schema is already linked to a sensor that is linked to this data source.",
                        Message = "This data source cant have a data schema because one of the sensors linked to it (" + sensorName + ") is already linked to a data schema",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR
                    });

                    return;
                }
            }
            else
                ds.DataSchemaID = null;


            if (!String.IsNullOrEmpty(dfStartDate.Text) && (dfStartDate.SelectedDate.Year >= 1900))
                ds.StartDate = dfStartDate.SelectedDate;
            if (!String.IsNullOrEmpty(dfEndDate.Text) && (dfEndDate.SelectedDate.Year >= 1900))
                ds.EndDate = dfEndDate.SelectedDate;
            ds.UserId = AuthHelper.GetLoggedInUserId;

            ds.Save();
            Auditing.Log("DataSources.Save", new Dictionary<string, object> { { "ID", ds.Id }, { "Code", ds.Code }, { "Name", ds.Name } });

            DataSourcesGrid.DataBind();

            DetailWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DataSources.Save");
            MessageBoxes.Error(ex, "Error", "Unable to save data source");
        }
    }

    protected void DataSourcesGridStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("VDataSource", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    #endregion

    #region Organisations

    protected void OrganisationLinksGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["DataSourceID"] != null && e.Parameters["DataSourceID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["DataSourceID"].ToString());
            VDataSourceOrganisationCollection col = new VDataSourceOrganisationCollection()
                .Where(VDataSourceOrganisation.Columns.DataSourceID, Id)
                .OrderByAsc(VDataSourceOrganisation.Columns.StartDate)
                .OrderByAsc(VDataSourceOrganisation.Columns.EndDate)
                .OrderByAsc(VDataSourceOrganisation.Columns.OrganisationName)
                .OrderByAsc(VDataSourceOrganisation.Columns.OrganisationRoleName)
                .Load();
            OrganisationLinksGrid.GetStore().DataSource = col;
            OrganisationLinksGrid.GetStore().DataBind();
        }
    }

    protected void LinkOrganisation_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = DataSourcesGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            DataSourceOrganisation dataSourceOrganisation = new DataSourceOrganisation(hID.Value);
            dataSourceOrganisation.DataSourceID = masterID;
            dataSourceOrganisation.OrganisationID = new Guid(cbOrganisation.SelectedItem.Value.Trim());
            dataSourceOrganisation.OrganisationRoleID = new Guid(cbOrganisationRole.SelectedItem.Value.Trim());
            if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
                dataSourceOrganisation.StartDate = dfOrganisationStartDate.SelectedDate;
            if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
                dataSourceOrganisation.EndDate = dfOrganisationEndDate.SelectedDate;
            dataSourceOrganisation.UserId = AuthHelper.GetLoggedInUserId;
            dataSourceOrganisation.Save();
            Auditing.Log("DataSources.AddOrganisationLink", new Dictionary<string, object> {
                { "DataSourceID", masterID },
                { "OrganisationID", dataSourceOrganisation.OrganisationID},
                { "OrganisationCode", dataSourceOrganisation.Organisation.Code},
                { "RoleID", dataSourceOrganisation.OrganisationRoleID },
                { "RoleCode", dataSourceOrganisation.OrganisationRole.Code},
            });
            OrganisationLinksGrid.DataBind();
            OrganisationLinkWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DataSources.LinkOrganisation_Click");
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
            new DataSourceOrganisationController().Delete(aID);
            Auditing.Log("DataSources.DeleteOrganisationLink", new Dictionary<string, object> { { "ID", aID } });
            OrganisationLinksGrid.DataBind();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DataSources.DeleteOrganisationLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete organisation link");
        }
    }

    //protected void OrganisationLink(object sender, DirectEventArgs e)
    //{
    //    string actionType = e.ExtraParams["type"];
    //    string recordID = e.ExtraParams["id"];
    //        try
    //        {
    //            if (actionType == "Edit")
    //            {
    //                OrganisationLinkFormPanel.SetValues(new DataSourceOrganisation(recordID));
    //                OrganisationLinkWindow.Show();
    //            }
    //            else if (actionType == "Delete") 
    //            {
    //                new DataSourceOrganisationController().Delete(recordID);
    //                Auditing.Log("DataSources.DeleteOrganisationLink", new Dictionary<string, object> { { "ID", recordID } });
    //                OrganisationLinksGrid.DataBind();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error(ex, "DataSources.OrganisationLink({ActionType},{RecordID})", actionType, recordID);
    //            MessageBoxes.Error(ex, "Unable to {0} organisation link", actionType);
    //        }
    //}
    #endregion

    #region Sensors
    protected void SensorsGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["DataSourceID"] != null && e.Parameters["DataSourceID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["DataSourceID"].ToString());
            VSensorCollection col = new VSensorCollection()
                .Where(VSensor.Columns.DataSourceID, Id)
                .OrderByAsc(VSensor.Columns.Code)
                .Load();
            SensorsGrid.GetStore().DataSource = col;
            SensorsGrid.GetStore().DataBind();
        }
    }

    protected void AvailableSensorsStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["DataSourceID"] != null && e.Parameters["DataSourceID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["DataSourceID"].ToString());
            SensorCollection col = new Select()
                .From(Sensor.Schema)
                .Where(Sensor.IdColumn)
                .NotIn(new Select(new string[] { Sensor.Columns.Id }).From(Sensor.Schema).Where(Sensor.IdColumn).IsEqualTo(Id))
                .And(Sensor.DataSourceIDColumn)
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
            RowSelectionModel masterRow = DataSourcesGrid.SelectionModel.Primary as RowSelectionModel;

            var masterID = new Guid(masterRow.SelectedRecordID);
            if (sm.SelectedRows.Count > 0)
            {
                foreach (SelectedRow row in sm.SelectedRows)
                {
                    Sensor sensor = new Sensor(row.RecordID);
                    if (sensor != null)
                    {
                        sensor.DataSourceID = masterID;
                        sensor.UserId = AuthHelper.GetLoggedInUserId;
                        sensor.Save();
                        Auditing.Log("DataSources.AddSensorLink", new Dictionary<string, object> {
                            { "DataSourceID", masterID }, { "ID", sensor.Id }, { "Code", sensor.Code }, { "Name", sensor.Name } });
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
            Log.Error(ex, "DataSources.LinkSensors_Click");
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
                sensor.DataSourceID = null;
                sensor.UserId = AuthHelper.GetLoggedInUserId;
                sensor.Save();
                Auditing.Log("DataSources.DeleteSensorLink", new Dictionary<string, object> {
                        { "ID", sensor.Id }, { "Code", sensor.Code }, { "Name", sensor.Name } });
                SensorsGrid.DataBind();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DataSources.DeleteSensorLink({aID})", aID);
            MessageBoxes.Error(ex, "Error", "Unable to delete data source link");
        }
    }

    #endregion
}