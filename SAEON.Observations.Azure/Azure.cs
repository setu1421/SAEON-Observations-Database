using SAEON.Azure.Storage;
using SAEON.Logs;
using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace SAEON.Observations.Azure
{
    public class Azure
    {
        private const string BlobStorageContainer = "saeon-observations";
        private const string ObservationsStorageTable = "Observations";   

        public static bool Enabled { get; private set; } = false;

        private AzureStorage Storage = null;

        static Azure() 
        {
            using (Logging.MethodCall(typeof(Azure)))
            {
                Enabled = Convert.ToBoolean(ConfigurationManager.AppSettings["AzureEnabled"]);
                Logging.Information("Azure.Enabled: {enabled}", Enabled);
            }
        }

        public Azure()
        {
            using (Logging.MethodCall(typeof(Azure)))
            {
                if (Enabled)
                {
                    Storage = new AzureStorage();
                }
            }
        }

        ~Azure()
        {
            Storage = null;   
        }

        public async Task InitializeAsync()
        {
            if (!Azure.Enabled) return;
            using (Logging.MethodCall(typeof(Azure)))
            {
                var storage = new AzureStorage();
                await storage.EnsureContainerAsync(Azure.BlobStorageContainer);
                //await storage.EnsureQueueAsync(WeatherAPIBase.LocationsJSONQueue);
                //await storage.EnsureQueueAsync(WeatherAPIBase.LocationsQueue); 
                //await storage.EnsureTableAsync(WeatherAPIBase.LocationsTable);
                //await storage.EnsureQueueAsync(WeatherAPIBase.ConditionsJSONQueue);
                //await storage.EnsureQueueAsync(WeatherAPIBase.ConditionsQueue);
                //await storage.EnsureTableAsync(WeatherAPIBase.ConditionsTable);
                //await storage.EnsureQueueAsync(WeatherAPIBase.ForecastsJSONQueue);
                //await storage.EnsureQueueAsync(WeatherAPIBase.ForecastsQueue);
                //await storage.EnsureTableAsync(WeatherAPIBase.ForecastsTable);
            }
        } 


        public void Initialize() 
        {
            if (!Azure.Enabled) return;
            using (Logging.MethodCall(typeof(Azure)))
            { 
                InitializeAsync().GetAwaiter().GetResult();
            }
        }

        public async Task UploadAsync(string folder, string fileName, string fileContents)
        {
            if (!Enabled) return;
            using (Logging.MethodCall(GetType()))
            {
                var date = DateTime.Now;
                var container = Storage.GetContainer(BlobStorageContainer);
                var bytes = Encoding.UTF8.GetBytes(fileContents);
                await container.UploadBlobAsync($"{folder}/{fileName}", bytes);
            }
        }

        public void Upload(string folder, string fileName, string fileContents)
        {
            if (!Enabled) return;  
            using (Logging.MethodCall(GetType())) 
            {
                UploadAsync(folder, fileName, fileContents).GetAwaiter().GetResult();
            }
        }
    }
}