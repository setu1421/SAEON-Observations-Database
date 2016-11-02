using SAEON.Observations.Data;
using SubSonic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_DataQueryTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GridView1.DataSource = new Select()
            .From(Organisation.Schema)
            .InnerJoin(OrganisationSite.Schema)
            .OrderAsc(Organisation.Columns.Name)
            .Distinct()
            .ExecuteDataSet();
        GridView1.DataBind();
    }
}