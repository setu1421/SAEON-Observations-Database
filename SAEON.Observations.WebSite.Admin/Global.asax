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
        if (Request.IsLocal || Request.Url.DnsSafeHost.ToLowerInvariant().Contains("-test"))
        {
            Session.Add("GA4Key", "G-ZG93KDRT46");
            Session.Add("ClarityKey","6j3zwxohjo");
            Session.Add("AppInsightsKey","29c1b7fd-89da-4cb9-8bd0-df6526e78dc2");
        }
        else
        {
            Session.Add("GA4Key", "G-5QHD56BYB0");
            Session.Add("ClarityKey","6dewe0cje3");
            Session.Add("AppInsightsKey","cb428f42-2a0c-4825-a0dc-1c6ec69fada8");
        }
    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

</script>
