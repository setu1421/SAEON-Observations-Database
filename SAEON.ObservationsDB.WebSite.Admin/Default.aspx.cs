using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.Web.Security;
using SAEON.ObservationsDB.Data;
using SubSonic;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.User.Identity.IsAuthenticated)
            FormsAuthentication.RedirectToLoginPage();

        if (!X.IsAjaxRequest)
        {
            CreateNavigationSections();

        }
    }

    protected void Logout(object sender, DirectEventArgs e)
    {
        FormsAuthentication.SignOut();
        Response.Redirect("~/Login.aspx");
    }


    protected void CreateNavigationSections()
    {
        string[] s = System.Web.Security.Roles.GetRolesForUser();

        List<Guid> moduleList = new List<Guid>();
        RoleModuleCollection roleModuleColFinal = new RoleModuleCollection();

        foreach (var item in s)
        {
            AspnetRole aspRole = new AspnetRole("RoleName", item.ToString());

            SqlQuery q = new Select().From(RoleModule.Schema)
                                     .InnerJoin(ModuleX.IdColumn,RoleModule.ModuleIDColumn)
                                     .Where(RoleModule.Columns.Id).IsNotNull().And(RoleModule.Columns.RoleId).IsEqualTo(aspRole.RoleId)
                                     .OrderAsc(ModuleX.IOrderColumn.QualifiedName);
            RoleModuleCollection roleModuleCol = q.ExecuteAsCollection<RoleModuleCollection>();

            foreach (var roleModule in roleModuleCol)
            {
                if (!moduleList.Contains(roleModule.ModuleID))
                {
                    moduleList.Add(roleModule.ModuleID);
                    roleModuleColFinal.Add(roleModule);
                }
            }
        }

        foreach (RoleModule roleModule in roleModuleColFinal)
        {
            ModuleX moduleX = new ModuleX(ModuleX.Columns.Id, roleModule.ModuleID);

            if (moduleX.ModuleID == null)
            {
                Ext.Net.TreePanel treePnl = new TreePanel();

                treePnl.ID = "treePnl_" + moduleX.Id.ToString().Replace("-", "_");
                treePnl.Title = moduleX.Name;
                treePnl.Height = Unit.Pixel(450);
                treePnl.Icon = (Icon)moduleX.Icon;
                treePnl.RootVisible = false;

                treePnl.Listeners.Click.Handler = " e.stopEvent();NewTab(node);";

                Ext.Net.TreeNode treeNodeBase = new Ext.Net.TreeNode();
                //treeNodeBase.Expanded = false;
                treeNodeBase.SingleClickExpand = true;
                treeNodeBase.NodeID = "base" + moduleX.Id.ToString();

                string[] tempitems = new string[moduleList.Count];
                for (int i = 0; i < moduleList.Count; i++)
                {
                    tempitems[i] = moduleList[i].ToString();
                }

                ModuleXCollection nodesCol = new ModuleXCollection().Where(ModuleX.Columns.ModuleID, SubSonic.Comparison.Equals, moduleX.Id)
                       .Where(ModuleX.Columns.Id, SubSonic.Comparison.In, tempitems).OrderByAsc(ModuleX.Columns.IOrder).Load();
                foreach (ModuleX node in nodesCol)
                {
                    Ext.Net.TreeNode treeNode = new Ext.Net.TreeNode();
                    treeNode.SingleClickExpand = true;
                    treeNode.Text = node.Name;
                    treeNode.Icon = (Icon)node.Icon;
                    treeNode.NodeID = "leafNode_" + node.Id.ToString();
                    treeNode.Href = node.Url;
                    treeNodeBase.Nodes.Add(treeNode);


                }

                treePnl.Root.Add(treeNodeBase);

                PanelW.Items.Add(treePnl);

            }
        }
    }


    protected void DoLogout(object sender, DirectEventArgs e)
    {
        Session.Abandon();
        FormsAuthentication.SignOut();
        Response.Redirect("login.aspx");

    }


}