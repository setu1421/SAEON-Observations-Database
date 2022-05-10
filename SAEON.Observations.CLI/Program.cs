using SAEON.Core;
using SAEON.Logs;
using System.CommandLine;

namespace SAEON.Observations.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            SAEONLogs.CreateConfiguration().Initialize();
            try
            {
                using (SAEONLogs.MethodCall(typeof(Program)))
                {
                    SAEONLogs.Information("Starting: {Application} Log: {LogLevel}", ApplicationHelper.ApplicationName, SAEONLogs.Level);
                    var rootCmd = new RootCommand("SAEON Observations Database Command Line Interface");
                    var createDatasetsCmd = new Command("CreateDatasets", "Create all datasets");
                    createDatasetsCmd.SetHandler(CreateDatasets);
                    rootCmd.AddCommand(createDatasetsCmd);
                    var createDatasetCmd = new Command("CreateDataset", "Create a dataset");
                    var datasetCodeArgument = new Argument<string>("code", "The Code of the dataset");
                    createDatasetCmd.AddArgument(datasetCodeArgument);
                    //createDatasetCmd.AddOption(new Option("--code", "The Code of the dataset"));
                    createDatasetCmd.SetHandler((string code) => CreateDataset(code), datasetCodeArgument);
                    rootCmd.AddCommand(createDatasetCmd);
                    return rootCmd.Invoke(args);
                }
            }
            finally
            {
                SAEONLogs.ShutDown();
            }
        }

        static void CreateDataset(string code)
        {
            using (SAEONLogs.MethodCall(typeof(Program), new MethodCallParameters { { "Code", code } }))
            {
                SAEONLogs.Information("CreateDataset: {Code}", code);
            }
        }

        static void CreateDatasets()
        {
            using (SAEONLogs.MethodCall(typeof(Program)))
            {
                SAEONLogs.Information("CreateDatasets");
            }
        }
    }
}
