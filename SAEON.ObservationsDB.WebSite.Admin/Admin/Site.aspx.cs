using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAEON.ObservationsDB.Data;
using Ext.Net;
using SubSonic;
using System.Xml;
using System.Xml.Xsl;

/// <summary>
/// Summary description for Site
/// </summary>
public partial class _Site : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!X.IsAjaxRequest)
        //{
        //    this.Store1.DataSource = new OrganisationCollection().OrderByAsc(Organisation.Columns.Name).Load();
        //    this.Store1.DataBind();
        //}
    }

    protected void SiteStore_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.SiteGrid.GetStore().DataSource = SiteRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ValidateField(object sender, RemoteValidationEventArgs e)
    {

        SiteCollection col = new SiteCollection();

        string checkColumn = String.Empty,
               errorMessage = String.Empty;

        if (e.ID == "tfCode")
        {
            checkColumn = Site.Columns.Code;
            errorMessage = "The specified Site Code already exists";
        }
        else if (e.ID == "tfName")
        {
            checkColumn = Site.Columns.Name;
            errorMessage = "The specified Site Name already exists";

        }

        if (String.IsNullOrEmpty(tfID.Text.ToString()))
            col = new SiteCollection().Where(checkColumn, e.Value.ToString().Trim()).Load();
        else
            col = new SiteCollection().Where(checkColumn, e.Value.ToString().Trim()).Where(Site.Columns.Id, SubSonic.Comparison.NotEquals, tfID.Text.Trim()).Load();

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

        Site prosite = new Site();

        if (String.IsNullOrEmpty(tfID.Text))
            prosite.Id = Guid.NewGuid();
        else
            prosite = new Site(tfID.Text.Trim());

        prosite.Code = tfCode.Text.Trim();
        prosite.Name = tfName.Text.Trim();
        prosite.Description = tfDescription.Text.Trim();
        //prosite.OrganisationID = new Guid(cbOrg.SelectedItem.Value.Trim());

        prosite.UserId = AuthHelper.GetLoggedInUserId;

        prosite.Save();

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
}