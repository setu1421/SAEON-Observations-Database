using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAEON.Observations.Data;
using Ext.Net;
using FileHelpers.Dynamic;
using FileHelpers;
using SubSonic;
using System.Xml;
using System.Xml.Xsl;
using SAEON.Logs;

public partial class _UnitsOfMeasure : System.Web.UI.Page
{

    #region UnitOfMeasure
    protected void UnitOfMeasureStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.UnitOfMeasureGrid.GetStore().DataSource = UnitOfMeasureRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {
        UnitOfMeasureCollection col = new UnitOfMeasureCollection();
        string checkColumn = String.Empty;
        string errorMessage = String.Empty;
        e.Success = true;

        if (e.ID == "tfCode" || e.ID == "tfName")
        {
            if (e.ID == "tfCode")
            {
                checkColumn = UnitOfMeasure.Columns.Code;
                errorMessage = "The specified Unit Code already exists";
            }
            else if (e.ID == "tfUnit")
            {
                checkColumn = UnitOfMeasure.Columns.Unit;
                errorMessage = "The specified Unit already exists";

            }

            if (tfID.IsEmpty)
                col = new UnitOfMeasureCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            else
                col = new UnitOfMeasureCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(UnitOfMeasure.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

            if (col.Count > 0)
            {
                e.Success = false;
                e.ErrorMessage = errorMessage;
            }
        }
    }
    protected void Save(object sender, DirectEventArgs e)
    {

        UnitOfMeasure prosite = new UnitOfMeasure();

        if (String.IsNullOrEmpty(tfID.Text))
            prosite.Id = Guid.NewGuid();
        else
            prosite = new UnitOfMeasure(tfID.Text.Trim());

        prosite.Code = tfCode.Text.Trim();
        prosite.Unit = tfUnit.Text.Trim();
        prosite.UnitSymbol = tfSymbol.Text.Trim();

        prosite.UserId = AuthHelper.GetLoggedInUserId;

        prosite.Save();

        UnitOfMeasureGrid.DataBind();

        this.DetailWindow.Hide();
    }

    protected void UnitOfMeasureStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        string js = BaseRepository.BuildExportQ("UnitOfMeasure", gridData, visCols, sortCol, sortDir);

        BaseRepository.doExport(type, js);
    }

    #endregion

    #region Phenomena
    protected void UnitOfMeasurePhenomenaGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            if (e.Parameters["UnitOfMeasureID"] != null && e.Parameters["UnitOfMeasureID"].ToString() != "-1")
            {
                Guid Id = Guid.Parse(e.Parameters["UnitOfMeasureID"].ToString());
                try
                {
                    var col = new VUnitOfMeasurePhenomenaCollection()
                        .Where(VUnitOfMeasurePhenomena.Columns.UnitOfMeasureID, Id)
                        .OrderByAsc(VUnitOfMeasurePhenomena.Columns.Name)
                        .Load();
                    UnitOfMeasurePhenomenaGridStore.DataSource = col;
                    UnitOfMeasurePhenomenaGridStore.DataBind();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    MessageBoxes.Error(ex, "Error", "Unable to refresh phenomena grid");
                }
            }
        }
    }
    #endregion
}