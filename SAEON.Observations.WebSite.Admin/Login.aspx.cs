using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Ext.Net;
using Serilog;
using System.Threading;
using SAEON.Logs;

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
            try
            {
                Logging.Information("Login: {UserName} {Valid}", txtUsername.Text, isValid);
                Auditing.Log(GetType(), new ParameterList {
                    { "UserName", txtUsername.Text },
                    { "Valid", isValid }
                });
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
            }
            if (isValid)
            {

                //X.MessageBox.Alert("Success", "Logged in").Show();

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
        catch (ThreadAbortException)
        { }
        catch (Exception ex)
        {
            Log.Error(ex, "Unable to login");
            MessageBoxes.Error(ex, "Error", "Unable to login");
        }
    }
}