using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Ext.Net;

/// <summary>
/// Summary description for Login
/// </summary>
public partial class _Login : System.Web.UI.Page
{
    protected void btnLogin_Click(object sender, DirectEventArgs e)
    {
        try
        {
            bool isValid = Membership.ValidateUser(this.txtUsername.Text, this.txtPassword.Text);

            if (isValid)
            {

                X.MessageBox.Alert("Success", "Logged in").Show();

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(this.txtUsername.Text, cbRememberMe.Checked, 480);
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, this.txtUsername.Text, ticket.CookiePath);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));

                if (cbRememberMe.Checked)
                    cookie.Expires = authTicket.Expiration;

                HttpContext.Current.Response.Cookies.Set(cookie);

                Response.Redirect("Default.aspx");

            }
            else
            {
                MessageBoxes.Error("Login Failed", "Invalid Username or Password");
            }
        }
        catch (Exception ex)
        {
            MessageBoxes.Error(ex, "Error", "Unable to login");
        }

    }
}