using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ext.Net;
using SAEON.Observations.Data;
using SubSonic;
using System.Xml;
using System.Xml.Xsl;

/// <summary>
/// Summary description for _Phenomenon
/// </summary>

public partial class _Phenomenon : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void PhenomenonStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.PhenomenonGrid.GetStore().DataSource = PhenomenonRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        PhenomenonCollection col = new PhenomenonCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = Phenomenon.Columns.Code;
            errorMessage = "The specified Phenomenon Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = Phenomenon.Columns.Name;
            errorMessage = "The specified Phenomenon Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new PhenomenonCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new PhenomenonCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Phenomenon.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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

        Phenomenon phenom = new Phenomenon();

        if (String.IsNullOrEmpty(tfID.Text))
            phenom.Id = Guid.NewGuid();
        else
            phenom = new Phenomenon(tfID.Text.Trim());

        phenom.Code = tfCode.Text.Trim();
        phenom.Name = tfName.Text.Trim();
        phenom.Description = tfDescription.Text.Trim();

        if (!String.IsNullOrEmpty(tfUrl.Text))
            phenom.Url = tfUrl.Text.Trim();

        phenom.UserId = AuthHelper.GetLoggedInUserId;

        phenom.Save();

        PhenomenonGrid.DataBind();

        this.DetailWindow.Hide();
    }

    protected void PhenomenonUOMGrid_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {

        if (e.Parameters["PhenomenonID"] != null && e.Parameters["PhenomenonID"].ToString() != "-1")
        {

            Guid Id = Guid.Parse(e.Parameters["PhenomenonID"].ToString());

            UnitOfMeasureCollection uomCol = new Select()
                      .From(UnitOfMeasure.Schema)
                      .InnerJoin(PhenomenonUOM.UnitOfMeasureIDColumn, UnitOfMeasure.IdColumn)
                      .Where(PhenomenonUOM.Columns.PhenomenonID).IsEqualTo(Id)
                      .ExecuteAsCollection<UnitOfMeasureCollection>();

            this.PhenomenonUOMGrid.GetStore().DataSource = uomCol;
            this.PhenomenonUOMGrid.GetStore().DataBind();
        }
    }


    protected void UnitOfMeasureStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["PhenomenonID"] != null && e.Parameters["PhenomenonID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["PhenomenonID"].ToString());

            UnitOfMeasureCollection uomavailCol = new Select()
                    .From(UnitOfMeasure.Schema)
                    .Where(UnitOfMeasure.IdColumn).NotIn(new Select(new String[] { PhenomenonUOM.Columns.UnitOfMeasureID })
                                                                    .From(PhenomenonUOM.Schema)
                                                                    .Where(PhenomenonUOM.PhenomenonIDColumn).IsEqualTo(Id))
                    .ExecuteAsCollection<UnitOfMeasureCollection>();

            Store3.DataSource = uomavailCol.ToList();
            Store3.DataBind();

        }
    }


    protected void PhenomenonOfferingGrid_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {

        if (e.Parameters["PhenomenonID"] != null && e.Parameters["PhenomenonID"].ToString() != "-1")
        {

            Guid Id = Guid.Parse(e.Parameters["PhenomenonID"].ToString());

            OfferingCollection offeringCol = new Select()
                     .From(Offering.Schema)
                     .InnerJoin(PhenomenonOffering.OfferingIDColumn, Offering.IdColumn)
                     .Where(PhenomenonOffering.Columns.PhenomenonID).IsEqualTo(Id)
                     .ExecuteAsCollection<OfferingCollection>();

            this.PhenomenonOfferingGrid.GetStore().DataSource = offeringCol;
            this.PhenomenonOfferingGrid.GetStore().DataBind();
        }
    }

    protected void OfferingGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["PhenomenonID"] != null && e.Parameters["PhenomenonID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["PhenomenonID"].ToString());

            OfferingCollection offavailCol = new Select()
                    .From(Offering.Schema)
                    .Where(Offering.IdColumn).NotIn(new Select(new String[] { PhenomenonOffering.Columns.OfferingID })
                                                                    .From(PhenomenonOffering.Schema)
                                                                    .Where(PhenomenonOffering.PhenomenonIDColumn).IsEqualTo(Id))
                    .ExecuteAsCollection<OfferingCollection>();

            OfferingGridStore.DataSource = offavailCol.ToList();
            OfferingGridStore.DataBind();

        }
    }

    protected void AcceptUOM_Click(object sender, DirectEventArgs e)
    {


        //if (UnitOfMeasureGrid.GetStore().co.Items.Count > 0)
        //{
        RowSelectionModel sm = this.UnitOfMeasureGrid.SelectionModel.Primary as RowSelectionModel;
        RowSelectionModel phenomenonrow = this.PhenomenonGrid.SelectionModel.Primary as RowSelectionModel;

        string PhenomenonID = phenomenonrow.SelectedRecordID;

        if (sm.SelectedRows.Count > 0)
        {
            foreach (SelectedRow row in sm.SelectedRows)
            {
                PhenomenonUOM unit = new PhenomenonUOM();
                unit.UnitOfMeasureID = new Guid(row.RecordID);
                unit.PhenomenonID = new Guid(PhenomenonID);

                unit.Save();
            }

            Store4.DataBind();
            AvailableUnitsWindow.Hide();
        }
        else
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Invalid Selection",
                Message = "Select at least one Unit",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO
            });
        }
        //}
    }

    protected void AcceptOfferingButton_Click(object sender, DirectEventArgs e)
    {

        RowSelectionModel sm = this.OfferingGrid.SelectionModel.Primary as RowSelectionModel;
        RowSelectionModel phenomenonrow = this.PhenomenonGrid.SelectionModel.Primary as RowSelectionModel;

        string PhenomenonID = phenomenonrow.SelectedRecordID;

        if (sm.SelectedRows.Count > 0)
        {
            foreach (SelectedRow row in sm.SelectedRows)
            {
                PhenomenonOffering offer = new PhenomenonOffering();
                offer.OfferingID = new Guid(row.RecordID);
                offer.PhenomenonID = new Guid(PhenomenonID);

                offer.Save();
            }

            PhenomenonOfferingGrid.GetStore().DataBind();
            AvailableOfferingsWindow.Hide();
        }
        else
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Invalid Selection",
                Message = "Select at least one Offering",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.INFO
            });
        }
    }

    protected void PhenomenonStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("Phenomenon", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    protected void DoDelete(object sender, DirectEventArgs e)
    {
        string ActionType = e.ExtraParams["type"];
        string recordID = e.ExtraParams["id"];	//offering id if ActionType = RemoveOffering || 
        string phenomenonID = e.ExtraParams["PhenomenonID"];

        if (ActionType == "RemoveOffering")	
        {
            PhenomenonOfferingCollection phenOffCol = new PhenomenonOfferingCollection().Where(PhenomenonOffering.Columns.OfferingID, recordID)
                .Where(PhenomenonOffering.Columns.PhenomenonID, phenomenonID).Load();
            if (phenOffCol.Count == 1)
            {
                ObservationCollection obsCol = new ObservationCollection().Where(Observation.Columns.PhenomenonOfferingID, phenOffCol[0].Id).Load();
                DataLogCollection dataLogCol = new DataLogCollection().Where(DataLog.Columns.PhenomenonOfferingID, phenOffCol[0].Id).Load();
                if (obsCol.Count == 0 && dataLogCol.Count == 0)
                {
                    PhenomenonOffering.Delete(phenOffCol[0].Id);

                    PhenomenonOfferingGrid.DataBind();
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Offerings cant be deleted when they are connected to data in the data log or observations.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR
                    });
                }
                
            }
        }
        else if (ActionType == "RemoveUOM")	
        {
            PhenomenonUOMCollection phenUOMCol = new PhenomenonUOMCollection().Where(PhenomenonUOM.Columns.UnitOfMeasureID, recordID)
                .Where(PhenomenonUOM.Columns.PhenomenonID, phenomenonID).Load();
            if (phenUOMCol.Count == 1)
            {
                ObservationCollection obsCol = new ObservationCollection().Where(Observation.Columns.PhenomenonUOMID, phenUOMCol[0].Id).Load();
                DataLogCollection dataLogCol = new DataLogCollection().Where(DataLog.Columns.PhenomenonUOMID, phenUOMCol[0].Id).Load();
                if (obsCol.Count == 0 && dataLogCol.Count == 0)
                {
                    PhenomenonUOM.Delete(phenUOMCol[0].Id);

                    PhenomenonUOMGrid.DataBind();
                }
                else
                {
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Unit of measurements cant be deleted when they are connected to data in the data log or observations.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR
                    });
                }

            }
        }
        else if (ActionType == "RemovePhenomenon")
        {
            Phenomenon phen = new Phenomenon(recordID);
            PhenomenonUOMCollection phenUOMCol = new PhenomenonUOMCollection().Where(PhenomenonUOM.Columns.PhenomenonID, recordID).Load();
            PhenomenonOfferingCollection phenOffCol = new PhenomenonOfferingCollection().Where(PhenomenonOffering.Columns.PhenomenonID, recordID).Load();
            SensorCollection SensorCol = new SensorCollection().Where(Sensor.Columns.PhenomenonID, recordID).Load();

            if (phenOffCol.Count != 0 || phenUOMCol.Count != 0 || SensorCol.Count != 0)
            {
                X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Error",
                        Message = "Phenomenon's cant be deleted when they have unit of measurements, offerings or sensors connected to them.",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.ERROR
                    });
            }
            else 
            {
                Phenomenon.Delete(phen.Id);
            }
        }
    }
}
