using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAEON.Observations.Data;
using Ext.Net;
using SubSonic;
using System.Xml;
using System.Xml.Xsl;
using SAEON.Logs;

/// <summary>
/// Summary description for Offering
/// </summary>
public partial class _Offerings : System.Web.UI.Page
{
    #region Offering
    protected void OfferingStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.OfferingGrid.GetStore().DataSource = OfferingRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {
        OfferingCollection col = new OfferingCollection();
        string checkColumn = String.Empty;
        string errorMessage = String.Empty;
        e.Success = true;
        tfCode.HasValue();
        tfName.HasValue();
        tfDescription.HasValue();

        if (e.ID == "tfCode" || e.ID == "tfName")
        {
            if (e.ID == "tfCode")
            {
                checkColumn = Offering.Columns.Code;
                errorMessage = "The specified Offering Code already exists";
            }
            else if (e.ID == "tfName")
            {
                checkColumn = Offering.Columns.Name;
                errorMessage = "The specified Offering Name already exists";

            }

            if (tfID.IsEmpty)
                col = new OfferingCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
            else
                col = new OfferingCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Offering.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

            if (col.Count > 0)
            {
                e.Success = false;
                e.ErrorMessage = errorMessage;
            }
        }
    }

    protected void Save(object sender, DirectEventArgs e)
    {

        Offering org = new Offering();

        if (!tfID.HasValue())
            org.Id = Guid.NewGuid();
        else
            org = new Offering(tfID.Text);

        if (tfCode.HasValue())
            org.Code = tfCode.Text;
        if (tfName.HasValue())
            org.Name = tfName.Text;
        if (tfDescription.HasValue())
            org.Description = tfDescription.Text;

        org.UserId = AuthHelper.GetLoggedInUserId;

        org.Save();

        OfferingGrid.DataBind();

        this.DetailWindow.Hide();
    }

    protected void OfferingStore_Submit(object sender, StoreSubmitDataEventArgs e)
    {
        string type = FormatType.Text;
        string visCols = VisCols.Value.ToString();
        string gridData = GridData.Text;
        string sortCol = SortInfo.Text.Substring(0, SortInfo.Text.IndexOf("|"));
        string sortDir = SortInfo.Text.Substring(SortInfo.Text.IndexOf("|") + 1);

        //string js = BaseRepository.BuildExportQ("Offering", gridData, visCols, sortCol, sortDir);
        //BaseRepository.doExport(type, js);
        BaseRepository.Export("Offering", gridData, visCols, sortCol, sortDir, type, "Offerings", Response);
    }
    #endregion

    #region Phenomena
    protected void OfferingPhenomenaGridStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        using (Logging.MethodCall(GetType()))
        {
            if (e.Parameters["OfferingID"] != null && e.Parameters["OfferingID"].ToString() != "-1")
            {
                Guid Id = Guid.Parse(e.Parameters["OfferingID"].ToString());
                try
                {
                    var col = new VOfferingPhenomenaCollection()
                        .Where(VOfferingPhenomena.Columns.OfferingID, Id)
                        .OrderByAsc(VOfferingPhenomena.Columns.Name)
                        .Load();
                    OfferingPhenomenaGridStore.DataSource = col;
                    OfferingPhenomenaGridStore.DataBind();
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