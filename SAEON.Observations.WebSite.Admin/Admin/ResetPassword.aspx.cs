using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Ext.Net;

/// <summary>
/// Summary description for Login
/// </summary>
public partial class Admin_ResetPassword : System.Web.UI.Page
{
    protected void btnResetPassword_Click(object sender, DirectEventArgs e)
    {
        try
        {
            MembershipUser u = Membership.GetUser(txtUsername.Text);
            u.ChangePassword(u.ResetPassword(), txtPassword.Text);
            FormsAuthentication.SignOut();
            Response.Redirect("~/Login.aspx");
        }
        catch (Exception ex)
        {
            MessageBoxes.Error(ex, "Error", "Unable to reset password");
        }

    }
}