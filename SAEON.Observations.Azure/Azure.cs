using SAEON.Azure.CosmosDB;
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
        private const string CosmosDBDatabase = "saeon-observations";
        private const string CosmosDBCollection = "Observations";
        private const string CosmosDBPartitionKey = "/SensorCode";

        public static bool Enabled { get; private set; } = false;
        public static bool StorageEnabled { get; private set; } = false;
        public static bool CosmosDBEnabled { get; private set; } = false;

        private AzureStorage Storage = null;
        private AzureCosmosDB<ObservationDocument> CosmosDB = null;

        static Azure()
        {
            using (Logging.MethodCall(typeof(Azure)))
            {
                try
                {
                    Enabled = Convert.ToBoolean(ConfigurationManager.AppSettings["AzureEnabled"] ?? "false");
                    if (Enabled)
                    {
                        StorageEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["AzureStorageEnabled"] ?? "false");
                        CosmosDBEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["AzureCosmosDBEnabled"] ?? "false");
                    }
                    Logging.Information("Azure: {Enabled} Storage: {StorageEnabled} CosmosDB: {CosmosDBEnabled}", Enabled, StorageEnabled, CosmosDBEnabled);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public Azure()
        {
            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    if (Enabled)
                    {
                        if (StorageEnabled)
                        {
                            Storage = new AzureStorage();
                        }
                        if (CosmosDBEnabled)
                        {
                            CosmosDB = new AzureCosmosDB<ObservationDocument>(CosmosDBDatabase, CosmosDBCollection, CosmosDBPartitionKey);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        ~Azure()
        {
            Storage = null;
            CosmosDB = null;
        }

        public async Task InitializeAsync()
        {
            if (!Azure.Enabled)
            {
                return;
            }

            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    if (StorageEnabled)
                    {
                        Logging.Verbose("Ensuring Storage Container exist");
                        await Storage.EnsureContainerAsync(Azure.BlobStorageContainer);
                    }
                    if (CosmosDBEnabled)
                    {
                        Logging.Verbose("Ensuring CosmosDB Collection exist");
                        await CosmosDB.EnsureCollectionAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }


        public void Initialize()
        {
            if (!Azure.Enabled)
            {
                return;
            }

            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    InitializeAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        #region Storage
        public async Task UploadAsync(string folder, string fileName, string fileContents)
        {
            if (!Enabled || !StorageEnabled)
            {
                return;
            }

            using (Logging.MethodCall(GetType(), new ParameterList { { "Folder", folder }, { "FileName", fileName } }))
            {
                try
                {
                    var date = DateTime.Now;
                    var container = Storage.GetContainer(BlobStorageContainer);
                    var bytes = Encoding.UTF8.GetBytes(fileContents);
                    await container.UploadBlobAsync($"{folder}/{fileName}", bytes);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }

        public void Upload(string folder, string fileName, string fileContents)
        {
            if (!Enabled || !StorageEnabled)
            {
                return;
            }

            using (Logging.MethodCall(GetType(), new ParameterList { { "Folder", folder }, { "FileName", fileName } }))
            {
                try
                {
                    UploadAsync(folder, fileName, fileContents).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
        #endregion

        #region CosmosDB
        public async Task AddObservationAsync(ObservationDocument document)
        {
            if (!Enabled || !CosmosDBEnabled)
            {
                return;
            }

            using (Logging.MethodCall(GetType())) 
            {
                try
                {
                    await CosmosDB.CreateItemAsync(document);
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                } 
            } 
        }

        public void AddObservation(ObservationDocument document)
        {
            if (!Enabled || !CosmosDBEnabled)
            {
                return;
            }

            using (Logging.MethodCall(GetType()))
            {
                try
                {
                    AddObservationAsync(document).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Logging.Exception(ex);
                    throw;
                }
            }
        }
        #endregion
    }
}