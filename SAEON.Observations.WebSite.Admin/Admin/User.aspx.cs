using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAEON.Observations.Data;
using Ext.Net;
using SubSonic;
using System.Web.Security;
using System.Text;

/// <summary>
/// Summary description for User
/// </summary>
public partial class _User : System.Web.UI.Page
{
    protected void Store1_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        this.GridPanel1.GetStore().DataSource = UserRepository.GetPagedList(e, e.Parameters[this.GridFilters1.ParamPrefix]);
    }

    protected void ShowDetails(object sender, DirectEventArgs e)
    {
        if (e != null)
        {
            lblInfo.Hidden = true;
            lblInfo.Text = "";
            tfPassword.Hidden = true;
            tfUserName.Disabled = true;
            string id = e.ExtraParams["id"];
            if (id != null)
            {
                MembershipUser usr = Membership.GetUser(new Guid(id));

                tfID.Text = id;
                tfUserName.Text = usr.UserName;
                tfEmail.Text = usr.Email;
                tfComment.Text = usr.Comment;
                tfCreateDate.Text = usr.CreationDate.ToString();

            }
        }

        this.Window1.Show();
    }

    protected void NewUser(object sender, DirectEventArgs e)
    {
        tfID.Text = "";
        tfUserName.Text = "";
        tfEmail.Text = "";
        tfComment.Text = "";
        tfCreateDate.Text = "";
        tfPassword.Text = "";
        tfPassword.Hidden = false;
        tfUserName.Disabled = false;

        ShowDetails(null, null);

    }

    protected void ChangePassword(object sender, DirectEventArgs e)
    {
        string temp = e.ExtraParams["Values"];
        string id = "";
        Dictionary<string, string>[] UserInfo = JSON.Deserialize<Dictionary<string, string>[]>(temp);
        //string id = UserInfo[0].Value;
        //tfPassUser = RowSelectionModel1.SelectedRows.items[0].RecordID;
        bool isDone = false;
        foreach (Dictionary<string, string> row in UserInfo)
        {
            foreach (KeyValuePair<string, string> keyValuePair in row)
            {
                if (!isDone)
                {
                    id = keyValuePair.Value;
                    isDone = true;
                }
            }
        }

        if (id != "")
        {
            tfPasswordHidden.Text = id;

            MembershipUser usr = Membership.GetUser(new Guid(id));
            tfPassUser.Text = usr.UserName;

            tfPassNew.Text = "";
            lblPassInfo.Text = "";
            this.Window2.Show();
        }
        else
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = "Invalid Selection",
                Message = "Please select the user who's password will be changed",
                Buttons = MessageBox.Button.OK,
                Icon = MessageBox.Icon.WARNING
            });

            tfPasswordHidden.Text = "";
            tfPassUser.Text = "";
        }

    }


    protected void SavePass(object sender, DirectEventArgs e)
    {
        if (tfPassNew.Text.Length >= 6)
        {
            string id = tfPasswordHidden.Text;

            if (id != null)
            {
                try
                {
                    MembershipUser usr = Membership.GetUser(new Guid(id));
                    bool DidWork = usr.ChangePassword(usr.ResetPassword(), tfPassNew.Text);
                    //Membership.UpdateUser(usr);
                    X.Msg.Show(new MessageBoxConfig
                    {
                        Title = "Success",
                        Message = "Password changed",
                        Buttons = MessageBox.Button.OK,
                        Icon = MessageBox.Icon.INFO
                    });

                    Window2.Hide();
                }
                catch (Exception ex1)
                {
                    lblPassInfo.Text = ex1.Message;
                    //lblPassInfo.Hidden = false;
                    //throw;
                }
                //Membership.UpdateUser(usr);

            }
        }
        else
        {
            lblPassInfo.Text = "Password should be more than 6 characters";
        }

    }

    protected void DoDelete(object sender, DirectEventArgs e)
    {
        string ActionType = e.ExtraParams["type"];
        string recordID = e.ExtraParams["id"];

        if (ActionType == "Delete")
        {
            //Membership.DeleteUser(recordID);
            MembershipUser usr = Membership.GetUser(new Guid(recordID));
            Membership.DeleteUser(usr.UserName);

            GridPanel1.DataBind();
        }

        else if (ActionType == "Edit")
        {
            ShowDetails(sender, e);
        }

        else if (ActionType == "RemoveRoles")
        {
            AspnetUsersInRoleCollection UiRCol = new AspnetUsersInRoleCollection().Where("UserId", e.ExtraParams["UserID"]).Where("RoleId", recordID).Load();
            if (UiRCol.Count == 1)
            {
                AspnetRole role = new AspnetRole(recordID);
                AspnetUser user = new AspnetUser(e.ExtraParams["UserID"]);

                Roles.RemoveUserFromRole(user.UserName, role.RoleName);
                //AspnetUsersInRole.Delete((UiRCol[0])
                Store4.DataBind();
            }
            else
            {

                X.Msg.Show(new MessageBoxConfig
                {
                    Title = "Error",
                    Message = "Entry not found",
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR
                });
            }

        }
    }

    protected void SaveUser(object sender, DirectEventArgs e)
    {
        if (tfID.Text != "")
        {
            string id = tfID.Text;

            if (id != null)
            {
                //MembershipUser usr = Membership.GetUser(id);
                MembershipUser usr = Membership.GetUser(new Guid(id));
                usr.Email = tfEmail.Text;
                usr.Comment = tfComment.Text;

                Membership.UpdateUser(usr);

                GridPanel1.DataBind();

                Window1.Hide();
            }
        }
        else  //new user
        {

            try
            {
                //MembershipCreateStatus status;
                //MembershipUser newUser = Membership.CreateUser(tfUserName.Text, tfPassword.Text, tfEmail.Text, "Name Of Company", "", true, out status);
                MembershipUser newUser = Membership.CreateUser(tfUserName.Text, tfPassword.Text, tfEmail.Text);

                if (newUser == null)
                {
                    //failed
                }
                else
                {
                    //success
                    newUser.Comment = tfComment.Text;
                    //newUser.IsApproved = true;
                    Membership.UpdateUser(newUser);
                    Window1.Hide();
                }
                GridPanel1.DataBind();

            }
            catch (Exception ex)
            {
                lblInfo.Text = ex.Message;
                lblInfo.Hidden = false;
                //throw;
            }
        }

    }

    ///////////////////////////////////////////////////////////

    protected void AcceptRole_Click(object sender, DirectEventArgs e)
    {
        StringBuilder result = new StringBuilder();


        RowSelectionModel sm = this.ARolesGrid.SelectionModel.Primary as RowSelectionModel;
        RowSelectionModel userRow = this.GridPanel1.SelectionModel.Primary as RowSelectionModel;

        string UserID = userRow.SelectedRecordID;

        foreach (SelectedRow row in sm.SelectedRows)
        {
            AspnetRole role = new AspnetRole(row.RecordID);
            AspnetUser user = new AspnetUser(UserID);
            Roles.AddUserToRole(user.UserName, role.RoleName);


            role.Save();
        }

        Store4.DataBind();
        AvailableRolesWindow.Hide();
    }

    protected void RolesGrid_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {

        if (e.Parameters["UserID"] != null && e.Parameters["UserID"].ToString() != "-1")
        {

            Guid Id = Guid.Parse(e.Parameters["UserID"].ToString());

            AspnetRoleCollection roleCol = new Select()
                      .From(AspnetRole.Schema)
                      .InnerJoin(AspnetUsersInRole.RoleIdColumn, AspnetRole.RoleIdColumn)
                      .Where(AspnetUsersInRole.Columns.UserId).IsEqualTo(Id)
                      .ExecuteAsCollection<AspnetRoleCollection>();

            this.RolesGrid.GetStore().DataSource = roleCol;
            this.RolesGrid.GetStore().DataBind();
        }
    }


    protected void ARoles_RefreshData(object sender, StoreRefreshDataEventArgs e)
    {
        if (e.Parameters["UserID"] != null && e.Parameters["UserID"].ToString() != "-1")
        {
            Guid Id = Guid.Parse(e.Parameters["UserID"].ToString());

            AspnetRoleCollection RoleavailCol = new Select()
                    .From(AspnetRole.Schema)
                    .Where(AspnetRole.RoleIdColumn).NotIn(new Select(new String[] { AspnetUsersInRole.Columns.RoleId })
                                                                    .From(AspnetUsersInRole.Schema)
                                                                    .Where(AspnetUsersInRole.UserIdColumn).IsEqualTo(Id))
                    .ExecuteAsCollection<AspnetRoleCollection>();

            Store3.DataSource = RoleavailCol.ToList();
            Store3.DataBind();

        }
    }
}