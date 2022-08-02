<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="SAEON.Logs" %>
<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        Startup.Run();
    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown
        SAEONLogs.ShutDown();
    }

    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started
        Session.Add("AppInsightsKey", ConfigurationManager.AppSettings["APPINSIGHTS_CONNECTIONSTRING"]);
        Session.Add("GA4Key", ConfigurationManager.AppSettings["GA4Key"]);
        Session.Add("GAUKey", ConfigurationManager.AppSettings["GAUKey"]);
        Session.Add("ClarityKey", ConfigurationManager.AppSettings["ClarityKey"]);
        //SAEONLogs.Information("GA4Key: {GA4Key} Clarity: {ClarityKey}", ConfigurationManager.AppSettings["GA4Key"], ConfigurationManager.AppSettings["ClarityKey"]);
    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

</script>
