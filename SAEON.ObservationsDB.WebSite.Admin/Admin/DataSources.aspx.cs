using Ext.Net;
using SAEON.ObservationsDB.Data;
using Serilog;
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
    protected void DataSourceStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        DataSourceGrid.GetStore().DataSource = DataSourceRepository.GetPagedList(e, e.Parameters[GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        DataSourceCollection col = new DataSourceCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = DataSource.Columns.Code;
            errorMessage = "The specified DataSource Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = DataSource.Columns.Name;
            errorMessage = "The specified DataSource Name already exists";

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

                SensorProcedureCollection spCol = new SensorProcedureCollection()
                    .Where(SensorProcedure.Columns.DataSourceID, ds.Id)
                    .Load();

                foreach (SensorProcedure sp in spCol)
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


            if (!String.IsNullOrEmpty(StartDate.Text))
                ds.StartDate = StartDate.SelectedDate;
            if (StartDate.SelectedDate.Date.Year < 1900)
            {
                ds.StartDate = null;
            }

            ds.UserId = AuthHelper.GetLoggedInUserId;

            ds.Save();
            Auditing.Log("Instrument.Save", new Dictionary<string, object> { { "ID", ds.Id }, { "Code", ds.Code }, { "Name", ds.Name } });

            DataSourceGrid.DataBind();

            DetailWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Save");
            MessageBoxes.Error(ex, "Unable to save site");
        }
    }

    protected void DataSourceStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("VDataSource", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    protected void DoDelete(object sender, DirectEventArgs e)
    {
        string ActionType = e.ExtraParams["type"];
        string recordID = e.ExtraParams["id"];
        try
        {
            if (ActionType == "Sensor")
            {
                //DataSource dataSource = new DataSource(recordID);
                //if (dataSource != null)
                //{
                //    dataSource.StationID = null;
                //    dataSource.UserId = AuthHelper.GetLoggedInUserId;
                //    dataSource.Save();
                //    Auditing.Log("Station.UnlinkSensor", new Dictionary<string, object> {
                //        { "ID", dataSource.Id }, { "Code", dataSource.Code }, { "Name", dataSource.Name } });
                //    DataSourceGrid.DataBind();
                //}
            }
            else if (ActionType == "Organisation")
            {
                new StationOrganisationController().Delete(recordID);
                Auditing.Log("DataSource.UnlinkOrganisation", new Dictionary<string, object> { { "ID", recordID } });
                OrganisationGrid.DataBind();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DoUnlink({ActionType},{RecordID})", ActionType, recordID);
            MessageBoxes.Error(ex, "Unable to unlink {0}", ActionType == "Organisation" ? "organisation" : "sensor");
        }
    }

    #endregion

    #region Organisations

    protected void OrganisationGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["DataSourceID"] != null && e.Parameters["DataSourceID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["DataSourceID"].ToString());
            //VDataSourceOrganisationCollection DataSourceOrganisationCol = new VDataSourceOrganisationCollection()
            //    .Where(VDataSourceOrganisation.Columns.StationID, Id)
            //    .OrderByAsc(VDataSourceOrganisation.Columns.StartDate)
            //    .OrderByAsc(VDataSourceOrganisation.Columns.EndDate)
            //    .OrderByAsc(VDataSourceOrganisation.Columns.OrganisationName)
            //    .OrderByAsc(VDataSourceOrganisation.Columns.OrganisationRoleName)
            //    .Load();
            //OrganisationGrid.GetStore().DataSource = DataSourceOrganisationCol;
            OrganisationGrid.GetStore().DataBind();
        }
    }

    protected void AcceptOrganisation_Click(object sender, DirectEventArgs e)
    {
        try
        {
            RowSelectionModel masterRow = DataSourceGrid.SelectionModel.Primary as RowSelectionModel;
            var masterID = new Guid(masterRow.SelectedRecordID);
            //DataSourceOrganisation dataSourceOrganisation = new DataSourceOrganisation();
            //dataSourceOrganisation.StationID = masterID;
            //dataSourceOrganisation.OrganisationID = new Guid(cbOrganisation.SelectedItem.Value.Trim());
            //dataSourceOrganisation.OrganisationRoleID = new Guid(cbOrganisationRole.SelectedItem.Value.Trim());
            //if (!String.IsNullOrEmpty(dfOrganisationStartDate.Text) && (dfOrganisationStartDate.SelectedDate.Year >= 1900))
            //    dataSourceOrganisation.StartDate = dfOrganisationStartDate.SelectedDate;
            //if (!String.IsNullOrEmpty(dfOrganisationEndDate.Text) && (dfOrganisationEndDate.SelectedDate.Year >= 1900))
            //    dataSourceOrganisation.EndDate = dfOrganisationEndDate.SelectedDate;
            //dataSourceOrganisation.UserId = AuthHelper.GetLoggedInUserId;
            //dataSourceOrganisation.Save();
            //Auditing.Log("Station.AddOrganisation", new Dictionary<string, object> {
            //    { "StationID", masterID },
            //    { "OrganisationID", dataSourceOrganisation.OrganisationID},
            //    { "OrganisationCode", dataSourceOrganisation.Organisation.Code},
            //    { "RoleID", dataSourceOrganisation.OrganisationRoleID },
            //    { "RoleCode", dataSourceOrganisation.OrganisationRole.Code},
            //});
            OrganisationGrid.DataBind();
            OrganisationWindow.Hide();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "AcceptOrganisation_Click");
            MessageBoxes.Error(ex, "Unable to save organisation");
        }
    }
    #endregion
}