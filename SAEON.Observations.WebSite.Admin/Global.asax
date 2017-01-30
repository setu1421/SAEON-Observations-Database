<%@ Application Language="C#" %>
<%@ Import Namespace="Serilog" %>
<%@ Import Namespace="Serilog.Settings.AppSettings" %>
<%@ Import Namespace="Serilog.Sinks.RollingFile" %>
<%@ Import Namespace="Serilog.Sinks.Seq" %>
<%@ Import Namespace="System.Web.Routing" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.AppSettings()
            .Enrich.FromLogContext()
            .WriteTo.RollingFile(Server.MapPath(@"~/App_Data/Logs/SAEON.Observations.WebSite.Admin-{Date}.txt"))
            .WriteTo.Seq("http://localhost:5341/")
            .CreateLogger();
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        string docPath = System.Web.Configuration.WebConfigurationManager.AppSettings["DocumentsPath"];
        if (!string.IsNullOrEmpty(docPath))
        {
            string path = Server.MapPath(docPath);
            if (!string.IsNullOrEmpty(path))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(path,"Uploads"));
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(path,"Downloads"));
            }
        }
    }

    void Application_End(object sender, EventArgs e)


    {
        //  Code that runs on application shutdown

    }

    void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started
    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

</script>
