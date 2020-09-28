using Ext.Net;
using SAEON.Logs;
using Serilog;
using System;
using System.Threading;
using System.Web.Security;

/// <summary>
/// Summary description for Login
/// </summary>
public partial class _Login : System.Web.UI.Page
{
    protected void btnLogin_Click(object sender, DirectEventArgs e)
    {
        try
        {
            bool isValid = Membership.ValidateUser(txtUsername.Text, this.txtPassword.Text);
            try
            {
                SAEONLogs.Information("Login: {UserName} {Valid}", txtUsername.Text, isValid);
                Auditing.Log(GetType(), new MethodCallParameters {
                    { "UserName", txtUsername.Text },
                    { "Valid", isValid }
                });
            }
            catch (Exception ex)
            {
                SAEONLogs.Exception(ex);
            }
            if (isValid)
            {

                //X.MessageBox.Alert("Success", "Logged in").Show();

                //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(this.txtUsername.Text, cbRememberMe.Checked, 480);
                //FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, this.txtUsername.Text, ticket.CookiePath);
                //HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));

                //if (cbRememberMe.Checked)
                //    cookie.Expires = authTicket.Expiration;

                //HttpContext.Current.Response.Cookies.Set(cookie);
                FormsAuthentication.RedirectFromLoginPage(txtUsername.Text, cbRememberMe.Checked);
                Response.Redirect("~/");
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